namespace ErpCore.Domain.Entities.InvoiceSalesB2B;

/// <summary>
/// B2B銷售單實體 (SYSG000_B2B - B2B銷售資料維護)
/// </summary>
public class B2BSalesOrder
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 銷售單號
    /// </summary>
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// 銷售日期
    /// </summary>
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// 單據類型 (SO:銷售, RT:退貨)
    /// </summary>
    public string OrderType { get; set; } = string.Empty;

    /// <summary>
    /// 分店代碼
    /// </summary>
    public string ShopId { get; set; } = string.Empty;

    /// <summary>
    /// 客戶代碼
    /// </summary>
    public string? CustomerId { get; set; }

    /// <summary>
    /// 狀態 (D:草稿, S:已送出, A:已審核, X:已取消, C:已結案)
    /// </summary>
    public string Status { get; set; } = "D";

    /// <summary>
    /// 申請人員
    /// </summary>
    public string? ApplyUserId { get; set; }

    /// <summary>
    /// 申請日期
    /// </summary>
    public DateTime? ApplyDate { get; set; }

    /// <summary>
    /// 審核人員
    /// </summary>
    public string? ApproveUserId { get; set; }

    /// <summary>
    /// 審核日期
    /// </summary>
    public DateTime? ApproveDate { get; set; }

    /// <summary>
    /// 總金額
    /// </summary>
    public decimal TotalAmount { get; set; } = 0;

    /// <summary>
    /// 總數量
    /// </summary>
    public decimal TotalQty { get; set; } = 0;

    /// <summary>
    /// 備註
    /// </summary>
    public string? Memo { get; set; }

    /// <summary>
    /// 預期交貨日期
    /// </summary>
    public DateTime? ExpectedDate { get; set; }

    /// <summary>
    /// 分公司代碼
    /// </summary>
    public string? SiteId { get; set; }

    /// <summary>
    /// 組織代碼
    /// </summary>
    public string? OrgId { get; set; }

    /// <summary>
    /// 幣別
    /// </summary>
    public string CurrencyId { get; set; } = "TWD";

    /// <summary>
    /// 匯率
    /// </summary>
    public decimal ExchangeRate { get; set; } = 1;

    /// <summary>
    /// B2B標記
    /// </summary>
    public string B2BFlag { get; set; } = "Y";

    /// <summary>
    /// 傳輸類型 (BREG, CTSG, EINB, EING等)
    /// </summary>
    public string? TransferType { get; set; }

    /// <summary>
    /// 傳輸狀態
    /// </summary>
    public string? TransferStatus { get; set; }

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

