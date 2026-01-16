using System.Data;

namespace ErpCore.Infrastructure.Repositories.Inventory;

/// <summary>
/// 庫存 Repository 介面
/// 用於更新庫存數量
/// </summary>
public interface IStockRepository
{
    /// <summary>
    /// 更新庫存數量
    /// </summary>
    /// <param name="shopId">分店代碼</param>
    /// <param name="goodsId">商品編號</param>
    /// <param name="qty">數量變更（正數為增加，負數為減少）</param>
    /// <param name="transaction">交易物件</param>
    Task UpdateStockQtyAsync(string shopId, string goodsId, decimal qty, IDbTransaction? transaction = null);

    /// <summary>
    /// 取得庫存數量
    /// </summary>
    Task<decimal> GetStockQtyAsync(string shopId, string goodsId);
}

