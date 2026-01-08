namespace ErpCore.Application.DTOs.System;

/// <summary>
/// 使用者異動記錄查詢 DTO (SYS0610)
/// </summary>
public class UserChangeLogQueryDto
{
    /// <summary>
    /// 異動使用者代碼
    /// </summary>
    public string? ChangeUserId { get; set; }

    /// <summary>
    /// 被異動的使用者代碼
    /// </summary>
    public string? TargetUserId { get; set; }

    /// <summary>
    /// 起始日期
    /// </summary>
    public DateTime BeginDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// 頁碼
    /// </summary>
    public int? PageIndex { get; set; } = 1;

    /// <summary>
    /// 每頁筆數
    /// </summary>
    public int? PageSize { get; set; } = 20;
}

/// <summary>
/// 角色異動記錄查詢 DTO (SYS0620)
/// </summary>
public class RoleChangeLogQueryDto
{
    /// <summary>
    /// 異動使用者代碼
    /// </summary>
    public string? ChangeUserId { get; set; }

    /// <summary>
    /// 角色代碼 (在 OLD_VALUE 或 NEW_VALUE 中搜尋)
    /// </summary>
    public string? RoleId { get; set; }

    /// <summary>
    /// 起始日期
    /// </summary>
    public DateTime BeginDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// 頁碼
    /// </summary>
    public int? PageIndex { get; set; } = 1;

    /// <summary>
    /// 每頁筆數
    /// </summary>
    public int? PageSize { get; set; } = 20;
}

/// <summary>
/// 異動記錄 DTO (SYS0610)
/// </summary>
public class ChangeLogDto
{
    /// <summary>
    /// 異動記錄編號
    /// </summary>
    public long LogId { get; set; }

    /// <summary>
    /// 程式代碼
    /// </summary>
    public string ProgramId { get; set; } = string.Empty;

    /// <summary>
    /// 異動使用者代碼
    /// </summary>
    public string? ChangeUserId { get; set; }

    /// <summary>
    /// 異動使用者名稱
    /// </summary>
    public string? ChangeUserName { get; set; }

    /// <summary>
    /// 異動時間
    /// </summary>
    public DateTime ChangeDate { get; set; }

    /// <summary>
    /// 異動狀態 (1=新增, 2=刪除, 3=修改)
    /// </summary>
    public string ChangeStatus { get; set; } = string.Empty;

    /// <summary>
    /// 異動狀態名稱
    /// </summary>
    public string ChangeStatusName { get; set; } = string.Empty;

    /// <summary>
    /// 異動欄位名稱 (多個欄位以逗號分隔)
    /// </summary>
    public string? ChangeField { get; set; }

    /// <summary>
    /// 異動前的值 (多個值以逗號分隔)
    /// </summary>
    public string? OldValue { get; set; }

    /// <summary>
    /// 異動後的值 (多個值以逗號分隔)
    /// </summary>
    public string? NewValue { get; set; }

    /// <summary>
    /// 異動欄位列表
    /// </summary>
    public List<string> ChangeFields { get; set; } = new();

    /// <summary>
    /// 異動前的值列表
    /// </summary>
    public List<string> OldValues { get; set; } = new();

    /// <summary>
    /// 異動後的值列表
    /// </summary>
    public List<string> NewValues { get; set; } = new();

    /// <summary>
    /// 異動欄位顯示文字
    /// </summary>
    public string ChangeFieldDisplay { get; set; } = string.Empty;

    /// <summary>
    /// 異動前的值顯示文字
    /// </summary>
    public string OldValueDisplay { get; set; } = string.Empty;

    /// <summary>
    /// 異動後的值顯示文字
    /// </summary>
    public string NewValueDisplay { get; set; } = string.Empty;
}

/// <summary>
/// 使用者角色對應設定異動記錄查詢 DTO (SYS0630)
/// </summary>
public class UserRoleChangeLogQueryDto
{
    /// <summary>
    /// 異動使用者代碼
    /// </summary>
    public string? ChangeUserId { get; set; }

    /// <summary>
    /// 被異動的使用者代碼
    /// </summary>
    public string? SearchUserId { get; set; }

    /// <summary>
    /// 被異動的角色代碼
    /// </summary>
    public string? SearchRoleId { get; set; }

    /// <summary>
    /// 起始日期
    /// </summary>
    public DateTime BeginDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// 頁碼
    /// </summary>
    public int? PageIndex { get; set; } = 1;

    /// <summary>
    /// 每頁筆數
    /// </summary>
    public int? PageSize { get; set; } = 20;
}

/// <summary>
/// 系統權限異動記錄查詢 DTO (SYS0640)
/// </summary>
public class SystemPermissionChangeLogQueryDto
{
    /// <summary>
    /// 異動使用者代碼
    /// </summary>
    public string? ChangeUserId { get; set; }

    /// <summary>
    /// 被異動的使用者代碼
    /// </summary>
    public string? SearchUserId { get; set; }

    /// <summary>
    /// 被異動的角色代碼
    /// </summary>
    public string? SearchRoleId { get; set; }

    /// <summary>
    /// 起始日期
    /// </summary>
    public DateTime BeginDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// 頁碼
    /// </summary>
    public int? PageIndex { get; set; } = 1;

    /// <summary>
    /// 每頁筆數
    /// </summary>
    public int? PageSize { get; set; } = 20;
}

/// <summary>
/// 可管控欄位異動記錄查詢 DTO (SYS0650)
/// </summary>
public class ControllableFieldChangeLogQueryDto
{
    /// <summary>
    /// 異動使用者代碼
    /// </summary>
    public string? ChangeUserId { get; set; }

    /// <summary>
    /// 被異動的使用者代碼
    /// </summary>
    public string? SearchUserId { get; set; }

    /// <summary>
    /// 欄位代碼
    /// </summary>
    public string? FieldId { get; set; }

    /// <summary>
    /// 起始日期
    /// </summary>
    public DateTime BeginDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// 頁碼
    /// </summary>
    public int? PageIndex { get; set; } = 1;

    /// <summary>
    /// 每頁筆數
    /// </summary>
    public int? PageSize { get; set; } = 20;
}

/// <summary>
/// 其他異動記錄查詢 DTO (SYS0660)
/// </summary>
public class OtherChangeLogQueryDto
{
    /// <summary>
    /// 異動使用者代碼
    /// </summary>
    public string? ChangeUserId { get; set; }

    /// <summary>
    /// 程式代碼
    /// </summary>
    public string? ProgramId { get; set; }

    /// <summary>
    /// 起始日期
    /// </summary>
    public DateTime BeginDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// 頁碼
    /// </summary>
    public int? PageIndex { get; set; } = 1;

    /// <summary>
    /// 每頁筆數
    /// </summary>
    public int? PageSize { get; set; } = 20;
}

