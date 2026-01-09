using System.Data;
using Dapper;
using ErpCore.Domain.Entities.StandardModule;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.StandardModule;

/// <summary>
/// STD5000 交易明細 Repository 實作 (SYS5310-SYS53C6 - 交易明細管理)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class Std5000TransactionDetailRepository : BaseRepository, IStd5000TransactionDetailRepository
{
    public Std5000TransactionDetailRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Std5000TransactionDetail?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Std5000TransactionDetails 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<Std5000TransactionDetail>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢STD5000交易明細失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<Std5000TransactionDetail>> GetByTransIdAsync(string transId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Std5000TransactionDetails 
                WHERE TransId = @TransId
                ORDER BY SeqNo ASC";

            return await QueryAsync<Std5000TransactionDetail>(sql, new { TransId = transId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢STD5000交易明細失敗: {transId}", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(Std5000TransactionDetail entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO Std5000TransactionDetails 
                (TransId, SeqNo, ProductId, ProductName, Qty, Price, Amount, Memo, CreatedBy, CreatedAt)
                VALUES 
                (@TransId, @SeqNo, @ProductId, @ProductName, @Qty, @Price, @Amount, @Memo, @CreatedBy, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var parameters = new DynamicParameters();
            parameters.Add("TransId", entity.TransId);
            parameters.Add("SeqNo", entity.SeqNo);
            parameters.Add("ProductId", entity.ProductId);
            parameters.Add("ProductName", entity.ProductName);
            parameters.Add("Qty", entity.Qty);
            parameters.Add("Price", entity.Price);
            parameters.Add("Amount", entity.Amount);
            parameters.Add("Memo", entity.Memo);
            parameters.Add("CreatedBy", entity.CreatedBy);
            parameters.Add("CreatedAt", entity.CreatedAt);

            return await QuerySingleAsync<long>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("新增STD5000交易明細失敗", ex);
            throw;
        }
    }

    public async Task UpdateAsync(Std5000TransactionDetail entity)
    {
        try
        {
            const string sql = @"
                UPDATE Std5000TransactionDetails 
                SET ProductId = @ProductId,
                    ProductName = @ProductName,
                    Qty = @Qty,
                    Price = @Price,
                    Amount = @Amount,
                    Memo = @Memo
                WHERE TKey = @TKey";

            var parameters = new DynamicParameters();
            parameters.Add("TKey", entity.TKey);
            parameters.Add("ProductId", entity.ProductId);
            parameters.Add("ProductName", entity.ProductName);
            parameters.Add("Qty", entity.Qty);
            parameters.Add("Price", entity.Price);
            parameters.Add("Amount", entity.Amount);
            parameters.Add("Memo", entity.Memo);

            await ExecuteAsync(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改STD5000交易明細失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM Std5000TransactionDetails 
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除STD5000交易明細失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteByTransIdAsync(string transId)
    {
        try
        {
            const string sql = @"
                DELETE FROM Std5000TransactionDetails 
                WHERE TransId = @TransId";

            await ExecuteAsync(sql, new { TransId = transId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除STD5000交易明細失敗: {transId}", ex);
            throw;
        }
    }
}

