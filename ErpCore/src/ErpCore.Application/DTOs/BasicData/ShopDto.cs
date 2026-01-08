namespace ErpCore.Application.DTOs.BasicData;

/// <summary>
/// 店舖資訊 DTO
/// </summary>
public class ShopDto
{
    /// <summary>
    /// 店舖編號
    /// </summary>
    public string ShopId { get; set; } = string.Empty;

    /// <summary>
    /// 店舖名稱
    /// </summary>
    public string ShopName { get; set; } = string.Empty;

    /// <summary>
    /// 狀態
    /// </summary>
    public string? Status { get; set; }
}

/// <summary>
/// 店舖查詢 DTO
/// </summary>
public class ShopQueryDto
{
    /// <summary>
    /// 店舖編號
    /// </summary>
    public string? ShopId { get; set; }

    /// <summary>
    /// 店舖名稱（模糊查詢）
    /// </summary>
    public string? ShopName { get; set; }

    /// <summary>
    /// 狀態
    /// </summary>
    public string? Status { get; set; }
}

