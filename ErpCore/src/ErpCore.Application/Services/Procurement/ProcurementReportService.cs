using ErpCore.Application.DTOs.Procurement;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Purchase;
using ErpCore.Infrastructure.Repositories.Purchase;
using ErpCore.Infrastructure.Repositories.Procurement;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Procurement;

/// <summary>
/// 採購報表服務實作 (SYSP410-SYSP4I0)
/// </summary>
public class ProcurementReportService : BaseService, IProcurementReportService
{
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly ExportHelper _exportHelper;

    public ProcurementReportService(
        IPurchaseOrderRepository purchaseOrderRepository,
        ISupplierRepository supplierRepository,
        ExportHelper exportHelper,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _supplierRepository = supplierRepository;
        _exportHelper = exportHelper;
    }

    public async Task<PagedResult<ProcurementReportDto>> QueryReportAsync(ProcurementReportQueryDto query)
    {
        try
        {
            _logger.LogInfo($"查詢採購報表: {query.ReportType}");

            // 建立查詢條件
            var repositoryQuery = new PurchaseOrderQuery
            {
                OrderId = query.PurchaseOrderNo,
                SupplierId = query.SupplierId,
                Status = query.Status,
                OrderDateFrom = query.PurchaseDateFrom,
                OrderDateTo = query.PurchaseDateTo,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

            // 查詢採購單
            var purchaseOrders = await _purchaseOrderRepository.QueryAsync(repositoryQuery);
            var totalCount = await _purchaseOrderRepository.GetCountAsync(repositoryQuery);

            // 轉換為報表 DTO
            var reportItems = new List<ProcurementReportDto>();
            foreach (var order in purchaseOrders)
            {
                // 查詢供應商名稱
                string? supplierName = null;
                if (!string.IsNullOrEmpty(order.SupplierId))
                {
                    var supplier = await _supplierRepository.GetByIdAsync(order.SupplierId);
                    supplierName = supplier?.SupplierName;
                }

                reportItems.Add(new ProcurementReportDto
                {
                    PurchaseOrderNo = order.OrderId,
                    PurchaseDate = order.OrderDate,
                    SupplierId = order.SupplierId,
                    SupplierName = supplierName,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status
                });
            }

            return new PagedResult<ProcurementReportDto>
            {
                Items = reportItems,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢採購報表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportReportAsync(ExportProcurementReportDto dto)
    {
        try
        {
            _logger.LogInfo($"匯出採購報表: {dto.ExportType}");

            // 查詢所有符合條件的資料（不分頁）
            var query = dto.Query;
            query.PageIndex = 1;
            query.PageSize = int.MaxValue;

            var result = await QueryReportAsync(query);

            // 定義匯出欄位
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "PurchaseOrderNo", DisplayName = "採購單號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "PurchaseDate", DisplayName = "採購日期", DataType = ExportDataType.Date },
                new ExportColumn { PropertyName = "SupplierId", DisplayName = "供應商代號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SupplierName", DisplayName = "供應商名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "TotalAmount", DisplayName = "總金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "Status", DisplayName = "狀態", DataType = ExportDataType.String }
            };

            // 根據匯出類型產生檔案
            if (dto.ExportType.ToUpper() == "EXCEL")
            {
                return _exportHelper.ExportToExcel(result.Items, columns, "採購報表", "採購報表");
            }
            else if (dto.ExportType.ToUpper() == "PDF")
            {
                return _exportHelper.ExportToPdf(result.Items, columns, "採購報表");
            }
            else
            {
                throw new ArgumentException($"不支援的匯出格式: {dto.ExportType}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出採購報表失敗", ex);
            throw;
        }
    }
}

