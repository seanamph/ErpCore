using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Core;
using ErpCore.Application.Services.Core;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Core;

/// <summary>
/// 系統功能控制器
/// 提供系統識別、系統註冊、關於頁面等功能
/// </summary>
[Route("api/v1/core/system-function")]
public class SystemFunctionController : BaseController
{
    private readonly ISystemFunctionService _service;

    public SystemFunctionController(
        ISystemFunctionService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    #region Identify - 系統識別功能

    /// <summary>
    /// 取得系統識別資訊
    /// </summary>
    [HttpGet("system-identity")]
    public async Task<ActionResult<ApiResponse<SystemIdentityDto>>> GetSystemIdentity()
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSystemIdentityAsync();
            if (result == null)
            {
                throw new Exception("系統識別資訊不存在");
            }
            return result;
        }, "取得系統識別資訊失敗");
    }

    /// <summary>
    /// 取得選單設定
    /// </summary>
    [HttpGet("system-identity/menu")]
    public async Task<ActionResult<ApiResponse<MenuConfigDto>>> GetMenuConfig()
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetMenuConfigAsync();
            return result;
        }, "取得選單設定失敗");
    }

    /// <summary>
    /// 更新系統識別設定
    /// </summary>
    [HttpPut("system-identity")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateSystemIdentity([FromBody] UpdateSystemIdentityDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateSystemIdentityAsync(dto);
        }, "更新系統識別設定失敗");
    }

    /// <summary>
    /// 系統初始化
    /// </summary>
    [HttpPost("system-identity/initialize")]
    public async Task<ActionResult<ApiResponse<SystemInitDto>>> InitializeSystem()
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.InitializeSystemAsync();
            return result;
        }, "系統初始化失敗");
    }

    #endregion

    #region MakeRegFile - 系統註冊功能

    /// <summary>
    /// 取得硬體資訊
    /// </summary>
    [HttpGet("registration/hardware-info")]
    public async Task<ActionResult<ApiResponse<HardwareInfoDto>>> GetHardwareInfo()
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetHardwareInfoAsync();
            return result;
        }, "取得硬體資訊失敗");
    }

    /// <summary>
    /// 生成註冊檔案
    /// </summary>
    [HttpPost("registration/generate")]
    public async Task<ActionResult<ApiResponse<RegistrationResultDto>>> GenerateRegistration([FromBody] GenerateRegistrationDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GenerateRegistrationAsync(dto);
            return result;
        }, "生成註冊檔案失敗");
    }

    /// <summary>
    /// 驗證註冊檔案
    /// </summary>
    [HttpPost("registration/verify")]
    public async Task<ActionResult<ApiResponse<VerificationResultDto>>> VerifyRegistration([FromBody] VerifyRegistrationDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.VerifyRegistrationAsync(dto);
            return result;
        }, "驗證註冊檔案失敗");
    }

    /// <summary>
    /// 上傳註冊檔案進行驗證
    /// </summary>
    [HttpPost("registration/upload")]
    public async Task<ActionResult<ApiResponse<VerificationResultDto>>> UploadRegistrationFile(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiResponse<VerificationResultDto>.Fail("請選擇檔案", "FILE_REQUIRED"));
            }

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var fileContent = memoryStream.ToArray();

            var result = await _service.VerifyRegistrationFileAsync(fileContent);
            return Ok(ApiResponse<VerificationResultDto>.Ok(result, "驗證完成"));
        }
        catch (Exception ex)
        {
            _logger.LogError("上傳註冊檔案失敗", ex);
            return BadRequest(ApiResponse<VerificationResultDto>.Fail(ex.Message, "UPLOAD_ERROR"));
        }
    }

    /// <summary>
    /// 下載註冊檔案
    /// </summary>
    [HttpGet("registration/files/{registrationId}/download")]
    public async Task<ActionResult> DownloadRegistrationFile(long registrationId)
    {
        try
        {
            var fileBytes = await _service.GetRegistrationFileAsync(registrationId);
            if (fileBytes == null)
            {
                return NotFound(ApiResponse<object>.Fail("檔案不存在", "FILE_NOT_FOUND"));
            }

            var fileName = $"RegSys_{registrationId}.dll";
            return File(fileBytes, "application/octet-stream", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError("下載註冊檔案失敗", ex);
            return BadRequest(ApiResponse<object>.Fail(ex.Message, "DOWNLOAD_ERROR"));
        }
    }

    /// <summary>
    /// 查詢註冊記錄
    /// </summary>
    [HttpGet("registrations")]
    public async Task<ActionResult<ApiResponse<PagedResult<SystemRegistrationDto>>>> GetSystemRegistrations([FromQuery] SystemRegistrationQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetSystemRegistrationsAsync(query);
            return result;
        }, "查詢註冊記錄失敗");
    }

    /// <summary>
    /// 撤銷註冊
    /// </summary>
    [HttpPut("registrations/{registrationId}/revoke")]
    public async Task<ActionResult<ApiResponse<object>>> RevokeRegistration(long registrationId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.RevokeRegistrationAsync(registrationId);
        }, "撤銷註冊失敗");
    }

    /// <summary>
    /// 網頁註冊
    /// </summary>
    [HttpPost("registration/web-register")]
    public async Task<ActionResult<string>> WebRegister([FromBody] WebRegisterDto dto)
    {
        try
        {
            var result = await _service.WebRegisterAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError("網頁註冊失敗", ex);
            return BadRequest(ex.Message);
        }
    }

    #endregion

    #region about - 關於頁面功能

    /// <summary>
    /// 取得關於頁面資訊
    /// </summary>
    [HttpGet("about")]
    public async Task<ActionResult<ApiResponse<AboutInfoDto>>> GetAboutInfo()
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetAboutInfoAsync();
            return result;
        }, "取得關於頁面資訊失敗");
    }

    #endregion
}

