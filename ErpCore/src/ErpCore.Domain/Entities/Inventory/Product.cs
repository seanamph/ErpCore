namespace ErpCore.Domain.Entities.Inventory;

/// <summary>
/// 商品主檔 (對應舊系統 GOODS_M)
/// </summary>
public class Product
{
    /// <summary>
    /// 進銷碼 (GOODS_ID) - 主鍵
    /// </summary>
    public string GoodsId { get; set; } = string.Empty;

    /// <summary>
    /// 商品名稱 (GOODS_NAME)
    /// </summary>
    public string GoodsName { get; set; } = string.Empty;

    /// <summary>
    /// 發票列印名稱 (INV_PRINT_NAME)
    /// </summary>
    public string? InvPrintName { get; set; }

    /// <summary>
    /// 商品規格 (GOODS_SPACE)
    /// </summary>
    public string? GoodsSpace { get; set; }

    /// <summary>
    /// 小分類代碼 (SC_ID)
    /// </summary>
    public string? ScId { get; set; }

    /// <summary>
    /// 稅別 (TAX, 1:應稅, 0:免稅)
    /// </summary>
    public string Tax { get; set; } = "1";

    /// <summary>
    /// 進價 (LPRC)
    /// </summary>
    public decimal Lprc { get; set; }

    /// <summary>
    /// 中價 (MPRC)
    /// </summary>
    public decimal Mprc { get; set; }

    /// <summary>
    /// 國際條碼 (BARCODE_ID)
    /// </summary>
    public string? BarcodeId { get; set; }

    /// <summary>
    /// 單位 (UNIT)
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// 換算率 (CONVERT_RATE)
    /// </summary>
    public int ConvertRate { get; set; } = 1;

    /// <summary>
    /// 容量 (CAPACITY)
    /// </summary>
    public int Capacity { get; set; }

    /// <summary>
    /// 容量單位 (CAPACITY_UNIT)
    /// </summary>
    public string? CapacityUnit { get; set; }

    /// <summary>
    /// 狀態 (STATUS, 1:正常, 2:停用)
    /// </summary>
    public string Status { get; set; } = "1";

    /// <summary>
    /// 可折扣 (DISCOUNT, Y/N)
    /// </summary>
    public string Discount { get; set; } = "N";

    /// <summary>
    /// 自動訂貨 (AUTO_ORDER, Y/N)
    /// </summary>
    public string AutoOrder { get; set; } = "N";

    /// <summary>
    /// 價格種類 (PRICE_KIND)
    /// </summary>
    public string PriceKind { get; set; } = "1";

    /// <summary>
    /// 成本種類 (COST_KIND)
    /// </summary>
    public string CostKind { get; set; } = "1";

    /// <summary>
    /// 安全庫存天數 (SAFE_DAYS)
    /// </summary>
    public int SafeDays { get; set; }

    /// <summary>
    /// 有效期限天數 (EXPIRATION_DAYS)
    /// </summary>
    public int ExpirationDays { get; set; }

    /// <summary>
    /// 國別 (NATIONAL)
    /// </summary>
    public string? National { get; set; }

    /// <summary>
    /// 產地 (PLACE)
    /// </summary>
    public string? Place { get; set; }

    /// <summary>
    /// 商品-深(公分) (GOODS_DEEP)
    /// </summary>
    public int GoodsDeep { get; set; }

    /// <summary>
    /// 商品-寬(公分) (GOODS_WIDE)
    /// </summary>
    public int GoodsWide { get; set; }

    /// <summary>
    /// 商品-高(公分) (GOODS_HIGH)
    /// </summary>
    public int GoodsHigh { get; set; }

    /// <summary>
    /// 包裝-深(公分) (PACK_DEEP)
    /// </summary>
    public int PackDeep { get; set; }

    /// <summary>
    /// 包裝-寬(公分) (PACK_WIDE)
    /// </summary>
    public int PackWide { get; set; }

    /// <summary>
    /// 包裝-高(公分) (PACK_HIGH)
    /// </summary>
    public int PackHigh { get; set; }

    /// <summary>
    /// 包裝-重量(KG) (PACK_WEIGHT)
    /// </summary>
    public int PackWeight { get; set; }

    /// <summary>
    /// 建立者 (BUSER)
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間 (BTIME)
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新者 (CUSER)
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 更新時間 (CTIME)
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// 建立者等級 (CPRIORITY)
    /// </summary>
    public int? CreatedPriority { get; set; }

    /// <summary>
    /// 建立者群組 (CGROUP)
    /// </summary>
    public string? CreatedGroup { get; set; }
}

