using ErpCore.Application.DTOs.TaxAccounting;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.TaxAccounting;

/// <summary>
/// 現金流量科目設定服務介面 (SYST133)
/// </summary>
public interface ICashFlowSubjectTypeService
{
    Task<PagedResult<CashFlowSubjectTypeDto>> GetCashFlowSubjectTypesAsync(CashFlowSubjectTypeQueryDto query);
    Task<CashFlowSubjectTypeDto> GetCashFlowSubjectTypeByIdAsync(string cashMTypeId, string cashSTypeId);
    Task<string> CreateCashFlowSubjectTypeAsync(CreateCashFlowSubjectTypeDto dto);
    Task UpdateCashFlowSubjectTypeAsync(string cashMTypeId, string cashSTypeId, UpdateCashFlowSubjectTypeDto dto);
    Task DeleteCashFlowSubjectTypeAsync(string cashMTypeId, string cashSTypeId);
    Task<bool> ExistsAsync(string cashMTypeId, string cashSTypeId);
}

