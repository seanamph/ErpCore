using ErpCore.Application.DTOs.TaxAccounting;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.TaxAccounting;
using ErpCore.Infrastructure.Repositories.TaxAccounting;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.TaxAccounting;

/// <summary>
/// 現金流量大分類服務實作 (SYST131)
/// </summary>
public class CashFlowLargeTypeService : BaseService, ICashFlowLargeTypeService
{
    private readonly ICashFlowLargeTypeRepository _repository;

    public CashFlowLargeTypeService(
        ICashFlowLargeTypeRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<CashFlowLargeTypeDto>> GetCashFlowLargeTypesAsync(CashFlowLargeTypeQueryDto query)
    {
        try
        {
            var repositoryQuery = new CashFlowLargeTypeQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                CashLTypeId = query.CashLTypeId,
                CashLTypeName = query.CashLTypeName,
                AbItem = query.AbItem
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<CashFlowLargeTypeDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢現金流量大分類列表失敗", ex);
            throw;
        }
    }

    public async Task<CashFlowLargeTypeDto> GetCashFlowLargeTypeByIdAsync(string cashLTypeId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(cashLTypeId);
            if (entity == null)
            {
                throw new InvalidOperationException($"現金流量大分類不存在: {cashLTypeId}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢現金流量大分類失敗: {cashLTypeId}", ex);
            throw;
        }
    }

    public async Task<string> CreateCashFlowLargeTypeAsync(CreateCashFlowLargeTypeDto dto)
    {
        try
        {
            // 檢查是否已存在
            var exists = await _repository.ExistsAsync(dto.CashLTypeId);
            if (exists)
            {
                throw new InvalidOperationException($"現金流量大分類代號已存在: {dto.CashLTypeId}");
            }

            var entity = new CashFlowLargeType
            {
                CashLTypeId = dto.CashLTypeId,
                CashLTypeName = dto.CashLTypeName,
                AbItem = dto.AbItem,
                Sn = dto.Sn,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(entity);

            return entity.CashLTypeId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增現金流量大分類失敗: {dto.CashLTypeId}", ex);
            throw;
        }
    }

    public async Task UpdateCashFlowLargeTypeAsync(string cashLTypeId, UpdateCashFlowLargeTypeDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(cashLTypeId);
            if (entity == null)
            {
                throw new InvalidOperationException($"現金流量大分類不存在: {cashLTypeId}");
            }

            entity.CashLTypeName = dto.CashLTypeName;
            entity.AbItem = dto.AbItem;
            entity.Sn = dto.Sn;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改現金流量大分類失敗: {cashLTypeId}", ex);
            throw;
        }
    }

    public async Task DeleteCashFlowLargeTypeAsync(string cashLTypeId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(cashLTypeId);
            if (entity == null)
            {
                throw new InvalidOperationException($"現金流量大分類不存在: {cashLTypeId}");
            }

            // 檢查是否有中分類資料
            var hasMediumTypes = await _repository.HasMediumTypesAsync(cashLTypeId);
            if (hasMediumTypes)
            {
                throw new InvalidOperationException($"現金流量大分類已有中分類資料，無法刪除: {cashLTypeId}");
            }

            await _repository.DeleteAsync(cashLTypeId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除現金流量大分類失敗: {cashLTypeId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string cashLTypeId)
    {
        try
        {
            return await _repository.ExistsAsync(cashLTypeId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查現金流量大分類是否存在失敗: {cashLTypeId}", ex);
            throw;
        }
    }

    private CashFlowLargeTypeDto MapToDto(CashFlowLargeType entity)
    {
        return new CashFlowLargeTypeDto
        {
            TKey = entity.TKey,
            CashLTypeId = entity.CashLTypeId,
            CashLTypeName = entity.CashLTypeName,
            AbItem = entity.AbItem,
            Sn = entity.Sn,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

