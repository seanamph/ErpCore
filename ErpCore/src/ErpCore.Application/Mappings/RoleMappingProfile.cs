namespace ErpCore.Application.Mappings;

using ErpCore.Domain.Entities.System;
using ErpCore.Application.DTOs.System;

/// <summary>
/// 角色物件對應設定
/// 用於 AutoMapper 配置（如採用 AutoMapper）
/// </summary>
public class RoleMappingProfile
{
    // 注意：此專案目前使用手動映射，此檔案作為模板參考
    // 如需使用 AutoMapper，請安裝 AutoMapper 套件並實作 Profile 類別
    
    /// <summary>
    /// 手動映射：Entity 轉 DTO
    /// </summary>
    public static RoleDto MapToDto(Role entity)
    {
        return new RoleDto
        {
            RoleId = entity.RoleId,
            RoleName = entity.RoleName,
            Description = entity.Description,
            Status = entity.Status,
            Notes = entity.Notes,
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
    public static Role MapToEntity(RoleDto dto)
    {
        return new Role
        {
            RoleId = dto.RoleId,
            RoleName = dto.RoleName,
            Description = dto.Description,
            Status = dto.Status,
            Notes = dto.Notes,
            CreatedBy = dto.CreatedBy,
            CreatedAt = dto.CreatedAt,
            UpdatedBy = dto.UpdatedBy,
            UpdatedAt = dto.UpdatedAt,
            CreatedPriority = dto.CreatedPriority,
            CreatedGroup = dto.CreatedGroup
        };
    }
}

