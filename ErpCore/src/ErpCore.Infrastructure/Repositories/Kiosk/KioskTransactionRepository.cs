using Dapper;
using ErpCore.Domain.Entities.Kiosk;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Kiosk;

/// <summary>
/// Kiosk交易 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class KioskTransactionRepository : BaseRepository, IKioskTransactionRepository
{
    public KioskTransactionRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<KioskTransaction?> GetByTransactionIdAsync(string transactionId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM KioskTransactions 
                WHERE TransactionId = @TransactionId";

            return await QueryFirstOrDefaultAsync<KioskTransaction>(sql, new { TransactionId = transactionId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢Kiosk交易失敗: {transactionId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<KioskTransaction>> QueryAsync(KioskTransactionQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM KioskTransactions
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.KioskId))
            {
                sql += " AND KioskId = @KioskId";
                parameters.Add("KioskId", query.KioskId);
            }

            if (!string.IsNullOrEmpty(query.FunctionCode))
            {
                sql += " AND FunctionCode = @FunctionCode";
                parameters.Add("FunctionCode", query.FunctionCode);
            }

            if (!string.IsNullOrEmpty(query.CardNumber))
            {
                sql += " AND CardNumber LIKE @CardNumber";
                parameters.Add("CardNumber", $"%{query.CardNumber}%");
            }

            if (!string.IsNullOrEmpty(query.MemberId))
            {
                sql += " AND MemberId = @MemberId";
                parameters.Add("MemberId", query.MemberId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (query.StartDate.HasValue)
            {
                sql += " AND CAST(TransactionDate AS DATE) >= @StartDate";
                parameters.Add("StartDate", query.StartDate.Value.Date);
            }

            if (query.EndDate.HasValue)
            {
                sql += " AND CAST(TransactionDate AS DATE) <= @EndDate";
                parameters.Add("EndDate", query.EndDate.Value.Date);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "TransactionDate" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<KioskTransaction>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM KioskTransactions
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.KioskId))
            {
                countSql += " AND KioskId = @KioskId";
                countParameters.Add("KioskId", query.KioskId);
            }
            if (!string.IsNullOrEmpty(query.FunctionCode))
            {
                countSql += " AND FunctionCode = @FunctionCode";
                countParameters.Add("FunctionCode", query.FunctionCode);
            }
            if (!string.IsNullOrEmpty(query.CardNumber))
            {
                countSql += " AND CardNumber LIKE @CardNumber";
                countParameters.Add("CardNumber", $"%{query.CardNumber}%");
            }
            if (!string.IsNullOrEmpty(query.MemberId))
            {
                countSql += " AND MemberId = @MemberId";
                countParameters.Add("MemberId", query.MemberId);
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }
            if (query.StartDate.HasValue)
            {
                countSql += " AND CAST(TransactionDate AS DATE) >= @StartDate";
                countParameters.Add("StartDate", query.StartDate.Value.Date);
            }
            if (query.EndDate.HasValue)
            {
                countSql += " AND CAST(TransactionDate AS DATE) <= @EndDate";
                countParameters.Add("EndDate", query.EndDate.Value.Date);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<KioskTransaction>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢Kiosk交易列表失敗", ex);
            throw;
        }
    }

    public async Task<KioskTransaction> CreateAsync(KioskTransaction transaction)
    {
        try
        {
            const string sql = @"
                INSERT INTO KioskTransactions 
                (TransactionId, KioskId, FunctionCode, CardNumber, MemberId, TransactionDate, 
                 RequestData, ResponseData, Status, ReturnCode, ErrorMessage, CreatedAt)
                VALUES 
                (@TransactionId, @KioskId, @FunctionCode, @CardNumber, @MemberId, @TransactionDate,
                 @RequestData, @ResponseData, @Status, @ReturnCode, @ErrorMessage, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var id = await QuerySingleAsync<long>(sql, transaction);
            transaction.Id = id;
            return transaction;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增Kiosk交易失敗", ex);
            throw;
        }
    }

    public async Task<KioskTransaction> UpdateAsync(KioskTransaction transaction)
    {
        try
        {
            const string sql = @"
                UPDATE KioskTransactions 
                SET ResponseData = @ResponseData,
                    Status = @Status,
                    ReturnCode = @ReturnCode,
                    ErrorMessage = @ErrorMessage
                WHERE Id = @Id";

            await ExecuteAsync(sql, transaction);
            return transaction;
        }
        catch (Exception ex)
        {
            _logger.LogError("修改Kiosk交易失敗", ex);
            throw;
        }
    }
}

