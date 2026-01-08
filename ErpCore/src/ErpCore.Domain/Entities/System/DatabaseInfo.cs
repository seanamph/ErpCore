namespace ErpCore.Domain.Entities.System;

/// <summary>
/// 資料庫資訊實體 (SYS0330)
/// </summary>
public class DatabaseInfo
{
    /// <summary>
    /// 資料庫名稱
    /// </summary>
    public string DbName { get; set; } = string.Empty;

    /// <summary>
    /// 資料庫描述
    /// </summary>
    public string? DbDescription { get; set; }

    /// <summary>
    /// 連線字串
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

