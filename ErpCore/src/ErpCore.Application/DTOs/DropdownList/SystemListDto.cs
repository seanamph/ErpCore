namespace ErpCore.Application.DTOs.DropdownList;

/// <summary>
/// 系統列表 DTO (SYSID_LIST)
/// </summary>
public class SystemListDto
{
    public string SystemId { get; set; } = string.Empty;
    public string SystemName { get; set; } = string.Empty;
    public string Status { get; set; } = "A";
}

/// <summary>
/// 系統列表查詢 DTO
/// </summary>
public class SystemListQueryDto
{
    public string? SystemId { get; set; }
    public string? SystemName { get; set; }
    public string? Status { get; set; }
    public string? ExcludeSystems { get; set; }
    public string? SortField { get; set; } = "SystemId";
    public string? SortOrder { get; set; } = "ASC";
}

/// <summary>
/// 使用者列表 DTO (USER_LIST)
/// </summary>
public class UserListDto
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? OrgId { get; set; }
    public string? OrgName { get; set; }
    public string? Title { get; set; }
    public string Status { get; set; } = "A";
}

/// <summary>
/// 使用者列表查詢 DTO
/// </summary>
public class UserListQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? OrgId { get; set; }
    public string? Status { get; set; } = "A";
    public string? SortField { get; set; } = "UserId";
    public string? SortOrder { get; set; } = "ASC";
    public string? ListType { get; set; } // user, dept, other
}

