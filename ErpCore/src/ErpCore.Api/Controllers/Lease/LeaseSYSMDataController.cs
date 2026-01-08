using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Lease;
using ErpCore.Application.Services.Lease;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Lease;

/// <summary>
/// 租賃資料維護控制器 (SYSM111-SYSM138)
/// </summary>
[ApiController]
[Route("api/v1/lease-sysm/data")]
public class LeaseSYSMDataController : BaseController
{
    private readonly IParkingSpaceService _parkingSpaceService;
    private readonly ILeaseContractService _leaseContractService;

    public LeaseSYSMDataController(
        IParkingSpaceService parkingSpaceService,
        ILeaseContractService leaseContractService,
        ILoggerService logger) : base(logger)
    {
        _parkingSpaceService = parkingSpaceService;
        _leaseContractService = leaseContractService;
    }

    #region 停車位資料 (ParkingSpace)

    /// <summary>
    /// 查詢停車位列表
    /// </summary>
    [HttpGet("parking-spaces")]
    public async Task<ActionResult<ApiResponse<PagedResult<ParkingSpaceDto>>>> GetParkingSpaces(
        [FromQuery] ParkingSpaceQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _parkingSpaceService.GetParkingSpacesAsync(query);
            return result;
        }, "查詢停車位列表失敗");
    }

    /// <summary>
    /// 查詢單筆停車位
    /// </summary>
    [HttpGet("parking-spaces/{parkingSpaceId}")]
    public async Task<ActionResult<ApiResponse<ParkingSpaceDto>>> GetParkingSpace(string parkingSpaceId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _parkingSpaceService.GetParkingSpaceByIdAsync(parkingSpaceId);
            return result;
        }, $"查詢停車位失敗: {parkingSpaceId}");
    }

    /// <summary>
    /// 查詢可用停車位
    /// </summary>
    [HttpGet("parking-spaces/available")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ParkingSpaceDto>>>> GetAvailableParkingSpaces(
        [FromQuery] string? shopId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _parkingSpaceService.GetAvailableParkingSpacesAsync(shopId);
            return result;
        }, "查詢可用停車位失敗");
    }

    /// <summary>
    /// 新增停車位
    /// </summary>
    [HttpPost("parking-spaces")]
    public async Task<ActionResult<ApiResponse<ParkingSpaceDto>>> CreateParkingSpace(
        [FromBody] CreateParkingSpaceDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _parkingSpaceService.CreateParkingSpaceAsync(dto);
            return result;
        }, "新增停車位失敗");
    }

    /// <summary>
    /// 修改停車位
    /// </summary>
    [HttpPut("parking-spaces/{parkingSpaceId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateParkingSpace(
        string parkingSpaceId,
        [FromBody] UpdateParkingSpaceDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _parkingSpaceService.UpdateParkingSpaceAsync(parkingSpaceId, dto);
        }, $"修改停車位失敗: {parkingSpaceId}");
    }

    /// <summary>
    /// 刪除停車位
    /// </summary>
    [HttpDelete("parking-spaces/{parkingSpaceId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteParkingSpace(string parkingSpaceId)
    {
        return await ExecuteAsync(async () =>
        {
            await _parkingSpaceService.DeleteParkingSpaceAsync(parkingSpaceId);
        }, $"刪除停車位失敗: {parkingSpaceId}");
    }

    /// <summary>
    /// 更新停車位狀態
    /// </summary>
    [HttpPut("parking-spaces/{parkingSpaceId}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateParkingSpaceStatus(
        string parkingSpaceId,
        [FromBody] UpdateParkingSpaceStatusDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _parkingSpaceService.UpdateParkingSpaceStatusAsync(parkingSpaceId, dto.Status);
        }, $"更新停車位狀態失敗: {parkingSpaceId}");
    }

    /// <summary>
    /// 檢查停車位是否存在
    /// </summary>
    [HttpGet("parking-spaces/{parkingSpaceId}/exists")]
    public async Task<ActionResult<ApiResponse<bool>>> ParkingSpaceExists(string parkingSpaceId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _parkingSpaceService.ExistsAsync(parkingSpaceId);
            return result;
        }, $"檢查停車位是否存在失敗: {parkingSpaceId}");
    }

    #endregion

    #region 租賃合同資料 (LeaseContract)

    /// <summary>
    /// 查詢租賃合同列表
    /// </summary>
    [HttpGet("contracts")]
    public async Task<ActionResult<ApiResponse<PagedResult<LeaseContractDto>>>> GetLeaseContracts(
        [FromQuery] LeaseContractQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _leaseContractService.GetLeaseContractsAsync(query);
            return result;
        }, "查詢租賃合同列表失敗");
    }

    /// <summary>
    /// 查詢單筆租賃合同
    /// </summary>
    [HttpGet("contracts/{contractNo}")]
    public async Task<ActionResult<ApiResponse<LeaseContractDto>>> GetLeaseContract(string contractNo)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _leaseContractService.GetLeaseContractByIdAsync(contractNo);
            return result;
        }, $"查詢租賃合同失敗: {contractNo}");
    }

    /// <summary>
    /// 根據租賃編號查詢租賃合同
    /// </summary>
    [HttpGet("leases/{leaseId}/contracts")]
    public async Task<ActionResult<ApiResponse<IEnumerable<LeaseContractDto>>>> GetLeaseContractsByLeaseId(string leaseId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _leaseContractService.GetLeaseContractsByLeaseIdAsync(leaseId);
            return result;
        }, $"查詢租賃合同失敗: {leaseId}");
    }

    /// <summary>
    /// 新增租賃合同
    /// </summary>
    [HttpPost("contracts")]
    public async Task<ActionResult<ApiResponse<LeaseContractDto>>> CreateLeaseContract(
        [FromBody] CreateLeaseContractDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _leaseContractService.CreateLeaseContractAsync(dto);
            return result;
        }, "新增租賃合同失敗");
    }

    /// <summary>
    /// 修改租賃合同
    /// </summary>
    [HttpPut("contracts/{contractNo}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateLeaseContract(
        string contractNo,
        [FromBody] UpdateLeaseContractDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _leaseContractService.UpdateLeaseContractAsync(contractNo, dto);
        }, $"修改租賃合同失敗: {contractNo}");
    }

    /// <summary>
    /// 刪除租賃合同
    /// </summary>
    [HttpDelete("contracts/{contractNo}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteLeaseContract(string contractNo)
    {
        return await ExecuteAsync(async () =>
        {
            await _leaseContractService.DeleteLeaseContractAsync(contractNo);
        }, $"刪除租賃合同失敗: {contractNo}");
    }

    /// <summary>
    /// 更新租賃合同狀態
    /// </summary>
    [HttpPut("contracts/{contractNo}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateLeaseContractStatus(
        string contractNo,
        [FromBody] UpdateLeaseContractStatusDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _leaseContractService.UpdateLeaseContractStatusAsync(contractNo, dto.Status);
        }, $"更新租賃合同狀態失敗: {contractNo}");
    }

    /// <summary>
    /// 檢查租賃合同是否存在
    /// </summary>
    [HttpGet("contracts/{contractNo}/exists")]
    public async Task<ActionResult<ApiResponse<bool>>> LeaseContractExists(string contractNo)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _leaseContractService.ExistsAsync(contractNo);
            return result;
        }, $"檢查租賃合同是否存在失敗: {contractNo}");
    }

    #endregion
}

