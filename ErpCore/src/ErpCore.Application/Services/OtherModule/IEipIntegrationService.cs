using ErpCore.Application.DTOs.OtherModule;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.OtherModule;

/// <summary>
/// EIP整合服務介面
/// 提供 EIP 系統整合功能
/// </summary>
public interface IEipIntegrationService
{
    /// <summary>
    /// 查詢EIP整合設定列表
    /// </summary>
    Task<PagedResult<EipIntegrationDto>> GetIntegrationsAsync(EipIntegrationQueryDto query);

    /// <summary>
    /// 根據作業編號和頁面代碼取得整合設定
    /// </summary>
    Task<EipIntegrationDto?> GetIntegrationAsync(string progId, string pageId);

    /// <summary>
    /// EIP單一登入
    /// </summary>
    Task<string> GetSsoUrlAsync();

    /// <summary>
    /// 傳送表單至EIP
    /// </summary>
    Task<EipTransactionDto> SendFormToEipAsync(SendFormToEipRequestDto request);

    /// <summary>
    /// 新增整合設定
    /// </summary>
    Task<long> CreateIntegrationAsync(CreateEipIntegrationDto dto);

    /// <summary>
    /// 修改整合設定
    /// </summary>
    Task UpdateIntegrationAsync(long integrationId, CreateEipIntegrationDto dto);

    /// <summary>
    /// 刪除整合設定
    /// </summary>
    Task DeleteIntegrationAsync(long integrationId);

    /// <summary>
    /// 查詢交易記錄列表
    /// </summary>
    Task<PagedResult<EipTransactionDto>> GetTransactionsAsync(string? progId, string? pageId, int pageIndex, int pageSize);
}

