using System.Data;

namespace ErpCore.Infrastructure.Repositories.Transfer;

/// <summary>
/// 調撥單 Repository 介面
/// 用於更新調撥單已退數量
/// </summary>
public interface ITransferOrderRepository
{
    /// <summary>
    /// 更新調撥單明細已退數量
    /// </summary>
    /// <param name="transferDetailId">調撥單明細ID</param>
    /// <param name="returnQty">驗退數量</param>
    /// <param name="transaction">交易物件</param>
    Task UpdateReturnQtyAsync(Guid transferDetailId, decimal returnQty, global::System.Data.IDbTransaction? transaction = null);

    /// <summary>
    /// 更新調撥單明細已收數量
    /// </summary>
    /// <param name="transferDetailId">調撥單明細ID</param>
    /// <param name="receiptQty">驗收數量</param>
    /// <param name="transaction">交易物件</param>
    Task UpdateReceiptQtyAsync(Guid transferDetailId, decimal receiptQty, global::System.Data.IDbTransaction? transaction = null);

    /// <summary>
    /// 取得調撥單資料（含明細）
    /// </summary>
    Task<TransferOrderInfo?> GetTransferOrderAsync(string transferId);

    /// <summary>
    /// 取得調撥單明細資料
    /// </summary>
    Task<IEnumerable<TransferOrderDetailInfo>> GetTransferOrderDetailsAsync(string transferId);
}

/// <summary>
/// 調撥單資訊
/// </summary>
public class TransferOrderInfo
{
    public string TransferId { get; set; } = string.Empty;
    public DateTime TransferDate { get; set; }
    public string FromShopId { get; set; } = string.Empty;
    public string ToShopId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// 調撥單明細資訊
/// </summary>
public class TransferOrderDetailInfo
{
    public Guid DetailId { get; set; }
    public string TransferId { get; set; } = string.Empty;
    public string GoodsId { get; set; } = string.Empty;
    public decimal TransferQty { get; set; }
    public decimal? ReturnQty { get; set; }
    public decimal? ReceiptQty { get; set; }
}

