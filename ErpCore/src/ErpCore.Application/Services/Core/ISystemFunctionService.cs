using ErpCore.Application.DTOs.Core;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Core;

/// <summary>
/// 系統功能服務介面
/// 提供系統識別、系統註冊、關於頁面等功能
/// </summary>
public interface ISystemFunctionService
{
    #region Identify - 系統識別功能
    
    /// <summary>
    /// 取得系統識別資訊
    /// </summary>
    Task<SystemIdentityDto?> GetSystemIdentityAsync();
    
    /// <summary>
    /// 取得選單設定
    /// </summary>
    Task<MenuConfigDto> GetMenuConfigAsync();
    
    /// <summary>
    /// 更新系統識別設定
    /// </summary>
    Task UpdateSystemIdentityAsync(UpdateSystemIdentityDto dto);
    
    /// <summary>
    /// 系統初始化
    /// </summary>
    Task<SystemInitDto> InitializeSystemAsync();
    
    #endregion

    #region MakeRegFile - 系統註冊功能
    
    /// <summary>
    /// 取得硬體資訊
    /// </summary>
    Task<HardwareInfoDto> GetHardwareInfoAsync();
    
    /// <summary>
    /// 生成註冊檔案
    /// </summary>
    Task<RegistrationResultDto> GenerateRegistrationAsync(GenerateRegistrationDto dto);
    
    /// <summary>
    /// 驗證註冊檔案
    /// </summary>
    Task<VerificationResultDto> VerifyRegistrationAsync(VerifyRegistrationDto dto);
    
    /// <summary>
    /// 上傳註冊檔案進行驗證
    /// </summary>
    Task<VerificationResultDto> VerifyRegistrationFileAsync(byte[] fileContent);
    
    /// <summary>
    /// 取得註冊檔案
    /// </summary>
    Task<byte[]?> GetRegistrationFileAsync(long registrationId);
    
    /// <summary>
    /// 查詢註冊記錄
    /// </summary>
    Task<PagedResult<SystemRegistrationDto>> GetSystemRegistrationsAsync(SystemRegistrationQueryDto query);
    
    /// <summary>
    /// 撤銷註冊
    /// </summary>
    Task RevokeRegistrationAsync(long registrationId);
    
    /// <summary>
    /// 網頁註冊
    /// </summary>
    Task<string> WebRegisterAsync(WebRegisterDto dto);
    
    #endregion

    #region about - 關於頁面功能
    
    /// <summary>
    /// 取得關於頁面資訊
    /// </summary>
    Task<AboutInfoDto> GetAboutInfoAsync();
    
    #endregion
}

