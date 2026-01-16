using System.Data;
using Dapper;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Inventory;

/// <summary>
/// 庫存 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class StockRepository : BaseRepository, IStockRepository
{
    public StockRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    /// <summary>
    /// 更新庫存數量
    /// 注意: 假設有 Stocks 資料表，包含 ShopId, GoodsId, Qty 欄位
    /// 如果實際資料表結構不同，請調整 SQL
    /// </summary>
    public async Task UpdateStockQtyAsync(string shopId, string goodsId, decimal qty, IDbTransaction? transaction = null)
    {
        try
        {
            // 使用 MERGE 語法，如果不存在則新增，存在則更新
            const string sql = @"
                MERGE INTO Stocks AS target
                USING (SELECT @ShopId AS ShopId, @GoodsId AS GoodsId, @Qty AS Qty) AS source
                ON target.ShopId = source.ShopId AND target.GoodsId = source.GoodsId
                WHEN MATCHED THEN
                    UPDATE SET Qty = Qty + @Qty, UpdatedAt = GETDATE()
                WHEN NOT MATCHED THEN
                    INSERT (ShopId, GoodsId, Qty, CreatedAt, UpdatedAt)
                    VALUES (@ShopId, @GoodsId, @Qty, GETDATE(), GETDATE());";

            var parameters = new { ShopId = shopId, GoodsId = goodsId, Qty = qty };

            if (transaction != null)
            {
                await transaction.Connection!.ExecuteAsync(sql, parameters, transaction);
            }
            else
            {
                await ExecuteAsync(sql, parameters);
            }

            _logger.LogInfo($"更新庫存成功: ShopId={shopId}, GoodsId={goodsId}, Qty={qty}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新庫存失敗: ShopId={shopId}, GoodsId={goodsId}, Qty={qty}", ex);
            throw;
        }
    }

    /// <summary>
    /// 取得庫存數量
    /// </summary>
    public async Task<decimal> GetStockQtyAsync(string shopId, string goodsId)
    {
        try
        {
            const string sql = @"
                SELECT ISNULL(Qty, 0) 
                FROM Stocks 
                WHERE ShopId = @ShopId AND GoodsId = @GoodsId";

            var qty = await ExecuteScalarAsync<decimal>(sql, new { ShopId = shopId, GoodsId = goodsId });
            return qty;
        }
        catch (Exception ex)
        {
            _logger.LogError($"取得庫存失敗: ShopId={shopId}, GoodsId={goodsId}", ex);
            throw;
        }
    }
}

