using ErpCore.Application.DTOs.Procurement;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Procurement;
using ErpCore.Infrastructure.Repositories.Procurement;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Procurement;

/// <summary>
/// 採購擴展功能服務實作 (SYSP610)
/// </summary>
public class PurchaseExtendedFunctionService : BaseService, IPurchaseExtendedFunctionService
{
    private readonly IPurchaseExtendedFunctionRepository _repository;

    public PurchaseExtendedFunctionService(
        IPurchaseExtendedFunctionRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<PurchaseExtendedFunctionDto>> GetPurchaseExtendedFunctionsAsync(PurchaseExtendedFunctionQueryDto query)
    {
        try
        {
            var repositoryQuery = new PurchaseExtendedFunctionQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                ExtFunctionId = query.ExtFunctionId,
                ExtFunctionName = query.ExtFunctionName,
                ExtFunctionType = query.ExtFunctionType,
                Status = query.Status
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<PurchaseExtendedFunctionDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢採購擴展功能列表失敗", ex);
            throw;
        }
    }

    public async Task<PurchaseExtendedFunctionDto> GetPurchaseExtendedFunctionByTKeyAsync(long tKey)
    {
        try
        {
            var purchaseExtendedFunction = await _repository.GetByIdAsync(tKey);
            if (purchaseExtendedFunction == null)
            {
                throw new InvalidOperationException($"採購擴展功能不存在: {tKey}");
            }

            return MapToDto(purchaseExtendedFunction);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢採購擴展功能失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<PurchaseExtendedFunctionDto> GetPurchaseExtendedFunctionByExtFunctionIdAsync(string extFunctionId)
    {
        try
        {
            var purchaseExtendedFunction = await _repository.GetByExtFunctionIdAsync(extFunctionId);
            if (purchaseExtendedFunction == null)
            {
                throw new InvalidOperationException($"採購擴展功能不存在: {extFunctionId}");
            }

            return MapToDto(purchaseExtendedFunction);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢採購擴展功能失敗: {extFunctionId}", ex);
            throw;
        }
    }

    public async Task<long> CreatePurchaseExtendedFunctionAsync(CreatePurchaseExtendedFunctionDto dto)
    {
        try
        {
            // 檢查功能代碼是否已存在
            if (await _repository.ExistsAsync(dto.ExtFunctionId))
            {
                throw new InvalidOperationException($"功能代碼已存在: {dto.ExtFunctionId}");
            }

            var purchaseExtendedFunction = new PurchaseExtendedFunction
            {
                ExtFunctionId = dto.ExtFunctionId,
                ExtFunctionName = dto.ExtFunctionName,
                ExtFunctionType = dto.ExtFunctionType,
                ExtFunctionDesc = dto.ExtFunctionDesc,
                ExtFunctionConfig = dto.ExtFunctionConfig,
                ParameterConfig = dto.ParameterConfig,
                Status = dto.Status,
                SeqNo = dto.SeqNo,
                Memo = dto.Memo,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(purchaseExtendedFunction);

            _logger.LogInfo($"新增採購擴展功能成功: {dto.ExtFunctionId}");
            return purchaseExtendedFunction.TKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增採購擴展功能失敗: {dto.ExtFunctionId}", ex);
            throw;
        }
    }

    public async Task UpdatePurchaseExtendedFunctionAsync(long tKey, UpdatePurchaseExtendedFunctionDto dto)
    {
        try
        {
            var purchaseExtendedFunction = await _repository.GetByIdAsync(tKey);
            if (purchaseExtendedFunction == null)
            {
                throw new InvalidOperationException($"採購擴展功能不存在: {tKey}");
            }

            purchaseExtendedFunction.ExtFunctionName = dto.ExtFunctionName;
            purchaseExtendedFunction.ExtFunctionType = dto.ExtFunctionType;
            purchaseExtendedFunction.ExtFunctionDesc = dto.ExtFunctionDesc;
            purchaseExtendedFunction.ExtFunctionConfig = dto.ExtFunctionConfig;
            purchaseExtendedFunction.ParameterConfig = dto.ParameterConfig;
            purchaseExtendedFunction.Status = dto.Status;
            purchaseExtendedFunction.SeqNo = dto.SeqNo;
            purchaseExtendedFunction.Memo = dto.Memo;
            purchaseExtendedFunction.UpdatedBy = GetCurrentUserId();
            purchaseExtendedFunction.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(purchaseExtendedFunction);

            _logger.LogInfo($"修改採購擴展功能成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改採購擴展功能失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeletePurchaseExtendedFunctionAsync(long tKey)
    {
        try
        {
            var purchaseExtendedFunction = await _repository.GetByIdAsync(tKey);
            if (purchaseExtendedFunction == null)
            {
                throw new InvalidOperationException($"採購擴展功能不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);

            _logger.LogInfo($"刪除採購擴展功能成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除採購擴展功能失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string extFunctionId)
    {
        try
        {
            return await _repository.ExistsAsync(extFunctionId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查採購擴展功能是否存在失敗: {extFunctionId}", ex);
            throw;
        }
    }

    private PurchaseExtendedFunctionDto MapToDto(PurchaseExtendedFunction purchaseExtendedFunction)
    {
        return new PurchaseExtendedFunctionDto
        {
            TKey = purchaseExtendedFunction.TKey,
            ExtFunctionId = purchaseExtendedFunction.ExtFunctionId,
            ExtFunctionName = purchaseExtendedFunction.ExtFunctionName,
            ExtFunctionType = purchaseExtendedFunction.ExtFunctionType,
            ExtFunctionDesc = purchaseExtendedFunction.ExtFunctionDesc,
            ExtFunctionConfig = purchaseExtendedFunction.ExtFunctionConfig,
            ParameterConfig = purchaseExtendedFunction.ParameterConfig,
            Status = purchaseExtendedFunction.Status,
            SeqNo = purchaseExtendedFunction.SeqNo,
            Memo = purchaseExtendedFunction.Memo,
            CreatedBy = purchaseExtendedFunction.CreatedBy,
            CreatedAt = purchaseExtendedFunction.CreatedAt,
            UpdatedBy = purchaseExtendedFunction.UpdatedBy,
            UpdatedAt = purchaseExtendedFunction.UpdatedAt
        };
    }
}
