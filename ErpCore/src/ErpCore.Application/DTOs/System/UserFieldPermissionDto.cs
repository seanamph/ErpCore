namespace ErpCore.Application.DTOs.System;

/// <summary>
/// 使用者欄位權限 DTO (SYS0340)
/// </summary>
public class UserFieldPermissionDto
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 使用者代碼
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 使用者名稱
    /// </summary>
    public string? UserName { get; set; }

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
/// 使用者欄位權限查詢 DTO
/// </summary>
public class UserFieldPermissionQueryDto
{
    /// <summary>
    /// 使用者代碼
    /// </summary>
    public string? UserId { get; set; }

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
/// 新增使用者欄位權限 DTO
/// </summary>
public class CreateUserFieldPermissionDto
{
    /// <summary>
    /// 使用者代碼
    /// </summary>
    public string UserId { get; set; } = string.Empty;

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
/// 修改使用者欄位權限 DTO
/// </summary>
public class UpdateUserFieldPermissionDto
{
    /// <summary>
    /// 權限類型 (READ, WRITE, HIDE)
    /// </summary>
    public string PermissionType { get; set; } = string.Empty;
}

/// <summary>
/// 批次設定使用者欄位權限 DTO
/// </summary>
public class BatchSetUserFieldPermissionDto
{
    /// <summary>
    /// 使用者代碼
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 權限列表
    /// </summary>
    public List<UserFieldPermissionItemDto> Permissions { get; set; } = new();
}

/// <summary>
/// 使用者欄位權限項目 DTO
/// </summary>
public class UserFieldPermissionItemDto
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

