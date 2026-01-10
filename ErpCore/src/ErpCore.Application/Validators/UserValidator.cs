namespace ErpCore.Application.Validators;

using ErpCore.Application.DTOs.System;

/// <summary>
/// 使用者資料驗證器
/// 用於 FluentValidation 配置（如採用 FluentValidation）
/// </summary>
public class UserValidator
{
    // 注意：此專案目前使用手動驗證，此檔案作為模板參考
    // 如需使用 FluentValidation，請安裝 FluentValidation 套件並實作 AbstractValidator 類別
    
    /// <summary>
    /// 手動驗證使用者資料
    /// </summary>
    public static (bool IsValid, string? ErrorMessage) Validate(UserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.UserId))
        {
            return (false, "使用者編號不能為空");
        }

        if (string.IsNullOrWhiteSpace(dto.UserName))
        {
            return (false, "使用者名稱不能為空");
        }

        if (dto.StartDate.HasValue && dto.EndDate.HasValue && dto.StartDate > dto.EndDate)
        {
            return (false, "帳號有效起始日不能晚於終止日");
        }

        return (true, null);
    }
}

