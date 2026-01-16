using ErpCore.Shared.Common;

namespace ErpCore.Application.DTOs.AnalysisReport;

/// <summary>
/// 庫存分析報表 DTO (SYSWC10)
/// </summary>
public class SYSWC10ReportDto
{
    public string SiteId { get; set; } = string.Empty;
    public string? SiteName { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public string GoodsName { get; set; } = string.Empty;
    public string? BigCategoryId { get; set; }
    public string? BigCategoryName { get; set; }
    public string? MidCategoryId { get; set; }
    public string? MidCategoryName { get; set; }
    public string? SmallCategoryId { get; set; }
    public string? SmallCategoryName { get; set; }
    public string? WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public decimal InQty { get; set; } // 入庫數量
    public decimal OutQty { get; set; } // 出庫數量
    public decimal CurrentQty { get; set; } // 當前庫存數量
    public decimal CurrentAmt { get; set; } // 當前庫存金額
    public DateTime? LastStockDate { get; set; } // 最後庫存異動日期
    public decimal SafeQty { get; set; } // 安全庫存量
    public bool IsLowStock { get; set; } // 是否低庫存
    public bool IsOverStock { get; set; } // 是否過量庫存
}

/// <summary>
/// 庫存分析報表查詢 DTO (SYSWC10)
/// </summary>
public class SYSWC10QueryDto : PagedQuery
{
    public string? GoodsIdFrom { get; set; } // 商品代碼起
    public string? GoodsIdTo { get; set; } // 商品代碼迄
    public string? GoodsName { get; set; } // 商品名稱
    public List<string>? SiteIds { get; set; } // 店別列表
    public List<string>? WarehouseIds { get; set; } // 庫別列表
    public List<string>? CategoryIds { get; set; } // 分類列表
    public string? DateFrom { get; set; } // 日期起
    public string? DateTo { get; set; } // 日期迄
    public decimal? MinQty { get; set; } // 最小數量
    public decimal? MaxQty { get; set; } // 最大數量
    public string? Status { get; set; } // 狀態
    public string? BId { get; set; } // 大分類
    public string? MId { get; set; } // 中分類
    public string? SId { get; set; } // 小分類
}
