namespace ErpCore.Domain.Entities.System;

/// <summary>
/// 表格資訊實體 (SYS0330)
/// </summary>
public class TableInfo
{
    /// <summary>
    /// 資料庫名稱
    /// </summary>
    public string DbName { get; set; } = string.Empty;

    /// <summary>
    /// 表格名稱
    /// </summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// 表格描述
    /// </summary>
    public string? TableDescription { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; } = true;
}

