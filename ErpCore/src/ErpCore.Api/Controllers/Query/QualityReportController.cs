using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Query;
using ErpCore.Application.Services.Query;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Query;

/// <summary>
/// 質量管理報表功能控制器 (SYSQ310-SYSQ340)
/// 提供零用金相關報表的查詢、列印、匯出功能
/// </summary>
[Route("api/v1/quality/report")]
public class QualityReportController : BaseController
{
    private readonly IPcCashService _pcCashService;

    public QualityReportController(
        IPcCashService pcCashService,
        ILoggerService logger) : base(logger)
    {
        _pcCashService = pcCashService;
    }

    #region 零用金支付表報表 (SYSQ310)

    /// <summary>
    /// 查詢零用金支付表報表
    /// </summary>
    [HttpPost("pc-cash-payment")]
    public async Task<ActionResult<ApiResponse<PagedResult<PcCashDto>>>> GetPcCashPaymentReport(
        [FromBody] PcCashQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            // 只查詢已申請、已請款、已拋轉、已審核的零用金
            query.CashStatus = "APPLIED,REQUESTED,TRANSFERRED,APPROVED";
            var result = await _pcCashService.QueryAsync(query);
            return result;
        }, "查詢零用金支付表報表失敗");
    }

    #endregion

    #region 支付申請單報表 (SYSQ320)

    /// <summary>
    /// 查詢支付申請單報表
    /// </summary>
    [HttpPost("payment-application")]
    public async Task<ActionResult<ApiResponse<PagedResult<PcCashDto>>>> GetPaymentApplicationReport(
        [FromBody] PcCashQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            // 只查詢已請款的零用金
            query.CashStatus = "REQUESTED";
            var result = await _pcCashService.QueryAsync(query);
            return result;
        }, "查詢支付申請單報表失敗");
    }

    #endregion

    #region 零用金撥補明細表報表 (SYSQ330)

    /// <summary>
    /// 查詢零用金撥補明細表報表
    /// </summary>
    [HttpPost("pc-cash-replenishment")]
    public async Task<ActionResult<ApiResponse<PagedResult<PcCashDto>>>> GetPcCashReplenishmentReport(
        [FromBody] PcCashQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            // 只查詢已申請、已請款、已拋轉、已審核的零用金
            query.CashStatus = "APPLIED,REQUESTED,TRANSFERRED,APPROVED";
            var result = await _pcCashService.QueryAsync(query);
            return result;
        }, "查詢零用金撥補明細表報表失敗");
    }

    #endregion

    #region 零用金報表 (SYSQ340)

    /// <summary>
    /// 查詢零用金報表
    /// </summary>
    [HttpPost("pc-cash-report")]
    public async Task<ActionResult<ApiResponse<PagedResult<PcCashDto>>>> GetPcCashReport(
        [FromBody] PcCashQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _pcCashService.QueryAsync(query);
            return result;
        }, "查詢零用金報表失敗");
    }

    #endregion
}

