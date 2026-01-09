namespace ErpCore.Domain.Entities.SystemExtensionH;

/// <summary>
/// 員工感應卡主檔 (SYSPH00 - 系統擴展PH)
/// </summary>
public class EmpCard
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 感應卡號
    /// </summary>
    public string CardNo { get; set; } = string.Empty;

    /// <summary>
    /// 員工代號
    /// </summary>
    public string EmpId { get; set; } = string.Empty;

    /// <summary>
    /// 開始日期
    /// </summary>
    public DateTime? BeginDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// 卡片狀態 (1:啟用, 0:停用)
    /// </summary>
    public string CardStatus { get; set; } = "1";

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? BUser { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime BTime { get; set; }

    /// <summary>
    /// 更新者
    /// </summary>
    public string? CUser { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime? CTime { get; set; }

    /// <summary>
    /// 建立者等級
    /// </summary>
    public int? CPriority { get; set; }

    /// <summary>
    /// 建立者群組
    /// </summary>
    public string? CGroup { get; set; }
}

