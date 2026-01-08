using Dapper;
using ErpCore.Domain.Entities.Pos;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Pos;

/// <summary>
/// POS交易 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class PosTransactionRepository : BaseRepository, IPosTransactionRepository
{
    public PosTransactionRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PosTransaction?> GetByTransactionIdAsync(string transactionId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM PosTransactions 
                WHERE TransactionId = @TransactionId";

            return await QueryFirstOrDefaultAsync<PosTransaction>(sql, new { TransactionId = transactionId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢POS交易失敗: {transactionId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<PosTransaction>> QueryAsync(PosTransactionQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM PosTransactions
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.TransactionId))
            {
                sql += " AND TransactionId LIKE @TransactionId";
                parameters.Add("TransactionId", $"%{query.TransactionId}%");
            }

            if (!string.IsNullOrEmpty(query.StoreId))
            {
                sql += " AND StoreId = @StoreId";
                parameters.Add("StoreId", query.StoreId);
            }

            if (!string.IsNullOrEmpty(query.PosId))
            {
                sql += " AND PosId = @PosId";
                parameters.Add("PosId", query.PosId);
            }

            if (query.StartDate.HasValue)
            {
                sql += " AND TransactionDate >= @StartDate";
                parameters.Add("StartDate", query.StartDate.Value);
            }

            if (query.EndDate.HasValue)
            {
                sql += " AND TransactionDate <= @EndDate";
                parameters.Add("EndDate", query.EndDate.Value);
            }

            if (!string.IsNullOrEmpty(query.TransactionType))
            {
                sql += " AND TransactionType = @TransactionType";
                parameters.Add("TransactionType", query.TransactionType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
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

            var items = await QueryAsync<PosTransaction>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM PosTransactions
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.TransactionId))
            {
                countSql += " AND TransactionId LIKE @TransactionId";
                countParameters.Add("TransactionId", $"%{query.TransactionId}%");
            }
            if (!string.IsNullOrEmpty(query.StoreId))
            {
                countSql += " AND StoreId = @StoreId";
                countParameters.Add("StoreId", query.StoreId);
            }
            if (!string.IsNullOrEmpty(query.PosId))
            {
                countSql += " AND PosId = @PosId";
                countParameters.Add("PosId", query.PosId);
            }
            if (query.StartDate.HasValue)
            {
                countSql += " AND TransactionDate >= @StartDate";
                countParameters.Add("StartDate", query.StartDate.Value);
            }
            if (query.EndDate.HasValue)
            {
                countSql += " AND TransactionDate <= @EndDate";
                countParameters.Add("EndDate", query.EndDate.Value);
            }
            if (!string.IsNullOrEmpty(query.TransactionType))
            {
                countSql += " AND TransactionType = @TransactionType";
                countParameters.Add("TransactionType", query.TransactionType);
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<PosTransaction>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢POS交易列表失敗", ex);
            throw;
        }
    }

    public async Task<PosTransaction> CreateAsync(PosTransaction transaction)
    {
        try
        {
            const string sql = @"
                INSERT INTO PosTransactions 
                    (TransactionId, StoreId, PosId, TransactionDate, TransactionType, 
                     TotalAmount, PaymentMethod, CustomerId, Status, CreatedAt, UpdatedAt)
                VALUES 
                    (@TransactionId, @StoreId, @PosId, @TransactionDate, @TransactionType, 
                     @TotalAmount, @PaymentMethod, @CustomerId, @Status, @CreatedAt, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var id = await ExecuteScalarAsync<long>(sql, new
            {
                transaction.TransactionId,
                transaction.StoreId,
                transaction.PosId,
                transaction.TransactionDate,
                transaction.TransactionType,
                transaction.TotalAmount,
                transaction.PaymentMethod,
                transaction.CustomerId,
                transaction.Status,
                transaction.CreatedAt,
                transaction.UpdatedAt
            });

            transaction.Id = id;
            return transaction;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增POS交易失敗: {transaction.TransactionId}", ex);
            throw;
        }
    }

    public async Task<PosTransaction> UpdateAsync(PosTransaction transaction)
    {
        try
        {
            const string sql = @"
                UPDATE PosTransactions 
                SET StoreId = @StoreId, PosId = @PosId, TransactionDate = @TransactionDate,
                    TransactionType = @TransactionType, TotalAmount = @TotalAmount,
                    PaymentMethod = @PaymentMethod, CustomerId = @CustomerId,
                    Status = @Status, SyncAt = @SyncAt, ErrorMessage = @ErrorMessage,
                    UpdatedAt = @UpdatedAt
                WHERE TransactionId = @TransactionId";

            await ExecuteAsync(sql, new
            {
                transaction.TransactionId,
                transaction.StoreId,
                transaction.PosId,
                transaction.TransactionDate,
                transaction.TransactionType,
                transaction.TotalAmount,
                transaction.PaymentMethod,
                transaction.CustomerId,
                transaction.Status,
                transaction.SyncAt,
                transaction.ErrorMessage,
                transaction.UpdatedAt
            });

            return transaction;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改POS交易失敗: {transaction.TransactionId}", ex);
            throw;
        }
    }

    public async Task<int> BatchCreateAsync(IEnumerable<PosTransaction> transactions)
    {
        try
        {
            const string sql = @"
                INSERT INTO PosTransactions 
                    (TransactionId, StoreId, PosId, TransactionDate, TransactionType, 
                     TotalAmount, PaymentMethod, CustomerId, Status, CreatedAt, UpdatedAt)
                VALUES 
                    (@TransactionId, @StoreId, @PosId, @TransactionDate, @TransactionType, 
                     @TotalAmount, @PaymentMethod, @CustomerId, @Status, @CreatedAt, @UpdatedAt)";

            return await ExecuteAsync(sql, transactions);
        }
        catch (Exception ex)
        {
            _logger.LogError("批次新增POS交易失敗", ex);
            throw;
        }
    }

    public async Task UpdateSyncStatusAsync(string transactionId, string status, DateTime? syncAt, string? errorMessage)
    {
        try
        {
            const string sql = @"
                UPDATE PosTransactions 
                SET Status = @Status, SyncAt = @SyncAt, ErrorMessage = @ErrorMessage, UpdatedAt = GETDATE()
                WHERE TransactionId = @TransactionId";

            await ExecuteAsync(sql, new
            {
                TransactionId = transactionId,
                Status = status,
                SyncAt = syncAt,
                ErrorMessage = errorMessage
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新POS交易同步狀態失敗: {transactionId}", ex);
            throw;
        }
    }
}

