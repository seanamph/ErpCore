namespace ErpCore.Domain.Entities.Recruitment;

/// <summary>
/// 潛客主檔資料實體 (SYSC165)
/// </summary>
public class ProspectMaster
{
    /// <summary>
    /// 主檔代碼
    /// </summary>
    public string MasterId { get; set; } = string.Empty;

    /// <summary>
    /// 主檔名稱
    /// </summary>
    public string MasterName { get; set; } = string.Empty;

    /// <summary>
    /// 主檔類型 (COMPANY:公司, INDIVIDUAL:個人, OTHER:其他)
    /// </summary>
    public string? MasterType { get; set; }

    /// <summary>
    /// 分類
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// 產業別
    /// </summary>
    public string? Industry { get; set; }

    /// <summary>
    /// 業種
    /// </summary>
    public string? BusinessType { get; set; }

    /// <summary>
    /// 狀態 (ACTIVE:有效, INACTIVE:無效, ARCHIVED:已歸檔)
    /// </summary>
    public string Status { get; set; } = "ACTIVE";

    /// <summary>
    /// 優先順序
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// 來源 (REFERRAL:推薦, ADVERTISEMENT:廣告, EXHIBITION:展覽, OTHER:其他)
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// 聯絡人
    /// </summary>
    public string? ContactPerson { get; set; }

    /// <summary>
    /// 聯絡電話
    /// </summary>
    public string? ContactTel { get; set; }

    /// <summary>
    /// 電子郵件
    /// </summary>
    public string? ContactEmail { get; set; }

    /// <summary>
    /// 聯絡地址
    /// </summary>
    public string? ContactAddress { get; set; }

    /// <summary>
    /// 網站
    /// </summary>
    public string? Website { get; set; }

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

    /// <summary>
    /// 建立者等級
    /// </summary>
    public int? CreatedPriority { get; set; }

    /// <summary>
    /// 建立者群組
    /// </summary>
    public string? CreatedGroup { get; set; }
}

