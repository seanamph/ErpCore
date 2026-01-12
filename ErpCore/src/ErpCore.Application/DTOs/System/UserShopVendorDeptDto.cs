namespace ErpCore.Application.DTOs.System;

/// <summary>
/// 使用者總公司/分店權限 DTO (SYS0113)
/// </summary>
public class UserShopDto
{
    public long Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? PShopId { get; set; }
    public string? PShopName { get; set; }
    public string? ShopId { get; set; }
    public string? ShopName { get; set; }
    public string? SiteId { get; set; }
    public string? SiteName { get; set; }
}

/// <summary>
/// 使用者廠商權限 DTO (SYS0113)
/// </summary>
public class UserVendorDto
{
    public long Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string VendorId { get; set; } = string.Empty;
    public string? VendorName { get; set; }
}

/// <summary>
/// 使用者部門權限 DTO (SYS0113)
/// </summary>
public class UserDepartmentDto
{
    public long Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string DeptId { get; set; } = string.Empty;
    public string? DeptName { get; set; }
}

/// <summary>
/// 總公司 DTO (SYS0113)
/// </summary>
public class ParentShopDto
{
    public string PShopId { get; set; } = string.Empty;
    public string PShopName { get; set; } = string.Empty;
}

/// <summary>
/// 分店 DTO (SYS0113)
/// </summary>
public class ShopDto
{
    public string ShopId { get; set; } = string.Empty;
    public string ShopName { get; set; } = string.Empty;
    public string? PShopId { get; set; }
}

/// <summary>
/// 據點 DTO (SYS0113)
/// </summary>
public class SiteDto
{
    public string SiteId { get; set; } = string.Empty;
    public string SiteName { get; set; } = string.Empty;
    public string? ShopId { get; set; }
}

/// <summary>
/// 廠商 DTO (SYS0113)
/// </summary>
public class VendorDto
{
    public string VendorId { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;
}

/// <summary>
/// 部門 DTO (SYS0113)
/// </summary>
public class DepartmentDto
{
    public string DeptId { get; set; } = string.Empty;
    public string DeptName { get; set; } = string.Empty;
}

/// <summary>
/// 新增使用者總公司/分店權限 DTO (SYS0113)
/// </summary>
public class CreateUserShopDto
{
    public string? PShopId { get; set; }
    public string? ShopId { get; set; }
    public string? SiteId { get; set; }
}

/// <summary>
/// 新增使用者廠商權限 DTO (SYS0113)
/// </summary>
public class CreateUserVendorDto
{
    public string VendorId { get; set; } = string.Empty;
}

/// <summary>
/// 新增使用者部門權限 DTO (SYS0113)
/// </summary>
public class CreateUserDepartmentDto
{
    public string DeptId { get; set; } = string.Empty;
}

/// <summary>
/// 新增使用者（含分店廠商部門設定）DTO (SYS0113)
/// </summary>
public class CreateUserWithShopsVendorsDeptsDto : CreateUserDto
{
    public List<CreateUserShopDto> Shops { get; set; } = new();
    public List<CreateUserVendorDto> Vendors { get; set; } = new();
    public List<CreateUserDepartmentDto> Departments { get; set; } = new();
    public List<CreateUserButtonDto> Buttons { get; set; } = new();
}

/// <summary>
/// 修改使用者（含分店廠商部門設定）DTO (SYS0113)
/// </summary>
public class UpdateUserWithShopsVendorsDeptsDto : UpdateUserDto
{
    public List<CreateUserShopDto> Shops { get; set; } = new();
    public List<CreateUserVendorDto> Vendors { get; set; } = new();
    public List<CreateUserDepartmentDto> Departments { get; set; } = new();
    public List<CreateUserButtonDto> Buttons { get; set; } = new();
}

/// <summary>
/// 使用者按鈕權限 DTO (SYS0113)
/// </summary>
public class UserButtonDto
{
    public long Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string ButtonId { get; set; } = string.Empty;
    public string? ButtonName { get; set; }
}

/// <summary>
/// 新增使用者按鈕權限 DTO (SYS0113)
/// </summary>
public class CreateUserButtonDto
{
    public string ButtonId { get; set; } = string.Empty;
}

/// <summary>
/// 重設密碼結果 DTO (SYS0113)
/// </summary>
public class ResetPasswordResultDto
{
    public string NewPassword { get; set; } = string.Empty;
}

/// <summary>
/// 重設密碼請求 DTO (SYS0113)
/// </summary>
public class ResetPasswordRequestDto
{
    public string? NewPassword { get; set; }
    public bool AutoGenerate { get; set; } = false;
}
