namespace ErpCore.Application.DTOs.Inventory;

/// <summary>
/// 商品查詢 DTO
/// </summary>
public class ProductQueryDto
{
    /// <summary>
    /// 商品編號
    /// </summary>
    public string? GoodsId { get; set; }

    /// <summary>
    /// 商品名稱（模糊查詢）
    /// </summary>
    public string? GoodsName { get; set; }

    /// <summary>
    /// 條碼
    /// </summary>
    public string? BarcodeId { get; set; }

    /// <summary>
    /// 狀態（1:正常, 2:停用）
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每頁筆數
    /// </summary>
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// 商品資訊 DTO
/// </summary>
public class ProductDto
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
    /// 進價
    /// </summary>
    public decimal Lprc { get; set; }

    /// <summary>
    /// 中價（售價）
    /// </summary>
    public decimal Mprc { get; set; }

    /// <summary>
    /// 條碼
    /// </summary>
    public string? BarcodeId { get; set; }

    /// <summary>
    /// 單位
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// 狀態
    /// </summary>
    public string Status { get; set; } = "1";
}

