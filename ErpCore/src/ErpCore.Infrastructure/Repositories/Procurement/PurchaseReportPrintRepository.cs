using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Procurement;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Procurement;

/// <summary>
/// 採購報表列印 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class PurchaseReportPrintRepository : BaseRepository, IPurchaseReportPrintRepository
{
    public PurchaseReportPrintRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PurchaseReportPrint?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM PurchaseReportPrints 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<PurchaseReportPrint>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢採購報表列印記錄失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<PurchaseReportPrint>> QueryAsync(PurchaseReportPrintQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM PurchaseReportPrints 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ReportType))
            {
                sql += " AND ReportType = @ReportType";
                parameters.Add("ReportType", query.ReportType);
            }

            if (!string.IsNullOrEmpty(query.ReportCode))
            {
                sql += " AND ReportCode LIKE @ReportCode";
                parameters.Add("ReportCode", $"%{query.ReportCode}%");
            }

            if (!string.IsNullOrEmpty(query.PrintUserId))
            {
                sql += " AND PrintUserId = @PrintUserId";
                parameters.Add("PrintUserId", query.PrintUserId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (query.StartDate.HasValue)
            {
                sql += " AND PrintDate >= @StartDate";
                parameters.Add("StartDate", query.StartDate.Value);
            }

            if (query.EndDate.HasValue)
            {
                sql += " AND PrintDate <= @EndDate";
                parameters.Add("EndDate", query.EndDate.Value);
            }

            // 排序
            if (!string.IsNullOrEmpty(query.SortField))
            {
                var sortOrder = query.SortOrder?.ToUpper() == "ASC" ? "ASC" : "DESC";
                sql += $" ORDER BY {query.SortField} {sortOrder}";
            }
            else
            {
                sql += " ORDER BY PrintDate DESC";
            }

            // 分頁
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<PurchaseReportPrint>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢採購報表列印記錄列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(PurchaseReportPrintQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM PurchaseReportPrints 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ReportType))
            {
                sql += " AND ReportType = @ReportType";
                parameters.Add("ReportType", query.ReportType);
            }

            if (!string.IsNullOrEmpty(query.ReportCode))
            {
                sql += " AND ReportCode LIKE @ReportCode";
                parameters.Add("ReportCode", $"%{query.ReportCode}%");
            }

            if (!string.IsNullOrEmpty(query.PrintUserId))
            {
                sql += " AND PrintUserId = @PrintUserId";
                parameters.Add("PrintUserId", query.PrintUserId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (query.StartDate.HasValue)
            {
                sql += " AND PrintDate >= @StartDate";
                parameters.Add("StartDate", query.StartDate.Value);
            }

            if (query.EndDate.HasValue)
            {
                sql += " AND PrintDate <= @EndDate";
                parameters.Add("EndDate", query.EndDate.Value);
            }

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢採購報表列印記錄數量失敗", ex);
            throw;
        }
    }

    public async Task<PurchaseReportPrint> CreateAsync(PurchaseReportPrint entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO PurchaseReportPrints (
                    ReportType, ReportCode, ReportName, PrintDate, PrintUserId, PrintUserName,
                    FilterConditions, PrintSettings, FileFormat, FilePath, FileName, FileSize,
                    Status, ErrorMessage, PageCount, RecordCount, Notes,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                ) VALUES (
                    @ReportType, @ReportCode, @ReportName, @PrintDate, @PrintUserId, @PrintUserName,
                    @FilterConditions, @PrintSettings, @FileFormat, @FilePath, @FileName, @FileSize,
                    @Status, @ErrorMessage, @PageCount, @RecordCount, @Notes,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                entity.ReportType,
                entity.ReportCode,
                entity.ReportName,
                entity.PrintDate,
                entity.PrintUserId,
                entity.PrintUserName,
                entity.FilterConditions,
                entity.PrintSettings,
                entity.FileFormat,
                entity.FilePath,
                entity.FileName,
                entity.FileSize,
                entity.Status,
                entity.ErrorMessage,
                entity.PageCount,
                entity.RecordCount,
                entity.Notes,
                entity.CreatedBy,
                entity.CreatedAt,
                entity.UpdatedBy,
                entity.UpdatedAt
            });

            entity.TKey = tKey;
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增採購報表列印記錄失敗", ex);
            throw;
        }
    }

    public async Task<PurchaseReportPrint> UpdateAsync(PurchaseReportPrint entity)
    {
        try
        {
            const string sql = @"
                UPDATE PurchaseReportPrints SET
                    ReportType = @ReportType,
                    ReportCode = @ReportCode,
                    ReportName = @ReportName,
                    PrintDate = @PrintDate,
                    PrintUserId = @PrintUserId,
                    PrintUserName = @PrintUserName,
                    FilterConditions = @FilterConditions,
                    PrintSettings = @PrintSettings,
                    FileFormat = @FileFormat,
                    FilePath = @FilePath,
                    FileName = @FileName,
                    FileSize = @FileSize,
                    Status = @Status,
                    ErrorMessage = @ErrorMessage,
                    PageCount = @PageCount,
                    RecordCount = @RecordCount,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new
            {
                entity.TKey,
                entity.ReportType,
                entity.ReportCode,
                entity.ReportName,
                entity.PrintDate,
                entity.PrintUserId,
                entity.PrintUserName,
                entity.FilterConditions,
                entity.PrintSettings,
                entity.FileFormat,
                entity.FilePath,
                entity.FileName,
                entity.FileSize,
                entity.Status,
                entity.ErrorMessage,
                entity.PageCount,
                entity.RecordCount,
                entity.Notes,
                entity.UpdatedBy,
                entity.UpdatedAt
            });

            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改採購報表列印記錄失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM PurchaseReportPrints 
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除採購報表列印記錄失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<List<PurchaseReportTemplate>> GetTemplatesAsync(string? reportType, string? reportCode)
    {
        try
        {
            var sql = @"
                SELECT * FROM PurchaseReportTemplates 
                WHERE Status = '1'";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(reportType))
            {
                sql += " AND ReportType = @ReportType";
                parameters.Add("ReportType", reportType);
            }

            if (!string.IsNullOrEmpty(reportCode))
            {
                sql += " AND ReportCode = @ReportCode";
                parameters.Add("ReportCode", reportCode);
            }

            sql += " ORDER BY IsDefault DESC, TemplateName";

            var result = await QueryAsync<PurchaseReportTemplate>(sql, parameters);
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢採購報表模板列表失敗", ex);
            throw;
        }
    }
}
