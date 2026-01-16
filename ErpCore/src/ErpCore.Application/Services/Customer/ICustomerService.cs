using ErpCore.Application.DTOs.Customer;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Customer;

/// <summary>
/// 客戶服務介面
/// </summary>
public interface ICustomerService
{
    /// <summary>
    /// 查詢客戶列表
    /// </summary>
    Task<PagedResult<CustomerDto>> GetCustomersAsync(CustomerQueryDto query);

    /// <summary>
    /// 查詢單筆客戶
    /// </summary>
    Task<CustomerDto> GetCustomerByIdAsync(string customerId);

    /// <summary>
    /// 新增客戶
    /// </summary>
    Task<string> CreateCustomerAsync(CreateCustomerDto dto);

    /// <summary>
    /// 修改客戶
    /// </summary>
    Task UpdateCustomerAsync(string customerId, UpdateCustomerDto dto);

    /// <summary>
    /// 刪除客戶
    /// </summary>
    Task DeleteCustomerAsync(string customerId);

    /// <summary>
    /// 批次刪除客戶
    /// </summary>
    Task BatchDeleteCustomersAsync(BatchDeleteCustomerDto dto);

    /// <summary>
    /// 驗證統一編號
    /// </summary>
    Task<GuiIdValidationResultDto> ValidateGuiIdAsync(ValidateGuiIdDto dto);

    /// <summary>
    /// 進階查詢客戶列表 (CUS5120)
    /// </summary>
    Task<PagedResult<CustomerDto>> AdvancedQueryAsync(CustomerAdvancedQueryDto query);

    /// <summary>
    /// 快速搜尋客戶 (CUS5120)
    /// </summary>
    Task<List<CustomerSearchResultDto>> SearchAsync(CustomerSearchDto query);

    /// <summary>
    /// 查詢客戶交易記錄 (CUS5120)
    /// </summary>
    Task<PagedResult<CustomerTransactionDto>> GetTransactionsAsync(CustomerTransactionQueryDto query);

    /// <summary>
    /// 儲存查詢歷史記錄 (CUS5120)
    /// </summary>
    Task<QueryHistoryDto> SaveQueryHistoryAsync(SaveQueryHistoryDto dto);

    /// <summary>
    /// 取得查詢歷史記錄列表 (CUS5120)
    /// </summary>
    Task<List<QueryHistoryDto>> GetQueryHistoryAsync(string moduleCode);

    /// <summary>
    /// 刪除查詢歷史記錄 (CUS5120)
    /// </summary>
    Task DeleteQueryHistoryAsync(Guid historyId);

    /// <summary>
    /// 匯出客戶查詢結果到 Excel (CUS5120)
    /// </summary>
    Task<byte[]> ExportToExcelAsync(CustomerAdvancedQueryDto query);

    /// <summary>
    /// 查詢客戶報表 (CUS5130)
    /// </summary>
    Task<PagedResult<CustomerReportDto>> GetReportAsync(CustomerReportQueryDto query);

    /// <summary>
    /// 匯出客戶報表到 Excel (CUS5130)
    /// </summary>
    Task<byte[]> ExportReportToExcelAsync(CustomerReportQueryDto query);

    /// <summary>
    /// 匯出客戶報表到 PDF (CUS5130)
    /// </summary>
    Task<byte[]> ExportReportToPdfAsync(CustomerReportQueryDto query);
}

