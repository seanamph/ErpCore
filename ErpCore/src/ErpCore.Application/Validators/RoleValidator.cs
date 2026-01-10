namespace ErpCore.Application.Validators;

using ErpCore.Application.DTOs.System;

/// <summary>
/// 角色資料驗證器
/// 用於 FluentValidation 配置（如採用 FluentValidation）
/// </summary>
public class RoleValidator
{
    // 注意：此專案目前使用手動驗證，此檔案作為模板參考
    // 如需使用 FluentValidation，請安裝 FluentValidation 套件並實作 AbstractValidator 類別
    
    /// <summary>
    /// 手動驗證角色資料
    /// </summary>
    public static (bool IsValid, string? ErrorMessage) Validate(RoleDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.RoleId))
        {
            return (false, "角色編號不能為空");
        }

        if (string.IsNullOrWhiteSpace(dto.RoleName))
        {
            return (false, "角色名稱不能為空");
        }

        return (true, null);
    }
}

