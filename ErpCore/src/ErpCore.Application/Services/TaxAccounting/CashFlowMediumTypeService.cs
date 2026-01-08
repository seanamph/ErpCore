using ErpCore.Application.DTOs.TaxAccounting;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.TaxAccounting;
using ErpCore.Infrastructure.Repositories.TaxAccounting;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.TaxAccounting;

/// <summary>
/// 現金流量中分類服務實作 (SYST132)
/// </summary>
public class CashFlowMediumTypeService : BaseService, ICashFlowMediumTypeService
{
    private readonly ICashFlowMediumTypeRepository _repository;
    private readonly ICashFlowLargeTypeRepository _largeTypeRepository;

    public CashFlowMediumTypeService(
        ICashFlowMediumTypeRepository repository,
        ICashFlowLargeTypeRepository largeTypeRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _largeTypeRepository = largeTypeRepository;
    }

    public async Task<PagedResult<CashFlowMediumTypeDto>> GetCashFlowMediumTypesAsync(CashFlowMediumTypeQueryDto query)
    {
        try
        {
            var repositoryQuery = new CashFlowMediumTypeQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                CashLTypeId = query.CashLTypeId,
                CashMTypeId = query.CashMTypeId,
                CashMTypeName = query.CashMTypeName,
                AbItem = query.AbItem
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<CashFlowMediumTypeDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢現金流量中分類列表失敗", ex);
            throw;
        }
    }

    public async Task<CashFlowMediumTypeDto> GetCashFlowMediumTypeByIdAsync(string cashLTypeId, string cashMTypeId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(cashLTypeId, cashMTypeId);
            if (entity == null)
            {
                throw new InvalidOperationException($"現金流量中分類不存在: {cashLTypeId}/{cashMTypeId}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢現金流量中分類失敗: {cashLTypeId}/{cashMTypeId}", ex);
            throw;
        }
    }

    public async Task<string> CreateCashFlowMediumTypeAsync(CreateCashFlowMediumTypeDto dto)
    {
        try
        {
            // 檢查大分類是否存在
            var largeTypeExists = await _largeTypeRepository.ExistsAsync(dto.CashLTypeId);
            if (!largeTypeExists)
            {
                throw new InvalidOperationException($"現金流量大分類不存在: {dto.CashLTypeId}");
            }

            // 檢查是否已存在
            var exists = await _repository.ExistsAsync(dto.CashLTypeId, dto.CashMTypeId);
            if (exists)
            {
                throw new InvalidOperationException($"現金流量中分類代號已存在: {dto.CashLTypeId}/{dto.CashMTypeId}");
            }

            var entity = new CashFlowMediumType
            {
                CashLTypeId = dto.CashLTypeId,
                CashMTypeId = dto.CashMTypeId,
                CashMTypeName = dto.CashMTypeName,
                AbItem = dto.AbItem,
                Sn = dto.Sn,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(entity);

            return entity.CashMTypeId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增現金流量中分類失敗: {dto.CashLTypeId}/{dto.CashMTypeId}", ex);
            throw;
        }
    }

    public async Task UpdateCashFlowMediumTypeAsync(string cashLTypeId, string cashMTypeId, UpdateCashFlowMediumTypeDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(cashLTypeId, cashMTypeId);
            if (entity == null)
            {
                throw new InvalidOperationException($"現金流量中分類不存在: {cashLTypeId}/{cashMTypeId}");
            }

            entity.CashMTypeName = dto.CashMTypeName;
            entity.AbItem = dto.AbItem;
            entity.Sn = dto.Sn;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改現金流量中分類失敗: {cashLTypeId}/{cashMTypeId}", ex);
            throw;
        }
    }

    public async Task DeleteCashFlowMediumTypeAsync(string cashLTypeId, string cashMTypeId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(cashLTypeId, cashMTypeId);
            if (entity == null)
            {
                throw new InvalidOperationException($"現金流量中分類不存在: {cashLTypeId}/{cashMTypeId}");
            }

            // 檢查是否有科目設定資料
            var hasSubjectTypes = await _repository.HasSubjectTypesAsync(cashMTypeId);
            if (hasSubjectTypes)
            {
                throw new InvalidOperationException($"現金流量中分類已有科目設定資料，無法刪除: {cashLTypeId}/{cashMTypeId}");
            }

            await _repository.DeleteAsync(cashLTypeId, cashMTypeId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除現金流量中分類失敗: {cashLTypeId}/{cashMTypeId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string cashLTypeId, string cashMTypeId)
    {
        try
        {
            return await _repository.ExistsAsync(cashLTypeId, cashMTypeId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查現金流量中分類是否存在失敗: {cashLTypeId}/{cashMTypeId}", ex);
            throw;
        }
    }

    private CashFlowMediumTypeDto MapToDto(CashFlowMediumType entity)
    {
        return new CashFlowMediumTypeDto
        {
            TKey = entity.TKey,
            CashLTypeId = entity.CashLTypeId,
            CashMTypeId = entity.CashMTypeId,
            CashMTypeName = entity.CashMTypeName,
            AbItem = entity.AbItem,
            Sn = entity.Sn,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

