namespace ErpCore.Domain.Entities.BusinessReport;

/// <summary>
/// 員餐卡欄位資料實體 (SYSL206/SYSL207)
/// </summary>
public class EmployeeMealCardField
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 欄位ID
    /// </summary>
    public string FieldId { get; set; } = string.Empty;

    /// <summary>
    /// 欄位名稱
    /// </summary>
    public string? FieldName { get; set; }

    /// <summary>
    /// 卡片類型
    /// </summary>
    public string? CardType { get; set; }

    /// <summary>
    /// 動作類型
    /// </summary>
    public string? ActionType { get; set; }

    /// <summary>
    /// 其他類型
    /// </summary>
    public string? OtherType { get; set; }

    /// <summary>
    /// 必填標誌 (Y:必填, N:非必填)
    /// </summary>
    public string? MustKeyinYn { get; set; }

    /// <summary>
    /// 唯讀標誌 (Y:唯讀, N:可編輯)
    /// </summary>
    public string? ReadonlyYn { get; set; }

    /// <summary>
    /// 按鈕標誌 (Y:顯示按鈕, N:不顯示)
    /// </summary>
    public string? BtnEtekYn { get; set; }

    /// <summary>
    /// 排序序號
    /// </summary>
    public int? SeqNo { get; set; }

    /// <summary>
    /// 狀態 (1:啟用, 0:停用)
    /// </summary>
    public string Status { get; set; } = "1";

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }

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

