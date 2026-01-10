namespace ErpCore.Domain.ValueObjects;

/// <summary>
/// 地址值物件
/// </summary>
public class Address
{
    /// <summary>
    /// 城市
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// 區域
    /// </summary>
    public string? Zone { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    public string? Street { get; set; }

    /// <summary>
    /// 郵遞區號
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// 完整地址
    /// </summary>
    public string FullAddress => $"{City}{Zone}{Street}";
}

