using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Loyalty;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Loyalty;

/// <summary>
/// 忠誠度點數交易 Repository 實作 (LPS - 忠誠度系統維護)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class LoyaltyPointTransactionRepository : BaseRepository, ILoyaltyPointTransactionRepository
{
    public LoyaltyPointTransactionRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<LoyaltyPointTransaction?> GetByRrnAsync(string rrn)
    {
        try
        {
            const string sql = @"
                SELECT * FROM LoyaltyPointTransactions 
                WHERE RRN = @RRN";

            return await QueryFirstOrDefaultAsync<LoyaltyPointTransaction>(sql, new { RRN = rrn });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢忠誠度點數交易失敗: {rrn}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<LoyaltyPointTransaction>> QueryAsync(LoyaltyPointTransactionQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM LoyaltyPointTransactions
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.RRN))
            {
                sql += " AND RRN LIKE @RRN";
                parameters.Add("RRN", $"%{query.RRN}%");
            }

            if (!string.IsNullOrEmpty(query.CardNo))
            {
                sql += " AND CardNo = @CardNo";
                parameters.Add("CardNo", query.CardNo);
            }

            if (!string.IsNullOrEmpty(query.TransType))
            {
                sql += " AND TransType = @TransType";
                parameters.Add("TransType", query.TransType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (query.TransTimeFrom.HasValue)
            {
                sql += " AND TransTime >= @TransTimeFrom";
                parameters.Add("TransTimeFrom", query.TransTimeFrom.Value);
            }

            if (query.TransTimeTo.HasValue)
            {
                sql += " AND TransTime <= @TransTimeTo";
                parameters.Add("TransTimeTo", query.TransTimeTo.Value);
            }

            // 排序
            if (!string.IsNullOrEmpty(query.SortField))
            {
                var sortOrder = query.SortOrder == "DESC" ? "DESC" : "ASC";
                sql += $" ORDER BY {query.SortField} {sortOrder}";
            }
            else
            {
                sql += " ORDER BY TransTime DESC";
            }

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<LoyaltyPointTransaction>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢忠誠度點數交易列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(LoyaltyPointTransactionQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM LoyaltyPointTransactions
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.RRN))
            {
                sql += " AND RRN LIKE @RRN";
                parameters.Add("RRN", $"%{query.RRN}%");
            }

            if (!string.IsNullOrEmpty(query.CardNo))
            {
                sql += " AND CardNo = @CardNo";
                parameters.Add("CardNo", query.CardNo);
            }

            if (!string.IsNullOrEmpty(query.TransType))
            {
                sql += " AND TransType = @TransType";
                parameters.Add("TransType", query.TransType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (query.TransTimeFrom.HasValue)
            {
                sql += " AND TransTime >= @TransTimeFrom";
                parameters.Add("TransTimeFrom", query.TransTimeFrom.Value);
            }

            if (query.TransTimeTo.HasValue)
            {
                sql += " AND TransTime <= @TransTimeTo";
                parameters.Add("TransTimeTo", query.TransTimeTo.Value);
            }

            return await QuerySingleAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢忠誠度點數交易數量失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(LoyaltyPointTransaction entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO LoyaltyPointTransactions 
                (RRN, CardNo, TraceNo, ExpDate, AwardPoints, RedeemPoints, ReversalFlag, Amount, VoidFlag, 
                 AuthCode, ForceDate, Invoice, TransType, TxnType, TransTime, Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@RRN, @CardNo, @TraceNo, @ExpDate, @AwardPoints, @RedeemPoints, @ReversalFlag, @Amount, @VoidFlag, 
                 @AuthCode, @ForceDate, @Invoice, @TransType, @TxnType, @TransTime, @Status, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as bigint)";

            var tKey = await QuerySingleAsync<long>(sql, entity);
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增忠誠度點數交易失敗: {entity.RRN}", ex);
            throw;
        }
    }

    public async Task UpdateAsync(LoyaltyPointTransaction entity)
    {
        try
        {
            const string sql = @"
                UPDATE LoyaltyPointTransactions 
                SET CardNo = @CardNo, 
                    TraceNo = @TraceNo, 
                    ExpDate = @ExpDate, 
                    AwardPoints = @AwardPoints, 
                    RedeemPoints = @RedeemPoints, 
                    ReversalFlag = @ReversalFlag, 
                    Amount = @Amount, 
                    VoidFlag = @VoidFlag, 
                    AuthCode = @AuthCode, 
                    ForceDate = @ForceDate, 
                    Invoice = @Invoice, 
                    TransType = @TransType, 
                    TxnType = @TxnType, 
                    TransTime = @TransTime, 
                    Status = @Status, 
                    UpdatedBy = @UpdatedBy, 
                    UpdatedAt = @UpdatedAt
                WHERE RRN = @RRN";

            await ExecuteAsync(sql, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改忠誠度點數交易失敗: {entity.RRN}", ex);
            throw;
        }
    }

    public async Task VoidTransactionAsync(string rrn, string reversalFlag, string voidFlag)
    {
        try
        {
            const string sql = @"
                UPDATE LoyaltyPointTransactions 
                SET ReversalFlag = @ReversalFlag, 
                    VoidFlag = @VoidFlag, 
                    Status = 'VOIDED', 
                    UpdatedAt = GETDATE()
                WHERE RRN = @RRN";

            await ExecuteAsync(sql, new { RRN = rrn, ReversalFlag = reversalFlag, VoidFlag = voidFlag });
        }
        catch (Exception ex)
        {
            _logger.LogError($"取消忠誠度點數交易失敗: {rrn}", ex);
            throw;
        }
    }

    public async Task<string> GenerateRrnAsync()
    {
        try
        {
            const string sql = @"
                SELECT TOP 1 RRN FROM LoyaltyPointTransactions 
                WHERE RRN LIKE 'RRN' + FORMAT(GETDATE(), 'yyyyMMdd') + '%'
                ORDER BY RRN DESC";

            var lastRrn = await QueryFirstOrDefaultAsync<string>(sql);

            if (string.IsNullOrEmpty(lastRrn))
            {
                return $"RRN{DateTime.Now:yyyyMMdd}001";
            }

            var sequence = int.Parse(lastRrn.Substring(lastRrn.Length - 3)) + 1;
            return $"RRN{DateTime.Now:yyyyMMdd}{sequence:D3}";
        }
        catch (Exception ex)
        {
            _logger.LogError("產生交易編號失敗", ex);
            throw;
        }
    }
}

