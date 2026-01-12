using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.System;

/// <summary>
/// Active Directory 服務實作 (SYS0114)
/// </summary>
public class ActiveDirectoryService : IActiveDirectoryService
{
    private readonly ILoggerService _logger;

    public ActiveDirectoryService(ILoggerService logger)
    {
        _logger = logger;
    }

    public async Task<AdUserInfo> ValidateUserAsync(string domain, string userPrincipalName)
    {
        try
        {
            return await Task.Run(() =>
            {
                using var context = new PrincipalContext(ContextType.Domain, domain);
                using var user = UserPrincipal.FindByIdentity(context, IdentityType.UserPrincipalName, userPrincipalName);

                if (user != null)
                {
                    return new AdUserInfo
                    {
                        Exists = true,
                        UserName = user.SamAccountName,
                        Email = user.EmailAddress,
                        DisplayName = user.DisplayName,
                        DistinguishedName = user.DistinguishedName
                    };
                }

                return new AdUserInfo { Exists = false };
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"驗證 Active Directory 使用者失敗: {domain}\\{userPrincipalName}", ex);
            return new AdUserInfo { Exists = false };
        }
    }

    public async Task<bool> AuthenticateAsync(string domain, string userPrincipalName, string password)
    {
        try
        {
            return await Task.Run(() =>
            {
                using var context = new PrincipalContext(ContextType.Domain, domain);
                return context.ValidateCredentials(userPrincipalName, password);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Active Directory 驗證失敗: {domain}\\{userPrincipalName}", ex);
            return false;
        }
    }

    public async Task<AdUserInfo> GetUserInfoAsync(string domain, string userPrincipalName)
    {
        return await ValidateUserAsync(domain, userPrincipalName);
    }
}
