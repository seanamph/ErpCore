using ErpCore.Application.DTOs.Purchase;
using ErpCore.Application.Services.Base;
using ErpCore.Infrastructure.Repositories.Purchase;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Purchase;

/// <summary>
/// 採購單查詢服務實作 (SYSP310-SYSP330)
/// </summary>
public class PurchaseOrderQueryService : BaseService, IPurchaseOrderQueryService
{
    private readonly IPurchaseOrderQueryRepository _queryRepository;

    public PurchaseOrderQueryService(
        IPurchaseOrderQueryRepository queryRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _queryRepository = queryRepository;
    }

    public async Task<PagedResult<PurchaseOrderQueryResultDto>> QueryPurchaseOrdersAsync(PurchaseOrderQueryRequestDto request)
    {
        try
        {
            // 轉換 DTO 為 Repository 模型
            var repositoryRequest = new PurchaseOrderQueryRequest
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                SortField = request.SortField,
                SortOrder = request.SortOrder,
                Filters = request.Filters != null ? new PurchaseOrderQueryFilters
                {
                    OrderId = request.Filters.OrderId,
                    OrderType = request.Filters.OrderType,
                    ShopId = request.Filters.ShopId,
                    SupplierId = request.Filters.SupplierId,
                    Status = request.Filters.Status,
                    OrderDateFrom = request.Filters.OrderDateFrom,
                    OrderDateTo = request.Filters.OrderDateTo,
                    ApplyUserId = request.Filters.ApplyUserId,
                    ApproveUserId = request.Filters.ApproveUserId,
                    ExpectedDateFrom = request.Filters.ExpectedDateFrom,
                    ExpectedDateTo = request.Filters.ExpectedDateTo,
                    MinTotalAmount = request.Filters.MinTotalAmount,
                    MaxTotalAmount = request.Filters.MaxTotalAmount
                } : null
            };

            var items = await _queryRepository.QueryPurchaseOrdersAsync(repositoryRequest);
            var totalCount = await _queryRepository.GetPurchaseOrderCountAsync(repositoryRequest);

            // 轉換 Repository 模型為 DTO
            var itemDtos = items.Select(item => new PurchaseOrderQueryResultDto
            {
                OrderId = item.OrderId,
                OrderDate = item.OrderDate,
                OrderType = item.OrderType,
                OrderTypeName = item.OrderTypeName,
                ShopId = item.ShopId,
                ShopName = item.ShopName,
                SupplierId = item.SupplierId,
                SupplierName = item.SupplierName,
                Status = item.Status,
                StatusName = item.StatusName,
                TotalAmount = item.TotalAmount,
                TotalQty = item.TotalQty,
                ApplyUserId = item.ApplyUserId,
                ApplyUserName = item.ApplyUserName,
                ApplyDate = item.ApplyDate,
                ApproveUserId = item.ApproveUserId,
                ApproveUserName = item.ApproveUserName,
                ApproveDate = item.ApproveDate,
                ExpectedDate = item.ExpectedDate,
                DetailCount = item.DetailCount,
                TotalReceivedQty = item.TotalReceivedQty,
                TotalReturnQty = item.TotalReturnQty,
                Memo = item.Memo,
                SiteId = item.SiteId,
                OrgId = item.OrgId,
                CurrencyId = item.CurrencyId,
                ExchangeRate = item.ExchangeRate,
                SourceProgram = item.SourceProgram
            });

            return new PagedResult<PurchaseOrderQueryResultDto>
            {
                Items = itemDtos.ToList(),
                TotalCount = totalCount,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢採購單列表失敗", ex);
            throw;
        }
    }

    public async Task<PurchaseOrderDetailQueryDto> GetPurchaseOrderDetailsAsync(string orderId)
    {
        try
        {
            var details = await _queryRepository.GetPurchaseOrderDetailsAsync(orderId);

            // 轉換 Repository 模型為 DTO
            var detailDtos = details.Select(d => new PurchaseOrderDetailQueryItemDto
            {
                DetailId = d.DetailId,
                LineNum = d.LineNum,
                GoodsId = d.GoodsId,
                GoodsName = d.GoodsName,
                BarcodeId = d.BarcodeId,
                OrderQty = d.OrderQty,
                UnitPrice = d.UnitPrice,
                Amount = d.Amount,
                ReceivedQty = d.ReceivedQty,
                ReturnQty = d.ReturnQty,
                PendingQty = d.PendingQty,
                UnitId = d.UnitId,
                TaxRate = d.TaxRate,
                TaxAmount = d.TaxAmount,
                Memo = d.Memo
            });

            return new PurchaseOrderDetailQueryDto
            {
                OrderId = orderId,
                Details = detailDtos.ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢採購單明細失敗: {orderId}", ex);
            throw;
        }
    }

    public async Task<PurchaseOrderStatisticsDto> GetPurchaseOrderStatisticsAsync(PurchaseOrderStatisticsRequestDto request)
    {
        try
        {
            // 轉換 DTO 為 Repository 模型
            var repositoryRequest = new PurchaseOrderStatisticsRequest
            {
                GroupBy = request.GroupBy,
                DateFrom = request.DateFrom,
                DateTo = request.DateTo,
                ShopId = request.ShopId,
                SupplierId = request.SupplierId,
                Status = request.Status
            };

            var statistics = await _queryRepository.GetPurchaseOrderStatisticsAsync(repositoryRequest);

            // 轉換 Repository 模型為 DTO
            return new PurchaseOrderStatisticsDto
            {
                Summary = new PurchaseOrderStatisticsSummaryDto
                {
                    TotalOrders = statistics.Summary.TotalOrders,
                    TotalAmount = statistics.Summary.TotalAmount,
                    TotalQty = statistics.Summary.TotalQty,
                    AvgAmount = statistics.Summary.AvgAmount
                },
                Details = statistics.Details.Select(d => new PurchaseOrderStatisticsDetailDto
                {
                    GroupKey = d.GroupKey,
                    GroupName = d.GroupName,
                    OrderCount = d.OrderCount,
                    TotalAmount = d.TotalAmount,
                    TotalQty = d.TotalQty
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢採購單統計失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportPurchaseOrdersAsync(PurchaseOrderExportRequestDto request)
    {
        try
        {
            // TODO: 實作匯出功能（Excel/CSV/PDF）
            // 目前先返回空陣列，後續可整合 EPPlus 或 NPOI 等套件
            _logger.LogInfo($"匯出採購單查詢結果: {request.ExportType}");
            
            // 暫時返回空陣列，實際實作時需要：
            // 1. 查詢資料
            // 2. 根據 ExportType 生成對應格式
            // 3. 返回檔案位元組陣列
            
            await Task.CompletedTask;
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出採購單查詢結果失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> PrintPurchaseOrderAsync(string orderId)
    {
        try
        {
            // TODO: 實作列印功能（PDF）
            // 目前先返回空陣列，後續可整合 iTextSharp 或 QuestPDF 等套件
            _logger.LogInfo($"列印採購單: {orderId}");
            
            // 暫時返回空陣列，實際實作時需要：
            // 1. 查詢採購單資料
            // 2. 生成 PDF 格式
            // 3. 返回檔案位元組陣列
            
            await Task.CompletedTask;
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"列印採購單失敗: {orderId}", ex);
            throw;
        }
    }
}
