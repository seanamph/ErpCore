namespace ErpCore.Domain.Entities.System;

/// <summary>
/// 欄位資訊實體 (SYS0330)
/// </summary>
public class FieldInfo
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
    /// 欄位名稱
    /// </summary>
    public string FieldName { get; set; } = string.Empty;

    /// <summary>
    /// 欄位類型
    /// </summary>
    public string? FieldType { get; set; }

    /// <summary>
    /// 欄位長度
    /// </summary>
    public int? FieldLength { get; set; }

    /// <summary>
    /// 欄位描述
    /// </summary>
    public string? FieldDescription { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; } = true;
}

