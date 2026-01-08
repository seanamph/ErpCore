namespace ErpCore.Application.DTOs.System;

/// <summary>
/// 角色欄位權限 DTO (SYS0330)
/// </summary>
public class RoleFieldPermissionDto
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
    /// 角色名稱
    /// </summary>
    public string? RoleName { get; set; }

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
}

/// <summary>
/// 角色欄位權限查詢 DTO
/// </summary>
public class RoleFieldPermissionQueryDto
{
    /// <summary>
    /// 角色代碼
    /// </summary>
    public string? RoleId { get; set; }

    /// <summary>
    /// 資料庫名稱
    /// </summary>
    public string? DbName { get; set; }

    /// <summary>
    /// 表格名稱
    /// </summary>
    public string? TableName { get; set; }

    /// <summary>
    /// 欄位名稱
    /// </summary>
    public string? FieldName { get; set; }

    /// <summary>
    /// 權限類型
    /// </summary>
    public string? PermissionType { get; set; }

    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每頁筆數
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 排序欄位
    /// </summary>
    public string? SortField { get; set; }

    /// <summary>
    /// 排序方向
    /// </summary>
    public string? SortOrder { get; set; }
}

/// <summary>
/// 新增角色欄位權限 DTO
/// </summary>
public class CreateRoleFieldPermissionDto
{
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
}

/// <summary>
/// 修改角色欄位權限 DTO
/// </summary>
public class UpdateRoleFieldPermissionDto
{
    /// <summary>
    /// 權限類型 (READ, WRITE, HIDE)
    /// </summary>
    public string PermissionType { get; set; } = string.Empty;
}

/// <summary>
/// 批次設定角色欄位權限 DTO
/// </summary>
public class BatchSetRoleFieldPermissionDto
{
    /// <summary>
    /// 角色代碼
    /// </summary>
    public string RoleId { get; set; } = string.Empty;

    /// <summary>
    /// 權限列表
    /// </summary>
    public List<RoleFieldPermissionItemDto> Permissions { get; set; } = new();
}

/// <summary>
/// 角色欄位權限項目 DTO
/// </summary>
public class RoleFieldPermissionItemDto
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
    /// 權限類型 (READ, WRITE, HIDE)
    /// </summary>
    public string PermissionType { get; set; } = string.Empty;
}

/// <summary>
/// 資料庫資訊 DTO
/// </summary>
public class DatabaseDto
{
    /// <summary>
    /// 資料庫名稱
    /// </summary>
    public string DbName { get; set; } = string.Empty;

    /// <summary>
    /// 資料庫描述
    /// </summary>
    public string? DbDescription { get; set; }
}

/// <summary>
/// 表格資訊 DTO
/// </summary>
public class TableDto
{
    /// <summary>
    /// 表格名稱
    /// </summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// 表格描述
    /// </summary>
    public string? TableDescription { get; set; }
}

/// <summary>
/// 欄位資訊 DTO
/// </summary>
public class FieldDto
{
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
}

