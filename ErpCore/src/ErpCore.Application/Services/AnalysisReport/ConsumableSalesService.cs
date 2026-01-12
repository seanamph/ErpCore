using Dapper;
using ErpCore.Application.DTOs.AnalysisReport;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.AnalysisReport;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.AnalysisReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using System.Data;

namespace ErpCore.Application.Services.AnalysisReport;

/// <summary>
/// 耗材出售單服務實作 (SYSA297)
/// </summary>
public class ConsumableSalesService : BaseService, IConsumableSalesService
{
    private readonly IConsumableSalesRepository _salesRepository;
    private readonly IConsumableReportRepository _consumableRepository;
    private readonly IConsumableTransactionRepository _transactionRepository;
    private readonly IDbConnectionFactory _connectionFactory;

    public ConsumableSalesService(
        IConsumableSalesRepository salesRepository,
        IConsumableReportRepository consumableRepository,
        IConsumableTransactionRepository transactionRepository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _salesRepository = salesRepository;
        _consumableRepository = consumableRepository;
        _transactionRepository = transactionRepository;
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<ConsumableSalesDto>> GetSalesAsync(ConsumableSalesQueryDto query)
    {
        try
        {
            var repositoryQuery = new ConsumableSalesQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TxnNo = query.Filters?.TxnNo,
                SiteId = query.Filters?.SiteId,
                Status = query.Filters?.Status,
                StartDate = query.Filters?.StartDate,
                EndDate = query.Filters?.EndDate,
                SortField = query.SortField,
                SortOrder = query.SortOrder
            };

            var result = await _salesRepository.GetSalesAsync(repositoryQuery);

            // 取得所有不重複的SiteId
            var siteIds = result.Items.Select(x => x.SiteId).Distinct().ToList();
            var siteNameMap = new Dictionary<string, string>();

            // 批量查詢SiteName
            if (siteIds.Any())
            {
                using var connection = _connectionFactory.CreateConnection();
                var sql = "SELECT ShopId, ShopName FROM Shops WHERE ShopId IN @SiteIds";
                var shops = await connection.QueryAsync<(string ShopId, string ShopName)>(sql, new { SiteIds = siteIds });
                siteNameMap = shops.ToDictionary(x => x.ShopId, x => x.ShopName);
            }

            var dtos = result.Items.Select(x => new ConsumableSalesDto
            {
                TxnNo = x.TxnNo,
                Rrn = x.Rrn,
                SiteId = x.SiteId,
                SiteName = siteNameMap.GetValueOrDefault(x.SiteId),
                PurchaseDate = x.PurchaseDate,
                Status = x.Status,
                StatusName = GetStatusName(x.Status),
                TotalAmount = x.TotalAmount,
                TaxAmount = x.TaxAmount,
                NetAmount = x.NetAmount,
                DetailCount = x.DetailCount,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt,
                ApprovedBy = x.ApprovedBy,
                ApprovedAt = x.ApprovedAt
            }).ToList();

            return new PagedResult<ConsumableSalesDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢耗材出售單列表失敗", ex);
            throw;
        }
    }

    public async Task<ConsumableSalesDetailDto> GetSalesDetailAsync(string txnNo)
    {
        try
        {
            var sales = await _salesRepository.GetSalesByTxnNoAsync(txnNo);
            if (sales == null)
            {
                throw new KeyNotFoundException($"耗材出售單不存在: {txnNo}");
            }

            // 取得SiteName
            string? siteName = null;
            if (!string.IsNullOrEmpty(sales.SiteId))
            {
                using var connection = _connectionFactory.CreateConnection();
                var sql = "SELECT ShopName FROM Shops WHERE ShopId = @SiteId";
                siteName = await connection.QueryFirstOrDefaultAsync<string>(sql, new { SiteId = sales.SiteId });
            }

            var dto = new ConsumableSalesDetailDto
            {
                TxnNo = sales.TxnNo,
                Rrn = sales.Rrn,
                SiteId = sales.SiteId,
                SiteName = siteName,
                PurchaseDate = sales.PurchaseDate,
                Status = sales.Status,
                StatusName = GetStatusName(sales.Status),
                TotalAmount = sales.TotalAmount,
                TaxAmount = sales.TaxAmount,
                NetAmount = sales.NetAmount,
                ApplyCount = sales.ApplyCount,
                DetailCount = sales.DetailCount,
                Notes = sales.Notes,
                Details = sales.Details.Select(d => new ConsumableSalesDetailItemDto
                {
                    DetailId = d.DetailId,
                    SeqNo = d.SeqNo,
                    ConsumableId = d.ConsumableId,
                    ConsumableName = d.ConsumableName,
                    Quantity = d.Quantity,
                    Unit = d.Unit,
                    UnitPrice = d.UnitPrice,
                    Amount = d.Amount,
                    Tax = d.Tax,
                    TaxAmount = d.TaxAmount,
                    NetAmount = d.NetAmount,
                    PurchaseStatus = d.PurchaseStatus,
                    Notes = d.Notes
                }).ToList(),
                CreatedBy = sales.CreatedBy,
                CreatedAt = sales.CreatedAt,
                UpdatedBy = sales.UpdatedBy,
                UpdatedAt = sales.UpdatedAt,
                ApprovedBy = sales.ApprovedBy,
                ApprovedAt = sales.ApprovedAt
            };

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢耗材出售單詳細資料失敗: {txnNo}", ex);
            throw;
        }
    }

    public async Task<string> CreateSalesAsync(CreateConsumableSalesDto dto, string userId)
    {
        try
        {
            // 生成交易單號
            var txnNo = await _salesRepository.GenerateTxnNoAsync(dto.SiteId, dto.PurchaseDate);

            // 計算金額
            var details = new List<ConsumableSalesDetail>();
            decimal totalAmount = 0;
            decimal totalTaxAmount = 0;
            decimal totalNetAmount = 0;

            for (int i = 0; i < dto.Details.Count; i++)
            {
                var detailDto = dto.Details[i];
                
                // 驗證耗材是否存在並取得耗材資訊
                var consumable = await _consumableRepository.GetConsumableByIdAsync(detailDto.ConsumableId);
                if (consumable == null)
                {
                    throw new ArgumentException($"耗材不存在: {detailDto.ConsumableId}");
                }

                var amount = detailDto.Quantity * detailDto.UnitPrice;
                var taxAmount = detailDto.Tax == "1" ? amount * 0.05m : 0; // 假設稅率為 5%
                var netAmount = amount - taxAmount;

                totalAmount += amount;
                totalTaxAmount += taxAmount;
                totalNetAmount += netAmount;

                var detail = new ConsumableSalesDetail
                {
                    DetailId = Guid.NewGuid(),
                    TxnNo = txnNo,
                    SeqNo = i + 1,
                    ConsumableId = detailDto.ConsumableId,
                    ConsumableName = consumable.ConsumableName,
                    Quantity = detailDto.Quantity,
                    Unit = consumable.Unit,
                    UnitPrice = detailDto.UnitPrice,
                    Amount = amount,
                    Tax = detailDto.Tax,
                    TaxAmount = taxAmount,
                    NetAmount = netAmount,
                    PurchaseStatus = "1",
                    Notes = detailDto.Notes,
                    CreatedBy = userId,
                    CreatedAt = DateTime.Now
                };

                details.Add(detail);
            }

            var sales = new ConsumableSales
            {
                TxnNo = txnNo,
                Rrn = Guid.NewGuid(),
                SiteId = dto.SiteId,
                PurchaseDate = dto.PurchaseDate,
                Status = "1",
                TotalAmount = totalAmount,
                TaxAmount = totalTaxAmount,
                NetAmount = totalNetAmount,
                ApplyCount = dto.Details.Count,
                DetailCount = dto.Details.Count,
                Notes = dto.Notes,
                CreatedBy = userId,
                CreatedAt = DateTime.Now,
                UpdatedBy = userId,
                UpdatedAt = DateTime.Now,
                Details = details
            };

            await _salesRepository.CreateSalesAsync(sales);
            return txnNo;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增耗材出售單失敗", ex);
            throw;
        }
    }

    public async Task UpdateSalesAsync(string txnNo, UpdateConsumableSalesDto dto, string userId)
    {
        try
        {
            var sales = await _salesRepository.GetSalesByTxnNoAsync(txnNo);
            if (sales == null)
            {
                throw new KeyNotFoundException($"耗材出售單不存在: {txnNo}");
            }

            if (sales.Status != "1")
            {
                throw new InvalidOperationException($"只有待審核狀態的耗材出售單才能修改: {txnNo}");
            }

            // 計算金額
            var details = new List<ConsumableSalesDetail>();
            decimal totalAmount = 0;
            decimal totalTaxAmount = 0;
            decimal totalNetAmount = 0;

            for (int i = 0; i < dto.Details.Count; i++)
            {
                var detailDto = dto.Details[i];
                
                // 驗證耗材是否存在並取得耗材資訊
                var consumable = await _consumableRepository.GetConsumableByIdAsync(detailDto.ConsumableId);
                if (consumable == null)
                {
                    throw new ArgumentException($"耗材不存在: {detailDto.ConsumableId}");
                }

                var amount = detailDto.Quantity * detailDto.UnitPrice;
                var taxAmount = detailDto.Tax == "1" ? amount * 0.05m : 0; // 假設稅率為 5%
                var netAmount = amount - taxAmount;

                totalAmount += amount;
                totalTaxAmount += taxAmount;
                totalNetAmount += netAmount;

                var detail = new ConsumableSalesDetail
                {
                    DetailId = Guid.NewGuid(),
                    TxnNo = txnNo,
                    SeqNo = i + 1,
                    ConsumableId = detailDto.ConsumableId,
                    ConsumableName = consumable.ConsumableName,
                    Quantity = detailDto.Quantity,
                    Unit = consumable.Unit,
                    UnitPrice = detailDto.UnitPrice,
                    Amount = amount,
                    Tax = detailDto.Tax,
                    TaxAmount = taxAmount,
                    NetAmount = netAmount,
                    PurchaseStatus = "1",
                    Notes = detailDto.Notes,
                    CreatedBy = userId,
                    CreatedAt = DateTime.Now
                };

                details.Add(detail);
            }

            sales.TotalAmount = totalAmount;
            sales.TaxAmount = totalTaxAmount;
            sales.NetAmount = totalNetAmount;
            sales.ApplyCount = dto.Details.Count;
            sales.DetailCount = dto.Details.Count;
            sales.Notes = dto.Notes;
            sales.UpdatedBy = userId;
            sales.UpdatedAt = DateTime.Now;
            sales.Details = details;

            await _salesRepository.UpdateSalesAsync(sales);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新耗材出售單失敗: {txnNo}", ex);
            throw;
        }
    }

    public async Task DeleteSalesAsync(string txnNo, string userId)
    {
        try
        {
            var sales = await _salesRepository.GetSalesByTxnNoAsync(txnNo);
            if (sales == null)
            {
                throw new KeyNotFoundException($"耗材出售單不存在: {txnNo}");
            }

            if (sales.Status != "1")
            {
                throw new InvalidOperationException($"只有待審核狀態的耗材出售單才能刪除: {txnNo}");
            }

            await _salesRepository.DeleteSalesAsync(txnNo);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除耗材出售單失敗: {txnNo}", ex);
            throw;
        }
    }

    public async Task ApproveSalesAsync(string txnNo, ApproveSalesDto dto, string userId)
    {
        try
        {
            var sales = await _salesRepository.GetSalesByTxnNoAsync(txnNo);
            if (sales == null)
            {
                throw new KeyNotFoundException($"耗材出售單不存在: {txnNo}");
            }

            if (sales.Status != "1")
            {
                throw new InvalidOperationException($"只有待審核狀態的耗材出售單才能審核: {txnNo}");
            }

            // 檢查庫存並扣減庫存（使用事務）
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                // 檢查庫存
                foreach (var detail in sales.Details)
                {
                    var availableQuantity = await _salesRepository.GetInventoryQuantityAsync(detail.ConsumableId, sales.SiteId);
                    if (availableQuantity < detail.Quantity)
                    {
                        transaction.Rollback();
                        throw new InvalidOperationException($"庫存不足: {detail.ConsumableId}，可用數量: {availableQuantity}，需求數量: {detail.Quantity}");
                    }
                }

                // 更新狀態
                sales.Status = "2";
                sales.ApprovedBy = userId;
                sales.ApprovedAt = DateTime.Now;
                sales.UpdatedBy = userId;
                sales.UpdatedAt = DateTime.Now;

                await _salesRepository.UpdateSalesAsync(sales);

                // 扣減庫存
                foreach (var detail in sales.Details)
                {
                    await _salesRepository.UpdateInventoryQuantityAsync(detail.ConsumableId, sales.SiteId, -detail.Quantity, transaction);
                }

                // 記錄異動
                foreach (var detail in sales.Details)
                {
                    var transactionRecord = new ConsumableTransaction
                    {
                        ConsumableId = detail.ConsumableId,
                        TransactionType = "5", // 5:出售
                        TransactionDate = sales.PurchaseDate,
                        Quantity = detail.Quantity,
                        UnitPrice = detail.UnitPrice,
                        Amount = detail.Amount,
                        SiteId = sales.SiteId,
                        SourceId = sales.TxnNo,
                        Notes = $"耗材出售單: {sales.TxnNo}",
                        CreatedBy = userId,
                        CreatedAt = DateTime.Now
                    };
                    await _transactionRepository.CreateTransactionAsync(transactionRecord);
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"審核耗材出售單失敗: {txnNo}", ex);
            throw;
        }
    }

    public async Task CancelSalesAsync(string txnNo, string userId)
    {
        try
        {
            var sales = await _salesRepository.GetSalesByTxnNoAsync(txnNo);
            if (sales == null)
            {
                throw new KeyNotFoundException($"耗材出售單不存在: {txnNo}");
            }

            if (sales.Status != "1")
            {
                throw new InvalidOperationException($"只有待審核狀態的耗材出售單才能取消: {txnNo}");
            }

            // 更新狀態
            sales.Status = "3";
            sales.UpdatedBy = userId;
            sales.UpdatedAt = DateTime.Now;

            await _salesRepository.UpdateSalesAsync(sales);
        }
        catch (Exception ex)
        {
            _logger.LogError($"取消耗材出售單失敗: {txnNo}", ex);
            throw;
        }
    }

    public async Task<string> GenerateTxnNoAsync(string siteId, DateTime date)
    {
        return await _salesRepository.GenerateTxnNoAsync(siteId, date);
    }

    private string GetStatusName(string status)
    {
        return status switch
        {
            "1" => "待審核",
            "2" => "已審核",
            "3" => "已取消",
            _ => status
        };
    }
}
