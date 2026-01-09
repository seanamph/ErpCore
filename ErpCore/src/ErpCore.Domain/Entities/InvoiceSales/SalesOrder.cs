namespace ErpCore.Domain.Entities.InvoiceSales;

/// <summary>
/// 銷售單實體 (SYSG410-SYSG460 - 銷售資料維護)
/// </summary>
public class SalesOrder
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
    public decimal? TotalAmount { get; set; } = 0;

    /// <summary>
    /// 總數量
    /// </summary>
    public decimal? TotalQty { get; set; } = 0;

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
    public string? CurrencyId { get; set; } = "TWD";

    /// <summary>
    /// 匯率
    /// </summary>
    public decimal? ExchangeRate { get; set; } = 1;

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
/// 銷售單明細實體
/// </summary>
public class SalesOrderDetail
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
    /// 行號
    /// </summary>
    public int LineNum { get; set; }

    /// <summary>
    /// 商品編號
    /// </summary>
    public string GoodsId { get; set; } = string.Empty;

    /// <summary>
    /// 條碼編號
    /// </summary>
    public string? BarcodeId { get; set; }

    /// <summary>
    /// 訂購數量
    /// </summary>
    public decimal OrderQty { get; set; } = 0;

    /// <summary>
    /// 單價
    /// </summary>
    public decimal? UnitPrice { get; set; }

    /// <summary>
    /// 金額
    /// </summary>
    public decimal? Amount { get; set; }

    /// <summary>
    /// 已出貨數量
    /// </summary>
    public decimal? ShippedQty { get; set; } = 0;

    /// <summary>
    /// 已退數量
    /// </summary>
    public decimal? ReturnQty { get; set; } = 0;

    /// <summary>
    /// 單位
    /// </summary>
    public string? UnitId { get; set; }

    /// <summary>
    /// 稅率
    /// </summary>
    public decimal? TaxRate { get; set; } = 0;

    /// <summary>
    /// 稅額
    /// </summary>
    public decimal? TaxAmount { get; set; } = 0;

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

