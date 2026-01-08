using ErpCore.Application.DTOs.TaxAccounting;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.TaxAccounting;

/// <summary>
/// 現金流量大分類服務介面 (SYST131)
/// </summary>
public interface ICashFlowLargeTypeService
{
    Task<PagedResult<CashFlowLargeTypeDto>> GetCashFlowLargeTypesAsync(CashFlowLargeTypeQueryDto query);
    Task<CashFlowLargeTypeDto> GetCashFlowLargeTypeByIdAsync(string cashLTypeId);
    Task<string> CreateCashFlowLargeTypeAsync(CreateCashFlowLargeTypeDto dto);
    Task UpdateCashFlowLargeTypeAsync(string cashLTypeId, UpdateCashFlowLargeTypeDto dto);
    Task DeleteCashFlowLargeTypeAsync(string cashLTypeId);
    Task<bool> ExistsAsync(string cashLTypeId);
}

