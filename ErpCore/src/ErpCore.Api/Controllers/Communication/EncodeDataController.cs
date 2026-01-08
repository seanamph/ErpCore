using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Communication;
using ErpCore.Application.Services.Communication;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Communication;

/// <summary>
/// 資料編碼作業控制器
/// </summary>
[Route("api/v1/utils/encode")]
public class EncodeDataController : BaseController
{
    private readonly IEncodeService _encodeService;

    public EncodeDataController(
        IEncodeService encodeService,
        ILoggerService logger) : base(logger)
    {
        _encodeService = encodeService;
    }

    /// <summary>
    /// Base64編碼
    /// </summary>
    [HttpPost("base64")]
    public async Task<ActionResult<ApiResponse<EncodeResultDto>>> Base64Encode([FromBody] Base64EncodeRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            return await _encodeService.Base64EncodeAsync(request);
        }, "Base64編碼失敗");
    }

    /// <summary>
    /// Base64解碼
    /// </summary>
    [HttpPost("base64/decode")]
    public async Task<ActionResult<ApiResponse<EncodeResultDto>>> Base64Decode([FromBody] Base64EncodeRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            return await _encodeService.Base64DecodeAsync(request);
        }, "Base64解碼失敗");
    }

    /// <summary>
    /// 字串加密
    /// </summary>
    [HttpPost("string")]
    public async Task<ActionResult<ApiResponse<EncodeResultDto>>> StringEncode([FromBody] StringEncodeRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            return await _encodeService.StringEncodeAsync(request);
        }, "字串加密失敗");
    }

    /// <summary>
    /// 字串解密
    /// </summary>
    [HttpPost("string/decode")]
    public async Task<ActionResult<ApiResponse<EncodeResultDto>>> StringDecode([FromBody] StringEncodeRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            return await _encodeService.StringDecodeAsync(request);
        }, "字串解密失敗");
    }

    /// <summary>
    /// 日期加密
    /// </summary>
    [HttpPost("date")]
    public async Task<ActionResult<ApiResponse<EncodeResultDto>>> DateEncode([FromBody] DateEncodeRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            return await _encodeService.DateEncodeAsync(request);
        }, "日期加密失敗");
    }

    /// <summary>
    /// 日期解密
    /// </summary>
    [HttpPost("date/decode")]
    public async Task<ActionResult<ApiResponse<EncodeResultDto>>> DateDecode([FromBody] DateEncodeRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            return await _encodeService.DateDecodeAsync(request);
        }, "日期解密失敗");
    }
}

