using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Application.Services.BusinessReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.BusinessReport;

/// <summary>
/// 銷退卡管理控制器 (SYSL310)
/// </summary>
[Route("api/v1/return-cards")]
public class ReturnCardController : BaseController
{
    private readonly IReturnCardService _service;

    public ReturnCardController(
        IReturnCardService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢銷退卡列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ReturnCardDto>>>> GetReturnCards(
        [FromQuery] ReturnCardQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetReturnCardsAsync(query);
            return result;
        }, "查詢銷退卡列表失敗");
    }

    /// <summary>
    /// 根據UUID查詢單筆銷退卡
    /// </summary>
    [HttpGet("{uuid}")]
    public async Task<ActionResult<ApiResponse<ReturnCardDto>>> GetReturnCardByUuid(Guid uuid)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetReturnCardByUuidAsync(uuid);
            if (result == null)
            {
                throw new Exception($"找不到銷退卡: {uuid}");
            }
            return result;
        }, "查詢銷退卡失敗");
    }

    /// <summary>
    /// 新增銷退卡
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreateReturnCard([FromBody] CreateReturnCardDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateReturnCardAsync(dto);
            return result;
        }, "新增銷退卡失敗");
    }

    /// <summary>
    /// 修改銷退卡
    /// </summary>
    [HttpPut("{uuid}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateReturnCard(
        Guid uuid,
        [FromBody] UpdateReturnCardDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateReturnCardAsync(uuid, dto);
            return (object)null!;
        }, "修改銷退卡失敗");
    }

    /// <summary>
    /// 刪除銷退卡
    /// </summary>
    [HttpDelete("{uuid}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteReturnCard(Guid uuid)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteReturnCardAsync(uuid);
            return (object)null!;
        }, "刪除銷退卡失敗");
    }
}

