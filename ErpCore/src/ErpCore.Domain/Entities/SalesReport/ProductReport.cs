namespace ErpCore.Domain.Entities.SalesReport;

/// <summary>
/// 商品報表資料 (SYS1000 - 商品報表)
/// </summary>
public class ProductReport
{
    /// <summary>
    /// 商品報表編號
    /// </summary>
    public Guid ProductReportId { get; set; }

    /// <summary>
    /// 商品編號
    /// </summary>
    public string? ProductId { get; set; }

    /// <summary>
    /// 商品條碼
    /// </summary>
    public string? BarcodeId { get; set; }

    /// <summary>
    /// 商品名稱
    /// </summary>
    public string? ProductName { get; set; }

    /// <summary>
    /// 店別代碼
    /// </summary>
    public string? ShopId { get; set; }

    /// <summary>
    /// 報表日期
    /// </summary>
    public DateTime? ReportDate { get; set; }

    /// <summary>
    /// 銷售數量
    /// </summary>
    public decimal SalesQty { get; set; }

    /// <summary>
    /// 銷售金額
    /// </summary>
    public decimal SalesAmount { get; set; }

    /// <summary>
    /// 成本金額
    /// </summary>
    public decimal CostAmount { get; set; }

    /// <summary>
    /// 利潤金額
    /// </summary>
    public decimal ProfitAmount { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

