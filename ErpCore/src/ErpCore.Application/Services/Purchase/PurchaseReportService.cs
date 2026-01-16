using ErpCore.Application.DTOs.Purchase;
using ErpCore.Application.Services.Base;
using ErpCore.Infrastructure.Repositories.Purchase;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Purchase;

/// <summary>
/// 採購報表查詢服務實作 (SYSP410-SYSP4I0)
/// </summary>
public class PurchaseReportService : BaseService, IPurchaseReportService
{
    private readonly IPurchaseReportRepository _repository;

    public PurchaseReportService(
        IPurchaseReportRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<PurchaseReportResultDto>> QueryPurchaseReportsAsync(PurchaseReportQueryDto query)
    {
        try
        {
            var repositoryQuery = new PurchaseReportQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                ReportType = query.ReportType,
                Filters = query.Filters != null ? new PurchaseReportFilters
                {
                    OrderId = query.Filters.OrderId,
                    OrderType = query.Filters.OrderType,
                    ShopId = query.Filters.ShopId,
                    SupplierId = query.Filters.SupplierId,
                    Status = query.Filters.Status,
                    OrderDateFrom = query.Filters.OrderDateFrom,
                    OrderDateTo = query.Filters.OrderDateTo,
                    GoodsId = query.Filters.GoodsId,
                    ApplyUserId = query.Filters.ApplyUserId,
                    ApproveUserId = query.Filters.ApproveUserId
                } : null
            };

            var items = await _repository.QueryPurchaseReportsAsync(repositoryQuery);
            var totalCount = await _repository.GetPurchaseReportCountAsync(repositoryQuery);

            var dtos = items.Select(x => new PurchaseReportResultDto
            {
                OrderId = x.OrderId,
                OrderDate = x.OrderDate,
                OrderType = x.OrderType,
                OrderTypeName = x.OrderTypeName,
                ShopId = x.ShopId,
                ShopName = x.ShopName,
                SupplierId = x.SupplierId,
                SupplierName = x.SupplierName,
                Status = x.Status,
                StatusName = x.StatusName,
                TotalAmount = x.TotalAmount,
                TotalQty = x.TotalQty,
                ApplyUserId = x.ApplyUserId,
                ApplyUserName = x.ApplyUserName,
                ApplyDate = x.ApplyDate,
                ApproveUserId = x.ApproveUserId,
                ApproveUserName = x.ApproveUserName,
                ApproveDate = x.ApproveDate,
                ExpectedDate = x.ExpectedDate,
                Memo = x.Memo,
                SiteId = x.SiteId,
                OrgId = x.OrgId,
                CurrencyId = x.CurrencyId,
                ExchangeRate = x.ExchangeRate,
                DetailCount = x.DetailCount,
                TotalReceivedQty = x.TotalReceivedQty,
                TotalReturnQty = x.TotalReturnQty,
                TotalDetailAmount = x.TotalDetailAmount
            }).ToList();

            return new PagedResult<PurchaseReportResultDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢採購報表列表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<PurchaseReportDetailResultDto>> QueryPurchaseReportDetailsAsync(PurchaseReportQueryDto query)
    {
        try
        {
            var repositoryQuery = new PurchaseReportQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                ReportType = query.ReportType,
                Filters = query.Filters != null ? new PurchaseReportFilters
                {
                    OrderId = query.Filters.OrderId,
                    OrderType = query.Filters.OrderType,
                    ShopId = query.Filters.ShopId,
                    SupplierId = query.Filters.SupplierId,
                    Status = query.Filters.Status,
                    OrderDateFrom = query.Filters.OrderDateFrom,
                    OrderDateTo = query.Filters.OrderDateTo,
                    GoodsId = query.Filters.GoodsId,
                    ApplyUserId = query.Filters.ApplyUserId,
                    ApproveUserId = query.Filters.ApproveUserId
                } : null
            };

            var items = await _repository.QueryPurchaseReportDetailsAsync(repositoryQuery);
            var totalCount = await _repository.GetPurchaseReportDetailCountAsync(repositoryQuery);

            var dtos = items.Select(x => new PurchaseReportDetailResultDto
            {
                DetailId = x.DetailId,
                OrderId = x.OrderId,
                OrderDate = x.OrderDate,
                OrderType = x.OrderType,
                OrderTypeName = x.OrderTypeName,
                ShopId = x.ShopId,
                ShopName = x.ShopName,
                SupplierId = x.SupplierId,
                SupplierName = x.SupplierName,
                Status = x.Status,
                StatusName = x.StatusName,
                LineNum = x.LineNum,
                GoodsId = x.GoodsId,
                GoodsName = x.GoodsName,
                BarcodeId = x.BarcodeId,
                OrderQty = x.OrderQty,
                UnitPrice = x.UnitPrice,
                Amount = x.Amount,
                ReceivedQty = x.ReceivedQty,
                ReturnQty = x.ReturnQty,
                PendingQty = x.PendingQty,
                UnitId = x.UnitId,
                TaxRate = x.TaxRate,
                TaxAmount = x.TaxAmount,
                Memo = x.Memo,
                OrderTotalAmount = x.OrderTotalAmount
            }).ToList();

            return new PagedResult<PurchaseReportDetailResultDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢採購報表明細列表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportPurchaseReportAsync(PurchaseReportExportDto exportDto)
    {
        try
        {
            // TODO: 實作匯出功能（Excel、PDF、CSV）
            // 目前先返回空陣列，待整合 EPPlus/NPOI 套件後實作
            _logger.LogInfo($"匯出採購報表: {exportDto.ExportType}");
            
            await Task.CompletedTask;
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出採購報表失敗", ex);
            throw;
        }
    }
}
