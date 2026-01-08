using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Sales;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Sales;

/// <summary>
/// 銷售單明細 Repository 實作 (SYSD110-SYSD140)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class SalesOrderDetailRepository : BaseRepository, ISalesOrderDetailRepository
{
    public SalesOrderDetailRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<IEnumerable<SalesOrderDetail>> GetByOrderIdAsync(string orderId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM SalesOrderDetails 
                WHERE OrderId = @OrderId
                ORDER BY LineNum";

            return await QueryAsync<SalesOrderDetail>(sql, new { OrderId = orderId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢銷售單明細失敗: {orderId}", ex);
            throw;
        }
    }

    public async Task<SalesOrderDetail?> GetByOrderIdAndLineNumAsync(string orderId, int lineNum)
    {
        try
        {
            const string sql = @"
                SELECT * FROM SalesOrderDetails 
                WHERE OrderId = @OrderId AND LineNum = @LineNum";

            return await QueryFirstOrDefaultAsync<SalesOrderDetail>(sql, new { OrderId = orderId, LineNum = lineNum });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢銷售單明細失敗: {orderId}, LineNum: {lineNum}", ex);
            throw;
        }
    }

    public async Task<SalesOrderDetail> CreateAsync(SalesOrderDetail detail)
    {
        try
        {
            const string sql = @"
                INSERT INTO SalesOrderDetails (
                    OrderId, LineNum, GoodsId, BarcodeId, OrderQty, UnitPrice, Amount,
                    ShippedQty, ReturnQty, UnitId, DiscountRate, DiscountAmount,
                    TaxRate, TaxAmount, Memo, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                ) VALUES (
                    @OrderId, @LineNum, @GoodsId, @BarcodeId, @OrderQty, @UnitPrice, @Amount,
                    @ShippedQty, @ReturnQty, @UnitId, @DiscountRate, @DiscountAmount,
                    @TaxRate, @TaxAmount, @Memo, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, detail);
            detail.TKey = tKey;

            return detail;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增銷售單明細失敗: {detail.OrderId}, LineNum: {detail.LineNum}", ex);
            throw;
        }
    }

    public async Task<SalesOrderDetail> UpdateAsync(SalesOrderDetail detail)
    {
        try
        {
            const string sql = @"
                UPDATE SalesOrderDetails SET
                    GoodsId = @GoodsId,
                    BarcodeId = @BarcodeId,
                    OrderQty = @OrderQty,
                    UnitPrice = @UnitPrice,
                    Amount = @Amount,
                    ShippedQty = @ShippedQty,
                    ReturnQty = @ReturnQty,
                    UnitId = @UnitId,
                    DiscountRate = @DiscountRate,
                    DiscountAmount = @DiscountAmount,
                    TaxRate = @TaxRate,
                    TaxAmount = @TaxAmount,
                    Memo = @Memo,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE OrderId = @OrderId AND LineNum = @LineNum";

            await ExecuteAsync(sql, detail);
            return detail;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改銷售單明細失敗: {detail.OrderId}, LineNum: {detail.LineNum}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string orderId, int lineNum)
    {
        try
        {
            const string sql = @"
                DELETE FROM SalesOrderDetails 
                WHERE OrderId = @OrderId AND LineNum = @LineNum";

            await ExecuteAsync(sql, new { OrderId = orderId, LineNum = lineNum });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除銷售單明細失敗: {orderId}, LineNum: {lineNum}", ex);
            throw;
        }
    }

    public async Task DeleteByOrderIdAsync(string orderId)
    {
        try
        {
            const string sql = @"
                DELETE FROM SalesOrderDetails 
                WHERE OrderId = @OrderId";

            await ExecuteAsync(sql, new { OrderId = orderId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除銷售單所有明細失敗: {orderId}", ex);
            throw;
        }
    }
}

