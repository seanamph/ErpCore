using ErpCore.Application.DTOs.OtherModule;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.OtherModule;
using ErpCore.Infrastructure.Repositories.OtherModule;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using System.Text.Json;

namespace ErpCore.Application.Services.OtherModule;

/// <summary>
/// EIP整合服務實作
/// 提供 EIP 系統整合功能
/// </summary>
public class EipIntegrationService : BaseService, IEipIntegrationService
{
    private readonly IEipIntegrationRepository _repository;

    public EipIntegrationService(
        IEipIntegrationRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<EipIntegrationDto>> GetIntegrationsAsync(EipIntegrationQueryDto query)
    {
        try
        {
            _logger.LogInfo("查詢EIP整合設定列表");

            var repositoryQuery = new EipIntegrationQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                ProgId = query.ProgId,
                PageId = query.PageId,
                Status = query.Status
            };

            var result = await _repository.QueryAsync(repositoryQuery);
            return new PagedResult<EipIntegrationDto>
            {
                Items = result.Items.Select(i => new EipIntegrationDto
                {
                    IntegrationId = i.IntegrationId,
                    ProgId = i.ProgId,
                    PageId = i.PageId,
                    EipUrl = i.EipUrl,
                    Fid = i.Fid,
                    SingleField = i.SingleField,
                    MultiField = i.MultiField,
                    DetailTable = i.DetailTable,
                    MultiMSeqNo = i.MultiMSeqNo,
                    Status = i.Status,
                    CreatedBy = i.CreatedBy,
                    CreatedAt = i.CreatedAt,
                    UpdatedBy = i.UpdatedBy,
                    UpdatedAt = i.UpdatedAt
                }).ToList(),
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢EIP整合設定列表失敗", ex);
            throw;
        }
    }

    public async Task<EipIntegrationDto?> GetIntegrationAsync(string progId, string pageId)
    {
        try
        {
            _logger.LogInfo($"取得EIP整合設定: {progId}/{pageId}");

            var integration = await _repository.GetByProgIdAndPageIdAsync(progId, pageId);
            if (integration == null)
            {
                return null;
            }

            return new EipIntegrationDto
            {
                IntegrationId = integration.IntegrationId,
                ProgId = integration.ProgId,
                PageId = integration.PageId,
                EipUrl = integration.EipUrl,
                Fid = integration.Fid,
                SingleField = integration.SingleField,
                MultiField = integration.MultiField,
                DetailTable = integration.DetailTable,
                MultiMSeqNo = integration.MultiMSeqNo,
                Status = integration.Status,
                CreatedBy = integration.CreatedBy,
                CreatedAt = integration.CreatedAt,
                UpdatedBy = integration.UpdatedBy,
                UpdatedAt = integration.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"取得EIP整合設定失敗: {progId}/{pageId}", ex);
            throw;
        }
    }

    public async Task<string> GetSsoUrlAsync()
    {
        try
        {
            _logger.LogInfo("取得EIP單一登入URL");

            // 這裡應該根據實際的EIP系統SSO機制生成URL
            // 實際應該包含認證token、重導向URL等參數
            var ssoUrl = "https://eip.example.com/sso?token=xxx"; // 實際應該動態生成

            return ssoUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError("取得EIP單一登入URL失敗", ex);
            throw;
        }
    }

    public async Task<EipTransactionDto> SendFormToEipAsync(SendFormToEipRequestDto request)
    {
        try
        {
            _logger.LogInfo($"傳送表單至EIP: {request.ProgId}/{request.PageId}");

            var integration = await _repository.GetByProgIdAndPageIdAsync(request.ProgId, request.PageId);
            if (integration == null)
            {
                throw new InvalidOperationException($"EIP整合設定不存在: {request.ProgId}/{request.PageId}");
            }

            // 這裡應該調用EIP系統API傳送表單資料
            // 實際應該根據integration的設定組裝資料並發送到EIP系統
            var requestData = JsonSerializer.Serialize(request);
            var responseData = "{}"; // 實際應該從EIP系統取得回應

            var transaction = new EipTransaction
            {
                IntegrationId = integration.IntegrationId,
                ProgId = request.ProgId,
                PageId = request.PageId,
                FlowId = null, // 實際應該從EIP回應中取得
                RequestData = requestData,
                ResponseData = responseData,
                Status = "PENDING", // 實際應該根據EIP回應設定
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            var result = await _repository.CreateTransactionAsync(transaction);

            return new EipTransactionDto
            {
                TransactionId = result.TransactionId,
                IntegrationId = result.IntegrationId,
                ProgId = result.ProgId,
                PageId = result.PageId,
                FlowId = result.FlowId,
                RequestData = result.RequestData,
                ResponseData = result.ResponseData,
                Status = result.Status,
                ErrorMessage = result.ErrorMessage,
                CreatedBy = result.CreatedBy,
                CreatedAt = result.CreatedAt,
                UpdatedAt = result.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"傳送表單至EIP失敗: {request.ProgId}/{request.PageId}", ex);
            throw;
        }
    }

    public async Task<long> CreateIntegrationAsync(CreateEipIntegrationDto dto)
    {
        try
        {
            _logger.LogInfo($"新增EIP整合設定: {dto.ProgId}/{dto.PageId}");

            var integration = new EipIntegration
            {
                ProgId = dto.ProgId,
                PageId = dto.PageId,
                EipUrl = dto.EipUrl,
                Fid = dto.Fid,
                SingleField = dto.SingleField,
                MultiField = dto.MultiField,
                DetailTable = dto.DetailTable,
                MultiMSeqNo = dto.MultiMSeqNo,
                Status = dto.Status,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var result = await _repository.CreateAsync(integration);
            return result.IntegrationId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增EIP整合設定失敗: {dto.ProgId}/{dto.PageId}", ex);
            throw;
        }
    }

    public async Task UpdateIntegrationAsync(long integrationId, CreateEipIntegrationDto dto)
    {
        try
        {
            _logger.LogInfo($"修改EIP整合設定: {integrationId}");

            var existingIntegration = await _repository.GetByProgIdAndPageIdAsync(dto.ProgId, dto.PageId);
            if (existingIntegration == null || existingIntegration.IntegrationId != integrationId)
            {
                throw new InvalidOperationException($"EIP整合設定不存在: {integrationId}");
            }

            existingIntegration.EipUrl = dto.EipUrl;
            existingIntegration.Fid = dto.Fid;
            existingIntegration.SingleField = dto.SingleField;
            existingIntegration.MultiField = dto.MultiField;
            existingIntegration.DetailTable = dto.DetailTable;
            existingIntegration.MultiMSeqNo = dto.MultiMSeqNo;
            existingIntegration.Status = dto.Status;
            existingIntegration.UpdatedBy = GetCurrentUserId();
            existingIntegration.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(existingIntegration);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改EIP整合設定失敗: {integrationId}", ex);
            throw;
        }
    }

    public async Task DeleteIntegrationAsync(long integrationId)
    {
        try
        {
            _logger.LogInfo($"刪除EIP整合設定: {integrationId}");

            await _repository.DeleteAsync(integrationId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除EIP整合設定失敗: {integrationId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<EipTransactionDto>> GetTransactionsAsync(string? progId, string? pageId, int pageIndex, int pageSize)
    {
        try
        {
            _logger.LogInfo($"查詢EIP交易記錄列表: progId={progId}, pageId={pageId}");

            var result = await _repository.GetTransactionsAsync(progId, pageId, pageIndex, pageSize);
            return new PagedResult<EipTransactionDto>
            {
                Items = result.Items.Select(t => new EipTransactionDto
                {
                    TransactionId = t.TransactionId,
                    IntegrationId = t.IntegrationId,
                    ProgId = t.ProgId,
                    PageId = t.PageId,
                    FlowId = t.FlowId,
                    RequestData = t.RequestData,
                    ResponseData = t.ResponseData,
                    Status = t.Status,
                    ErrorMessage = t.ErrorMessage,
                    CreatedBy = t.CreatedBy,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                }).ToList(),
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢EIP交易記錄列表失敗", ex);
            throw;
        }
    }
}

