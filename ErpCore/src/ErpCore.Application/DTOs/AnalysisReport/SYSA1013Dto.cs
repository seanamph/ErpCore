using ErpCore.Shared.Common;

namespace ErpCore.Application.DTOs.AnalysisReport;

/// <summary>
/// 耗材出庫明細表 DTO (SYSA1013)
/// </summary>
public class SYSA1013ReportDto
{
    public string TxnNo { get; set; } = string.Empty; // 出庫單號
    public DateTime TxnDate { get; set; } // 出庫日期
    public string? BId { get; set; } // 大分類
    public string? MId { get; set; } // 中分類
    public string? SId { get; set; } // 小分類
    public string GoodsId { get; set; } = string.Empty; // 商品代碼
    public string GoodsName { get; set; } = string.Empty; // 商品名稱
    public string? PackUnit { get; set; } // 包裝單位
    public string? Unit { get; set; } // 單位
    public decimal Amt { get; set; } // 單價
    public decimal ApplyQty { get; set; } // 申請數量
    public decimal Qty { get; set; } // 數量
    public decimal NAmt { get; set; } // 未稅金額
    public string? Use { get; set; } // 用途
    public string? Vendor { get; set; } // 廠商
    public string? StocksType { get; set; } // 庫存類型
    public string? OrgId { get; set; } // 單位
    public string? OrgAllocation { get; set; } // 單位分攤
}

/// <summary>
/// 耗材出庫明細表查詢 DTO (SYSA1013)
/// </summary>
public class SYSA1013QueryDto : PagedQuery
{
    public string? SiteId { get; set; }
    public string? BId { get; set; }
    public string? MId { get; set; }
    public string? SId { get; set; }
    public string? OrgId { get; set; }
    public string? GoodsId { get; set; }
    public string? BeginDate { get; set; }
    public string? EndDate { get; set; }
    public string? SupplierId { get; set; }
    public string? Use { get; set; }
    public string? FilterType { get; set; } // 篩選類型 (全部、特定狀態等)
}
