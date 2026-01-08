using ErpCore.Application.DTOs.TaxAccounting;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.TaxAccounting;
using ErpCore.Infrastructure.Repositories.Accounting;
using ErpCore.Infrastructure.Repositories.TaxAccounting;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.TaxAccounting;

/// <summary>
/// 現金流量科目設定服務實作 (SYST133)
/// </summary>
public class CashFlowSubjectTypeService : BaseService, ICashFlowSubjectTypeService
{
    private readonly ICashFlowSubjectTypeRepository _repository;
    private readonly ICashFlowMediumTypeRepository _mediumTypeRepository;
    private readonly IAccountSubjectRepository _accountSubjectRepository;

    public CashFlowSubjectTypeService(
        ICashFlowSubjectTypeRepository repository,
        ICashFlowMediumTypeRepository mediumTypeRepository,
        IAccountSubjectRepository accountSubjectRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _mediumTypeRepository = mediumTypeRepository;
        _accountSubjectRepository = accountSubjectRepository;
    }

    public async Task<PagedResult<CashFlowSubjectTypeDto>> GetCashFlowSubjectTypesAsync(CashFlowSubjectTypeQueryDto query)
    {
        try
        {
            var repositoryQuery = new CashFlowSubjectTypeQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                CashMTypeId = query.CashMTypeId,
                CashSTypeId = query.CashSTypeId
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<CashFlowSubjectTypeDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢現金流量科目設定列表失敗", ex);
            throw;
        }
    }

    public async Task<CashFlowSubjectTypeDto> GetCashFlowSubjectTypeByIdAsync(string cashMTypeId, string cashSTypeId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(cashMTypeId, cashSTypeId);
            if (entity == null)
            {
                throw new InvalidOperationException($"現金流量科目設定不存在: {cashMTypeId}/{cashSTypeId}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢現金流量科目設定失敗: {cashMTypeId}/{cashSTypeId}", ex);
            throw;
        }
    }

    public async Task<string> CreateCashFlowSubjectTypeAsync(CreateCashFlowSubjectTypeDto dto)
    {
        try
        {
            // 檢查中分類是否存在（需要檢查所有大分類下的中分類）
            var mediumTypeQuery = new CashFlowMediumTypeQuery
            {
                PageIndex = 1,
                PageSize = 1,
                CashMTypeId = dto.CashMTypeId
            };
            var mediumTypes = await _mediumTypeRepository.QueryAsync(mediumTypeQuery);
            if (mediumTypes.TotalCount == 0)
            {
                throw new InvalidOperationException($"現金流量中分類不存在: {dto.CashMTypeId}");
            }

            // 檢查會計科目是否存在
            var accountSubjectExists = await _accountSubjectRepository.ExistsAsync(dto.CashSTypeId);
            if (!accountSubjectExists)
            {
                throw new InvalidOperationException($"會計科目不存在: {dto.CashSTypeId}");
            }

            // 檢查是否已存在
            var exists = await _repository.ExistsAsync(dto.CashMTypeId, dto.CashSTypeId);
            if (exists)
            {
                throw new InvalidOperationException($"現金流量科目設定已存在: {dto.CashMTypeId}/{dto.CashSTypeId}");
            }

            var entity = new CashFlowSubjectType
            {
                CashMTypeId = dto.CashMTypeId,
                CashSTypeId = dto.CashSTypeId,
                AbItem = dto.AbItem,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(entity);

            return entity.CashSTypeId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增現金流量科目設定失敗: {dto.CashMTypeId}/{dto.CashSTypeId}", ex);
            throw;
        }
    }

    public async Task UpdateCashFlowSubjectTypeAsync(string cashMTypeId, string cashSTypeId, UpdateCashFlowSubjectTypeDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(cashMTypeId, cashSTypeId);
            if (entity == null)
            {
                throw new InvalidOperationException($"現金流量科目設定不存在: {cashMTypeId}/{cashSTypeId}");
            }

            entity.AbItem = dto.AbItem;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改現金流量科目設定失敗: {cashMTypeId}/{cashSTypeId}", ex);
            throw;
        }
    }

    public async Task DeleteCashFlowSubjectTypeAsync(string cashMTypeId, string cashSTypeId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(cashMTypeId, cashSTypeId);
            if (entity == null)
            {
                throw new InvalidOperationException($"現金流量科目設定不存在: {cashMTypeId}/{cashSTypeId}");
            }

            await _repository.DeleteAsync(cashMTypeId, cashSTypeId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除現金流量科目設定失敗: {cashMTypeId}/{cashSTypeId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string cashMTypeId, string cashSTypeId)
    {
        try
        {
            return await _repository.ExistsAsync(cashMTypeId, cashSTypeId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查現金流量科目設定是否存在失敗: {cashMTypeId}/{cashSTypeId}", ex);
            throw;
        }
    }

    private CashFlowSubjectTypeDto MapToDto(CashFlowSubjectType entity)
    {
        return new CashFlowSubjectTypeDto
        {
            TKey = entity.TKey,
            CashMTypeId = entity.CashMTypeId,
            CashSTypeId = entity.CashSTypeId,
            AbItem = entity.AbItem,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

