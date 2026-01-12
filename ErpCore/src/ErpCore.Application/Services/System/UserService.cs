using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using System.Security.Cryptography;
using System.Text;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 使用者服務實作
/// </summary>
public class UserService : BaseService, IUserService
{
    private readonly IUserRepository _repository;
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ExportHelper _exportHelper;
    private readonly IUserBusinessTypeRepository _userBusinessTypeRepository;
    private readonly IUserWarehouseAreaRepository _userWarehouseAreaRepository;
    private readonly IUserStoreRepository _userStoreRepository;
    private readonly IUserShopRepository _userShopRepository;
    private readonly IUserVendorRepository _userVendorRepository;
    private readonly IUserDepartmentRepository _userDepartmentRepository;
    private readonly IUserButtonRepository _userButtonRepository;

    public UserService(
        IUserRepository repository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext,
        ExportHelper exportHelper,
        IUserBusinessTypeRepository userBusinessTypeRepository,
        IUserWarehouseAreaRepository userWarehouseAreaRepository,
        IUserStoreRepository userStoreRepository,
        IUserShopRepository userShopRepository,
        IUserVendorRepository userVendorRepository,
        IUserDepartmentRepository userDepartmentRepository,
        IUserButtonRepository userButtonRepository) : base(logger, userContext)
    {
        _repository = repository;
        _connectionFactory = connectionFactory;
        _exportHelper = exportHelper;
        _userBusinessTypeRepository = userBusinessTypeRepository;
        _userWarehouseAreaRepository = userWarehouseAreaRepository;
        _userStoreRepository = userStoreRepository;
        _userShopRepository = userShopRepository;
        _userVendorRepository = userVendorRepository;
        _userDepartmentRepository = userDepartmentRepository;
        _userButtonRepository = userButtonRepository;
    }

    public async Task<PagedResult<UserDto>> GetUsersAsync(UserQueryDto query)
    {
        try
        {
            var repositoryQuery = new UserQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                UserId = query.UserId,
                UserName = query.UserName,
                OrgId = query.OrgId,
                Status = query.Status,
                UserType = query.UserType,
                Title = query.Title,
                ShopId = query.ShopId,
                StartDateFrom = query.StartDateFrom,
                StartDateTo = query.StartDateTo,
                EndDateFrom = query.EndDateFrom,
                EndDateTo = query.EndDateTo,
                LastLoginDateFrom = query.LastLoginDateFrom,
                LastLoginDateTo = query.LastLoginDateTo
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => new UserDto
            {
                UserId = x.UserId,
                UserName = x.UserName,
                Title = x.Title,
                OrgId = x.OrgId,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                LastLoginDate = x.LastLoginDate,
                LastLoginIp = x.LastLoginIp,
                Status = x.Status,
                UserType = x.UserType,
                Notes = x.Notes,
                UserPriority = x.UserPriority,
                ShopId = x.ShopId,
                LoginCount = x.LoginCount,
                ChangePwdDate = x.ChangePwdDate,
                FloorId = x.FloorId,
                AreaId = x.AreaId,
                BtypeId = x.BtypeId,
                StoreId = x.StoreId,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return new PagedResult<UserDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢使用者列表失敗", ex);
            throw;
        }
    }

    public async Task<UserDto> GetUserAsync(string userId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(userId);
            if (entity == null)
            {
                throw new InvalidOperationException($"使用者不存在: {userId}");
            }

            return new UserDto
            {
                UserId = entity.UserId,
                UserName = entity.UserName,
                Title = entity.Title,
                OrgId = entity.OrgId,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                LastLoginDate = entity.LastLoginDate,
                LastLoginIp = entity.LastLoginIp,
                Status = entity.Status,
                UserType = entity.UserType,
                Notes = entity.Notes,
                UserPriority = entity.UserPriority,
                ShopId = entity.ShopId,
                LoginCount = entity.LoginCount,
                ChangePwdDate = entity.ChangePwdDate,
                FloorId = entity.FloorId,
                AreaId = entity.AreaId,
                BtypeId = entity.BtypeId,
                StoreId = entity.StoreId,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢使用者失敗: {userId}", ex);
            throw;
        }
    }

    public async Task<string> CreateUserAsync(CreateUserDto dto)
    {
        try
        {
            // 驗證資料
            ValidateCreateDto(dto);

            // 檢查是否已存在
            var exists = await _repository.ExistsAsync(dto.UserId);
            if (exists)
            {
                throw new InvalidOperationException($"使用者已存在: {dto.UserId}");
            }

            // 加密密碼
            var hashedPassword = !string.IsNullOrEmpty(dto.UserPassword)
                ? HashPassword(dto.UserPassword)
                : null;

            var entity = new User
            {
                UserId = dto.UserId,
                UserName = dto.UserName,
                UserPassword = hashedPassword,
                Title = dto.Title,
                OrgId = dto.OrgId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status,
                UserType = dto.UserType,
                Notes = dto.Notes,
                UserPriority = dto.UserPriority,
                ShopId = dto.ShopId,
                LoginCount = 0,
                ChangePwdDate = hashedPassword != null ? DateTime.Now : null,
                FloorId = dto.FloorId,
                AreaId = dto.AreaId,
                BtypeId = dto.BtypeId,
                StoreId = dto.StoreId,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(entity);

            return entity.UserId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增使用者失敗: {dto.UserId}", ex);
            throw;
        }
    }

    public async Task UpdateUserAsync(string userId, UpdateUserDto dto)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(userId);
            if (entity == null)
            {
                throw new InvalidOperationException($"使用者不存在: {userId}");
            }

            entity.UserName = dto.UserName;
            entity.Title = dto.Title;
            entity.OrgId = dto.OrgId;
            entity.StartDate = dto.StartDate;
            entity.EndDate = dto.EndDate;
            entity.Status = dto.Status;
            entity.UserType = dto.UserType;
            entity.Notes = dto.Notes;
            entity.UserPriority = dto.UserPriority;
            entity.ShopId = dto.ShopId;
            entity.FloorId = dto.FloorId;
            entity.AreaId = dto.AreaId;
            entity.BtypeId = dto.BtypeId;
            entity.StoreId = dto.StoreId;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改使用者失敗: {userId}", ex);
            throw;
        }
    }

    public async Task DeleteUserAsync(string userId)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(userId);
            if (entity == null)
            {
                throw new InvalidOperationException($"使用者不存在: {userId}");
            }

            await _repository.DeleteAsync(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除使用者失敗: {userId}", ex);
            throw;
        }
    }

    public async Task DeleteUsersBatchAsync(BatchDeleteUserDto dto)
    {
        try
        {
            foreach (var userId in dto.UserIds)
            {
                await DeleteUserAsync(userId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除使用者失敗", ex);
            throw;
        }
    }

    public async Task ChangePasswordAsync(string userId, ChangePasswordDto dto)
    {
        try
        {
            // 檢查使用者是否存在
            var entity = await _repository.GetByIdAsync(userId);
            if (entity == null)
            {
                throw new InvalidOperationException($"使用者不存在: {userId}");
            }

            // 驗證舊密碼（如果存在）
            if (!string.IsNullOrEmpty(entity.UserPassword))
            {
                var oldPasswordHash = HashPassword(dto.OldPassword);
                if (entity.UserPassword != oldPasswordHash)
                {
                    throw new InvalidOperationException("舊密碼不正確");
                }
            }

            // 加密新密碼
            var newPasswordHash = HashPassword(dto.NewPassword);

            // 更新密碼
            await _repository.UpdatePasswordAsync(userId, newPasswordHash);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改密碼失敗: {userId}", ex);
            throw;
        }
    }

    private void ValidateCreateDto(CreateUserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.UserId))
        {
            throw new ArgumentException("使用者編號不能為空");
        }

        if (string.IsNullOrWhiteSpace(dto.UserName))
        {
            throw new ArgumentException("使用者名稱不能為空");
        }

        if (dto.Status != "A" && dto.Status != "I" && dto.Status != "L")
        {
            throw new ArgumentException("狀態必須為 A(啟用)、I(停用) 或 L(鎖定)");
        }
    }

    /// <summary>
    /// 驗證使用者編號和密碼 (SYS0130)
    /// </summary>
    public async Task<UserValidationResultDto> ValidateUserAsync(string userId, string password)
    {
        try
        {
            // 檢查使用者是否存在
            var entity = await _repository.GetByIdAsync(userId);
            if (entity == null)
            {
                return new UserValidationResultDto
                {
                    IsValid = false,
                    ErrorCode = "USER_INVALID",
                    ErrorMessage = "使用者編號錯誤"
                };
            }

            // 驗證密碼
            if (string.IsNullOrEmpty(entity.UserPassword))
            {
                return new UserValidationResultDto
                {
                    IsValid = false,
                    ErrorCode = "PASSWORD_INVALID",
                    ErrorMessage = "使用者尚未設定密碼"
                };
            }

            var passwordHash = HashPassword(password);
            if (entity.UserPassword != passwordHash)
            {
                return new UserValidationResultDto
                {
                    IsValid = false,
                    ErrorCode = "PASSWORD_INVALID",
                    ErrorMessage = "使用者密碼錯誤"
                };
            }

            return new UserValidationResultDto
            {
                IsValid = true,
                UserId = entity.UserId,
                UserName = entity.UserName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"驗證使用者失敗: {userId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 更新使用者帳戶原則 (SYS0130)
    /// </summary>
    public async Task UpdateAccountPolicyAsync(string userId, string? newPassword, DateTime? endDate)
    {
        try
        {
            // 檢查使用者是否存在
            var entity = await _repository.GetByIdAsync(userId);
            if (entity == null)
            {
                throw new InvalidOperationException($"使用者不存在: {userId}");
            }

            // 更新密碼（如果有提供）
            if (!string.IsNullOrEmpty(newPassword))
            {
                var newPasswordHash = HashPassword(newPassword);
                await _repository.UpdatePasswordAsync(userId, newPasswordHash);
            }

            // 更新帳號終止日
            if (endDate.HasValue)
            {
                entity.EndDate = endDate.Value;
                entity.UpdatedBy = GetCurrentUserId();
                entity.UpdatedAt = DateTime.Now;
                await _repository.UpdateAsync(entity);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新使用者帳戶原則失敗: {userId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 更新帳號終止日 (SYS0130)
    /// </summary>
    public async Task UpdateEndDateAsync(string userId, DateTime? endDate)
    {
        try
        {
            // 檢查使用者是否存在
            var entity = await _repository.GetByIdAsync(userId);
            if (entity == null)
            {
                throw new InvalidOperationException($"使用者不存在: {userId}");
            }

            // 更新帳號終止日
            entity.EndDate = endDate;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;
            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新帳號終止日失敗: {userId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 匯出使用者查詢結果 (SYS0910)
    /// </summary>
    public async Task<byte[]> ExportUserQueryAsync(UserQueryDto query, string exportFormat)
    {
        try
        {
            // 查詢所有資料（不分頁）
            var allDataQuery = new UserQueryDto
            {
                PageIndex = 1,
                PageSize = int.MaxValue,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                UserId = query.UserId,
                UserName = query.UserName,
                OrgId = query.OrgId,
                Status = query.Status,
                UserType = query.UserType,
                Title = query.Title,
                ShopId = query.ShopId
            };

            var result = await GetUsersAsync(allDataQuery);

            // 定義匯出欄位
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "UserId", DisplayName = "使用者代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "UserName", DisplayName = "使用者名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Title", DisplayName = "職稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "OrgId", DisplayName = "組織代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "UserType", DisplayName = "使用者型態", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Status", DisplayName = "帳號狀態", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "StartDate", DisplayName = "啟用日期", DataType = ExportDataType.Date },
                new ExportColumn { PropertyName = "EndDate", DisplayName = "終止日期", DataType = ExportDataType.Date },
                new ExportColumn { PropertyName = "LastLoginDate", DisplayName = "最後登入日期", DataType = ExportDataType.DateTime },
                new ExportColumn { PropertyName = "LastLoginIp", DisplayName = "最後登入IP", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "LoginCount", DisplayName = "登入次數", DataType = ExportDataType.Number },
                new ExportColumn { PropertyName = "ChangePwdDate", DisplayName = "密碼變更日期", DataType = ExportDataType.DateTime },
                new ExportColumn { PropertyName = "ShopId", DisplayName = "商店代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Notes", DisplayName = "備註", DataType = ExportDataType.String }
            };

            var title = "使用者查詢結果";

            if (exportFormat.ToLower() == "excel")
            {
                return _exportHelper.ExportToExcel(result.Items, columns, "使用者查詢結果", title);
            }
            else if (exportFormat.ToLower() == "pdf")
            {
                return _exportHelper.ExportToPdf(result.Items, columns, title);
            }
            else
            {
                throw new ArgumentException($"不支援的匯出格式: {exportFormat}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出使用者查詢結果失敗", ex);
            throw;
        }
    }

    public async Task<UserDto> GetCurrentUserAsync()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(userId) || userId == "SYSTEM")
            {
                throw new UnauthorizedAccessException("未登入或使用者資訊不存在");
            }

            return await GetUserAsync(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError("取得當前登入使用者資訊失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 重設密碼 (SYS0110)
    /// </summary>
    public async Task ResetPasswordAsync(string userId, ResetPasswordDto dto)
    {
        try
        {
            // 檢查使用者是否存在
            var entity = await _repository.GetByIdAsync(userId);
            if (entity == null)
            {
                throw new InvalidOperationException($"使用者不存在: {userId}");
            }

            // 驗證新密碼
            if (string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                throw new ArgumentException("新密碼不能為空");
            }

            // 加密新密碼
            var newPasswordHash = HashPassword(dto.NewPassword);

            // 更新密碼
            await _repository.UpdatePasswordAsync(userId, newPasswordHash);

            _logger.LogInfo($"管理員重設使用者密碼成功: {userId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"重設密碼失敗: {userId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 更新使用者狀態 (SYS0110)
    /// </summary>
    public async Task UpdateStatusAsync(string userId, UpdateStatusDto dto)
    {
        try
        {
            // 檢查使用者是否存在
            var entity = await _repository.GetByIdAsync(userId);
            if (entity == null)
            {
                throw new InvalidOperationException($"使用者不存在: {userId}");
            }

            // 驗證狀態值
            if (dto.Status != "A" && dto.Status != "I" && dto.Status != "L")
            {
                throw new ArgumentException("狀態必須為 A(啟用)、I(停用) 或 L(鎖定)");
            }

            // 更新狀態
            entity.Status = dto.Status;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);

            _logger.LogInfo($"更新使用者狀態成功: {userId}, 狀態: {dto.Status}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新使用者狀態失敗: {userId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 密碼雜湊（使用 SHA256）
    /// 注意：實際應用中應使用更安全的加密方式，如 BCrypt 或 Argon2
    /// </summary>
    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    /// <summary>
    /// 查詢使用者詳細資料（含業種儲位）(SYS0111)
    /// </summary>
    public async Task<UserDetailDto> GetUserDetailAsync(string userId)
    {
        try
        {
            var user = await _repository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"使用者不存在: {userId}");
            }

            var businessTypes = await _userBusinessTypeRepository.GetByUserIdAsync(userId);
            var warehouseAreas = await _userWarehouseAreaRepository.GetByUserIdAsync(userId);
            var stores = await _userStoreRepository.GetByUserIdAsync(userId);
            var shops = await _userShopRepository.GetByUserIdAsync(userId);
            var vendors = await _userVendorRepository.GetByUserIdAsync(userId);
            var departments = await _userDepartmentRepository.GetByUserIdAsync(userId);
            var buttons = await _userButtonRepository.GetByUserIdAsync(userId);

            return new UserDetailDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Title = user.Title,
                OrgId = user.OrgId,
                StartDate = user.StartDate,
                EndDate = user.EndDate,
                Status = user.Status,
                UserType = user.UserType,
                Notes = user.Notes,
                UserPriority = user.UserPriority,
                ShopId = user.ShopId,
                BusinessTypes = businessTypes.Select(x => new UserBusinessTypeDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    BtypeMId = x.BtypeMId,
                    BtypeId = x.BtypeId,
                    PtypeId = x.PtypeId
                }).ToList(),
                WarehouseAreas = warehouseAreas.Select(x => new UserWarehouseAreaDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    WareaId1 = x.WareaId1,
                    WareaId2 = x.WareaId2,
                    WareaId3 = x.WareaId3
                }).ToList(),
                Stores = stores.Select(x => new UserStoreDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    StoreId = x.StoreId
                }).ToList(),
                Shops = shops.Select(x => new UserShopDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    PShopId = x.PShopId,
                    ShopId = x.ShopId,
                    SiteId = x.SiteId
                }).ToList(),
                Vendors = vendors.Select(x => new UserVendorDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    VendorId = x.VendorId
                }).ToList(),
                Departments = departments.Select(x => new UserDepartmentDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    DeptId = x.DeptId
                }).ToList(),
                Buttons = buttons.Select(x => new UserButtonDto
                {
                    Id = x.TKey,
                    UserId = x.UserId,
                    ButtonId = x.ButtonId
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢使用者詳細資料失敗: {userId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 查詢業種大分類列表 (SYS0111)
    /// </summary>
    public async Task<List<BusinessTypeMajorDto>> GetBusinessTypeMajorsAsync()
    {
        try
        {
            // TODO: 實作查詢業種大分類的邏輯，這裡需要根據實際資料表結構實作
            // 暫時返回空列表，實際應該從資料庫查詢
            return new List<BusinessTypeMajorDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢業種大分類列表失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 查詢業種中分類列表 (SYS0111)
    /// </summary>
    public async Task<List<BusinessTypeMiddleDto>> GetBusinessTypeMiddlesAsync(string btypeMId)
    {
        try
        {
            // TODO: 實作查詢業種中分類的邏輯，這裡需要根據實際資料表結構實作
            // 暫時返回空列表，實際應該從資料庫查詢
            return new List<BusinessTypeMiddleDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢業種中分類列表失敗: {btypeMId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 查詢業種小分類列表 (SYS0111)
    /// </summary>
    public async Task<List<BusinessTypeMinorDto>> GetBusinessTypeMinorsAsync(string btypeId)
    {
        try
        {
            // TODO: 實作查詢業種小分類的邏輯，這裡需要根據實際資料表結構實作
            // 暫時返回空列表，實際應該從資料庫查詢
            return new List<BusinessTypeMinorDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢業種小分類列表失敗: {btypeId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 查詢儲位列表 (SYS0111)
    /// </summary>
    public async Task<List<WarehouseAreaDto>> GetWarehouseAreasAsync(int? level, string? parentId)
    {
        try
        {
            // TODO: 實作查詢儲位的邏輯，這裡需要根據實際資料表結構實作
            // 暫時返回空列表，實際應該從資料庫查詢
            return new List<WarehouseAreaDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢儲位列表失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 查詢7X承租分店列表 (SYS0111)
    /// </summary>
    public async Task<List<StoreDto>> GetStoresAsync()
    {
        try
        {
            // TODO: 實作查詢7X承租分店的邏輯，這裡需要根據實際資料表結構實作
            // 暫時返回空列表，實際應該從資料庫查詢
            return new List<StoreDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢7X承租分店列表失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 新增使用者（含業種儲位設定）(SYS0111)
    /// </summary>
    public async Task<string> CreateUserWithBusinessTypesAsync(CreateUserWithBusinessTypesDto dto)
    {
        try
        {
            // 先建立基本使用者資料
            var createUserDto = new CreateUserDto
            {
                UserId = dto.UserId,
                UserName = dto.UserName,
                UserPassword = dto.UserPassword,
                Title = dto.Title,
                OrgId = dto.OrgId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status,
                UserType = dto.UserType,
                Notes = dto.Notes,
                UserPriority = dto.UserPriority,
                ShopId = dto.ShopId,
                FloorId = dto.FloorId,
                AreaId = dto.AreaId,
                BtypeId = dto.BtypeId,
                StoreId = dto.StoreId
            };

            var userId = await CreateUserAsync(createUserDto);

            // 建立業種權限
            if (dto.BusinessTypes != null && dto.BusinessTypes.Count > 0)
            {
                var businessTypes = dto.BusinessTypes.Select(x => new UserBusinessType
                {
                    UserId = userId,
                    BtypeMId = x.BtypeMId,
                    BtypeId = x.BtypeId,
                    PtypeId = x.PtypeId,
                    CreatedBy = GetCurrentUserId(),
                    CreatedAt = DateTime.Now
                }).ToList();

                await _userBusinessTypeRepository.CreateBatchAsync(businessTypes);
            }

            // 建立儲位權限
            if (dto.WarehouseAreas != null && dto.WarehouseAreas.Count > 0)
            {
                var warehouseAreas = dto.WarehouseAreas.Select(x => new UserWarehouseArea
                {
                    UserId = userId,
                    WareaId1 = x.WareaId1,
                    WareaId2 = x.WareaId2,
                    WareaId3 = x.WareaId3,
                    CreatedBy = GetCurrentUserId(),
                    CreatedAt = DateTime.Now
                }).ToList();

                await _userWarehouseAreaRepository.CreateBatchAsync(warehouseAreas);
            }

            // 建立7X承租分店權限
            if (dto.Stores != null && dto.Stores.Count > 0)
            {
                var stores = dto.Stores.Select(x => new UserStore
                {
                    UserId = userId,
                    StoreId = x.StoreId,
                    CreatedBy = GetCurrentUserId(),
                    CreatedAt = DateTime.Now
                }).ToList();

                await _userStoreRepository.CreateBatchAsync(stores);
            }

            return userId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增使用者（含業種儲位設定）失敗: {dto.UserId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 修改使用者（含業種儲位設定）(SYS0111)
    /// </summary>
    public async Task UpdateUserWithBusinessTypesAsync(string userId, UpdateUserWithBusinessTypesDto dto)
    {
        try
        {
            // 先更新基本使用者資料
            var updateUserDto = new UpdateUserDto
            {
                UserName = dto.UserName,
                Title = dto.Title,
                OrgId = dto.OrgId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status,
                UserType = dto.UserType,
                Notes = dto.Notes,
                UserPriority = dto.UserPriority,
                ShopId = dto.ShopId,
                FloorId = dto.FloorId,
                AreaId = dto.AreaId,
                BtypeId = dto.BtypeId,
                StoreId = dto.StoreId
            };

            await UpdateUserAsync(userId, updateUserDto);

            // 刪除舊的業種權限
            await _userBusinessTypeRepository.DeleteByUserIdAsync(userId);

            // 建立新的業種權限
            if (dto.BusinessTypes != null && dto.BusinessTypes.Count > 0)
            {
                var businessTypes = dto.BusinessTypes.Select(x => new UserBusinessType
                {
                    UserId = userId,
                    BtypeMId = x.BtypeMId,
                    BtypeId = x.BtypeId,
                    PtypeId = x.PtypeId,
                    CreatedBy = GetCurrentUserId(),
                    CreatedAt = DateTime.Now
                }).ToList();

                await _userBusinessTypeRepository.CreateBatchAsync(businessTypes);
            }

            // 刪除舊的儲位權限
            await _userWarehouseAreaRepository.DeleteByUserIdAsync(userId);

            // 建立新的儲位權限
            if (dto.WarehouseAreas != null && dto.WarehouseAreas.Count > 0)
            {
                var warehouseAreas = dto.WarehouseAreas.Select(x => new UserWarehouseArea
                {
                    UserId = userId,
                    WareaId1 = x.WareaId1,
                    WareaId2 = x.WareaId2,
                    WareaId3 = x.WareaId3,
                    CreatedBy = GetCurrentUserId(),
                    CreatedAt = DateTime.Now
                }).ToList();

                await _userWarehouseAreaRepository.CreateBatchAsync(warehouseAreas);
            }

            // 刪除舊的7X承租分店權限
            await _userStoreRepository.DeleteByUserIdAsync(userId);

            // 建立新的7X承租分店權限
            if (dto.Stores != null && dto.Stores.Count > 0)
            {
                var stores = dto.Stores.Select(x => new UserStore
                {
                    UserId = userId,
                    StoreId = x.StoreId,
                    CreatedBy = GetCurrentUserId(),
                    CreatedAt = DateTime.Now
                }).ToList();

                await _userStoreRepository.CreateBatchAsync(stores);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改使用者（含業種儲位設定）失敗: {userId}", ex);
            throw;
        }
    }

    public async Task<List<ParentShopDto>> GetParentShopsAsync()
    {
        try
        {
            // TODO: 根據實際資料表結構實現查詢邏輯
            // 這裡需要根據實際的總公司資料表查詢
            const string sql = @"
                SELECT DISTINCT PShopId AS PShopId, PShopName AS PShopName
                FROM Shops
                WHERE PShopId IS NOT NULL
                ORDER BY PShopId";

            var result = await QueryAsync<ParentShopDto>(sql);
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢總公司列表失敗", ex);
            throw;
        }
    }

    public async Task<List<ShopDto>> GetShopsAsync(string? pShopId)
    {
        try
        {
            // TODO: 根據實際資料表結構實現查詢邏輯
            const string sql = @"
                SELECT ShopId, ShopName, PShopId
                FROM Shops
                WHERE (@PShopId IS NULL OR PShopId = @PShopId)
                ORDER BY ShopId";

            var result = await QueryAsync<ShopDto>(sql, new { PShopId = pShopId });
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢分店列表失敗: {pShopId}", ex);
            throw;
        }
    }

    public async Task<List<SiteDto>> GetSitesAsync(string? shopId)
    {
        try
        {
            // TODO: 根據實際資料表結構實現查詢邏輯
            const string sql = @"
                SELECT SiteId, SiteName, ShopId
                FROM Sites
                WHERE (@ShopId IS NULL OR ShopId = @ShopId)
                ORDER BY SiteId";

            var result = await QueryAsync<SiteDto>(sql, new { ShopId = shopId });
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢據點列表失敗: {shopId}", ex);
            throw;
        }
    }

    public async Task<List<VendorDto>> GetVendorsAsync()
    {
        try
        {
            // TODO: 根據實際資料表結構實現查詢邏輯
            const string sql = @"
                SELECT VendorId, VendorName
                FROM Vendors
                ORDER BY VendorId";

            var result = await QueryAsync<VendorDto>(sql);
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢廠商列表失敗", ex);
            throw;
        }
    }

    public async Task<List<DepartmentDto>> GetDepartmentsAsync()
    {
        try
        {
            // TODO: 根據實際資料表結構實現查詢邏輯
            const string sql = @"
                SELECT DeptId, DeptName
                FROM Departments
                ORDER BY DeptId";

            var result = await QueryAsync<DepartmentDto>(sql);
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢部門列表失敗", ex);
            throw;
        }
    }

    public async Task<string> CreateUserWithShopsVendorsDeptsAsync(CreateUserWithShopsVendorsDeptsDto dto)
    {
        try
        {
            // 先建立使用者基本資料
            var userId = await CreateUserAsync(dto);

            // 建立總公司/分店權限
            if (dto.Shops != null && dto.Shops.Count > 0)
            {
                var shops = dto.Shops.Select(x => new UserShop
                {
                    UserId = userId,
                    PShopId = x.PShopId,
                    ShopId = x.ShopId,
                    SiteId = x.SiteId,
                    CreatedBy = GetCurrentUserId(),
                    CreatedAt = DateTime.Now
                }).ToList();

                await _userShopRepository.CreateBatchAsync(shops);
            }

            // 建立廠商權限
            if (dto.Vendors != null && dto.Vendors.Count > 0)
            {
                var vendors = dto.Vendors.Select(x => new UserVendor
                {
                    UserId = userId,
                    VendorId = x.VendorId,
                    CreatedBy = GetCurrentUserId(),
                    CreatedAt = DateTime.Now
                }).ToList();

                await _userVendorRepository.CreateBatchAsync(vendors);
            }

            // 建立部門權限
            if (dto.Departments != null && dto.Departments.Count > 0)
            {
                var departments = dto.Departments.Select(x => new UserDepartment
                {
                    UserId = userId,
                    DeptId = x.DeptId,
                    CreatedBy = GetCurrentUserId(),
                    CreatedAt = DateTime.Now
                }).ToList();

                await _userDepartmentRepository.CreateBatchAsync(departments);
            }

            // 建立按鈕權限
            if (dto.Buttons != null && dto.Buttons.Count > 0)
            {
                var buttons = dto.Buttons.Select(x => new UserButton
                {
                    UserId = userId,
                    ButtonId = x.ButtonId,
                    CreatedBy = GetCurrentUserId(),
                    CreatedAt = DateTime.Now,
                    CreatedPriority = GetCurrentUserPriority(),
                    CreatedGroup = GetCurrentUserGroup()
                }).ToList();

                await _userButtonRepository.CreateBatchAsync(buttons);
            }

            return userId;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增使用者（含分店廠商部門設定）失敗", ex);
            throw;
        }
    }

    public async Task UpdateUserWithShopsVendorsDeptsAsync(string userId, UpdateUserWithShopsVendorsDeptsDto dto)
    {
        try
        {
            // 先更新使用者基本資料
            await UpdateUserAsync(userId, dto);

            // 刪除舊的總公司/分店權限
            await _userShopRepository.DeleteByUserIdAsync(userId);

            // 建立新的總公司/分店權限
            if (dto.Shops != null && dto.Shops.Count > 0)
            {
                var shops = dto.Shops.Select(x => new UserShop
                {
                    UserId = userId,
                    PShopId = x.PShopId,
                    ShopId = x.ShopId,
                    SiteId = x.SiteId,
                    CreatedBy = GetCurrentUserId(),
                    CreatedAt = DateTime.Now
                }).ToList();

                await _userShopRepository.CreateBatchAsync(shops);
            }

            // 刪除舊的廠商權限
            await _userVendorRepository.DeleteByUserIdAsync(userId);

            // 建立新的廠商權限
            if (dto.Vendors != null && dto.Vendors.Count > 0)
            {
                var vendors = dto.Vendors.Select(x => new UserVendor
                {
                    UserId = userId,
                    VendorId = x.VendorId,
                    CreatedBy = GetCurrentUserId(),
                    CreatedAt = DateTime.Now
                }).ToList();

                await _userVendorRepository.CreateBatchAsync(vendors);
            }

            // 刪除舊的部門權限
            await _userDepartmentRepository.DeleteByUserIdAsync(userId);

            // 建立新的部門權限
            if (dto.Departments != null && dto.Departments.Count > 0)
            {
                var departments = dto.Departments.Select(x => new UserDepartment
                {
                    UserId = userId,
                    DeptId = x.DeptId,
                    CreatedBy = GetCurrentUserId(),
                    CreatedAt = DateTime.Now
                }).ToList();

                await _userDepartmentRepository.CreateBatchAsync(departments);
            }

            // 刪除舊的按鈕權限
            await _userButtonRepository.DeleteByUserIdAsync(userId);

            // 建立新的按鈕權限
            if (dto.Buttons != null && dto.Buttons.Count > 0)
            {
                var buttons = dto.Buttons.Select(x => new UserButton
                {
                    UserId = userId,
                    ButtonId = x.ButtonId,
                    CreatedBy = GetCurrentUserId(),
                    CreatedAt = DateTime.Now,
                    CreatedPriority = GetCurrentUserPriority(),
                    CreatedGroup = GetCurrentUserGroup()
                }).ToList();

                await _userButtonRepository.CreateBatchAsync(buttons);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改使用者（含分店廠商部門設定）失敗: {userId}", ex);
            throw;
        }
    }

    public async Task<ResetPasswordResultDto> ResetPasswordWithAutoAsync(string userId, ResetPasswordRequestDto dto)
    {
        try
        {
            string newPassword;
            
            if (dto.AutoGenerate)
            {
                // 自動產生密碼
                newPassword = GenerateRandomPassword();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(dto.NewPassword))
                {
                    throw new ArgumentException("新密碼不能為空");
                }
                newPassword = dto.NewPassword;
            }

            // 重設密碼
            var resetPasswordDto = new ResetPasswordDto
            {
                NewPassword = newPassword
            };
            
            await ResetPasswordAsync(userId, resetPasswordDto);

            return new ResetPasswordResultDto
            {
                NewPassword = newPassword
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"重設密碼（含自動產生）失敗: {userId}", ex);
            throw;
        }
    }

    private string GenerateRandomPassword()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
        var random = new Random();
        var password = new string(Enumerable.Repeat(chars, 12)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        return password;
    }

    private async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<T>(sql, param);
    }
}

