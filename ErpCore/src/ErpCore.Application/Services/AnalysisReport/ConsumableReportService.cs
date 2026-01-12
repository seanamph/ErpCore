using ErpCore.Application.DTOs.AnalysisReport;
using ErpCore.Application.Services.Base;
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

    public ConsumableReportService(
        IConsumableReportRepository reportRepository,
        IConsumableTransactionRepository transactionRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _reportRepository = reportRepository;
        _transactionRepository = transactionRepository;
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

            // 根據匯出類型生成檔案
            if (exportDto.ExportType == "Excel")
            {
                // TODO: 實作 Excel 匯出邏輯
                // 可以使用 EPPlus 或 ClosedXML 等套件
                throw new NotImplementedException("Excel 匯出功能尚未實作");
            }
            else if (exportDto.ExportType == "PDF")
            {
                // TODO: 實作 PDF 匯出邏輯
                // 可以使用 iTextSharp 或 QuestPDF 等套件
                throw new NotImplementedException("PDF 匯出功能尚未實作");
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
                SiteName = null, // 需要從關聯表取得
                WarehouseId = x.WarehouseId,
                WarehouseName = null, // 需要從關聯表取得
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
