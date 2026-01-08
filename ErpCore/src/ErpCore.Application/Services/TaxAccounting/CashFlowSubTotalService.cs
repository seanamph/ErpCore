using ErpCore.Application.DTOs.TaxAccounting;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.TaxAccounting;
using ErpCore.Infrastructure.Repositories.TaxAccounting;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.TaxAccounting;

/// <summary>
/// 現金流量小計設定服務實作 (SYST134)
/// </summary>
public class CashFlowSubTotalService : BaseService, ICashFlowSubTotalService
{
    private readonly ICashFlowSubTotalRepository _repository;
    private readonly ICashFlowLargeTypeRepository _largeTypeRepository;

    public CashFlowSubTotalService(
        ICashFlowSubTotalRepository repository,
        ICashFlowLargeTypeRepository largeTypeRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _largeTypeRepository = largeTypeRepository;
    }

    public async Task<PagedResult<CashFlowSubTotalDto>> GetCashFlowSubTotalsAsync(CashFlowSubTotalQueryDto query)
    {
        try
        {
            var repositoryQuery = new CashFlowSubTotalQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                CashLTypeId = query.CashLTypeId,
                CashSubId = query.CashSubId
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<CashFlowSubTotalDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢現金流量小計設定列表失敗", ex);
            throw;
        }
    }

    public async Task<CashFlowSubTotalDto> GetCashFlowSubTotalByIdAsync(string cashLTypeId, string cashSubId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(cashLTypeId, cashSubId);
            if (entity == null)
            {
                throw new InvalidOperationException($"現金流量小計設定不存在: {cashLTypeId}/{cashSubId}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢現金流量小計設定失敗: {cashLTypeId}/{cashSubId}", ex);
            throw;
        }
    }

    public async Task<string> CreateCashFlowSubTotalAsync(CreateCashFlowSubTotalDto dto)
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
            var exists = await _repository.ExistsAsync(dto.CashLTypeId, dto.CashSubId);
            if (exists)
            {
                throw new InvalidOperationException($"現金流量小計設定代號已存在: {dto.CashLTypeId}/{dto.CashSubId}");
            }

            var entity = new CashFlowSubTotal
            {
                CashLTypeId = dto.CashLTypeId,
                CashSubId = dto.CashSubId,
                CashSubName = dto.CashSubName,
                CashMTypeIdB = dto.CashMTypeIdB,
                CashMTypeIdE = dto.CashMTypeIdE,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(entity);

            return entity.CashSubId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增現金流量小計設定失敗: {dto.CashLTypeId}/{dto.CashSubId}", ex);
            throw;
        }
    }

    public async Task UpdateCashFlowSubTotalAsync(string cashLTypeId, string cashSubId, UpdateCashFlowSubTotalDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(cashLTypeId, cashSubId);
            if (entity == null)
            {
                throw new InvalidOperationException($"現金流量小計設定不存在: {cashLTypeId}/{cashSubId}");
            }

            entity.CashSubName = dto.CashSubName;
            entity.CashMTypeIdB = dto.CashMTypeIdB;
            entity.CashMTypeIdE = dto.CashMTypeIdE;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改現金流量小計設定失敗: {cashLTypeId}/{cashSubId}", ex);
            throw;
        }
    }

    public async Task DeleteCashFlowSubTotalAsync(string cashLTypeId, string cashSubId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(cashLTypeId, cashSubId);
            if (entity == null)
            {
                throw new InvalidOperationException($"現金流量小計設定不存在: {cashLTypeId}/{cashSubId}");
            }

            await _repository.DeleteAsync(cashLTypeId, cashSubId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除現金流量小計設定失敗: {cashLTypeId}/{cashSubId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string cashLTypeId, string cashSubId)
    {
        try
        {
            return await _repository.ExistsAsync(cashLTypeId, cashSubId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查現金流量小計設定是否存在失敗: {cashLTypeId}/{cashSubId}", ex);
            throw;
        }
    }

    private CashFlowSubTotalDto MapToDto(CashFlowSubTotal entity)
    {
        return new CashFlowSubTotalDto
        {
            TKey = entity.TKey,
            CashLTypeId = entity.CashLTypeId,
            CashSubId = entity.CashSubId,
            CashSubName = entity.CashSubName,
            CashMTypeIdB = entity.CashMTypeIdB,
            CashMTypeIdE = entity.CashMTypeIdE,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

