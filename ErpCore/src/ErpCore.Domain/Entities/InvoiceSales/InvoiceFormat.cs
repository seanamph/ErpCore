namespace ErpCore.Domain.Entities.InvoiceSales;

/// <summary>
/// 發票格式代號實體 (SYSG140 - 發票格式代號維護)
/// </summary>
public class InvoiceFormat
{
    /// <summary>
    /// 格式代號
    /// </summary>
    public string FormatId { get; set; } = string.Empty;

    /// <summary>
    /// 格式名稱
    /// </summary>
    public string FormatName { get; set; } = string.Empty;

    /// <summary>
    /// 格式英文名稱
    /// </summary>
    public string? FormatNameEn { get; set; }

    /// <summary>
    /// 說明
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新者
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

