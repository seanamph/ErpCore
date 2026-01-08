using ErpCore.Application.DTOs.TaxAccounting;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.TaxAccounting;

/// <summary>
/// 現金流量中分類服務介面 (SYST132)
/// </summary>
public interface ICashFlowMediumTypeService
{
    Task<PagedResult<CashFlowMediumTypeDto>> GetCashFlowMediumTypesAsync(CashFlowMediumTypeQueryDto query);
    Task<CashFlowMediumTypeDto> GetCashFlowMediumTypeByIdAsync(string cashLTypeId, string cashMTypeId);
    Task<string> CreateCashFlowMediumTypeAsync(CreateCashFlowMediumTypeDto dto);
    Task UpdateCashFlowMediumTypeAsync(string cashLTypeId, string cashMTypeId, UpdateCashFlowMediumTypeDto dto);
    Task DeleteCashFlowMediumTypeAsync(string cashLTypeId, string cashMTypeId);
    Task<bool> ExistsAsync(string cashLTypeId, string cashMTypeId);
}

