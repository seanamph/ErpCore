using Dapper;
using ErpCore.Domain.Entities.TaxAccounting;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.TaxAccounting;

/// <summary>
/// 稅務報表列印 Repository 實作 (SYST510-SYST530)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class TaxReportPrintRepository : BaseRepository, ITaxReportPrintRepository
{
    public TaxReportPrintRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PagedResult<SapBankTotal>> GetSapBankTotalPagedAsync(SapBankTotalQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM SapBankTotal
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (query.DateFrom.HasValue)
            {
                sql += " AND SapDate >= @DateFrom";
                parameters.Add("DateFrom", query.DateFrom.Value.ToString("yyyyMMdd"));
            }

            if (query.DateTo.HasValue)
            {
                sql += " AND SapDate <= @DateTo";
                parameters.Add("DateTo", query.DateTo.Value.ToString("yyyyMMdd"));
            }

            if (!string.IsNullOrEmpty(query.CompId))
            {
                sql += " AND CompId = @CompId";
                parameters.Add("CompId", query.CompId);
            }

            if (!string.IsNullOrEmpty(query.SapStypeId))
            {
                sql += " AND SapStypeId = @SapStypeId";
                parameters.Add("SapStypeId", query.SapStypeId);
            }

            // 排序
            sql += " ORDER BY SapDate DESC, CompId";

            // 總數查詢
            var countSql = sql.Replace("SELECT *", "SELECT COUNT(*)");
            var totalCount = await QuerySingleAsync<int>(countSql, parameters);

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<SapBankTotal>(sql, parameters);

            return new PagedResult<SapBankTotal>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢SAP銀行往來資料失敗", ex);
            throw;
        }
    }

    public async Task<List<SapBankTotal>> GetSapBankTotalListAsync(SapBankTotalQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM SapBankTotal
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (query.DateFrom.HasValue)
            {
                sql += " AND SapDate >= @DateFrom";
                parameters.Add("DateFrom", query.DateFrom.Value.ToString("yyyyMMdd"));
            }

            if (query.DateTo.HasValue)
            {
                sql += " AND SapDate <= @DateTo";
                parameters.Add("DateTo", query.DateTo.Value.ToString("yyyyMMdd"));
            }

            if (!string.IsNullOrEmpty(query.CompId))
            {
                sql += " AND CompId = @CompId";
                parameters.Add("CompId", query.CompId);
            }

            if (!string.IsNullOrEmpty(query.SapStypeId))
            {
                sql += " AND SapStypeId = @SapStypeId";
                parameters.Add("SapStypeId", query.SapStypeId);
            }

            sql += " ORDER BY SapDate DESC, CompId";

            return (await QueryAsync<SapBankTotal>(sql, parameters)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢SAP銀行往來資料列表失敗", ex);
            throw;
        }
    }

    public async Task<TaxReportPrint?> GetPrintLogByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM TaxReportPrints 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<TaxReportPrint>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢列印記錄失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<PagedResult<TaxReportPrint>> GetPrintLogsPagedAsync(TaxReportPrintQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM TaxReportPrints
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ReportType))
            {
                sql += " AND ReportType = @ReportType";
                parameters.Add("ReportType", query.ReportType);
            }

            if (query.DateFrom.HasValue)
            {
                sql += " AND ReportDate >= @DateFrom";
                parameters.Add("DateFrom", query.DateFrom.Value);
            }

            if (query.DateTo.HasValue)
            {
                sql += " AND ReportDate <= @DateTo";
                parameters.Add("DateTo", query.DateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.PrintStatus))
            {
                sql += " AND PrintStatus = @PrintStatus";
                parameters.Add("PrintStatus", query.PrintStatus);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "ReportDate" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 總數查詢
            var countSql = sql.Replace("SELECT *", "SELECT COUNT(*)");
            var totalCount = await QuerySingleAsync<int>(countSql, parameters);

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<TaxReportPrint>(sql, parameters);

            return new PagedResult<TaxReportPrint>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢列印記錄列表失敗", ex);
            throw;
        }
    }

    public async Task<TaxReportPrint> CreatePrintLogAsync(TaxReportPrint printLog)
    {
        try
        {
            const string sql = @"
                INSERT INTO TaxReportPrints (
                    ReportType, ReportDate, DateFrom, DateTo, CompId,
                    FileName, FileFormat, PrintStatus, PrintCount, Notes,
                    CreatedBy, CreatedAt
                )
                OUTPUT INSERTED.*
                VALUES (
                    @ReportType, @ReportDate, @DateFrom, @DateTo, @CompId,
                    @FileName, @FileFormat, @PrintStatus, @PrintCount, @Notes,
                    @CreatedBy, @CreatedAt
                )";

            return await QuerySingleAsync<TaxReportPrint>(sql, printLog);
        }
        catch (Exception ex)
        {
            _logger.LogError("新增列印記錄失敗", ex);
            throw;
        }
    }

    public async Task<TaxReportPrint> UpdatePrintLogAsync(TaxReportPrint printLog)
    {
        try
        {
            const string sql = @"
                UPDATE TaxReportPrints SET
                    ReportDate = @ReportDate,
                    DateFrom = @DateFrom,
                    DateTo = @DateTo,
                    CompId = @CompId,
                    FileName = @FileName,
                    FileFormat = @FileFormat,
                    PrintStatus = @PrintStatus,
                    PrintCount = @PrintCount,
                    Notes = @Notes
                OUTPUT INSERTED.*
                WHERE TKey = @TKey";

            return await QuerySingleAsync<TaxReportPrint>(sql, printLog);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改列印記錄失敗: {printLog.TKey}", ex);
            throw;
        }
    }

    public async Task DeletePrintLogAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM TaxReportPrints 
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除列印記錄失敗: {tKey}", ex);
            throw;
        }
    }
}

