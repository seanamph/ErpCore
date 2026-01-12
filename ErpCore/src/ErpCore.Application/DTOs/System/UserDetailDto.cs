namespace ErpCore.Application.DTOs.System;

/// <summary>
/// 使用者詳細資料 DTO (SYS0111)
/// </summary>
public class UserDetailDto
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? OrgId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = "A";
    public string? UserType { get; set; }
    public string? Notes { get; set; }
    public int? UserPriority { get; set; }
    public string? ShopId { get; set; }
    
    /// <summary>
    /// 業種權限列表
    /// </summary>
    public List<UserBusinessTypeDto> BusinessTypes { get; set; } = new();
    
    /// <summary>
    /// 儲位權限列表
    /// </summary>
    public List<UserWarehouseAreaDto> WarehouseAreas { get; set; } = new();
    
    /// <summary>
    /// 7X承租分店權限列表
    /// </summary>
    public List<UserStoreDto> Stores { get; set; } = new();
    
    /// <summary>
    /// 總公司/分店權限列表 (SYS0113)
    /// </summary>
    public List<UserShopDto> Shops { get; set; } = new();
    
    /// <summary>
    /// 廠商權限列表 (SYS0113)
    /// </summary>
    public List<UserVendorDto> Vendors { get; set; } = new();
    
    /// <summary>
    /// 部門權限列表 (SYS0113)
    /// </summary>
    public List<UserDepartmentDto> Departments { get; set; } = new();
    
    /// <summary>
    /// 按鈕權限列表 (SYS0113)
    /// </summary>
    public List<UserButtonDto> Buttons { get; set; } = new();
}

/// <summary>
/// 使用者業種權限 DTO (SYS0111)
/// </summary>
public class UserBusinessTypeDto
{
    public long Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? BtypeMId { get; set; }
    public string? BtypeMName { get; set; }
    public string? BtypeId { get; set; }
    public string? BtypeName { get; set; }
    public string? PtypeId { get; set; }
    public string? PtypeName { get; set; }
}

/// <summary>
/// 使用者儲位權限 DTO (SYS0111)
/// </summary>
public class UserWarehouseAreaDto
{
    public long Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? WareaId1 { get; set; }
    public string? WareaName1 { get; set; }
    public string? WareaId2 { get; set; }
    public string? WareaName2 { get; set; }
    public string? WareaId3 { get; set; }
    public string? WareaName3 { get; set; }
}

/// <summary>
/// 使用者7X承租分店權限 DTO (SYS0111)
/// </summary>
public class UserStoreDto
{
    public long Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string StoreId { get; set; } = string.Empty;
    public string? StoreName { get; set; }
}

/// <summary>
/// 業種大分類 DTO (SYS0111)
/// </summary>
public class BusinessTypeMajorDto
{
    public string BtypeMId { get; set; } = string.Empty;
    public string BtypeMName { get; set; } = string.Empty;
}

/// <summary>
/// 業種中分類 DTO (SYS0111)
/// </summary>
public class BusinessTypeMiddleDto
{
    public string BtypeId { get; set; } = string.Empty;
    public string BtypeName { get; set; } = string.Empty;
    public string? BtypeMId { get; set; }
}

/// <summary>
/// 業種小分類 DTO (SYS0111)
/// </summary>
public class BusinessTypeMinorDto
{
    public string PtypeId { get; set; } = string.Empty;
    public string PtypeName { get; set; } = string.Empty;
    public string? BtypeId { get; set; }
}

/// <summary>
/// 儲位 DTO (SYS0111)
/// </summary>
public class WarehouseAreaDto
{
    public string WareaId { get; set; } = string.Empty;
    public string WareaName { get; set; } = string.Empty;
    public int Level { get; set; }
    public string? ParentId { get; set; }
}

/// <summary>
/// 7X承租分店 DTO (SYS0111)
/// </summary>
public class StoreDto
{
    public string StoreId { get; set; } = string.Empty;
    public string StoreName { get; set; } = string.Empty;
}

/// <summary>
/// 新增/修改使用者業種儲位 DTO (SYS0111)
/// </summary>
public class CreateUserWithBusinessTypesDto : CreateUserDto
{
    public List<CreateUserBusinessTypeDto> BusinessTypes { get; set; } = new();
    public List<CreateUserWarehouseAreaDto> WarehouseAreas { get; set; } = new();
    public List<CreateUserStoreDto> Stores { get; set; } = new();
}

/// <summary>
/// 新增使用者業種權限 DTO (SYS0111)
/// </summary>
public class CreateUserBusinessTypeDto
{
    public string? BtypeMId { get; set; }
    public string? BtypeId { get; set; }
    public string? PtypeId { get; set; }
}

/// <summary>
/// 新增使用者儲位權限 DTO (SYS0111)
/// </summary>
public class CreateUserWarehouseAreaDto
{
    public string? WareaId1 { get; set; }
    public string? WareaId2 { get; set; }
    public string? WareaId3 { get; set; }
}

/// <summary>
/// 新增使用者7X承租分店權限 DTO (SYS0111)
/// </summary>
public class CreateUserStoreDto
{
    public string StoreId { get; set; } = string.Empty;
}

/// <summary>
/// 修改使用者業種儲位 DTO (SYS0111)
/// </summary>
public class UpdateUserWithBusinessTypesDto : UpdateUserDto
{
    public List<CreateUserBusinessTypeDto> BusinessTypes { get; set; } = new();
    public List<CreateUserWarehouseAreaDto> WarehouseAreas { get; set; } = new();
    public List<CreateUserStoreDto> Stores { get; set; } = new();
}
