namespace ErpCore.Domain.Entities.CustomerInvoice;

/// <summary>
/// 客戶資料實體 (SYS2000 - 客戶資料維護)
/// </summary>
public class CustomerData
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 客戶編號
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// 客戶名稱
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// 客戶類型 (C:客戶, V:廠商)
    /// </summary>
    public string? CustomerType { get; set; }

    /// <summary>
    /// 統一編號
    /// </summary>
    public string? TaxId { get; set; }

    /// <summary>
    /// 聯絡人
    /// </summary>
    public string? ContactPerson { get; set; }

    /// <summary>
    /// 聯絡電話
    /// </summary>
    public string? ContactPhone { get; set; }

    /// <summary>
    /// 聯絡信箱
    /// </summary>
    public string? ContactEmail { get; set; }

    /// <summary>
    /// 傳真號碼
    /// </summary>
    public string? ContactFax { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// 城市代碼
    /// </summary>
    public string? CityId { get; set; }

    /// <summary>
    /// 區域代碼
    /// </summary>
    public string? ZoneId { get; set; }

    /// <summary>
    /// 郵遞區號
    /// </summary>
    public string? ZipCode { get; set; }

    /// <summary>
    /// 付款條件
    /// </summary>
    public string? PaymentTerm { get; set; }

    /// <summary>
    /// 信用額度
    /// </summary>
    public decimal CreditLimit { get; set; }

    /// <summary>
    /// 幣別
    /// </summary>
    public string CurrencyId { get; set; } = "TWD";

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// 備註
    /// </summary>
    public string? Memo { get; set; }

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

/// <summary>
/// 客戶聯絡人實體
/// </summary>
public class CustomerContact
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 客戶編號
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// 聯絡人姓名
    /// </summary>
    public string ContactName { get; set; } = string.Empty;

    /// <summary>
    /// 職稱
    /// </summary>
    public string? ContactTitle { get; set; }

    /// <summary>
    /// 電話
    /// </summary>
    public string? ContactPhone { get; set; }

    /// <summary>
    /// 手機
    /// </summary>
    public string? ContactMobile { get; set; }

    /// <summary>
    /// 信箱
    /// </summary>
    public string? ContactEmail { get; set; }

    /// <summary>
    /// 傳真
    /// </summary>
    public string? ContactFax { get; set; }

    /// <summary>
    /// 是否主要聯絡人
    /// </summary>
    public bool IsPrimary { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Memo { get; set; }

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

/// <summary>
/// 客戶地址實體
/// </summary>
public class CustomerAddress
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 客戶編號
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// 地址類型 (BILLING:帳單地址, SHIPPING:送貨地址, REGISTERED:登記地址)
    /// </summary>
    public string AddressType { get; set; } = string.Empty;

    /// <summary>
    /// 地址
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// 城市代碼
    /// </summary>
    public string? CityId { get; set; }

    /// <summary>
    /// 區域代碼
    /// </summary>
    public string? ZoneId { get; set; }

    /// <summary>
    /// 郵遞區號
    /// </summary>
    public string? ZipCode { get; set; }

    /// <summary>
    /// 是否預設地址
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Memo { get; set; }

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

/// <summary>
/// 客戶銀行帳戶實體
/// </summary>
public class CustomerBankAccount
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 客戶編號
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// 銀行代碼
    /// </summary>
    public string BankId { get; set; } = string.Empty;

    /// <summary>
    /// 帳號
    /// </summary>
    public string AccountNo { get; set; } = string.Empty;

    /// <summary>
    /// 戶名
    /// </summary>
    public string? AccountName { get; set; }

    /// <summary>
    /// 是否預設帳戶
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Memo { get; set; }

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

