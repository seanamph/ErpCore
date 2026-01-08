namespace ErpCore.Domain.Entities.BasicData;

/// <summary>
/// 廠商資料實體 (SYSB206)
/// </summary>
public class Vendor
{
    /// <summary>
    /// 廠商編號
    /// </summary>
    public string VendorId { get; set; } = string.Empty;

    /// <summary>
    /// 統一編號/身份證字號/自編編號
    /// </summary>
    public string GuiId { get; set; } = string.Empty;

    /// <summary>
    /// 識別類型 (1:統一編號, 2:身份證字號, 3:自編編號)
    /// </summary>
    public string GuiType { get; set; } = "1";

    /// <summary>
    /// 廠商名稱(中文)
    /// </summary>
    public string VendorName { get; set; } = string.Empty;

    /// <summary>
    /// 廠商名稱(英文)
    /// </summary>
    public string? VendorNameE { get; set; }

    /// <summary>
    /// 廠商簡稱
    /// </summary>
    public string? VendorNameS { get; set; }

    /// <summary>
    /// 郵電費負擔
    /// </summary>
    public string? Mcode { get; set; }

    /// <summary>
    /// 公司登記地址
    /// </summary>
    public string? VendorRegAddr { get; set; }

    /// <summary>
    /// 發票地址
    /// </summary>
    public string? TaxAddr { get; set; }

    /// <summary>
    /// 公司登記電話
    /// </summary>
    public string? VendorRegTel { get; set; }

    /// <summary>
    /// 公司傳真
    /// </summary>
    public string? VendorFax { get; set; }

    /// <summary>
    /// 公司電子郵件
    /// </summary>
    public string? VendorEmail { get; set; }

    /// <summary>
    /// 發票電子郵件
    /// </summary>
    public string? InvEmail { get; set; }

    /// <summary>
    /// 聯絡人
    /// </summary>
    public string? ChargeStaff { get; set; }

    /// <summary>
    /// 聯絡人職稱
    /// </summary>
    public string? ChargeTitle { get; set; }

    /// <summary>
    /// 聯絡人身份證字號
    /// </summary>
    public string? ChargePid { get; set; }

    /// <summary>
    /// 聯絡人電話
    /// </summary>
    public string? ChargeTel { get; set; }

    /// <summary>
    /// 聯絡人聯絡地址
    /// </summary>
    public string? ChargeAddr { get; set; }

    /// <summary>
    /// 聯絡人電子郵件
    /// </summary>
    public string? ChargeEmail { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// 系統別
    /// </summary>
    public string SysId { get; set; } = "1";

    /// <summary>
    /// 付款方式
    /// </summary>
    public string? PayType { get; set; }

    /// <summary>
    /// 匯款銀行代碼
    /// </summary>
    public string? SuplBankId { get; set; }

    /// <summary>
    /// 匯款銀行帳號
    /// </summary>
    public string? SuplBankAcct { get; set; }

    /// <summary>
    /// 帳戶名稱
    /// </summary>
    public string? SuplAcctName { get; set; }

    /// <summary>
    /// 票據別
    /// </summary>
    public string? TicketBe { get; set; }

    /// <summary>
    /// 支票抬頭
    /// </summary>
    public string? CheckTitle { get; set; }

    /// <summary>
    /// 組織代碼
    /// </summary>
    public string? OrgId { get; set; }

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

