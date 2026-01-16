using Dapper;
using ErpCore.Application.DTOs.Inventory;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Inventory;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.Inventory;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Inventory;

/// <summary>
/// 商品永久變價服務實作
/// </summary>
public class PriceChangeService : BaseService, IPriceChangeService
{
    private readonly IPriceChangeRepository _repository;
    private readonly IDbConnectionFactory _connectionFactory;

    public PriceChangeService(
        IPriceChangeRepository repository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<PriceChangeDto>> GetPriceChangesAsync(PriceChangeQueryDto query)
    {
        try
        {
            var repositoryQuery = new PriceChangeQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                PriceChangeId = query.PriceChangeId,
                PriceChangeType = query.PriceChangeType,
                SupplierId = query.SupplierId,
                LogoId = query.LogoId,
                Status = query.Status,
                ApplyDateFrom = query.ApplyDateFrom,
                ApplyDateTo = query.ApplyDateTo,
                StartDateFrom = query.StartDateFrom,
                StartDateTo = query.StartDateTo
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => new PriceChangeDto
            {
                PriceChangeId = x.PriceChangeId,
                PriceChangeType = x.PriceChangeType,
                PriceChangeTypeName = x.PriceChangeType == "1" ? "進價" : "售價",
                SupplierId = x.SupplierId,
                LogoId = x.LogoId,
                ApplyEmpId = x.ApplyEmpId,
                ApplyOrgId = x.ApplyOrgId,
                ApplyDate = x.ApplyDate,
                StartDate = x.StartDate,
                ApproveEmpId = x.ApproveEmpId,
                ApproveDate = x.ApproveDate,
                ConfirmEmpId = x.ConfirmEmpId,
                ConfirmDate = x.ConfirmDate,
                Status = x.Status,
                StatusName = GetStatusName(x.Status),
                TotalAmount = x.TotalAmount,
                Notes = x.Notes,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return new PagedResult<PriceChangeDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢變價單列表失敗", ex);
            throw;
        }
    }

    public async Task<PriceChangeDetailDto> GetPriceChangeByIdAsync(string priceChangeId, string priceChangeType)
    {
        try
        {
            var master = await _repository.GetByIdAsync(priceChangeId, priceChangeType);
            if (master == null)
            {
                throw new InvalidOperationException($"變價單不存在: {priceChangeId}/{priceChangeType}");
            }

            var details = await _repository.GetDetailsAsync(priceChangeId, priceChangeType);

            var dto = new PriceChangeDetailDto
            {
                PriceChangeId = master.PriceChangeId,
                PriceChangeType = master.PriceChangeType,
                PriceChangeTypeName = master.PriceChangeType == "1" ? "進價" : "售價",
                SupplierId = master.SupplierId,
                LogoId = master.LogoId,
                ApplyEmpId = master.ApplyEmpId,
                ApplyOrgId = master.ApplyOrgId,
                ApplyDate = master.ApplyDate,
                StartDate = master.StartDate,
                ApproveEmpId = master.ApproveEmpId,
                ApproveDate = master.ApproveDate,
                ConfirmEmpId = master.ConfirmEmpId,
                ConfirmDate = master.ConfirmDate,
                Status = master.Status,
                StatusName = GetStatusName(master.Status),
                TotalAmount = master.TotalAmount,
                Notes = master.Notes,
                CreatedBy = master.CreatedBy,
                CreatedAt = master.CreatedAt,
                UpdatedBy = master.UpdatedBy,
                UpdatedAt = master.UpdatedAt,
                Details = details.Select(d => new PriceChangeDetailItemDto
                {
                    Id = d.Id,
                    LineNum = d.LineNum,
                    GoodsId = d.GoodsId,
                    BeforePrice = d.BeforePrice,
                    AfterPrice = d.AfterPrice,
                    ChangeQty = d.ChangeQty,
                    Notes = d.Notes
                }).ToList()
            };

            // 填充商品名稱
            await FillGoodsNamesAsync(dto.Details);

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢變價單失敗: {priceChangeId}/{priceChangeType}", ex);
            throw;
        }
    }

    public async Task<string> CreatePriceChangeAsync(CreatePriceChangeDto dto)
    {
        try
        {
            // 驗證資料
            ValidateCreateDto(dto);

            // 生成變價單號
            var priceChangeId = await GeneratePriceChangeIdAsync(dto.PriceChangeType);

            // 計算總金額
            var totalAmount = dto.Details.Sum(d => d.AfterPrice * (d.ChangeQty > 0 ? d.ChangeQty : 1));

            var master = new PriceChangeMaster
            {
                PriceChangeId = priceChangeId,
                PriceChangeType = dto.PriceChangeType,
                SupplierId = dto.SupplierId,
                LogoId = dto.LogoId,
                ApplyEmpId = dto.ApplyEmpId ?? GetCurrentUserId(),
                ApplyOrgId = dto.ApplyOrgId ?? GetCurrentOrgId(),
                ApplyDate = dto.ApplyDate ?? DateTime.Now,
                StartDate = dto.StartDate,
                Status = "1", // 已申請
                TotalAmount = totalAmount,
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(master);

            // 新增明細
            foreach (var detailDto in dto.Details)
            {
                // 取得商品當前價格
                var currentPrice = await GetCurrentPriceAsync(detailDto.GoodsId, dto.PriceChangeType);
                
                var detail = new PriceChangeDetail
                {
                    PriceChangeId = priceChangeId,
                    PriceChangeType = dto.PriceChangeType,
                    LineNum = detailDto.LineNum,
                    GoodsId = detailDto.GoodsId,
                    BeforePrice = detailDto.BeforePrice > 0 ? detailDto.BeforePrice : currentPrice,
                    AfterPrice = detailDto.AfterPrice,
                    ChangeQty = detailDto.ChangeQty,
                    Notes = detailDto.Notes,
                    CreatedBy = GetCurrentUserId(),
                    CreatedAt = DateTime.Now
                };

                await _repository.CreateDetailAsync(detail);
            }

            return priceChangeId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增變價單失敗", ex);
            throw;
        }
    }

    public async Task UpdatePriceChangeAsync(string priceChangeId, string priceChangeType, UpdatePriceChangeDto dto)
    {
        try
        {
            // 驗證資料
            ValidateUpdateDto(dto);

            // 檢查是否存在
            var master = await _repository.GetByIdAsync(priceChangeId, priceChangeType);
            if (master == null)
            {
                throw new InvalidOperationException($"變價單不存在: {priceChangeId}/{priceChangeType}");
            }

            // 只有狀態為「已申請」的變價單可以修改
            if (master.Status != "1")
            {
                throw new InvalidOperationException($"只有狀態為「已申請」的變價單可以修改，當前狀態: {GetStatusName(master.Status)}");
            }

            // 計算總金額
            var totalAmount = dto.Details.Sum(d => d.AfterPrice * (d.ChangeQty > 0 ? d.ChangeQty : 1));

            // 更新主檔
            master.SupplierId = dto.SupplierId;
            master.LogoId = dto.LogoId;
            master.ApplyEmpId = dto.ApplyEmpId;
            master.ApplyOrgId = dto.ApplyOrgId;
            master.ApplyDate = dto.ApplyDate;
            master.StartDate = dto.StartDate;
            master.TotalAmount = totalAmount;
            master.Notes = dto.Notes;
            master.UpdatedBy = GetCurrentUserId();
            master.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(master);

            // 刪除舊明細
            await _repository.DeleteDetailsAsync(priceChangeId, priceChangeType);

            // 新增新明細
            foreach (var detailDto in dto.Details)
            {
                // 取得商品當前價格
                var currentPrice = await GetCurrentPriceAsync(detailDto.GoodsId, priceChangeType);
                
                var detail = new PriceChangeDetail
                {
                    PriceChangeId = priceChangeId,
                    PriceChangeType = priceChangeType,
                    LineNum = detailDto.LineNum,
                    GoodsId = detailDto.GoodsId,
                    BeforePrice = detailDto.BeforePrice > 0 ? detailDto.BeforePrice : currentPrice,
                    AfterPrice = detailDto.AfterPrice,
                    ChangeQty = detailDto.ChangeQty,
                    Notes = detailDto.Notes,
                    CreatedBy = GetCurrentUserId(),
                    CreatedAt = DateTime.Now
                };

                await _repository.CreateDetailAsync(detail);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改變價單失敗: {priceChangeId}/{priceChangeType}", ex);
            throw;
        }
    }

    public async Task DeletePriceChangeAsync(string priceChangeId, string priceChangeType)
    {
        try
        {
            // 檢查是否存在
            var master = await _repository.GetByIdAsync(priceChangeId, priceChangeType);
            if (master == null)
            {
                throw new InvalidOperationException($"變價單不存在: {priceChangeId}/{priceChangeType}");
            }

            // 只有狀態為「已申請」的變價單可以刪除
            if (master.Status != "1")
            {
                throw new InvalidOperationException($"只有狀態為「已申請」的變價單可以刪除，當前狀態: {GetStatusName(master.Status)}");
            }

            await _repository.DeleteAsync(priceChangeId, priceChangeType);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除變價單失敗: {priceChangeId}/{priceChangeType}", ex);
            throw;
        }
    }

    public async Task ApprovePriceChangeAsync(string priceChangeId, string priceChangeType, ApprovePriceChangeDto dto)
    {
        try
        {
            // 檢查是否存在
            var master = await _repository.GetByIdAsync(priceChangeId, priceChangeType);
            if (master == null)
            {
                throw new InvalidOperationException($"變價單不存在: {priceChangeId}/{priceChangeType}");
            }

            // 只有狀態為「已申請」的變價單可以審核
            if (master.Status != "1")
            {
                throw new InvalidOperationException($"只有狀態為「已申請」的變價單可以審核，當前狀態: {GetStatusName(master.Status)}");
            }

            master.Status = "2"; // 已審核
            master.ApproveEmpId = GetCurrentUserId();
            master.ApproveDate = dto.ApproveDate ?? DateTime.Now;
            master.UpdatedBy = GetCurrentUserId();
            master.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(master);
        }
        catch (Exception ex)
        {
            _logger.LogError($"審核變價單失敗: {priceChangeId}/{priceChangeType}", ex);
            throw;
        }
    }

    public async Task ConfirmPriceChangeAsync(string priceChangeId, string priceChangeType, ConfirmPriceChangeDto dto)
    {
        try
        {
            // 檢查是否存在
            var master = await _repository.GetByIdAsync(priceChangeId, priceChangeType);
            if (master == null)
            {
                throw new InvalidOperationException($"變價單不存在: {priceChangeId}/{priceChangeType}");
            }

            // 只有狀態為「已審核」的變價單可以確認
            if (master.Status != "2")
            {
                throw new InvalidOperationException($"只有狀態為「已審核」的變價單可以確認，當前狀態: {GetStatusName(master.Status)}");
            }

            // 取得明細
            var details = await _repository.GetDetailsAsync(priceChangeId, priceChangeType);

            using var connection = _connectionFactory.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                // 更新商品主檔價格
                foreach (var detail in details)
                {
                    if (priceChangeType == "1") // 進價
                    {
                        const string updateSql = @"
                            UPDATE Products 
                            SET Lprc = @AfterPrice, UpdatedBy = @UpdatedBy, UpdatedAt = @UpdatedAt
                            WHERE GoodsId = @GoodsId";
                        
                        await connection.ExecuteAsync(updateSql, new
                        {
                            AfterPrice = detail.AfterPrice,
                            GoodsId = detail.GoodsId,
                            UpdatedBy = GetCurrentUserId(),
                            UpdatedAt = DateTime.Now
                        }, transaction);
                    }
                    else if (priceChangeType == "2") // 售價
                    {
                        const string updateSql = @"
                            UPDATE Products 
                            SET Mprc = @AfterPrice, UpdatedBy = @UpdatedBy, UpdatedAt = @UpdatedAt
                            WHERE GoodsId = @GoodsId";
                        
                        await connection.ExecuteAsync(updateSql, new
                        {
                            AfterPrice = detail.AfterPrice,
                            GoodsId = detail.GoodsId,
                            UpdatedBy = GetCurrentUserId(),
                            UpdatedAt = DateTime.Now
                        }, transaction);
                    }
                }

                // 更新變價單狀態
                master.Status = "10"; // 已確認
                master.ConfirmEmpId = GetCurrentUserId();
                master.ConfirmDate = dto.ConfirmDate ?? DateTime.Now;
                master.UpdatedBy = GetCurrentUserId();
                master.UpdatedAt = DateTime.Now;

                const string updateMasterSql = @"
                    UPDATE PriceChangeMasters SET
                        Status = @Status,
                        ConfirmEmpId = @ConfirmEmpId,
                        ConfirmDate = @ConfirmDate,
                        UpdatedBy = @UpdatedBy,
                        UpdatedAt = @UpdatedAt
                    WHERE PriceChangeId = @PriceChangeId AND PriceChangeType = @PriceChangeType";

                await connection.ExecuteAsync(updateMasterSql, master, transaction);

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
            _logger.LogError($"確認變價單失敗: {priceChangeId}/{priceChangeType}", ex);
            throw;
        }
    }

    public async Task CancelPriceChangeAsync(string priceChangeId, string priceChangeType)
    {
        try
        {
            // 檢查是否存在
            var master = await _repository.GetByIdAsync(priceChangeId, priceChangeType);
            if (master == null)
            {
                throw new InvalidOperationException($"變價單不存在: {priceChangeId}/{priceChangeType}");
            }

            // 已作廢的變價單不能再作廢
            if (master.Status == "9")
            {
                throw new InvalidOperationException("變價單已作廢，無法再次作廢");
            }

            // 已確認的變價單不能作廢
            if (master.Status == "10")
            {
                throw new InvalidOperationException("變價單已確認，無法作廢");
            }

            master.Status = "9"; // 已作廢
            master.UpdatedBy = GetCurrentUserId();
            master.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(master);
        }
        catch (Exception ex)
        {
            _logger.LogError($"作廢變價單失敗: {priceChangeId}/{priceChangeType}", ex);
            throw;
        }
    }

    private string GetStatusName(string status)
    {
        return status switch
        {
            "1" => "已申請",
            "2" => "已審核",
            "9" => "已作廢",
            "10" => "已確認",
            _ => "未知"
        };
    }

    private async Task<string> GeneratePriceChangeIdAsync(string priceChangeType)
    {
        try
        {
            // 生成變價單號格式: PC{類型}{YYYYMMDD}{流水號}
            var prefix = $"PC{priceChangeType}{DateTime.Now:yyyyMMdd}";
            var sql = @"
                SELECT ISNULL(MAX(CAST(SUBSTRING(PriceChangeId, LEN(@Prefix) + 1, LEN(PriceChangeId)) AS INT)), 0) + 1
                FROM PriceChangeMasters
                WHERE PriceChangeId LIKE @Prefix + '%'";

            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("Prefix", prefix);

            var sequence = await connection.ExecuteScalarAsync<int>(sql, parameters);
            return $"{prefix}{sequence:D3}";
        }
        catch (Exception ex)
        {
            _logger.LogError("生成變價單號失敗", ex);
            throw;
        }
    }

    private async Task<decimal> GetCurrentPriceAsync(string goodsId, string priceChangeType)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            
            if (priceChangeType == "1") // 進價
            {
                const string sql = "SELECT Lprc FROM Products WHERE GoodsId = @GoodsId";
                var price = await connection.QueryFirstOrDefaultAsync<decimal?>(sql, new { GoodsId = goodsId });
                return price ?? 0;
            }
            else // 售價
            {
                const string sql = "SELECT Mprc FROM Products WHERE GoodsId = @GoodsId";
                var price = await connection.QueryFirstOrDefaultAsync<decimal?>(sql, new { GoodsId = goodsId });
                return price ?? 0;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"取得商品當前價格失敗: {goodsId}", ex);
            throw;
        }
    }

    private async Task FillGoodsNamesAsync(List<PriceChangeDetailItemDto> details)
    {
        try
        {
            if (details.Count == 0) return;

            using var connection = _connectionFactory.CreateConnection();
            var goodsIds = details.Select(d => d.GoodsId).Distinct().ToList();
            
            const string sql = "SELECT GoodsId, GoodsName FROM Products WHERE GoodsId IN @GoodsIds";
            var goods = await connection.QueryAsync<(string GoodsId, string GoodsName)>(sql, new { GoodsIds = goodsIds });

            var goodsDict = goods.ToDictionary(g => g.GoodsId, g => g.GoodsName);

            foreach (var detail in details)
            {
                if (goodsDict.TryGetValue(detail.GoodsId, out var goodsName))
                {
                    detail.GoodsName = goodsName;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("填充商品名稱失敗", ex);
            // 不拋出異常，只記錄日誌
        }
    }

    private void ValidateCreateDto(CreatePriceChangeDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.PriceChangeType))
        {
            throw new ArgumentException("變價類型不能為空");
        }

        if (dto.PriceChangeType != "1" && dto.PriceChangeType != "2")
        {
            throw new ArgumentException("變價類型必須為 1(進價) 或 2(售價)");
        }

        if (dto.Details == null || dto.Details.Count == 0)
        {
            throw new ArgumentException("變價明細不能為空");
        }

        foreach (var detail in dto.Details)
        {
            if (string.IsNullOrWhiteSpace(detail.GoodsId))
            {
                throw new ArgumentException("商品編號不能為空");
            }

            if (detail.AfterPrice <= 0)
            {
                throw new ArgumentException("調整後單價必須大於0");
            }
        }
    }

    private void ValidateUpdateDto(UpdatePriceChangeDto dto)
    {
        if (dto.Details == null || dto.Details.Count == 0)
        {
            throw new ArgumentException("變價明細不能為空");
        }

        foreach (var detail in dto.Details)
        {
            if (string.IsNullOrWhiteSpace(detail.GoodsId))
            {
                throw new ArgumentException("商品編號不能為空");
            }

            if (detail.AfterPrice <= 0)
            {
                throw new ArgumentException("調整後單價必須大於0");
            }
        }
    }

}
