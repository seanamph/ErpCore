using Dapper;
using ErpCore.Domain.Entities.Pos;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Pos;

/// <summary>
/// POS交易明細 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class PosTransactionDetailRepository : BaseRepository, IPosTransactionDetailRepository
{
    public PosTransactionDetailRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<IEnumerable<PosTransactionDetail>> GetByTransactionIdAsync(string transactionId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM PosTransactionDetails 
                WHERE TransactionId = @TransactionId
                ORDER BY LineNo";

            return await QueryAsync<PosTransactionDetail>(sql, new { TransactionId = transactionId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢POS交易明細失敗: {transactionId}", ex);
            throw;
        }
    }

    public async Task<PosTransactionDetail> CreateAsync(PosTransactionDetail detail)
    {
        try
        {
            const string sql = @"
                INSERT INTO PosTransactionDetails 
                    (TransactionId, LineNo, ProductId, ProductName, Quantity, 
                     UnitPrice, Amount, Discount, CreatedAt)
                VALUES 
                    (@TransactionId, @LineNo, @ProductId, @ProductName, @Quantity, 
                     @UnitPrice, @Amount, @Discount, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var id = await ExecuteScalarAsync<long>(sql, new
            {
                detail.TransactionId,
                detail.LineNo,
                detail.ProductId,
                detail.ProductName,
                detail.Quantity,
                detail.UnitPrice,
                detail.Amount,
                detail.Discount,
                detail.CreatedAt
            });

            detail.Id = id;
            return detail;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增POS交易明細失敗: {detail.TransactionId}", ex);
            throw;
        }
    }

    public async Task<int> BatchCreateAsync(IEnumerable<PosTransactionDetail> details)
    {
        try
        {
            const string sql = @"
                INSERT INTO PosTransactionDetails 
                    (TransactionId, LineNo, ProductId, ProductName, Quantity, 
                     UnitPrice, Amount, Discount, CreatedAt)
                VALUES 
                    (@TransactionId, @LineNo, @ProductId, @ProductName, @Quantity, 
                     @UnitPrice, @Amount, @Discount, @CreatedAt)";

            return await ExecuteAsync(sql, details);
        }
        catch (Exception ex)
        {
            _logger.LogError("批次新增POS交易明細失敗", ex);
            throw;
        }
    }
}

