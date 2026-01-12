namespace ErpCore.Application.Services.System;

/// <summary>
/// Active Directory 服務介面 (SYS0114)
/// </summary>
public interface IActiveDirectoryService
{
    /// <summary>
    /// 驗證 Active Directory 使用者是否存在
    /// </summary>
    Task<AdUserInfo> ValidateUserAsync(string domain, string userPrincipalName);

    /// <summary>
    /// 驗證 Active Directory 使用者帳號密碼
    /// </summary>
    Task<bool> AuthenticateAsync(string domain, string userPrincipalName, string password);

    /// <summary>
    /// 取得 Active Directory 使用者資訊
    /// </summary>
    Task<AdUserInfo> GetUserInfoAsync(string domain, string userPrincipalName);
}

/// <summary>
/// Active Directory 使用者資訊
/// </summary>
public class AdUserInfo
{
    public bool Exists { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? DisplayName { get; set; }
    public string? DistinguishedName { get; set; }
}
