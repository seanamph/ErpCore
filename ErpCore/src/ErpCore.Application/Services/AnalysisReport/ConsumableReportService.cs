using Dapper;
using ErpCore.Application.DTOs.AnalysisReport;
using ErpCore.Application.Services.Base;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.AnalysisReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.AnalysisReport;

/// <summary>
/// 耗材管理報表服務實作 (SYSA255)
/// </summary>
public class ConsumableReportService : BaseService, IConsumableReportService
{
    private readonly IConsumableReportRepository _reportRepository;
    private readonly IConsumableTransactionRepository _transactionRepository;
    private readonly ExportHelper _exportHelper;
    private readonly IDbConnectionFactory _connectionFactory;

    public ConsumableReportService(
        IConsumableReportRepository reportRepository,
        IConsumableTransactionRepository transactionRepository,
        ExportHelper exportHelper,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _reportRepository = reportRepository;
        _transactionRepository = transactionRepository;
        _exportHelper = exportHelper;
        _connectionFactory = connectionFactory;
    }

    public async Task<ConsumableReportResponseDto> GetReportAsync(ConsumableReportQueryDto query)
    {
        try
        {
            var repositoryQuery = new ConsumableReportQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                ConsumableId = query.ConsumableId,
                ConsumableName = query.ConsumableName,
                CategoryId = query.CategoryId,
                SiteIds = query.SiteIds,
                WarehouseIds = query.WarehouseIds,
                Status = query.Status,
                AssetStatus = query.AssetStatus,
                DateFrom = query.DateFrom,
                DateTo = query.DateTo,
                ReportType = query.ReportType,
                SortField = query.SortField,
                SortOrder = query.SortOrder
            };

            var reportData = await _reportRepository.GetReportDataAsync(repositoryQuery);
            var summary = await _reportRepository.GetReportSummaryAsync(repositoryQuery);

            var items = reportData.Items.Select(x => new ConsumableReportItemDto
            {
                ConsumableId = x.ConsumableId,
                ConsumableName = x.ConsumableName,
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName,
                SiteId = x.SiteId,
                SiteName = x.SiteName,
                WarehouseId = x.WarehouseId,
                WarehouseName = x.WarehouseName,
                Unit = x.Unit,
                Specification = x.Specification,
                Brand = x.Brand,
                Model = x.Model,
                BarCode = x.BarCode,
                Status = x.Status,
                StatusName = x.StatusName,
                AssetStatus = x.AssetStatus,
                AssetStatusName = x.AssetStatusName,
                Location = x.Location,
                Quantity = x.Quantity,
                MinQuantity = x.MinQuantity,
                MaxQuantity = x.MaxQuantity,
                Price = x.Price,
                CurrentQty = x.CurrentQty,
                CurrentAmt = x.CurrentAmt,
                InQty = x.InQty,
                OutQty = x.OutQty,
                InAmt = x.InAmt,
                OutAmt = x.OutAmt,
                IsLowStock = x.IsLowStock,
                IsOverStock = x.IsOverStock
            }).ToList();

            var totalPages = (int)Math.Ceiling((double)reportData.TotalCount / query.PageSize);

            return new ConsumableReportResponseDto
            {
                Items = items,
                TotalCount = reportData.TotalCount,
                PageIndex = reportData.PageIndex,
                PageSize = reportData.PageSize,
                TotalPages = totalPages,
                Summary = new ConsumableReportSummaryDto
                {
                    TotalConsumables = summary.TotalConsumables,
                    TotalCurrentQty = summary.TotalCurrentQty,
                    TotalCurrentAmt = summary.TotalCurrentAmt,
                    TotalInQty = summary.TotalInQty,
                    TotalOutQty = summary.TotalOutQty,
                    TotalInAmt = summary.TotalInAmt,
                    TotalOutAmt = summary.TotalOutAmt
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢耗材管理報表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportReportAsync(ConsumableReportExportDto exportDto)
    {
        try
        {
            // 查詢所有符合條件的資料（不分頁）
            var query = exportDto.Filters ?? new ConsumableReportQueryDto();
            query.PageIndex = 1;
            query.PageSize = int.MaxValue;

            var reportData = await GetReportAsync(query);

            // 定義匯出欄位
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "ConsumableId", DisplayName = "耗材編號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ConsumableName", DisplayName = "耗材名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "CategoryName", DisplayName = "分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SiteName", DisplayName = "店別", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "WarehouseName", DisplayName = "庫別", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Unit", DisplayName = "單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Specification", DisplayName = "規格", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Brand", DisplayName = "品牌", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Model", DisplayName = "型號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "BarCode", DisplayName = "條碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "StatusName", DisplayName = "狀態", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "AssetStatusName", DisplayName = "資產狀態", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Location", DisplayName = "位置", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "CurrentQty", DisplayName = "當前庫存數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "CurrentAmt", DisplayName = "當前庫存金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "InQty", DisplayName = "入庫數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "OutQty", DisplayName = "出庫數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "InAmt", DisplayName = "入庫金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "OutAmt", DisplayName = "出庫金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "MinQuantity", DisplayName = "最小庫存量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "MaxQuantity", DisplayName = "最大庫存量", DataType = ExportDataType.Decimal }
            };

            // 根據匯出類型生成檔案
            if (exportDto.ExportType == "Excel")
            {
                return _exportHelper.ExportToExcel(reportData.Items, columns, "耗材管理報表", "耗材管理報表 (SYSA255)");
            }
            else if (exportDto.ExportType == "PDF")
            {
                return _exportHelper.ExportToPdf(reportData.Items, columns, "耗材管理報表 (SYSA255)");
            }
            else
            {
                throw new ArgumentException($"不支援的匯出類型: {exportDto.ExportType}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出耗材管理報表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<ConsumableTransactionDto>> GetTransactionsAsync(ConsumableTransactionQueryDto query)
    {
        try
        {
            var repositoryQuery = new ConsumableTransactionQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                ConsumableId = query.ConsumableId,
                DateFrom = query.DateFrom,
                DateTo = query.DateTo,
                TransactionType = query.TransactionType,
                SortField = query.SortField,
                SortOrder = query.SortOrder
            };

            var result = await _transactionRepository.GetTransactionsAsync(repositoryQuery);

            // 批量查詢 SiteName
            var siteIds = result.Items.Where(x => !string.IsNullOrEmpty(x.SiteId))
                .Select(x => x.SiteId!)
                .Distinct()
                .ToList();
            var siteNameMap = new Dictionary<string, string>();
            if (siteIds.Any())
            {
                using var connection = _connectionFactory.CreateConnection();
                var sql = @"
                    SELECT ShopId, ShopName FROM Shops WHERE ShopId IN @SiteIds
                    UNION
                    SELECT SiteId, SiteName FROM Sites WHERE SiteId IN @SiteIds";
                var sites = await connection.QueryAsync<(string SiteId, string SiteName)>(sql, new { SiteIds = siteIds });
                siteNameMap = sites.ToDictionary(x => x.SiteId, x => x.SiteName);
            }

            // 批量查詢 WarehouseName
            var warehouseKeys = result.Items
                .Where(x => !string.IsNullOrEmpty(x.SiteId) && !string.IsNullOrEmpty(x.WarehouseId))
                .Select(x => (x.SiteId!, x.WarehouseId!))
                .Distinct()
                .ToList();
            var warehouseNameMap = new Dictionary<(string SiteId, string WarehouseId), string>();
            if (warehouseKeys.Any())
            {
                using var connection = _connectionFactory.CreateConnection();
                var sql = "SELECT SiteId, WarehouseId, WarehouseName FROM Warehouses WHERE (SiteId, WarehouseId) IN @WarehouseKeys";
                var warehouses = await connection.QueryAsync<(string SiteId, string WarehouseId, string WarehouseName)>(
                    sql, 
                    new { WarehouseKeys = warehouseKeys.Select(k => new { k.Item1, k.Item2 }).ToList() });
                warehouseNameMap = warehouses.ToDictionary(x => (x.SiteId, x.WarehouseId), x => x.WarehouseName);
            }

            var dtos = result.Items.Select(x => new ConsumableTransactionDto
            {
                TransactionId = x.TransactionId,
                ConsumableId = x.ConsumableId,
                TransactionType = x.TransactionType,
                TransactionTypeName = GetTransactionTypeName(x.TransactionType),
                TransactionDate = x.TransactionDate,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice,
                Amount = x.Amount,
                SiteId = x.SiteId,
                SiteName = !string.IsNullOrEmpty(x.SiteId) && siteNameMap.TryGetValue(x.SiteId, out var siteName) ? siteName : null,
                WarehouseId = x.WarehouseId,
                WarehouseName = !string.IsNullOrEmpty(x.SiteId) && !string.IsNullOrEmpty(x.WarehouseId) 
                    && warehouseNameMap.TryGetValue((x.SiteId, x.WarehouseId), out var warehouseName) 
                    ? warehouseName : null,
                SourceId = x.SourceId,
                Notes = x.Notes,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt
            }).ToList();

            return new PagedResult<ConsumableTransactionDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢耗材使用明細失敗", ex);
            throw;
        }
    }

    private string GetTransactionTypeName(string transactionType)
    {
        return transactionType switch
        {
            "1" => "入庫",
            "2" => "出庫",
            "3" => "退貨",
            "4" => "報廢",
            "5" => "出售",
            "6" => "領用",
            _ => transactionType
        };
    }
}
