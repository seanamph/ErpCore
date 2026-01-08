namespace ErpCore.Application.DTOs.Purchase;

/// <summary>
/// 現場打單作業 - 根據條碼查詢商品 DTO
/// </summary>
public class GoodsByBarcodeDto
{
    /// <summary>
    /// 商品編號
    /// </summary>
    public string GoodsId { get; set; } = string.Empty;

    /// <summary>
    /// 商品名稱
    /// </summary>
    public string GoodsName { get; set; } = string.Empty;

    /// <summary>
    /// 條碼編號
    /// </summary>
    public string BarcodeId { get; set; } = string.Empty;

    /// <summary>
    /// 單價
    /// </summary>
    public decimal? UnitPrice { get; set; }

    /// <summary>
    /// 單位
    /// </summary>
    public string? Unit { get; set; }
}

