using ErpCore.Shared.Common;

namespace ErpCore.Application.DTOs.Inventory;

/// <summary>
/// POP列印商品 DTO
/// </summary>
public class PopPrintProductDto
{
    public string GoodsId { get; set; } = string.Empty;
    public string GoodsName { get; set; } = string.Empty;
    public string? BarCode { get; set; }
    public string? VendorGoodsId { get; set; }
    public string? LogoId { get; set; }
    public decimal? Price { get; set; }
    public decimal? Mprc { get; set; }
    public string? Unit { get; set; }
    public string? UnitName { get; set; }
}

/// <summary>
/// POP列印商品查詢 DTO
/// </summary>
public class PopPrintProductQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? GoodsId { get; set; }
    public string? GoodsName { get; set; }
    public string? BarCode { get; set; }
    public string? VendorGoodsId { get; set; }
    public string? LogoId { get; set; }
    public string? BClassId { get; set; }
    public string? MClassId { get; set; }
    public string? SClassId { get; set; }
}

/// <summary>
/// 產生列印資料 DTO
/// </summary>
public class GeneratePrintDataDto
{
    public List<string> GoodsIds { get; set; } = new();
    public string PrintType { get; set; } = string.Empty; // POP, PRODUCT_CARD
    public string PrintFormat { get; set; } = string.Empty; // PR1, PR2, PR3, PR4, PR5, PR6, PR1_AP, PR2_AP等
    public string? ShopId { get; set; }
    public string? Version { get; set; } // AP, UA, STANDARD
    public PrintOptionsDto? Options { get; set; }
}

/// <summary>
/// 列印選項 DTO
/// </summary>
public class PrintOptionsDto
{
    public bool IncludeBarcode { get; set; } = true;
    public bool IncludePrice { get; set; } = true;
    public bool IncludeNote { get; set; } = false;
}

/// <summary>
/// 列印資料 DTO
/// </summary>
public class PopPrintDataDto
{
    public List<PopPrintDataItemDto> PrintData { get; set; } = new();
    public int TotalCount { get; set; }
}

/// <summary>
/// 列印資料項目 DTO
/// </summary>
public class PopPrintDataItemDto
{
    public string GoodsId { get; set; } = string.Empty;
    public string GoodsName { get; set; } = string.Empty;
    public string? BarCode { get; set; }
    public string? BarCodeText { get; set; }
    public string? BarCodeImageBase64 { get; set; } // 條碼圖片 Base64 字串
    public decimal? Price { get; set; }
    public string? Unit { get; set; }
    public string PrintFormat { get; set; } = string.Empty;
}

/// <summary>
/// 列印請求 DTO
/// </summary>
public class PrintRequestDto
{
    public List<string> GoodsIds { get; set; } = new();
    public string PrintType { get; set; } = string.Empty;
    public string PrintFormat { get; set; } = string.Empty;
    public int PrintCount { get; set; } = 1;
    public string? ShopId { get; set; }
    public string? Version { get; set; } // AP, UA, STANDARD
}

/// <summary>
/// 列印工作 DTO
/// </summary>
public class PrintJobDto
{
    public string PrintJobId { get; set; } = string.Empty;
    public int PrintedCount { get; set; }
}

/// <summary>
/// POP列印設定 DTO
/// </summary>
public class PopPrintSettingDto
{
    public Guid SettingId { get; set; }
    public string? ShopId { get; set; }
    public string? Ip { get; set; }
    public string? TypeId { get; set; }
    public string? Version { get; set; } // AP, UA, STANDARD
    public bool DebugMode { get; set; }
    public int HeaderHeightPadding { get; set; }
    public int HeaderHeightPaddingRemain { get; set; }
    public int PageHeaderHeightPadding { get; set; }
    public string? PagePadding { get; set; }
    public string? PageSize { get; set; }
    public string? ApSpecificSettings { get; set; } // AP版本專屬設定 (JSON格式)
}

/// <summary>
/// 更新POP列印設定 DTO
/// </summary>
public class UpdatePopPrintSettingDto
{
    public string? Ip { get; set; }
    public string? TypeId { get; set; }
    public string? Version { get; set; } // AP, UA, STANDARD
    public bool DebugMode { get; set; }
    public int HeaderHeightPadding { get; set; }
    public int HeaderHeightPaddingRemain { get; set; }
    public int PageHeaderHeightPadding { get; set; }
    public string? PagePadding { get; set; }
    public string? PageSize { get; set; }
    public string? ApSpecificSettings { get; set; } // AP版本專屬設定 (JSON格式)
}

/// <summary>
/// POP列印記錄 DTO
/// </summary>
public class PopPrintLogDto
{
    public Guid LogId { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public string? GoodsName { get; set; }
    public string? PrintType { get; set; }
    public string? PrintFormat { get; set; }
    public string? Version { get; set; } // AP, UA, STANDARD
    public int PrintCount { get; set; }
    public DateTime PrintDate { get; set; }
    public string? PrintedBy { get; set; }
    public string? ShopId { get; set; }
}

/// <summary>
/// POP列印記錄查詢 DTO
/// </summary>
public class PopPrintLogQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? GoodsId { get; set; }
    public string? PrintType { get; set; }
    public string? PrintFormat { get; set; }
    public string? Version { get; set; } // AP, UA, STANDARD
    public string? ShopId { get; set; }
    public DateTime? PrintDateFrom { get; set; }
    public DateTime? PrintDateTo { get; set; }
}

