namespace ErpCore.Domain.Entities.System;

/// <summary>
/// 角色欄位權限實體 (SYS0330)
/// </summary>
public class RoleFieldPermission
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 角色代碼
    /// </summary>
    public string RoleId { get; set; } = string.Empty;

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
    /// 權限類型 (READ, WRITE, HIDE)
    /// </summary>
    public string PermissionType { get; set; } = string.Empty;

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

