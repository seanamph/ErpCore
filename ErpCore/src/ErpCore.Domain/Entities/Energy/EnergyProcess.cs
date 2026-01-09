namespace ErpCore.Domain.Entities.Energy;

/// <summary>
/// 能源處理記錄 (SYSO310 - 能源處理功能)
/// </summary>
public class EnergyProcess
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 處理編號
    /// </summary>
    public string ProcessId { get; set; } = string.Empty;

    /// <summary>
    /// 能源編號
    /// </summary>
    public string EnergyId { get; set; } = string.Empty;

    /// <summary>
    /// 處理日期
    /// </summary>
    public DateTime ProcessDate { get; set; }

    /// <summary>
    /// 處理類型
    /// </summary>
    public string? ProcessType { get; set; }

    /// <summary>
    /// 數量
    /// </summary>
    public decimal? Amount { get; set; }

    /// <summary>
    /// 成本
    /// </summary>
    public decimal? Cost { get; set; }

    /// <summary>
    /// 狀態
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

