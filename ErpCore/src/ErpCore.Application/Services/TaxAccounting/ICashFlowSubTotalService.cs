using ErpCore.Application.DTOs.TaxAccounting;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.TaxAccounting;

/// <summary>
/// 現金流量小計設定服務介面 (SYST134)
/// </summary>
public interface ICashFlowSubTotalService
{
    Task<PagedResult<CashFlowSubTotalDto>> GetCashFlowSubTotalsAsync(CashFlowSubTotalQueryDto query);
    Task<CashFlowSubTotalDto> GetCashFlowSubTotalByIdAsync(string cashLTypeId, string cashSubId);
    Task<string> CreateCashFlowSubTotalAsync(CreateCashFlowSubTotalDto dto);
    Task UpdateCashFlowSubTotalAsync(string cashLTypeId, string cashSubId, UpdateCashFlowSubTotalDto dto);
    Task DeleteCashFlowSubTotalAsync(string cashLTypeId, string cashSubId);
    Task<bool> ExistsAsync(string cashLTypeId, string cashSubId);
}

