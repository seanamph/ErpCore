using Dapper;
using ErpCore.Domain.Entities.TaxAccounting;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.TaxAccounting;

/// <summary>
/// 傳票轉入 Repository 實作 (SYST002-SYST003)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class VoucherImportRepository : BaseRepository, IVoucherImportRepository
{
    public VoucherImportRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<VoucherImportLog?> GetImportLogByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM VoucherImportLog 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<VoucherImportLog>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢轉入記錄失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<PagedResult<VoucherImportLog>> GetImportLogsPagedAsync(VoucherImportLogQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM VoucherImportLog
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ImportType))
            {
                sql += " AND ImportType = @ImportType";
                parameters.Add("ImportType", query.ImportType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (query.ImportDateFrom.HasValue)
            {
                sql += " AND ImportDate >= @ImportDateFrom";
                parameters.Add("ImportDateFrom", query.ImportDateFrom.Value);
            }

            if (query.ImportDateTo.HasValue)
            {
                sql += " AND ImportDate <= @ImportDateTo";
                parameters.Add("ImportDateTo", query.ImportDateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.FileName))
            {
                sql += " AND FileName LIKE @FileName";
                parameters.Add("FileName", $"%{query.FileName}%");
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "ImportDate" : query.SortField;
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

            var items = await QueryAsync<VoucherImportLog>(sql, parameters);

            return new PagedResult<VoucherImportLog>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢轉入記錄列表失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateImportLogAsync(VoucherImportLog log)
    {
        try
        {
            const string sql = @"
                INSERT INTO VoucherImportLog (
                    ImportType, FileName, FilePath, ImportDate,
                    TotalCount, SuccessCount, FailCount, SkipCount,
                    Status, ErrorMessage, CreatedBy, CreatedAt
                )
                OUTPUT INSERTED.TKey
                VALUES (
                    @ImportType, @FileName, @FilePath, @ImportDate,
                    @TotalCount, @SuccessCount, @FailCount, @SkipCount,
                    @Status, @ErrorMessage, @CreatedBy, @CreatedAt
                )";

            return await QuerySingleAsync<long>(sql, log);
        }
        catch (Exception ex)
        {
            _logger.LogError("新增轉入記錄失敗", ex);
            throw;
        }
    }

    public async Task UpdateImportLogAsync(VoucherImportLog log)
    {
        try
        {
            const string sql = @"
                UPDATE VoucherImportLog SET
                    TotalCount = @TotalCount,
                    SuccessCount = @SuccessCount,
                    FailCount = @FailCount,
                    SkipCount = @SkipCount,
                    Status = @Status,
                    ErrorMessage = @ErrorMessage,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, log);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改轉入記錄失敗: {log.TKey}", ex);
            throw;
        }
    }

    public async Task<VoucherImportDetail?> GetImportDetailByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM VoucherImportDetail 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<VoucherImportDetail>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢轉入明細失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<List<VoucherImportDetail>> GetImportDetailsAsync(long importLogTKey, VoucherImportDetailQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM VoucherImportDetail
                WHERE ImportLogTKey = @ImportLogTKey";

            var parameters = new DynamicParameters();
            parameters.Add("ImportLogTKey", importLogTKey);

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            sql += " ORDER BY RowNumber";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            return (await QueryAsync<VoucherImportDetail>(sql, parameters)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢轉入明細列表失敗: {importLogTKey}", ex);
            throw;
        }
    }

    public async Task<long> CreateImportDetailAsync(VoucherImportDetail detail)
    {
        try
        {
            const string sql = @"
                INSERT INTO VoucherImportDetail (
                    ImportLogTKey, RowNumber, VoucherTKey,
                    Status, ErrorMessage, SourceData,
                    CreatedBy, CreatedAt
                )
                OUTPUT INSERTED.TKey
                VALUES (
                    @ImportLogTKey, @RowNumber, @VoucherTKey,
                    @Status, @ErrorMessage, @SourceData,
                    @CreatedBy, @CreatedAt
                )";

            return await QuerySingleAsync<long>(sql, detail);
        }
        catch (Exception ex)
        {
            _logger.LogError("新增轉入明細失敗", ex);
            throw;
        }
    }

    public async Task UpdateImportDetailAsync(VoucherImportDetail detail)
    {
        try
        {
            const string sql = @"
                UPDATE VoucherImportDetail SET
                    VoucherTKey = @VoucherTKey,
                    Status = @Status,
                    ErrorMessage = @ErrorMessage
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, detail);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改轉入明細失敗: {detail.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteImportDetailsAsync(long importLogTKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM VoucherImportDetail 
                WHERE ImportLogTKey = @ImportLogTKey";

            await ExecuteAsync(sql, new { ImportLogTKey = importLogTKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除轉入明細失敗: {importLogTKey}", ex);
            throw;
        }
    }

    public async Task<long> CreateTmpVoucherAsync(TmpVoucherM voucher)
    {
        try
        {
            const string sql = @"
                INSERT INTO TmpVoucherM (
                    VoucherId, VoucherDate, TypeId, SysId, Status, UpFlag,
                    Notes, VendorId, StoreId, SiteId, SlipType, SlipNo,
                    CreatedBy, CreatedAt
                )
                OUTPUT INSERTED.TKey
                VALUES (
                    @VoucherId, @VoucherDate, @TypeId, @SysId, @Status, @UpFlag,
                    @Notes, @VendorId, @StoreId, @SiteId, @SlipType, @SlipNo,
                    @CreatedBy, @CreatedAt
                )";

            return await QuerySingleAsync<long>(sql, voucher);
        }
        catch (Exception ex)
        {
            _logger.LogError("新增暫存傳票失敗", ex);
            throw;
        }
    }

    public async Task<bool> CheckDuplicateVoucherAsync(string slipType, string slipNo)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM TmpVoucherM 
                WHERE SlipType = @SlipType AND SlipNo = @SlipNo";

            var count = await QuerySingleAsync<int>(sql, new { SlipType = slipType, SlipNo = slipNo });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError("檢查重複傳票失敗", ex);
            throw;
        }
    }
}

