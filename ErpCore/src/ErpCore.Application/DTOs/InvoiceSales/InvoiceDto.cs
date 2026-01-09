namespace ErpCore.Application.DTOs.InvoiceSales;

/// <summary>
/// 發票 DTO (SYSG110-SYSG190 - 發票資料維護)
/// </summary>
public class InvoiceDto
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 發票編號
    /// </summary>
    public string InvoiceId { get; set; } = string.Empty;

    /// <summary>
    /// 發票類型 (01:統一發票, 02:電子發票, 03:收據)
    /// </summary>
    public string InvoiceType { get; set; } = string.Empty;

    /// <summary>
    /// 發票年份
    /// </summary>
    public int InvoiceYear { get; set; }

    /// <summary>
    /// 發票月份
    /// </summary>
    public int InvoiceMonth { get; set; }

    /// <summary>
    /// 發票年月 (YYYYMM格式)
    /// </summary>
    public string InvoiceYm { get; set; } = string.Empty;

    /// <summary>
    /// 字軌
    /// </summary>
    public string? Track { get; set; }

    /// <summary>
    /// 發票號碼起
    /// </summary>
    public string? InvoiceNoB { get; set; }

    /// <summary>
    /// 發票號碼迄
    /// </summary>
    public string? InvoiceNoE { get; set; }

    /// <summary>
    /// 發票格式代號
    /// </summary>
    public string? InvoiceFormat { get; set; }

    /// <summary>
    /// 統一編號
    /// </summary>
    public string? TaxId { get; set; }

    /// <summary>
    /// 公司名稱
    /// </summary>
    public string? CompanyName { get; set; }

    /// <summary>
    /// 公司英文名稱
    /// </summary>
    public string? CompanyNameEn { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// 區域
    /// </summary>
    public string? Zone { get; set; }

    /// <summary>
    /// 郵遞區號
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// 電話
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 傳真
    /// </summary>
    public string? Fax { get; set; }

    /// <summary>
    /// 電子郵件
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 分公司代碼
    /// </summary>
    public string? SiteId { get; set; }

    /// <summary>
    /// 副聯
    /// </summary>
    public string? SubCopy { get; set; }

    /// <summary>
    /// 副聯值
    /// </summary>
    public string? SubCopyValue { get; set; }

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
}

/// <summary>
/// 發票查詢 DTO
/// </summary>
public class InvoiceQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? InvoiceId { get; set; }
    public string? InvoiceType { get; set; }
    public string? InvoiceYm { get; set; }
    public string? TaxId { get; set; }
    public string? SiteId { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 建立發票 DTO
/// </summary>
public class CreateInvoiceDto
{
    /// <summary>
    /// 發票編號
    /// </summary>
    public string InvoiceId { get; set; } = string.Empty;

    /// <summary>
    /// 發票類型 (01:統一發票, 02:電子發票, 03:收據)
    /// </summary>
    public string InvoiceType { get; set; } = string.Empty;

    /// <summary>
    /// 發票年份
    /// </summary>
    public int InvoiceYear { get; set; }

    /// <summary>
    /// 發票月份
    /// </summary>
    public int InvoiceMonth { get; set; }

    /// <summary>
    /// 字軌
    /// </summary>
    public string? Track { get; set; }

    /// <summary>
    /// 發票號碼起
    /// </summary>
    public string? InvoiceNoB { get; set; }

    /// <summary>
    /// 發票號碼迄
    /// </summary>
    public string? InvoiceNoE { get; set; }

    /// <summary>
    /// 發票格式代號
    /// </summary>
    public string? InvoiceFormat { get; set; }

    /// <summary>
    /// 統一編號
    /// </summary>
    public string? TaxId { get; set; }

    /// <summary>
    /// 公司名稱
    /// </summary>
    public string? CompanyName { get; set; }

    /// <summary>
    /// 公司英文名稱
    /// </summary>
    public string? CompanyNameEn { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// 區域
    /// </summary>
    public string? Zone { get; set; }

    /// <summary>
    /// 郵遞區號
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// 電話
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 傳真
    /// </summary>
    public string? Fax { get; set; }

    /// <summary>
    /// 電子郵件
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 分公司代碼
    /// </summary>
    public string? SiteId { get; set; }

    /// <summary>
    /// 副聯
    /// </summary>
    public string? SubCopy { get; set; }

    /// <summary>
    /// 副聯值
    /// </summary>
    public string? SubCopyValue { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// 修改發票 DTO
/// </summary>
public class UpdateInvoiceDto
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 發票編號
    /// </summary>
    public string InvoiceId { get; set; } = string.Empty;

    /// <summary>
    /// 發票類型 (01:統一發票, 02:電子發票, 03:收據)
    /// </summary>
    public string InvoiceType { get; set; } = string.Empty;

    /// <summary>
    /// 發票年份
    /// </summary>
    public int InvoiceYear { get; set; }

    /// <summary>
    /// 發票月份
    /// </summary>
    public int InvoiceMonth { get; set; }

    /// <summary>
    /// 字軌
    /// </summary>
    public string? Track { get; set; }

    /// <summary>
    /// 發票號碼起
    /// </summary>
    public string? InvoiceNoB { get; set; }

    /// <summary>
    /// 發票號碼迄
    /// </summary>
    public string? InvoiceNoE { get; set; }

    /// <summary>
    /// 發票格式代號
    /// </summary>
    public string? InvoiceFormat { get; set; }

    /// <summary>
    /// 統一編號
    /// </summary>
    public string? TaxId { get; set; }

    /// <summary>
    /// 公司名稱
    /// </summary>
    public string? CompanyName { get; set; }

    /// <summary>
    /// 公司英文名稱
    /// </summary>
    public string? CompanyNameEn { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// 區域
    /// </summary>
    public string? Zone { get; set; }

    /// <summary>
    /// 郵遞區號
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// 電話
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 傳真
    /// </summary>
    public string? Fax { get; set; }

    /// <summary>
    /// 電子郵件
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 分公司代碼
    /// </summary>
    public string? SiteId { get; set; }

    /// <summary>
    /// 副聯
    /// </summary>
    public string? SubCopy { get; set; }

    /// <summary>
    /// 副聯值
    /// </summary>
    public string? SubCopyValue { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }
}

