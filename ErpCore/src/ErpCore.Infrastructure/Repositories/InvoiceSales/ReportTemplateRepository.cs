using System.Data;
using Dapper;
using ErpCore.Domain.Entities.InvoiceSales;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.InvoiceSales;

/// <summary>
/// 報表模板 Repository 實作 (SYSG710-SYSG7I0 - 報表列印作業)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ReportTemplateRepository : BaseRepository, IReportTemplateRepository
{
    public ReportTemplateRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<ReportTemplate?> GetByIdAsync(string templateId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM ReportTemplates 
                WHERE TemplateId = @TemplateId";

            return await QueryFirstOrDefaultAsync<ReportTemplate>(sql, new { TemplateId = templateId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢報表模板失敗: {templateId}", ex);
            throw;
        }
    }

    public async Task<List<ReportTemplate>> GetByReportTypeAsync(string reportType, string? status = null)
    {
        try
        {
            var sql = @"
                SELECT * FROM ReportTemplates 
                WHERE ReportType = @ReportType";

            var parameters = new DynamicParameters();
            parameters.Add("ReportType", reportType);

            if (!string.IsNullOrEmpty(status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", status);
            }

            sql += " ORDER BY TemplateName";

            var result = await QueryAsync<ReportTemplate>(sql, parameters);
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢報表模板列表失敗: {reportType}", ex);
            throw;
        }
    }

    public async Task<int> CreateAsync(ReportTemplate template)
    {
        try
        {
            const string sql = @"
                INSERT INTO ReportTemplates (
                    TemplateId, TemplateName, TemplateType, ReportType,
                    TemplateContent, TemplateFile, Parameters, Status,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                ) VALUES (
                    @TemplateId, @TemplateName, @TemplateType, @ReportType,
                    @TemplateContent, @TemplateFile, @Parameters, @Status,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                )";

            return await ExecuteAsync(sql, template);
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增報表模板失敗: {template.TemplateId}", ex);
            throw;
        }
    }

    public async Task<int> UpdateAsync(ReportTemplate template)
    {
        try
        {
            const string sql = @"
                UPDATE ReportTemplates SET
                    TemplateName = @TemplateName,
                    TemplateType = @TemplateType,
                    ReportType = @ReportType,
                    TemplateContent = @TemplateContent,
                    TemplateFile = @TemplateFile,
                    Parameters = @Parameters,
                    Status = @Status,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TemplateId = @TemplateId";

            return await ExecuteAsync(sql, template);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改報表模板失敗: {template.TemplateId}", ex);
            throw;
        }
    }

    public async Task<int> DeleteAsync(string templateId)
    {
        try
        {
            const string sql = @"
                DELETE FROM ReportTemplates 
                WHERE TemplateId = @TemplateId";

            return await ExecuteAsync(sql, new { TemplateId = templateId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除報表模板失敗: {templateId}", ex);
            throw;
        }
    }
}

/// <summary>
/// 報表列印記錄 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ReportPrintLogRepository : BaseRepository, IReportPrintLogRepository
{
    public ReportPrintLogRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<ReportPrintLog?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM ReportPrintLogs 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<ReportPrintLog>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢報表列印記錄失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<ReportPrintLog?> GetByReportIdAsync(string reportId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM ReportPrintLogs 
                WHERE ReportId = @ReportId";

            return await QueryFirstOrDefaultAsync<ReportPrintLog>(sql, new { ReportId = reportId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢報表列印記錄失敗: {reportId}", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(ReportPrintLog log)
    {
        try
        {
            const string sql = @"
                INSERT INTO ReportPrintLogs (
                    ReportId, ReportType, TemplateId, PrintUserId,
                    PrintDate, PrintFormat, FileUrl, Parameters,
                    Status, ErrorMessage, CreatedAt
                ) VALUES (
                    @ReportId, @ReportType, @TemplateId, @PrintUserId,
                    @PrintDate, @PrintFormat, @FileUrl, @Parameters,
                    @Status, @ErrorMessage, @CreatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() as bigint);";

            var tKey = await ExecuteScalarAsync<long>(sql, log);
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增報表列印記錄失敗: {log.ReportId}", ex);
            throw;
        }
    }

    public async Task<int> UpdateAsync(ReportPrintLog log)
    {
        try
        {
            const string sql = @"
                UPDATE ReportPrintLogs SET
                    FileUrl = @FileUrl,
                    Status = @Status,
                    ErrorMessage = @ErrorMessage
                WHERE TKey = @TKey";

            return await ExecuteAsync(sql, log);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改報表列印記錄失敗: {log.TKey}", ex);
            throw;
        }
    }
}

