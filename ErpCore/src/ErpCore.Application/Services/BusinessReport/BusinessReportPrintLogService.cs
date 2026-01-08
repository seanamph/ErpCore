using System.Text.Json;
using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.BusinessReport;
using ErpCore.Infrastructure.Repositories.BusinessReport;
using ErpCore.Infrastructure.Services.FileStorage;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.BusinessReport;

/// <summary>
/// 業務報表列印記錄服務實作 (SYSL161)
/// </summary>
public class BusinessReportPrintLogService : BaseService, IBusinessReportPrintLogService
{
    private readonly IBusinessReportPrintLogRepository _repository;
    private readonly ExportHelper _exportHelper;
    private readonly IFileStorageService _fileStorageService;
    private readonly IBusinessReportService _businessReportService;
    private readonly IEmployeeMealCardReportService _employeeMealCardReportService;
    private readonly IReturnCardReportService _returnCardReportService;
    private readonly IOvertimePaymentReportService _overtimePaymentReportService;

    public BusinessReportPrintLogService(
        IBusinessReportPrintLogRepository repository,
        ExportHelper exportHelper,
        IFileStorageService fileStorageService,
        IBusinessReportService businessReportService,
        IEmployeeMealCardReportService employeeMealCardReportService,
        IReturnCardReportService returnCardReportService,
        IOvertimePaymentReportService overtimePaymentReportService,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _exportHelper = exportHelper;
        _fileStorageService = fileStorageService;
        _businessReportService = businessReportService;
        _employeeMealCardReportService = employeeMealCardReportService;
        _returnCardReportService = returnCardReportService;
        _overtimePaymentReportService = overtimePaymentReportService;
    }

    public async Task<PagedResult<BusinessReportPrintLogDto>> GetBusinessReportPrintLogsAsync(BusinessReportPrintLogQueryDto query)
    {
        try
        {
            var repositoryQuery = new BusinessReportPrintLogQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                ReportId = query.Filters.ReportId,
                ReportName = query.Filters.ReportName,
                PrintUserId = query.Filters.PrintUserId,
                PrintDateFrom = query.Filters.PrintDateFrom,
                PrintDateTo = query.Filters.PrintDateTo,
                Status = query.Filters.Status
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<BusinessReportPrintLogDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢業務報表列印記錄列表失敗", ex);
            throw;
        }
    }

    public async Task<BusinessReportPrintLogDto?> GetBusinessReportPrintLogByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                return null;
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢業務報表列印記錄失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<List<BusinessReportPrintLogDto>> GetBusinessReportPrintLogsByReportIdAsync(string reportId)
    {
        try
        {
            var entities = await _repository.GetByReportIdAsync(reportId);
            return entities.Select(x => MapToDto(x)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢業務報表列印記錄失敗: ReportId={reportId}", ex);
            throw;
        }
    }

    public async Task<BusinessReportPrintResultDto> PrintReportAsync(string reportId, BusinessReportPrintRequestDto request)
    {
        try
        {
            _logger.LogInfo($"列印業務報表: ReportId={reportId}, Format={request.PrintFormat}");

            // 驗證列印格式（列印僅支援 PDF）
            if (!request.PrintFormat.Equals("PDF", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("列印僅支援 PDF 格式");
            }

            // 根據 reportId 獲取報表資料和名稱
            var (reportData, reportName, reportType) = await GetReportDataAsync(reportId, request.PrintParams);

            // 建立列印記錄
            var logEntity = new BusinessReportPrintLog
            {
                ReportId = reportId,
                ReportName = reportName,
                ReportType = reportType,
                PrintDate = DateTime.Now,
                PrintUserId = GetCurrentUserId(),
                PrintUserName = GetCurrentUserName(),
                PrintParams = request.PrintParams != null ? JsonSerializer.Serialize(request.PrintParams) : null,
                PrintFormat = request.PrintFormat,
                Status = "1",
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            try
            {
                // 定義列印欄位
                var columns = GetExportColumnsForReport(reportId, reportType);

                // 建立列印標題（包含日期時間）
                var printTitle = $"{reportName}";
                if (request.PrintParams != null && request.PrintParams.ContainsKey("IncludeDateTime") && 
                    request.PrintParams["IncludeDateTime"] is bool includeDateTime && includeDateTime)
                {
                    printTitle += $" ({DateTime.Now:yyyy/MM/dd HH:mm:ss})";
                }

                // 產生 PDF 檔案
                var fileBytes = _exportHelper.ExportToPdf(reportData, columns, printTitle);
                var fileName = $"{reportName}_{DateTime.Now:yyyyMMddHHmmss}.pdf";

                // 保存檔案到檔案系統
                var filePath = await _fileStorageService.SaveFileAsync(fileBytes, fileName, "PrintLogs");
                
                // 更新記錄的檔案路徑
                logEntity.FilePath = filePath;
                logEntity.Status = "1";
                var logTKey = await _repository.CreateAsync(logEntity);

                // 建立下載 URL
                var fileUrl = $"/api/v1/business-report-print-logs/{logTKey}/download";

                return new BusinessReportPrintResultDto
                {
                    PrintLogId = logTKey,
                    FileUrl = fileUrl,
                    FileName = fileName
                };
            }
            catch (Exception ex)
            {
                logEntity.Status = "0";
                logEntity.ErrorMessage = ex.Message;
                await _repository.CreateAsync(logEntity);
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"列印業務報表失敗: ReportId={reportId}", ex);
            throw;
        }
    }

    public async Task<object> PreviewReportAsync(string reportId, BusinessReportPrintRequestDto request)
    {
        try
        {
            _logger.LogInfo($"預覽業務報表: ReportId={reportId}");

            // 根據 reportId 獲取報表資料和名稱
            var (reportData, reportName, reportType) = await GetReportDataAsync(reportId, request.PrintParams);

            // 返回預覽資料（僅返回前 100 筆）
            var previewData = new
            {
                ReportId = reportId,
                ReportName = reportName,
                ReportType = reportType,
                TotalCount = reportData.Count,
                PreviewCount = Math.Min(100, reportData.Count),
                Data = reportData.Take(100).ToList()
            };

            return previewData;
        }
        catch (Exception ex)
        {
            _logger.LogError($"預覽業務報表失敗: ReportId={reportId}", ex);
            throw;
        }
    }

    public async Task<BusinessReportPrintResultDto> ExportReportAsync(string reportId, BusinessReportExportRequestDto request)
    {
        try
        {
            _logger.LogInfo($"匯出業務報表: ReportId={reportId}, Format={request.ExportFormat}");

            // 驗證匯出格式
            if (!request.ExportFormat.Equals("Excel", StringComparison.OrdinalIgnoreCase) && 
                !request.ExportFormat.Equals("PDF", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"不支援的匯出格式: {request.ExportFormat}");
            }

            // 根據 reportId 獲取報表資料和名稱
            var (reportData, reportName, reportType) = await GetReportDataAsync(reportId, request.ExportParams);

            // 建立列印記錄
            var logEntity = new BusinessReportPrintLog
            {
                ReportId = reportId,
                ReportName = reportName,
                ReportType = reportType,
                PrintDate = DateTime.Now,
                PrintUserId = GetCurrentUserId(),
                PrintUserName = GetCurrentUserName(),
                PrintParams = request.ExportParams != null ? JsonSerializer.Serialize(request.ExportParams) : null,
                PrintFormat = request.ExportFormat,
                Status = "1",
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            try
            {
                // 定義匯出欄位
                var columns = GetExportColumnsForReport(reportId, reportType);

                // 產生匯出檔案
                byte[] fileBytes;
                string fileExtension = request.ExportFormat.ToLower();
                
                if (request.ExportFormat.Equals("Excel", StringComparison.OrdinalIgnoreCase))
                {
                    fileBytes = _exportHelper.ExportToExcel(reportData, columns, reportName, reportName);
                }
                else
                {
                    fileBytes = _exportHelper.ExportToPdf(reportData, columns, reportName);
                }

                var fileName = $"{reportName}_{DateTime.Now:yyyyMMddHHmmss}.{fileExtension}";

                // 保存檔案到檔案系統
                var filePath = await _fileStorageService.SaveFileAsync(fileBytes, fileName, "PrintLogs");
                
                // 更新記錄的檔案路徑
                logEntity.FilePath = filePath;
                logEntity.Status = "1";
                var logTKey = await _repository.CreateAsync(logEntity);

                // 建立下載 URL
                var fileUrl = $"/api/v1/business-report-print-logs/{logTKey}/download";

                return new BusinessReportPrintResultDto
                {
                    PrintLogId = logTKey,
                    FileUrl = fileUrl,
                    FileName = fileName
                };
            }
            catch (Exception ex)
            {
                logEntity.Status = "0";
                logEntity.ErrorMessage = ex.Message;
                await _repository.CreateAsync(logEntity);
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"匯出業務報表失敗: ReportId={reportId}", ex);
            throw;
        }
    }

    public async Task<bool> DeleteBusinessReportPrintLogAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new Exception($"找不到業務報表列印記錄: {tKey}");
            }

            // 刪除檔案
            if (!string.IsNullOrEmpty(entity.FilePath))
            {
                try
                {
                    await _fileStorageService.DeleteFileAsync(entity.FilePath);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"刪除檔案失敗: {entity.FilePath}", ex);
                    // 檔案刪除失敗不影響記錄刪除
                }
            }

            return await _repository.DeleteAsync(tKey);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除業務報表列印記錄失敗: {tKey}", ex);
            throw;
        }
    }

    /// <summary>
    /// 取得檔案位元組陣列（用於下載）
    /// </summary>
    public async Task<byte[]> GetFileBytesAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new Exception($"找不到業務報表列印記錄: {tKey}");
            }

            if (string.IsNullOrEmpty(entity.FilePath))
            {
                throw new Exception($"檔案路徑不存在: {tKey}");
            }

            return await _fileStorageService.ReadFileAsync(entity.FilePath);
        }
        catch (Exception ex)
        {
            _logger.LogError($"讀取檔案失敗: {tKey}", ex);
            throw;
        }
    }

    /// <summary>
    /// 取得檔案名稱
    /// </summary>
    public async Task<string> GetFileNameAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new Exception($"找不到業務報表列印記錄: {tKey}");
            }

            if (string.IsNullOrEmpty(entity.FilePath))
            {
                throw new Exception($"檔案路徑不存在: {tKey}");
            }

            // 從檔案路徑中提取檔案名稱
            var fileName = Path.GetFileName(entity.FilePath);
            // 移除時間戳和 GUID 前綴（格式：yyyyMMddHHmmss_{Guid}_{原檔名}）
            if (fileName.Contains('_'))
            {
                var parts = fileName.Split('_');
                if (parts.Length >= 3)
                {
                    fileName = string.Join("_", parts.Skip(2));
                }
            }

            return fileName;
        }
        catch (Exception ex)
        {
            _logger.LogError($"取得檔案名稱失敗: {tKey}", ex);
            throw;
        }
    }

    /// <summary>
    /// 根據 reportId 獲取報表資料
    /// </summary>
    private async Task<(List<object> Data, string ReportName, string? ReportType)> GetReportDataAsync(
        string reportId, 
        Dictionary<string, object>? parameters)
    {
        try
        {
            // 根據 reportId 判斷報表類型
            if (reportId.StartsWith("SYSL135", StringComparison.OrdinalIgnoreCase))
            {
                // 業務報表查詢
                var query = BuildBusinessReportQuery(parameters);
                var result = await _businessReportService.GetBusinessReportsAsync(query);
                var data = result.Items.Cast<object>().ToList();
                return (data, "業務報表", "SYSL135");
            }
            else if (reportId.StartsWith("SYSL210", StringComparison.OrdinalIgnoreCase))
            {
                // 員餐卡報表
                var reportType = reportId.Replace("SYSL210", "");
                var query = BuildEmployeeMealCardReportQuery(parameters);
                query.ReportType = reportType;
                var result = await _employeeMealCardReportService.GetReportsAsync(query);
                var data = result.Items.Cast<object>().ToList();
                return (data, $"員餐卡報表-{reportType}", "SYSL210");
            }
            else if (reportId.StartsWith("SYSL310", StringComparison.OrdinalIgnoreCase))
            {
                // 銷退卡報表
                var query = BuildReturnCardReportQuery(parameters);
                var result = await _returnCardReportService.GetReportAsync(query);
                var data = result.Items.Cast<object>().ToList();
                return (data, "銷退卡報表", "SYSL310");
            }
            else if (reportId.StartsWith("SYSL510", StringComparison.OrdinalIgnoreCase))
            {
                // 加班發放報表
                var query = BuildOvertimePaymentReportQuery(parameters);
                var result = await _overtimePaymentReportService.GetReportAsync(query);
                var data = result.Items.Cast<object>().ToList();
                return (data, "加班發放報表", "SYSL510");
            }
            else
            {
                throw new ArgumentException($"不支援的報表類型: {reportId}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"獲取報表資料失敗: ReportId={reportId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 建立業務報表查詢條件
    /// </summary>
    private BusinessReportQueryDto BuildBusinessReportQuery(Dictionary<string, object>? parameters)
    {
        var query = new BusinessReportQueryDto
        {
            PageIndex = 1,
            PageSize = int.MaxValue
        };

        if (parameters != null)
        {
            if (parameters.ContainsKey("SiteId") && parameters["SiteId"] is string siteId)
                query.SiteId = siteId;
            if (parameters.ContainsKey("CardType") && parameters["CardType"] is string cardType)
                query.CardType = cardType;
            if (parameters.ContainsKey("VendorId") && parameters["VendorId"] is string vendorId)
                query.VendorId = vendorId;
            if (parameters.ContainsKey("StoreId") && parameters["StoreId"] is string storeId)
                query.StoreId = storeId;
            if (parameters.ContainsKey("OrgId") && parameters["OrgId"] is string orgId)
                query.OrgId = orgId;
            if (parameters.ContainsKey("StartDate") && parameters["StartDate"] is DateTime startDate)
                query.StartDate = startDate;
            if (parameters.ContainsKey("EndDate") && parameters["EndDate"] is DateTime endDate)
                query.EndDate = endDate;
        }

        return query;
    }

    /// <summary>
    /// 建立員餐卡報表查詢條件
    /// </summary>
    private EmployeeMealCardReportQueryDto BuildEmployeeMealCardReportQuery(Dictionary<string, object>? parameters)
    {
        var query = new EmployeeMealCardReportQueryDto
        {
            PageIndex = 1,
            PageSize = int.MaxValue
        };

        if (parameters != null)
        {
            if (parameters.ContainsKey("SiteId") && parameters["SiteId"] is string siteId)
                query.SiteId = siteId;
            if (parameters.ContainsKey("OrgId") && parameters["OrgId"] is string orgId)
                query.OrgId = orgId;
            if (parameters.ContainsKey("YearMonth") && parameters["YearMonth"] is string yearMonth)
                query.YearMonth = yearMonth;
        }

        return query;
    }

    /// <summary>
    /// 建立銷退卡報表查詢條件
    /// </summary>
    private ReturnCardReportQueryDto BuildReturnCardReportQuery(Dictionary<string, object>? parameters)
    {
        var query = new ReturnCardReportQueryDto
        {
            PageIndex = 1,
            PageSize = int.MaxValue
        };

        if (parameters != null)
        {
            if (parameters.ContainsKey("SiteId") && parameters["SiteId"] is string siteId)
                query.SiteId = siteId;
            if (parameters.ContainsKey("OrgId") && parameters["OrgId"] is string orgId)
                query.OrgId = orgId;
            if (parameters.ContainsKey("StartDate") && parameters["StartDate"] is DateTime startDate)
                query.StartDate = startDate;
            if (parameters.ContainsKey("EndDate") && parameters["EndDate"] is DateTime endDate)
                query.EndDate = endDate;
        }

        return query;
    }

    /// <summary>
    /// 建立加班發放報表查詢條件
    /// </summary>
    private OvertimePaymentReportQueryDto BuildOvertimePaymentReportQuery(Dictionary<string, object>? parameters)
    {
        var query = new OvertimePaymentReportQueryDto
        {
            PageIndex = 1,
            PageSize = int.MaxValue
        };

        if (parameters != null)
        {
            if (parameters.ContainsKey("StartDate") && parameters["StartDate"] is DateTime startDate)
                query.StartDate = startDate;
            if (parameters.ContainsKey("EndDate") && parameters["EndDate"] is DateTime endDate)
                query.EndDate = endDate;
            if (parameters.ContainsKey("DepartmentId") && parameters["DepartmentId"] is string departmentId)
                query.DepartmentId = departmentId;
            if (parameters.ContainsKey("EmployeeId") && parameters["EmployeeId"] is string employeeId)
                query.EmployeeId = employeeId;
        }

        return query;
    }

    /// <summary>
    /// 根據報表類型取得匯出欄位定義
    /// </summary>
    private List<ExportColumn> GetExportColumnsForReport(string reportId, string? reportType)
    {
        if (reportId.StartsWith("SYSL135", StringComparison.OrdinalIgnoreCase))
        {
            // 業務報表欄位
            return new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SiteId", DisplayName = "店別代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SiteName", DisplayName = "店別名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "CardType", DisplayName = "卡片類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "CardTypeName", DisplayName = "卡片類型名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "VendorId", DisplayName = "廠商代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "VendorName", DisplayName = "廠商名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "StoreId", DisplayName = "專櫃代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "StoreName", DisplayName = "專櫃名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "OrgId", DisplayName = "組織代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "OrgName", DisplayName = "組織名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ActionType", DisplayName = "動作類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ActionTypeName", DisplayName = "動作類型名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ReportDate", DisplayName = "報表日期", DataType = ExportDataType.Date },
                new ExportColumn { PropertyName = "Status", DisplayName = "狀態", DataType = ExportDataType.String }
            };
        }
        else if (reportId.StartsWith("SYSL210", StringComparison.OrdinalIgnoreCase))
        {
            // 員餐卡報表欄位
            return new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SiteId", DisplayName = "店別代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SiteName", DisplayName = "店別名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "OrgId", DisplayName = "組織代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "OrgName", DisplayName = "組織名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "TxnNo", DisplayName = "交易單號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Act1Name", DisplayName = "動作類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Amt1", DisplayName = "金額1", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "Amt4", DisplayName = "金額4", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "Amt5", DisplayName = "金額5", DataType = ExportDataType.Decimal }
            };
        }
        else if (reportId.StartsWith("SYSL310", StringComparison.OrdinalIgnoreCase))
        {
            // 銷退卡報表欄位
            return new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SiteId", DisplayName = "店別代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SiteName", DisplayName = "店別名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "OrgId", DisplayName = "組織代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "OrgName", DisplayName = "組織名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "EmployeeId", DisplayName = "員工編號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "EmployeeName", DisplayName = "員工姓名", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "CardYear", DisplayName = "卡片年度", DataType = ExportDataType.Number },
                new ExportColumn { PropertyName = "CardMonth", DisplayName = "卡片月份", DataType = ExportDataType.Number },
                new ExportColumn { PropertyName = "ReturnDate", DisplayName = "退卡日期", DataType = ExportDataType.Date },
                new ExportColumn { PropertyName = "ReturnReason", DisplayName = "退卡原因", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Amount", DisplayName = "金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "Status", DisplayName = "狀態", DataType = ExportDataType.String }
            };
        }
        else if (reportId.StartsWith("SYSL510", StringComparison.OrdinalIgnoreCase))
        {
            // 加班發放報表欄位
            return new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "PaymentNo", DisplayName = "發放單號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "PaymentDate", DisplayName = "發放日期", DataType = ExportDataType.Date },
                new ExportColumn { PropertyName = "EmployeeId", DisplayName = "員工編號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "EmployeeName", DisplayName = "員工姓名", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "DepartmentId", DisplayName = "部門代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "DepartmentName", DisplayName = "部門名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "OvertimeHours", DisplayName = "加班時數", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "OvertimeAmount", DisplayName = "加班金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "StartDate", DisplayName = "開始日期", DataType = ExportDataType.Date },
                new ExportColumn { PropertyName = "EndDate", DisplayName = "結束日期", DataType = ExportDataType.Date },
                new ExportColumn { PropertyName = "Status", DisplayName = "狀態", DataType = ExportDataType.String }
            };
        }
        else
        {
            throw new ArgumentException($"不支援的報表類型: {reportId}");
        }
    }

    /// <summary>
    /// 將 Entity 轉換為 DTO
    /// </summary>
    private BusinessReportPrintLogDto MapToDto(BusinessReportPrintLog entity)
    {
        return new BusinessReportPrintLogDto
        {
            TKey = entity.TKey,
            ReportId = entity.ReportId,
            ReportName = entity.ReportName,
            ReportType = entity.ReportType,
            PrintDate = entity.PrintDate,
            PrintUserId = entity.PrintUserId,
            PrintUserName = entity.PrintUserName,
            PrintParams = entity.PrintParams,
            PrintFormat = entity.PrintFormat,
            Status = entity.Status,
            ErrorMessage = entity.ErrorMessage,
            FilePath = entity.FilePath,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

