using System.Data;
using Dapper;
using ErpCore.Domain.Entities.InvoiceSalesB2B;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.InvoiceSalesB2B;

/// <summary>
/// B2B銷售單明細 Repository 實作 (SYSG000_B2B - B2B銷售資料維護)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class B2BSalesOrderDetailRepository : BaseRepository, IB2BSalesOrderDetailRepository
{
    public B2BSalesOrderDetailRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<List<B2BSalesOrderDetail>> GetByOrderIdAsync(string orderId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM B2BSalesOrderDetails 
                WHERE OrderId = @OrderId
                ORDER BY LineNum";

            var items = await QueryAsync<B2BSalesOrderDetail>(sql, new { OrderId = orderId });
            return items.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢B2B銷售單明細失敗: {orderId}", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(B2BSalesOrderDetail detail)
    {
        try
        {
            const string sql = @"
                INSERT INTO B2BSalesOrderDetails (
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
            _logger.LogError($"新增B2B銷售單明細失敗: {detail.OrderId}", ex);
            throw;
        }
    }

    public async Task<int> UpdateAsync(B2BSalesOrderDetail detail)
    {
        try
        {
            const string sql = @"
                UPDATE B2BSalesOrderDetails SET
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
            _logger.LogError($"修改B2B銷售單明細失敗: {detail.TKey}", ex);
            throw;
        }
    }

    public async Task<int> DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM B2BSalesOrderDetails 
                WHERE TKey = @TKey";

            return await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除B2B銷售單明細失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<int> DeleteByOrderIdAsync(string orderId)
    {
        try
        {
            const string sql = @"
                DELETE FROM B2BSalesOrderDetails 
                WHERE OrderId = @OrderId";

            return await ExecuteAsync(sql, new { OrderId = orderId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除B2B銷售單明細失敗: {orderId}", ex);
            throw;
        }
    }
}

