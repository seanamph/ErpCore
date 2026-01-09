namespace ErpCore.Domain.Entities.ChartTools;

/// <summary>
/// 圖表配置實體 (SYS1000)
/// </summary>
public class ChartConfig
{
    /// <summary>
    /// 圖表配置ID
    /// </summary>
    public Guid ChartConfigId { get; set; }

    /// <summary>
    /// 圖表名稱
    /// </summary>
    public string ChartName { get; set; } = string.Empty;

    /// <summary>
    /// 圖表類型 (BAR, LINE, PIE, COLUMN, AREA等)
    /// </summary>
    public string ChartType { get; set; } = string.Empty;

    /// <summary>
    /// 資料來源 (SQL查詢或API端點)
    /// </summary>
    public string? DataSource { get; set; }

    /// <summary>
    /// X軸欄位
    /// </summary>
    public string? XField { get; set; }

    /// <summary>
    /// Y軸欄位
    /// </summary>
    public string? YField { get; set; }

    /// <summary>
    /// 標題
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// X軸標題
    /// </summary>
    public string? XAxisTitle { get; set; }

    /// <summary>
    /// Y軸標題
    /// </summary>
    public string? YAxisTitle { get; set; }

    /// <summary>
    /// 寬度
    /// </summary>
    public int Width { get; set; } = 800;

    /// <summary>
    /// 高度
    /// </summary>
    public int Height { get; set; } = 400;

    /// <summary>
    /// 顏色 (JSON陣列)
    /// </summary>
    public string? Colors { get; set; }

    /// <summary>
    /// 選項 (JSON物件)
    /// </summary>
    public string? Options { get; set; }

    /// <summary>
    /// 建立人員
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新人員
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

