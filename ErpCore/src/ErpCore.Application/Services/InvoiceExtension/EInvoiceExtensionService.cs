using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.InvoiceExtension;
using ErpCore.Infrastructure.Repositories.InvoiceExtension;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.InvoiceExtension;

/// <summary>
/// 電子發票擴展服務實作
/// </summary>
public class EInvoiceExtensionService : BaseService, IEInvoiceExtensionService
{
    private readonly IEInvoiceExtensionRepository _repository;

    public EInvoiceExtensionService(
        IEInvoiceExtensionRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<EInvoiceExtensionDto>> GetExtensionsAsync(EInvoiceExtensionQueryDto query)
    {
        try
        {
            var repositoryQuery = new EInvoiceExtensionQuery
            {
                InvoiceId = query.InvoiceId,
                ExtensionType = query.ExtensionType,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(x => new EInvoiceExtensionDto
            {
                ExtensionId = x.ExtensionId,
                InvoiceId = x.InvoiceId,
                ExtensionType = x.ExtensionType,
                CreatedAt = x.CreatedAt
            }).ToList();

            return new PagedResult<EInvoiceExtensionDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢電子發票擴展列表失敗", ex);
            throw;
        }
    }

    public async Task<EInvoiceExtensionDto?> GetExtensionByIdAsync(long extensionId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(extensionId);
            if (entity == null) return null;

            return new EInvoiceExtensionDto
            {
                ExtensionId = entity.ExtensionId,
                InvoiceId = entity.InvoiceId,
                ExtensionType = entity.ExtensionType,
                CreatedAt = entity.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢電子發票擴展失敗: {extensionId}", ex);
            throw;
        }
    }

    public async Task<long> CreateExtensionAsync(CreateEInvoiceExtensionDto dto)
    {
        try
        {
            var entity = new EInvoiceExtension
            {
                InvoiceId = dto.InvoiceId,
                ExtensionType = dto.ExtensionType,
                ExtensionData = dto.ExtensionData,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var id = await _repository.CreateAsync(entity);
            _logger.LogInfo($"新增電子發票擴展成功: {id}");
            return id;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增電子發票擴展失敗", ex);
            throw;
        }
    }

    public async Task UpdateExtensionAsync(long extensionId, UpdateEInvoiceExtensionDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(extensionId);
            if (entity == null)
            {
                throw new Exception($"電子發票擴展不存在: {extensionId}");
            }

            entity.ExtensionType = dto.ExtensionType;
            entity.ExtensionData = dto.ExtensionData;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"修改電子發票擴展成功: {extensionId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改電子發票擴展失敗: {extensionId}", ex);
            throw;
        }
    }

    public async Task DeleteExtensionAsync(long extensionId)
    {
        try
        {
            await _repository.DeleteAsync(extensionId);
            _logger.LogInfo($"刪除電子發票擴展成功: {extensionId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除電子發票擴展失敗: {extensionId}", ex);
            throw;
        }
    }
}

