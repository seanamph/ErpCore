using System.Data;
using Dapper;
using ErpCore.Domain.Entities.InvoiceSales;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.InvoiceSales;

/// <summary>
/// 銷售單明細 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class SalesOrderDetailRepository : BaseRepository, ISalesOrderDetailRepository
{
    public SalesOrderDetailRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<List<SalesOrderDetail>> GetByOrderIdAsync(string orderId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM SalesOrderDetails 
                WHERE OrderId = @OrderId
                ORDER BY LineNum";

            var items = await QueryAsync<SalesOrderDetail>(sql, new { OrderId = orderId });
            return items.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢銷售單明細失敗: {orderId}", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(SalesOrderDetail detail)
    {
        try
        {
            const string sql = @"
                INSERT INTO SalesOrderDetails (
                    OrderId, LineNum, GoodsId, BarcodeId, OrderQty, UnitPrice,
                    Amount, ShippedQty, ReturnQty, UnitId, TaxRate, TaxAmount, Memo,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                )
                VALUES (
                    @OrderId, @LineNum, @GoodsId, @BarcodeId, @OrderQty, @UnitPrice,
                    @Amount, @ShippedQty, @ReturnQty, @UnitId, @TaxRate, @TaxAmount, @Memo,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, detail);
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增銷售單明細失敗: {detail.OrderId}", ex);
            throw;
        }
    }

    public async Task<int> UpdateAsync(SalesOrderDetail detail)
    {
        try
        {
            const string sql = @"
                UPDATE SalesOrderDetails SET
                    OrderId = @OrderId,
                    LineNum = @LineNum,
                    GoodsId = @GoodsId,
                    BarcodeId = @BarcodeId,
                    OrderQty = @OrderQty,
                    UnitPrice = @UnitPrice,
                    Amount = @Amount,
                    ShippedQty = @ShippedQty,
                    ReturnQty = @ReturnQty,
                    UnitId = @UnitId,
                    TaxRate = @TaxRate,
                    TaxAmount = @TaxAmount,
                    Memo = @Memo,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            return await ExecuteAsync(sql, detail);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改銷售單明細失敗: {detail.TKey}", ex);
            throw;
        }
    }

    public async Task<int> DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM SalesOrderDetails 
                WHERE TKey = @TKey";

            return await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除銷售單明細失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<int> DeleteByOrderIdAsync(string orderId)
    {
        try
        {
            const string sql = @"
                DELETE FROM SalesOrderDetails 
                WHERE OrderId = @OrderId";

            return await ExecuteAsync(sql, new { OrderId = orderId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除銷售單明細失敗: {orderId}", ex);
            throw;
        }
    }
}

