using Dapper;
using ErpCore.Domain.Entities.BusinessReport;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.BusinessReport;

/// <summary>
/// 業務報表列印明細 Repository 實作 (SYSL160)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class BusinessReportPrintDetailRepository : BaseRepository, IBusinessReportPrintDetailRepository
{
    public BusinessReportPrintDetailRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PagedResult<BusinessReportPrintDetail>> QueryAsync(BusinessReportPrintDetailQuery query)
    {
        try
        {
            var sql = @"
                SELECT 
                    brpd.TKey,
                    brpd.PrintId,
                    brpd.LeaveId,
                    brpd.LeaveName,
                    brpd.ActEvent,
                    brpd.DeductionQty,
                    brpd.DeductionQtyDefaultEmpty,
                    brpd.Status,
                    brpd.CreatedBy,
                    brpd.CreatedAt,
                    brpd.UpdatedBy,
                    brpd.UpdatedAt
                FROM BusinessReportPrintDetail brpd
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (query.PrintId.HasValue)
            {
                sql += " AND brpd.PrintId = @PrintId";
                parameters.Add("PrintId", query.PrintId.Value);
            }

            if (!string.IsNullOrEmpty(query.LeaveId))
            {
                sql += " AND brpd.LeaveId LIKE @LeaveId";
                parameters.Add("LeaveId", $"%{query.LeaveId}%");
            }

            if (!string.IsNullOrEmpty(query.ActEvent))
            {
                sql += " AND brpd.ActEvent LIKE @ActEvent";
                parameters.Add("ActEvent", $"%{query.ActEvent}%");
            }

            // 計算總筆數
            var countSql = @"
                SELECT COUNT(*) 
                FROM BusinessReportPrintDetail brpd
                WHERE 1=1";
            if (query.PrintId.HasValue)
            {
                countSql += " AND brpd.PrintId = @PrintId";
            }
            if (!string.IsNullOrEmpty(query.LeaveId))
            {
                countSql += " AND brpd.LeaveId LIKE @LeaveId";
            }
            if (!string.IsNullOrEmpty(query.ActEvent))
            {
                countSql += " AND brpd.ActEvent LIKE @ActEvent";
            }
            var totalCount = await ExecuteScalarAsync<int>(countSql, parameters);

            // 排序
            sql += " ORDER BY brpd.TKey DESC";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<BusinessReportPrintDetail>(sql, parameters);

            return new PagedResult<BusinessReportPrintDetail>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢業務報表列印明細列表失敗", ex);
            throw;
        }
    }

    public async Task<BusinessReportPrintDetail?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT 
                    TKey,
                    PrintId,
                    LeaveId,
                    LeaveName,
                    ActEvent,
                    DeductionQty,
                    DeductionQtyDefaultEmpty,
                    Status,
                    CreatedBy,
                    CreatedAt,
                    UpdatedBy,
                    UpdatedAt
                FROM BusinessReportPrintDetail
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<BusinessReportPrintDetail>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢業務報表列印明細失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<List<BusinessReportPrintDetail>> GetByPrintIdAsync(long printId)
    {
        try
        {
            const string sql = @"
                SELECT 
                    TKey,
                    PrintId,
                    LeaveId,
                    LeaveName,
                    ActEvent,
                    DeductionQty,
                    DeductionQtyDefaultEmpty,
                    Status,
                    CreatedBy,
                    CreatedAt,
                    UpdatedBy,
                    UpdatedAt
                FROM BusinessReportPrintDetail
                WHERE PrintId = @PrintId
                ORDER BY TKey";

            var items = await QueryAsync<BusinessReportPrintDetail>(sql, new { PrintId = printId });
            return items.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢業務報表列印明細失敗: PrintId={printId}", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(BusinessReportPrintDetail entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO BusinessReportPrintDetail 
                (PrintId, LeaveId, LeaveName, ActEvent, DeductionQty, DeductionQtyDefaultEmpty, Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@PrintId, @LeaveId, @LeaveName, @ActEvent, @DeductionQty, @DeductionQtyDefaultEmpty, @Status, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, entity);
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增業務報表列印明細失敗", ex);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(BusinessReportPrintDetail entity)
    {
        try
        {
            const string sql = @"
                UPDATE BusinessReportPrintDetail SET
                    LeaveId = @LeaveId,
                    LeaveName = @LeaveName,
                    ActEvent = @ActEvent,
                    DeductionQty = @DeductionQty,
                    DeductionQtyDefaultEmpty = @DeductionQtyDefaultEmpty,
                    Status = @Status,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            var affectedRows = await ExecuteAsync(sql, entity);
            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改業務報表列印明細失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(long tKey)
    {
        try
        {
            const string sql = "DELETE FROM BusinessReportPrintDetail WHERE TKey = @TKey";
            var affectedRows = await ExecuteAsync(sql, new { TKey = tKey });
            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除業務報表列印明細失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<BatchProcessResult> BatchProcessAsync(
        List<BusinessReportPrintDetail> createItems,
        List<BusinessReportPrintDetail> updateItems,
        List<long> deleteTKeys)
    {
        try
        {
            var result = new BatchProcessResult();

            using var connection = _connectionFactory.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                // 新增
                foreach (var item in createItems)
                {
                    const string createSql = @"
                        INSERT INTO BusinessReportPrintDetail 
                        (PrintId, LeaveId, LeaveName, ActEvent, DeductionQty, DeductionQtyDefaultEmpty, Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                        VALUES 
                        (@PrintId, @LeaveId, @LeaveName, @ActEvent, @DeductionQty, @DeductionQtyDefaultEmpty, @Status, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                        SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";
                    await connection.ExecuteScalarAsync<long>(createSql, item, transaction);
                    result.CreateCount++;
                }

                // 修改
                foreach (var item in updateItems)
                {
                    const string updateSql = @"
                        UPDATE BusinessReportPrintDetail SET
                            LeaveId = @LeaveId,
                            LeaveName = @LeaveName,
                            ActEvent = @ActEvent,
                            DeductionQty = @DeductionQty,
                            DeductionQtyDefaultEmpty = @DeductionQtyDefaultEmpty,
                            Status = @Status,
                            UpdatedBy = @UpdatedBy,
                            UpdatedAt = @UpdatedAt
                        WHERE TKey = @TKey";
                    var affectedRows = await connection.ExecuteAsync(updateSql, item, transaction);
                    if (affectedRows > 0)
                    {
                        result.UpdateCount++;
                    }
                    else
                    {
                        result.FailCount++;
                    }
                }

                // 刪除
                if (deleteTKeys.Any())
                {
                    const string deleteSql = "DELETE FROM BusinessReportPrintDetail WHERE TKey IN @TKeys";
                    var deleteCount = await connection.ExecuteAsync(deleteSql, new { TKeys = deleteTKeys }, transaction);
                    result.DeleteCount = deleteCount;
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("批次處理業務報表列印明細失敗", ex);
            throw;
        }
    }
}

