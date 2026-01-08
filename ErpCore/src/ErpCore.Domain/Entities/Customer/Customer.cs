namespace ErpCore.Domain.Entities.Customer;

/// <summary>
/// 客戶資料實體 (CUS5110)
/// </summary>
public class Customer
{
    /// <summary>
    /// 客戶編號
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// 統一編號/身份證字號
    /// </summary>
    public string? GuiId { get; set; }

    /// <summary>
    /// 識別類型 (1:統一編號, 2:身份證字號, 3:自編編號)
    /// </summary>
    public string? GuiType { get; set; }

    /// <summary>
    /// 客戶名稱
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// 客戶名稱(英文)
    /// </summary>
    public string? CustomerNameE { get; set; }

    /// <summary>
    /// 簡稱/抬頭
    /// </summary>
    public string? ShortName { get; set; }

    /// <summary>
    /// 聯絡人
    /// </summary>
    public string? ContactStaff { get; set; }

    /// <summary>
    /// 住家電話
    /// </summary>
    public string? HomeTel { get; set; }

    /// <summary>
    /// 公司電話
    /// </summary>
    public string? CompTel { get; set; }

    /// <summary>
    /// 傳真
    /// </summary>
    public string? Fax { get; set; }

    /// <summary>
    /// 手機
    /// </summary>
    public string? Cell { get; set; }

    /// <summary>
    /// 電子郵件
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 性別 (M:男, F:女)
    /// </summary>
    public string? Sex { get; set; }

    /// <summary>
    /// 稱謂/職稱
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// 區域
    /// </summary>
    public string? Canton { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    public string? Addr { get; set; }

    /// <summary>
    /// 發票地址
    /// </summary>
    public string? TaxAddr { get; set; }

    /// <summary>
    /// 送貨地址
    /// </summary>
    public string? DelyAddr { get; set; }

    /// <summary>
    /// 郵遞區號
    /// </summary>
    public string? PostId { get; set; }

    /// <summary>
    /// 是否享有折扣 (Y:是, N:否)
    /// </summary>
    public string? DiscountYn { get; set; }

    /// <summary>
    /// 折扣類別代碼
    /// </summary>
    public string? DiscountNo { get; set; }

    /// <summary>
    /// 業務員
    /// </summary>
    public string? SalesId { get; set; }

    /// <summary>
    /// 最近交易日期
    /// </summary>
    public DateTime? TransDate { get; set; }

    /// <summary>
    /// 最近交易序號
    /// </summary>
    public string? TransNo { get; set; }

    /// <summary>
    /// 消費累積金額
    /// </summary>
    public decimal AccAmt { get; set; }

    /// <summary>
    /// 是否為月結客戶 (Y:是, N:否)
    /// </summary>
    public string? MonthlyYn { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

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

