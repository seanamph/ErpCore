using ErpCore.Application.DTOs.Core;
using ErpCore.Application.Services.Base;
using ErpCore.Application.Services.Communication;
using ErpCore.Infrastructure.Data;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using Dapper;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ErpCore.Application.Services.Core;

/// <summary>
/// 工具服務實作
/// 提供 Excel匯出、字串編碼、ASP轉ASPX 等功能
/// </summary>
public class ToolsService : BaseService, IToolsService
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ExportHelper _exportHelper;
    private readonly IEncodeService _encodeService;
    private static readonly Dictionary<string, ExcelExportTaskDto> _exportTasks = new();
    private static readonly Dictionary<string, byte[]> _exportFiles = new();

    public ToolsService(
        IDbConnectionFactory connectionFactory,
        ExportHelper exportHelper,
        IEncodeService encodeService,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _connectionFactory = connectionFactory;
        _exportHelper = exportHelper;
        _encodeService = encodeService;
    }

    #region Export_Excel - Excel匯出功能

    public async Task<List<ExcelExportConfigDto>> GetExportConfigsAsync(string moduleCode)
    {
        try
        {
            _logger.LogInfo($"取得匯出設定列表: moduleCode={moduleCode}");

            var sql = @"
                SELECT ConfigId, ModuleCode, ExportName, ExportFields, ExportSettings, 
                       TemplatePath, Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                FROM ExcelExportConfigs
                WHERE ModuleCode = @ModuleCode AND Status = '1'
                ORDER BY ExportName";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<ExcelExportConfigDto>(sql, new { ModuleCode = moduleCode });
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("取得匯出設定列表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportExcelAsync(ExcelExportRequestDto request)
    {
        try
        {
            _logger.LogInfo($"匯出Excel: moduleCode={request.ModuleCode}");

            // 取得設定（如果有）
            ExcelExportConfigDto? config = null;
            if (request.ConfigId.HasValue)
            {
                var sql = "SELECT * FROM ExcelExportConfigs WHERE ConfigId = @ConfigId";
                using var connection = _connectionFactory.CreateConnection();
                config = await connection.QueryFirstOrDefaultAsync<ExcelExportConfigDto>(sql, new { ConfigId = request.ConfigId.Value });
            }

            // 查詢資料（這裡需要根據實際需求實現）
            var data = await QueryDataForExportAsync(request.ModuleCode, request.Filters);

            // 構建匯出欄位
            var columns = new List<ExportColumn>();
            if (request.ExportFields != null && request.ExportFields.Any())
            {
                foreach (var field in request.ExportFields)
                {
                    var dataType = Enum.TryParse<ExportDataType>(field.DataType, out var dt) ? dt : ExportDataType.String;
                    columns.Add(new ExportColumn
                    {
                        PropertyName = field.PropertyName,
                        DisplayName = field.DisplayName,
                        DataType = dataType
                    });
                }
            }
            else if (config != null && !string.IsNullOrEmpty(config.ExportFields))
            {
                // 從設定讀取欄位
                var fields = JsonSerializer.Deserialize<List<ExportColumnDto>>(config.ExportFields);
                if (fields != null)
                {
                    foreach (var field in fields)
                    {
                        var dataType = Enum.TryParse<ExportDataType>(field.DataType, out var dt) ? dt : ExportDataType.String;
                        columns.Add(new ExportColumn
                        {
                            PropertyName = field.PropertyName,
                            DisplayName = field.DisplayName,
                            DataType = dataType
                        });
                    }
                }
            }

            // 匯出Excel
            var excelBytes = _exportHelper.ExportToExcel(data, columns, request.SheetName ?? "Sheet1", request.Title);

            // 記錄匯出日誌
            await SaveExportLogAsync(request.ModuleCode, request.ConfigId, $"export_{DateTime.Now:yyyyMMddHHmmss}.xlsx", excelBytes.Length, data.Count);

            return excelBytes;
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出Excel失敗", ex);
            throw;
        }
    }

    public async Task<string> CreateExportTaskAsync(ExcelExportRequestDto request)
    {
        try
        {
            _logger.LogInfo($"建立背景匯出任務: moduleCode={request.ModuleCode}");

            var taskId = Guid.NewGuid().ToString();
            var task = new ExcelExportTaskDto
            {
                TaskId = taskId,
                Status = "PENDING",
                CreatedAt = DateTime.Now
            };
            _exportTasks[taskId] = task;

            // 背景執行匯出
            _ = Task.Run(async () =>
            {
                try
                {
                    task.Status = "PROCESSING";
                    var excelBytes = await ExportExcelAsync(request);
                    var fileName = $"export_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    _exportFiles[taskId] = excelBytes;
                    task.Status = "COMPLETED";
                    task.FileName = fileName;
                    task.DownloadUrl = $"/api/v1/core/tools/excel-export/download/{taskId}";
                    task.CompletedAt = DateTime.Now;
                }
                catch (Exception ex)
                {
                    task.Status = "FAILED";
                    task.ErrorMessage = ex.Message;
                    task.CompletedAt = DateTime.Now;
                    _logger.LogError("背景匯出任務失敗", ex);
                }
            });

            return taskId;
        }
        catch (Exception ex)
        {
            _logger.LogError("建立背景匯出任務失敗", ex);
            throw;
        }
    }

    public Task<ExcelExportTaskDto?> GetExportTaskStatusAsync(string taskId)
    {
        _exportTasks.TryGetValue(taskId, out var task);
        return Task.FromResult(task);
    }

    public Task<byte[]?> DownloadExportFileAsync(string taskId)
    {
        _exportFiles.TryGetValue(taskId, out var file);
        return Task.FromResult(file);
    }

    public async Task<long> CreateExportConfigAsync(CreateExcelExportConfigDto dto)
    {
        try
        {
            _logger.LogInfo($"新增匯出設定: moduleCode={dto.ModuleCode}, exportName={dto.ExportName}");

            var sql = @"
                INSERT INTO ExcelExportConfigs (ModuleCode, ExportName, ExportFields, ExportSettings, TemplatePath, Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES (@ModuleCode, @ExportName, @ExportFields, @ExportSettings, @TemplatePath, @Status, @CreatedBy, GETDATE(), @UpdatedBy, GETDATE());
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            using var connection = _connectionFactory.CreateConnection();
            var configId = await connection.ExecuteScalarAsync<long>(sql, new
            {
                dto.ModuleCode,
                dto.ExportName,
                dto.ExportFields,
                dto.ExportSettings,
                dto.TemplatePath,
                dto.Status,
                CreatedBy = GetCurrentUserId(),
                UpdatedBy = GetCurrentUserId()
            });

            return configId;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增匯出設定失敗", ex);
            throw;
        }
    }

    public async Task UpdateExportConfigAsync(long configId, CreateExcelExportConfigDto dto)
    {
        try
        {
            _logger.LogInfo($"修改匯出設定: configId={configId}");

            var sql = @"
                UPDATE ExcelExportConfigs
                SET ExportName = @ExportName, ExportFields = @ExportFields, ExportSettings = @ExportSettings,
                    TemplatePath = @TemplatePath, Status = @Status, UpdatedBy = @UpdatedBy, UpdatedAt = GETDATE()
                WHERE ConfigId = @ConfigId";

            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                configId,
                dto.ExportName,
                dto.ExportFields,
                dto.ExportSettings,
                dto.TemplatePath,
                dto.Status,
                UpdatedBy = GetCurrentUserId()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("修改匯出設定失敗", ex);
            throw;
        }
    }

    public async Task DeleteExportConfigAsync(long configId)
    {
        try
        {
            _logger.LogInfo($"刪除匯出設定: configId={configId}");

            var sql = "UPDATE ExcelExportConfigs SET Status = '0', UpdatedBy = @UpdatedBy, UpdatedAt = GETDATE() WHERE ConfigId = @ConfigId";

            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(sql, new { configId, UpdatedBy = GetCurrentUserId() });
        }
        catch (Exception ex)
        {
            _logger.LogError("刪除匯出設定失敗", ex);
            throw;
        }
    }

    private async Task<List<Dictionary<string, object>>> QueryDataForExportAsync(string moduleCode, Dictionary<string, object>? filters)
    {
        // 這裡需要根據實際需求實現資料查詢
        // 暫時返回空列表
        await Task.CompletedTask;
        return new List<Dictionary<string, object>>();
    }

    private async Task SaveExportLogAsync(string moduleCode, long? configId, string fileName, long fileSize, int recordCount)
    {
        try
        {
            var sql = @"
                INSERT INTO ExcelExportLogs (ModuleCode, ConfigId, UserId, FileName, FileSize, RecordCount, Status, CreatedAt, CompletedAt)
                VALUES (@ModuleCode, @ConfigId, @UserId, @FileName, @FileSize, @RecordCount, 'COMPLETED', GETDATE(), GETDATE())";

            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                moduleCode,
                configId,
                UserId = GetCurrentUserId(),
                fileName,
                fileSize,
                recordCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("儲存匯出記錄失敗", ex);
            // 不拋出異常，避免影響主要功能
        }
    }

    #endregion

    #region Encode_String - 字串編碼工具

    public async Task<EncodeStringResultDto> EncodeStringAsync(EncodeStringRequestDto request)
    {
        try
        {
            _logger.LogInfo($"編碼字串: encodeType={request.EncodeType}");

            // 使用現有的 EncodeService
            var encodeRequest = new ErpCore.Application.DTOs.Communication.StringEncodeRequestDto
            {
                Data = request.Text,
                KeyKind = request.EncodeType,
                SaveLog = false
            };

            var result = await _encodeService.StringEncodeAsync(encodeRequest);

            return new EncodeStringResultDto
            {
                OriginalText = result.OriginalData,
                EncodedText = result.EncodedData
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("編碼字串失敗", ex);
            throw;
        }
    }

    public async Task<DecodeStringResultDto> DecodeStringAsync(DecodeStringRequestDto request)
    {
        try
        {
            _logger.LogInfo($"解碼字串: encodeType={request.EncodeType}");

            // 使用現有的 EncodeService
            var decodeRequest = new ErpCore.Application.DTOs.Communication.StringEncodeRequestDto
            {
                Data = request.EncodedText,
                KeyKind = request.EncodeType,
                SaveLog = false
            };

            var result = await _encodeService.StringDecodeAsync(decodeRequest);

            return new DecodeStringResultDto
            {
                EncodedText = request.EncodedText,
                DecodedText = result.OriginalData
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("解碼字串失敗", ex);
            throw;
        }
    }

    #endregion

    #region ASPXTOASP - ASP轉ASPX工具

    public async Task<string> ConvertAspxToAspAsync(PageTransitionDto dto)
    {
        try
        {
            _logger.LogInfo($"ASPX轉ASP: targetUrl={dto.TargetUrl}");

            // 記錄轉換
            await SavePageTransitionAsync("ASPX", "ASP", dto, "SUCCESS", null);

            // 生成HTML表單，自動提交到目標ASP頁面
            var html = GenerateTransitionForm(dto.TargetUrl, dto.QueryParams, dto.FormData, dto.SessionData, dto.CookieData);
            return html;
        }
        catch (Exception ex)
        {
            _logger.LogError("ASPX轉ASP失敗", ex);
            await SavePageTransitionAsync("ASPX", "ASP", dto, "FAILED", ex.Message);
            throw;
        }
    }

    public async Task<string> ConvertAspToAspxAsync(PageTransitionDto dto)
    {
        try
        {
            _logger.LogInfo($"ASP轉ASPX: targetUrl={dto.TargetUrl}");

            // 記錄轉換
            await SavePageTransitionAsync("ASP", "ASPX", dto, "SUCCESS", null);

            // 生成HTML表單，自動提交到目標ASPX頁面
            var html = GenerateTransitionForm(dto.TargetUrl, dto.QueryParams, dto.FormData, dto.SessionData, dto.CookieData);
            return html;
        }
        catch (Exception ex)
        {
            _logger.LogError("ASP轉ASPX失敗", ex);
            await SavePageTransitionAsync("ASP", "ASPX", dto, "FAILED", ex.Message);
            throw;
        }
    }

    public async Task<PagedResult<PageTransitionLogDto>> GetPageTransitionsAsync(PageTransitionQueryDto query)
    {
        try
        {
            _logger.LogInfo("查詢頁面轉換記錄");

            var whereClause = new List<string> { "1=1" };
            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.UserId))
            {
                whereClause.Add("UserId = @UserId");
                parameters.Add("UserId", query.UserId);
            }
            if (!string.IsNullOrEmpty(query.SourceUrl))
            {
                whereClause.Add("SourceUrl LIKE @SourceUrl");
                parameters.Add("SourceUrl", $"%{query.SourceUrl}%");
            }
            if (!string.IsNullOrEmpty(query.TargetUrl))
            {
                whereClause.Add("TargetUrl LIKE @TargetUrl");
                parameters.Add("TargetUrl", $"%{query.TargetUrl}%");
            }
            if (query.StartDate.HasValue)
            {
                whereClause.Add("CreatedAt >= @StartDate");
                parameters.Add("StartDate", query.StartDate.Value);
            }
            if (query.EndDate.HasValue)
            {
                whereClause.Add("CreatedAt <= @EndDate");
                parameters.Add("EndDate", query.EndDate.Value);
            }

            var whereSql = string.Join(" AND ", whereClause);
            var sql = $@"
                SELECT COUNT(*) FROM PageTransitions WHERE {whereSql};
                SELECT TransitionId, SourceUrl, TargetUrl, SourceType, TargetType, UserId, SessionId, Status, ErrorMessage, CreatedAt
                FROM PageTransitions
                WHERE {whereSql}
                ORDER BY CreatedAt DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            using var connection = _connectionFactory.CreateConnection();
            using var multi = await connection.QueryMultipleAsync(sql, parameters);

            var totalCount = await multi.ReadSingleAsync<int>();
            var items = (await multi.ReadAsync<PageTransitionLogDto>()).ToList();

            return new PagedResult<PageTransitionLogDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢頁面轉換記錄失敗", ex);
            throw;
        }
    }

    public async Task<List<PageTransitionMappingDto>> GetPageTransitionMappingsAsync()
    {
        try
        {
            _logger.LogInfo("取得頁面轉換對應設定列表");

            var sql = @"
                SELECT MappingId, SourcePage, TargetPage, SourceType, TargetType, 
                       ParameterMapping, SessionMapping, Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                FROM PageTransitionMappings
                WHERE Status = '1'
                ORDER BY SourcePage, TargetPage";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<PageTransitionMappingDto>(sql);
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("取得頁面轉換對應設定列表失敗", ex);
            throw;
        }
    }

    public async Task<long> CreatePageTransitionMappingAsync(CreatePageTransitionMappingDto dto)
    {
        try
        {
            _logger.LogInfo($"新增頁面轉換對應設定: sourcePage={dto.SourcePage}, targetPage={dto.TargetPage}");

            var sql = @"
                INSERT INTO PageTransitionMappings (SourcePage, TargetPage, SourceType, TargetType, ParameterMapping, SessionMapping, Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES (@SourcePage, @TargetPage, @SourceType, @TargetType, @ParameterMapping, @SessionMapping, @Status, @CreatedBy, GETDATE(), @UpdatedBy, GETDATE());
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            using var connection = _connectionFactory.CreateConnection();
            var mappingId = await connection.ExecuteScalarAsync<long>(sql, new
            {
                dto.SourcePage,
                dto.TargetPage,
                dto.SourceType,
                dto.TargetType,
                dto.ParameterMapping,
                dto.SessionMapping,
                dto.Status,
                CreatedBy = GetCurrentUserId(),
                UpdatedBy = GetCurrentUserId()
            });

            return mappingId;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增頁面轉換對應設定失敗", ex);
            throw;
        }
    }

    public async Task UpdatePageTransitionMappingAsync(long mappingId, CreatePageTransitionMappingDto dto)
    {
        try
        {
            _logger.LogInfo($"修改頁面轉換對應設定: mappingId={mappingId}");

            var sql = @"
                UPDATE PageTransitionMappings
                SET SourcePage = @SourcePage, TargetPage = @TargetPage, SourceType = @SourceType, TargetType = @TargetType,
                    ParameterMapping = @ParameterMapping, SessionMapping = @SessionMapping, Status = @Status,
                    UpdatedBy = @UpdatedBy, UpdatedAt = GETDATE()
                WHERE MappingId = @MappingId";

            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                mappingId,
                dto.SourcePage,
                dto.TargetPage,
                dto.SourceType,
                dto.TargetType,
                dto.ParameterMapping,
                dto.SessionMapping,
                dto.Status,
                UpdatedBy = GetCurrentUserId()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("修改頁面轉換對應設定失敗", ex);
            throw;
        }
    }

    public async Task DeletePageTransitionMappingAsync(long mappingId)
    {
        try
        {
            _logger.LogInfo($"刪除頁面轉換對應設定: mappingId={mappingId}");

            var sql = "UPDATE PageTransitionMappings SET Status = '0', UpdatedBy = @UpdatedBy, UpdatedAt = GETDATE() WHERE MappingId = @MappingId";

            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(sql, new { mappingId, UpdatedBy = GetCurrentUserId() });
        }
        catch (Exception ex)
        {
            _logger.LogError("刪除頁面轉換對應設定失敗", ex);
            throw;
        }
    }

    private string GenerateTransitionForm(string targetUrl, Dictionary<string, string>? queryParams, Dictionary<string, object>? formData, Dictionary<string, object>? sessionData, Dictionary<string, string>? cookieData)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html>");
        sb.AppendLine("<head>");
        sb.AppendLine("<meta charset='utf-8'>");
        sb.AppendLine("<title>頁面轉換中...</title>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine($"<form id='transitionForm' method='post' action='{targetUrl}'>");

        // QueryString參數
        if (queryParams != null)
        {
            foreach (var param in queryParams)
            {
                sb.AppendLine($"<input type='hidden' name='{param.Key}' value='{param.Value}'>");
            }
        }

        // Form表單資料
        if (formData != null)
        {
            foreach (var data in formData)
            {
                sb.AppendLine($"<input type='hidden' name='{data.Key}' value='{data.Value}'>");
            }
        }

        // Session資料（轉換為隱藏欄位）
        if (sessionData != null)
        {
            foreach (var session in sessionData)
            {
                sb.AppendLine($"<input type='hidden' name='Session_{session.Key}' value='{session.Value}'>");
            }
        }

        sb.AppendLine("</form>");
        sb.AppendLine("<script>");
        sb.AppendLine("document.getElementById('transitionForm').submit();");
        sb.AppendLine("</script>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        return sb.ToString();
    }

    private async Task SavePageTransitionAsync(string sourceType, string targetType, PageTransitionDto dto, string status, string? errorMessage)
    {
        try
        {
            var sql = @"
                INSERT INTO PageTransitions (SourceUrl, TargetUrl, SourceType, TargetType, UserId, SessionId, 
                                            QueryString, FormData, SessionData, CookieData, Status, ErrorMessage, CreatedAt)
                VALUES (@SourceUrl, @TargetUrl, @SourceType, @TargetType, @UserId, @SessionId, 
                        @QueryString, @FormData, @SessionData, @CookieData, @Status, @ErrorMessage, GETDATE())";

            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                SourceUrl = "",
                TargetUrl = dto.TargetUrl,
                SourceType = sourceType,
                TargetType = targetType,
                UserId = GetCurrentUserId(),
                SessionId = "",
                QueryString = dto.QueryParams != null ? JsonSerializer.Serialize(dto.QueryParams) : null,
                FormData = dto.FormData != null ? JsonSerializer.Serialize(dto.FormData) : null,
                SessionData = dto.SessionData != null ? JsonSerializer.Serialize(dto.SessionData) : null,
                CookieData = dto.CookieData != null ? JsonSerializer.Serialize(dto.CookieData) : null,
                Status = status,
                ErrorMessage = errorMessage
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("儲存頁面轉換記錄失敗", ex);
            // 不拋出異常，避免影響主要功能
        }
    }

    #endregion
}

