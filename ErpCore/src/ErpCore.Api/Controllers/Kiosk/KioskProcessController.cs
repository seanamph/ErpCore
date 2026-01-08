using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Kiosk;
using ErpCore.Application.Services.Kiosk;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Kiosk;

/// <summary>
/// Kiosk資料處理作業控制器
/// </summary>
[Route("api/v1/kiosk/process")]
public class KioskProcessController : BaseController
{
    private readonly IKioskProcessService _service;

    public KioskProcessController(
        IKioskProcessService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 處理Kiosk請求
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<KioskProcessResponseDto>>> ProcessRequest(
        [FromBody] KioskProcessRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.ProcessRequestAsync(request);
            return result;
        }, "處理Kiosk請求失敗");
    }
}

