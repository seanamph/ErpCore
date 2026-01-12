using ErpCore.Application.DTOs.System;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 使用者服務介面
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 查詢使用者列表
    /// </summary>
    Task<PagedResult<UserDto>> GetUsersAsync(UserQueryDto query);

    /// <summary>
    /// 查詢單筆使用者
    /// </summary>
    Task<UserDto> GetUserAsync(string userId);

    /// <summary>
    /// 新增使用者
    /// </summary>
    Task<string> CreateUserAsync(CreateUserDto dto);

    /// <summary>
    /// 修改使用者
    /// </summary>
    Task UpdateUserAsync(string userId, UpdateUserDto dto);

    /// <summary>
    /// 刪除使用者
    /// </summary>
    Task DeleteUserAsync(string userId);

    /// <summary>
    /// 批次刪除使用者
    /// </summary>
    Task DeleteUsersBatchAsync(BatchDeleteUserDto dto);

    /// <summary>
    /// 修改密碼
    /// </summary>
    Task ChangePasswordAsync(string userId, ChangePasswordDto dto);

    /// <summary>
    /// 驗證使用者編號和密碼 (SYS0130)
    /// </summary>
    Task<UserValidationResultDto> ValidateUserAsync(string userId, string password);

    /// <summary>
    /// 更新使用者帳戶原則 (SYS0130)
    /// </summary>
    Task UpdateAccountPolicyAsync(string userId, string? newPassword, DateTime? endDate);

    /// <summary>
    /// 更新帳號終止日 (SYS0130)
    /// </summary>
    Task UpdateEndDateAsync(string userId, DateTime? endDate);

    /// <summary>
    /// 匯出使用者查詢結果 (SYS0910)
    /// </summary>
    Task<byte[]> ExportUserQueryAsync(UserQueryDto query, string exportFormat);

    /// <summary>
    /// 取得當前登入使用者資訊
    /// </summary>
    Task<UserDto> GetCurrentUserAsync();

    /// <summary>
    /// 重設密碼 (SYS0110)
    /// </summary>
    Task ResetPasswordAsync(string userId, ResetPasswordDto dto);

    /// <summary>
    /// 更新使用者狀態 (SYS0110)
    /// </summary>
    Task UpdateStatusAsync(string userId, UpdateStatusDto dto);

    /// <summary>
    /// 查詢使用者詳細資料（含業種儲位）(SYS0111)
    /// </summary>
    Task<UserDetailDto> GetUserDetailAsync(string userId);

    /// <summary>
    /// 查詢業種大分類列表 (SYS0111)
    /// </summary>
    Task<List<BusinessTypeMajorDto>> GetBusinessTypeMajorsAsync();

    /// <summary>
    /// 查詢業種中分類列表 (SYS0111)
    /// </summary>
    Task<List<BusinessTypeMiddleDto>> GetBusinessTypeMiddlesAsync(string btypeMId);

    /// <summary>
    /// 查詢業種小分類列表 (SYS0111)
    /// </summary>
    Task<List<BusinessTypeMinorDto>> GetBusinessTypeMinorsAsync(string btypeId);

    /// <summary>
    /// 查詢儲位列表 (SYS0111)
    /// </summary>
    Task<List<WarehouseAreaDto>> GetWarehouseAreasAsync(int? level, string? parentId);

    /// <summary>
    /// 查詢7X承租分店列表 (SYS0111)
    /// </summary>
    Task<List<StoreDto>> GetStoresAsync();

    /// <summary>
    /// 新增使用者（含業種儲位設定）(SYS0111)
    /// </summary>
    Task<string> CreateUserWithBusinessTypesAsync(CreateUserWithBusinessTypesDto dto);

    /// <summary>
    /// 修改使用者（含業種儲位設定）(SYS0111)
    /// </summary>
    Task UpdateUserWithBusinessTypesAsync(string userId, UpdateUserWithBusinessTypesDto dto);

    /// <summary>
    /// 查詢總公司列表 (SYS0113)
    /// </summary>
    Task<List<ParentShopDto>> GetParentShopsAsync();

    /// <summary>
    /// 查詢分店列表 (SYS0113)
    /// </summary>
    Task<List<ShopDto>> GetShopsAsync(string? pShopId);

    /// <summary>
    /// 查詢據點列表 (SYS0113)
    /// </summary>
    Task<List<SiteDto>> GetSitesAsync(string? shopId);

    /// <summary>
    /// 查詢廠商列表 (SYS0113)
    /// </summary>
    Task<List<VendorDto>> GetVendorsAsync();

    /// <summary>
    /// 查詢部門列表 (SYS0113)
    /// </summary>
    Task<List<DepartmentDto>> GetDepartmentsAsync();

    /// <summary>
    /// 新增使用者（含分店廠商部門設定）(SYS0113)
    /// </summary>
    Task<string> CreateUserWithShopsVendorsDeptsAsync(CreateUserWithShopsVendorsDeptsDto dto);

    /// <summary>
    /// 修改使用者（含分店廠商部門設定）(SYS0113)
    /// </summary>
    Task UpdateUserWithShopsVendorsDeptsAsync(string userId, UpdateUserWithShopsVendorsDeptsDto dto);

    /// <summary>
    /// 重設密碼（含自動產生）(SYS0113)
    /// </summary>
    Task<ResetPasswordResultDto> ResetPasswordWithAutoAsync(string userId, ResetPasswordRequestDto dto);

    /// <summary>
    /// 查詢使用者詳細資料（含AD和組織）(SYS0114)
    /// </summary>
    Task<UserDetailWithAdOrgsDto> GetUserDetailWithAdOrgsAsync(string userId);

    /// <summary>
    /// 查詢組織列表 (SYS0114)
    /// </summary>
    Task<List<OrganizationDto>> GetOrganizationsAsync();

    /// <summary>
    /// 新增使用者（含AD和組織設定）(SYS0114)
    /// </summary>
    Task<string> CreateUserWithAdOrgsAsync(CreateUserWithAdOrgsDto dto);

    /// <summary>
    /// 修改使用者（含AD和組織設定）(SYS0114)
    /// </summary>
    Task UpdateUserWithAdOrgsAsync(string userId, UpdateUserWithAdOrgsDto dto);

    /// <summary>
    /// 驗證Active Directory使用者 (SYS0114)
    /// </summary>
    Task<ValidateAdUserResultDto> ValidateAdUserAsync(ValidateAdUserDto dto);
}

