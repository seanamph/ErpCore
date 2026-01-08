namespace ErpCore.Domain.Entities.Recruitment;

/// <summary>
/// 潛客資料實體 (SYSC180)
/// </summary>
public class Prospect
{
    /// <summary>
    /// 潛客代碼
    /// </summary>
    public string ProspectId { get; set; } = string.Empty;

    /// <summary>
    /// 潛客名稱
    /// </summary>
    public string ProspectName { get; set; } = string.Empty;

    /// <summary>
    /// 聯絡人
    /// </summary>
    public string? ContactPerson { get; set; }

    /// <summary>
    /// 聯絡電話
    /// </summary>
    public string? ContactTel { get; set; }

    /// <summary>
    /// 傳真
    /// </summary>
    public string? ContactFax { get; set; }

    /// <summary>
    /// 電子郵件
    /// </summary>
    public string? ContactEmail { get; set; }

    /// <summary>
    /// 聯絡地址
    /// </summary>
    public string? ContactAddress { get; set; }

    /// <summary>
    /// 店名
    /// </summary>
    public string? StoreName { get; set; }

    /// <summary>
    /// 店電話
    /// </summary>
    public string? StoreTel { get; set; }

    /// <summary>
    /// 據點代碼
    /// </summary>
    public string? SiteId { get; set; }

    /// <summary>
    /// 招商代碼
    /// </summary>
    public string? RecruitId { get; set; }

    /// <summary>
    /// 店別代碼
    /// </summary>
    public string? StoreId { get; set; }

    /// <summary>
    /// 廠商代碼
    /// </summary>
    public string? VendorId { get; set; }

    /// <summary>
    /// 組織代碼
    /// </summary>
    public string? OrgId { get; set; }

    /// <summary>
    /// 業種代碼
    /// </summary>
    public string? BtypeId { get; set; }

    /// <summary>
    /// 銷售型態
    /// </summary>
    public string? SalesType { get; set; }

    /// <summary>
    /// 狀態 (PENDING:待訪談, INTERVIEWING:訪談中, SIGNED:已簽約, CANCELLED:已取消)
    /// </summary>
    public string Status { get; set; } = "PENDING";

    /// <summary>
    /// 整體狀態
    /// </summary>
    public string? OverallStatus { get; set; }

    /// <summary>
    /// 營業型態
    /// </summary>
    public string? PaperType { get; set; }

    /// <summary>
    /// 正櫃櫃別
    /// </summary>
    public string? LocationType { get; set; }

    /// <summary>
    /// 裝潢方式
    /// </summary>
    public string? DecoType { get; set; }

    /// <summary>
    /// 抽成別
    /// </summary>
    public string? CommType { get; set; }

    /// <summary>
    /// 抽成別
    /// </summary>
    public string? PdType { get; set; }

    /// <summary>
    /// 基本租金
    /// </summary>
    public decimal? BaseRent { get; set; }

    /// <summary>
    /// 保證金
    /// </summary>
    public decimal? Deposit { get; set; }

    /// <summary>
    /// 信用卡
    /// </summary>
    public string? CreditCard { get; set; }

    /// <summary>
    /// 目標金額(月)
    /// </summary>
    public decimal? TargetAmountM { get; set; }

    /// <summary>
    /// 目標金額(年)
    /// </summary>
    public decimal? TargetAmountV { get; set; }

    /// <summary>
    /// 運動費用
    /// </summary>
    public decimal? ExerciseFees { get; set; }

    /// <summary>
    /// 檢查日
    /// </summary>
    public int? CheckDay { get; set; }

    /// <summary>
    /// 會議日期起
    /// </summary>
    public DateTime? AgmDateB { get; set; }

    /// <summary>
    /// 會議日期迄
    /// </summary>
    public DateTime? AgmDateE { get; set; }

    /// <summary>
    /// 合約專案代碼起
    /// </summary>
    public string? ContractProidB { get; set; }

    /// <summary>
    /// 合約專案代碼迄
    /// </summary>
    public string? ContractProidE { get; set; }

    /// <summary>
    /// 回饋日期
    /// </summary>
    public DateTime? FeedbackDate { get; set; }

    /// <summary>
    /// 到期日
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// 聯絡日期
    /// </summary>
    public DateTime? ContactDate { get; set; }

    /// <summary>
    /// 版本號
    /// </summary>
    public string? VersionNo { get; set; }

    /// <summary>
    /// GUI ID
    /// </summary>
    public string? GuiId { get; set; }

    /// <summary>
    /// 銀行代碼
    /// </summary>
    public string? BankId { get; set; }

    /// <summary>
    /// 帳戶名稱
    /// </summary>
    public string? AccName { get; set; }

    /// <summary>
    /// 帳戶號碼
    /// </summary>
    public string? AccNo { get; set; }

    /// <summary>
    /// 發票電子郵件
    /// </summary>
    public string? InvEmail { get; set; }

    /// <summary>
    /// 是否使用業主商店代號 (Y:是, N:否)
    /// </summary>
    public string EdcYn { get; set; } = "N";

    /// <summary>
    /// 是否開立業主發票 (Y:是, N:否)
    /// </summary>
    public string ReceYn { get; set; } = "N";

    /// <summary>
    /// 是否使用業主收銀機 (Y:是, N:否)
    /// </summary>
    public string PosYn { get; set; } = "N";

    /// <summary>
    /// 是否為業主收現 (Y:是, N:否)
    /// </summary>
    public string CashYn { get; set; } = "N";

    /// <summary>
    /// 是否為抽成 (Y:是, N:否)
    /// </summary>
    public string CommYn { get; set; } = "N";

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

