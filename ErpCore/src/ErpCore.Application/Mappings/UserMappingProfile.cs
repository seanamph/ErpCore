namespace ErpCore.Application.Mappings;

using ErpCore.Domain.Entities.System;
using ErpCore.Application.DTOs.System;

/// <summary>
/// 使用者物件對應設定
/// 用於 AutoMapper 配置（如採用 AutoMapper）
/// </summary>
public class UserMappingProfile
{
    // 注意：此專案目前使用手動映射，此檔案作為模板參考
    // 如需使用 AutoMapper，請安裝 AutoMapper 套件並實作 Profile 類別
    
    /// <summary>
    /// 手動映射：Entity 轉 DTO
    /// </summary>
    public static UserDto MapToDto(User entity)
    {
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
            UpdatedAt = entity.UpdatedAt,
            CreatedPriority = entity.CreatedPriority,
            CreatedGroup = entity.CreatedGroup
        };
    }

    /// <summary>
    /// 手動映射：DTO 轉 Entity
    /// </summary>
    public static User MapToEntity(UserDto dto)
    {
        return new User
        {
            UserId = dto.UserId,
            UserName = dto.UserName,
            Title = dto.Title,
            OrgId = dto.OrgId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            LastLoginDate = dto.LastLoginDate,
            LastLoginIp = dto.LastLoginIp,
            Status = dto.Status,
            UserType = dto.UserType,
            Notes = dto.Notes,
            UserPriority = dto.UserPriority,
            ShopId = dto.ShopId,
            LoginCount = dto.LoginCount,
            ChangePwdDate = dto.ChangePwdDate,
            FloorId = dto.FloorId,
            AreaId = dto.AreaId,
            BtypeId = dto.BtypeId,
            StoreId = dto.StoreId,
            CreatedBy = dto.CreatedBy,
            CreatedAt = dto.CreatedAt,
            UpdatedBy = dto.UpdatedBy,
            UpdatedAt = dto.UpdatedAt,
            CreatedPriority = dto.CreatedPriority,
            CreatedGroup = dto.CreatedGroup
        };
    }
}

