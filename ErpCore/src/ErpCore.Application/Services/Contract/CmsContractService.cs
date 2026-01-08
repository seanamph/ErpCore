using ErpCore.Application.DTOs.Contract;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Contract;
using ErpCore.Infrastructure.Repositories.Contract;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Contract;

/// <summary>
/// CMS合同服務實作 (CMS2310-CMS2320)
/// </summary>
public class CmsContractService : BaseService, ICmsContractService
{
    private readonly ICmsContractRepository _repository;

    public CmsContractService(
        ICmsContractRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<CmsContractDto>> GetCmsContractsAsync(CmsContractQueryDto query)
    {
        try
        {
            var repositoryQuery = new CmsContractQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                CmsContractId = query.CmsContractId,
                VendorId = query.VendorId,
                ContractType = query.ContractType,
                Status = query.Status
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<CmsContractDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢CMS合同列表失敗", ex);
            throw;
        }
    }

    public async Task<CmsContractDto> GetCmsContractByIdAsync(long tKey)
    {
        try
        {
            var contract = await _repository.GetByIdAsync(tKey);
            if (contract == null)
            {
                throw new InvalidOperationException($"CMS合同不存在: {tKey}");
            }

            return MapToDto(contract);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢CMS合同失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<CmsContractDto> GetCmsContractByContractIdAsync(string cmsContractId, int version)
    {
        try
        {
            var contract = await _repository.GetByContractIdAsync(cmsContractId, version);
            if (contract == null)
            {
                throw new InvalidOperationException($"CMS合同不存在: {cmsContractId}, Version: {version}");
            }

            return MapToDto(contract);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢CMS合同失敗: {cmsContractId}, Version: {version}", ex);
            throw;
        }
    }

    public async Task<CmsContractResultDto> CreateCmsContractAsync(CreateCmsContractDto dto)
    {
        try
        {
            // 檢查CMS合同編號和版本是否已存在
            if (await _repository.ExistsAsync(dto.CmsContractId, dto.Version))
            {
                throw new InvalidOperationException($"CMS合同編號和版本已存在: {dto.CmsContractId}, Version: {dto.Version}");
            }

            // 驗證日期範圍
            if (dto.EffectiveDate.HasValue && dto.ExpiryDate.HasValue && dto.EffectiveDate > dto.ExpiryDate)
            {
                throw new InvalidOperationException("生效日期不能晚於到期日期");
            }

            // 驗證總金額
            if (dto.TotalAmount < 0)
            {
                throw new InvalidOperationException("總金額不能小於0");
            }

            var contract = new CmsContract
            {
                CmsContractId = dto.CmsContractId,
                ContractType = dto.ContractType,
                Version = dto.Version,
                VendorId = dto.VendorId,
                VendorName = dto.VendorName,
                SignDate = dto.SignDate,
                EffectiveDate = dto.EffectiveDate,
                ExpiryDate = dto.ExpiryDate,
                Status = dto.Status,
                TotalAmount = dto.TotalAmount,
                CurrencyId = dto.CurrencyId,
                ExchangeRate = dto.ExchangeRate,
                LocationId = dto.LocationId,
                Memo = dto.Memo,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(contract);

            _logger.LogInfo($"新增CMS合同成功: {dto.CmsContractId}, Version: {dto.Version}");
            return new CmsContractResultDto
            {
                TKey = contract.TKey,
                CmsContractId = contract.CmsContractId,
                Version = contract.Version
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增CMS合同失敗: {dto.CmsContractId}, Version: {dto.Version}", ex);
            throw;
        }
    }

    public async Task UpdateCmsContractAsync(long tKey, UpdateCmsContractDto dto)
    {
        try
        {
            var contract = await _repository.GetByIdAsync(tKey);
            if (contract == null)
            {
                throw new InvalidOperationException($"CMS合同不存在: {tKey}");
            }

            // 驗證日期範圍
            if (dto.EffectiveDate.HasValue && dto.ExpiryDate.HasValue && dto.EffectiveDate > dto.ExpiryDate)
            {
                throw new InvalidOperationException("生效日期不能晚於到期日期");
            }

            // 驗證總金額
            if (dto.TotalAmount < 0)
            {
                throw new InvalidOperationException("總金額不能小於0");
            }

            contract.ContractType = dto.ContractType;
            contract.VendorId = dto.VendorId;
            contract.VendorName = dto.VendorName;
            contract.SignDate = dto.SignDate;
            contract.EffectiveDate = dto.EffectiveDate;
            contract.ExpiryDate = dto.ExpiryDate;
            contract.Status = dto.Status;
            contract.TotalAmount = dto.TotalAmount;
            contract.CurrencyId = dto.CurrencyId;
            contract.ExchangeRate = dto.ExchangeRate;
            contract.LocationId = dto.LocationId;
            contract.Memo = dto.Memo;
            contract.UpdatedBy = GetCurrentUserId();
            contract.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(contract);

            _logger.LogInfo($"修改CMS合同成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改CMS合同失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteCmsContractAsync(long tKey)
    {
        try
        {
            var contract = await _repository.GetByIdAsync(tKey);
            if (contract == null)
            {
                throw new InvalidOperationException($"CMS合同不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);

            _logger.LogInfo($"刪除CMS合同成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除CMS合同失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<int> BatchDeleteCmsContractsAsync(List<long> tKeys)
    {
        try
        {
            if (tKeys == null || tKeys.Count == 0)
            {
                return 0;
            }

            var count = await _repository.BatchDeleteAsync(tKeys);

            _logger.LogInfo($"批次刪除CMS合同成功: 刪除 {count} 筆");
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除CMS合同失敗", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string cmsContractId, int version)
    {
        try
        {
            return await _repository.ExistsAsync(cmsContractId, version);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查CMS合同是否存在失敗: {cmsContractId}, Version: {version}", ex);
            throw;
        }
    }

    private CmsContractDto MapToDto(CmsContract contract)
    {
        return new CmsContractDto
        {
            TKey = contract.TKey,
            CmsContractId = contract.CmsContractId,
            ContractType = contract.ContractType,
            Version = contract.Version,
            VendorId = contract.VendorId,
            VendorName = contract.VendorName,
            SignDate = contract.SignDate,
            EffectiveDate = contract.EffectiveDate,
            ExpiryDate = contract.ExpiryDate,
            Status = contract.Status,
            TotalAmount = contract.TotalAmount,
            CurrencyId = contract.CurrencyId,
            ExchangeRate = contract.ExchangeRate,
            LocationId = contract.LocationId,
            Memo = contract.Memo,
            CreatedBy = contract.CreatedBy,
            CreatedAt = contract.CreatedAt,
            UpdatedBy = contract.UpdatedBy,
            UpdatedAt = contract.UpdatedAt
        };
    }
}

