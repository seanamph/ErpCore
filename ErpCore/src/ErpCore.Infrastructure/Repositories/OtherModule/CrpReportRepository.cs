using Dapper;
using ErpCore.Domain.Entities.OtherModule;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.OtherModule;

/// <summary>
/// CRP報表 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class CrpReportRepository : BaseRepository, ICrpReportRepository
{
    public CrpReportRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<CrystalReport?> GetByReportCodeAsync(string reportCode)
    {
        try
        {
            const string sql = @"
                SELECT * FROM CrystalReports 
                WHERE ReportCode = @ReportCode";

            return await QueryFirstOrDefaultAsync<CrystalReport>(sql, new { ReportCode = reportCode });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢報表設定失敗: {reportCode}", ex);
            throw;
        }
    }

    public async Task<List<CrystalReport>> GetAllAsync()
    {
        try
        {
            const string sql = @"
                SELECT * FROM CrystalReports 
                WHERE Status = '1'
                ORDER BY ReportCode";

            var result = await QueryAsync<CrystalReport>(sql);
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢報表設定列表失敗", ex);
            throw;
        }
    }

    public async Task<CrystalReport> CreateAsync(CrystalReport report)
    {
        try
        {
            const string sql = @"
                INSERT INTO CrystalReports (
                    ReportCode, ReportName, ReportPath, MdbName, Parameters, ExportOptions, Status,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                )
                OUTPUT INSERTED.*
                VALUES (
                    @ReportCode, @ReportName, @ReportPath, @MdbName, @Parameters, @ExportOptions, @Status,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                )";

            var result = await QueryFirstOrDefaultAsync<CrystalReport>(sql, report);
            if (result == null)
            {
                throw new InvalidOperationException("新增報表設定失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增報表設定失敗: {report.ReportCode}", ex);
            throw;
        }
    }

    public async Task<CrystalReport> UpdateAsync(CrystalReport report)
    {
        try
        {
            const string sql = @"
                UPDATE CrystalReports SET
                    ReportName = @ReportName,
                    ReportPath = @ReportPath,
                    MdbName = @MdbName,
                    Parameters = @Parameters,
                    ExportOptions = @ExportOptions,
                    Status = @Status,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                OUTPUT INSERTED.*
                WHERE ReportId = @ReportId";

            var result = await QueryFirstOrDefaultAsync<CrystalReport>(sql, report);
            if (result == null)
            {
                throw new InvalidOperationException($"報表設定不存在: {report.ReportId}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改報表設定失敗: {report.ReportId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long reportId)
    {
        try
        {
            const string sql = @"
                DELETE FROM CrystalReports 
                WHERE ReportId = @ReportId";

            await ExecuteAsync(sql, new { ReportId = reportId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除報表設定失敗: {reportId}", ex);
            throw;
        }
    }

    public async Task<CrystalReportLog> CreateLogAsync(CrystalReportLog log)
    {
        try
        {
            const string sql = @"
                INSERT INTO CrystalReportLogs (
                    ReportId, ReportCode, OperationType, Parameters, Status, ErrorMessage, FileSize, Duration,
                    CreatedBy, CreatedAt
                )
                OUTPUT INSERTED.*
                VALUES (
                    @ReportId, @ReportCode, @OperationType, @Parameters, @Status, @ErrorMessage, @FileSize, @Duration,
                    @CreatedBy, @CreatedAt
                )";

            var result = await QueryFirstOrDefaultAsync<CrystalReportLog>(sql, log);
            if (result == null)
            {
                throw new InvalidOperationException("新增操作記錄失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增操作記錄失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<CrystalReportLog>> GetLogsAsync(string? reportCode, int pageIndex, int pageSize)
    {
        try
        {
            var sql = @"
                SELECT * FROM CrystalReportLogs
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(reportCode))
            {
                sql += " AND ReportCode = @ReportCode";
                parameters.Add("ReportCode", reportCode);
            }

            sql += " ORDER BY CreatedAt DESC";

            var offset = (pageIndex - 1) * pageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", pageSize);

            var items = await QueryAsync<CrystalReportLog>(sql, parameters);

            var countSql = @"
                SELECT COUNT(*) FROM CrystalReportLogs
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(reportCode))
            {
                countSql += " AND ReportCode = @ReportCode";
                countParameters.Add("ReportCode", reportCode);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<CrystalReportLog>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢操作記錄列表失敗", ex);
            throw;
        }
    }
}

