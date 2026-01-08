namespace ErpCore.Domain.Entities.System;

/// <summary>
/// 可管控欄位實體 (SYS0510)
/// </summary>
public class ControllableField
{
    /// <summary>
    /// 欄位代碼
    /// </summary>
    public string FieldId { get; set; } = string.Empty;

    /// <summary>
    /// 欄位名稱
    /// </summary>
    public string FieldName { get; set; } = string.Empty;

    /// <summary>
    /// 資料庫名稱
    /// </summary>
    public string DbName { get; set; } = string.Empty;

    /// <summary>
    /// 表格名稱
    /// </summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// 資料庫欄位名稱
    /// </summary>
    public string FieldNameInDb { get; set; } = string.Empty;

    /// <summary>
    /// 欄位類型
    /// </summary>
    public string? FieldType { get; set; }

    /// <summary>
    /// 欄位描述
    /// </summary>
    public string? FieldDescription { get; set; }

    /// <summary>
    /// 是否必填
    /// </summary>
    public bool IsRequired { get; set; } = false;

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 排序順序
    /// </summary>
    public int SortOrder { get; set; } = 0;

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
    public DateTime? UpdatedAt { get; set; }
}

