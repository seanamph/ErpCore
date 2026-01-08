using ErpCore.Application.DTOs.Contract;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Contract;
using ErpCore.Infrastructure.Repositories.Contract;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Contract;

/// <summary>
/// 合同擴展服務實作 (SYSF350-SYSF540)
/// </summary>
public class ContractExtensionService : BaseService, IContractExtensionService
{
    private readonly IContractExtensionRepository _repository;
    private readonly IContractRepository _contractRepository;

    public ContractExtensionService(
        IContractExtensionRepository repository,
        IContractRepository contractRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _contractRepository = contractRepository;
    }

    public async Task<PagedResult<ContractExtensionDto>> GetContractExtensionsAsync(ContractExtensionQueryDto query)
    {
        try
        {
            var repositoryQuery = new ContractExtensionQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                ContractId = query.ContractId,
                VendorId = query.VendorId,
                ExtensionType = query.ExtensionType,
                Status = query.Status
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<ContractExtensionDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢合同擴展列表失敗", ex);
            throw;
        }
    }

    public async Task<ContractExtensionDto> GetContractExtensionByIdAsync(long tKey)
    {
        try
        {
            var extension = await _repository.GetByIdAsync(tKey);
            if (extension == null)
            {
                throw new InvalidOperationException($"合同擴展不存在: {tKey}");
            }

            return MapToDto(extension);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢合同擴展失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<ContractExtensionResultDto> CreateContractExtensionAsync(CreateContractExtensionDto dto)
    {
        try
        {
            // 檢查合同是否存在
            if (!await _contractRepository.ExistsAsync(dto.ContractId, dto.Version))
            {
                throw new InvalidOperationException($"合同不存在: {dto.ContractId}, Version: {dto.Version}");
            }

            // 驗證擴展金額
            if (dto.ExtensionAmount < 0)
            {
                throw new InvalidOperationException("擴展金額不能小於0");
            }

            var extension = new ContractExtension
            {
                ContractId = dto.ContractId,
                Version = dto.Version,
                ExtensionType = dto.ExtensionType,
                VendorId = dto.VendorId,
                VendorName = dto.VendorName,
                ExtensionDate = dto.ExtensionDate,
                ExtensionAmount = dto.ExtensionAmount,
                Status = dto.Status,
                Memo = dto.Memo,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(extension);

            _logger.LogInfo($"新增合同擴展成功: {dto.ContractId}, Version: {dto.Version}");
            return new ContractExtensionResultDto
            {
                TKey = extension.TKey,
                ContractId = extension.ContractId,
                Version = extension.Version
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增合同擴展失敗: {dto.ContractId}, Version: {dto.Version}", ex);
            throw;
        }
    }

    public async Task UpdateContractExtensionAsync(long tKey, UpdateContractExtensionDto dto)
    {
        try
        {
            var extension = await _repository.GetByIdAsync(tKey);
            if (extension == null)
            {
                throw new InvalidOperationException($"合同擴展不存在: {tKey}");
            }

            // 驗證擴展金額
            if (dto.ExtensionAmount < 0)
            {
                throw new InvalidOperationException("擴展金額不能小於0");
            }

            extension.ExtensionType = dto.ExtensionType;
            extension.VendorId = dto.VendorId;
            extension.VendorName = dto.VendorName;
            extension.ExtensionDate = dto.ExtensionDate;
            extension.ExtensionAmount = dto.ExtensionAmount;
            extension.Status = dto.Status;
            extension.Memo = dto.Memo;
            extension.UpdatedBy = GetCurrentUserId();
            extension.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(extension);

            _logger.LogInfo($"修改合同擴展成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改合同擴展失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteContractExtensionAsync(long tKey)
    {
        try
        {
            var extension = await _repository.GetByIdAsync(tKey);
            if (extension == null)
            {
                throw new InvalidOperationException($"合同擴展不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);

            _logger.LogInfo($"刪除合同擴展成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除合同擴展失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<int> BatchDeleteContractExtensionsAsync(List<long> tKeys)
    {
        try
        {
            if (tKeys == null || tKeys.Count == 0)
            {
                return 0;
            }

            var count = await _repository.BatchDeleteAsync(tKeys);

            _logger.LogInfo($"批次刪除合同擴展成功: 刪除 {count} 筆");
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除合同擴展失敗", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(long tKey)
    {
        try
        {
            return await _repository.ExistsAsync(tKey);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查合同擴展是否存在失敗: {tKey}", ex);
            throw;
        }
    }

    private ContractExtensionDto MapToDto(ContractExtension extension)
    {
        return new ContractExtensionDto
        {
            TKey = extension.TKey,
            ContractId = extension.ContractId,
            Version = extension.Version,
            ExtensionType = extension.ExtensionType,
            VendorId = extension.VendorId,
            VendorName = extension.VendorName,
            ExtensionDate = extension.ExtensionDate,
            ExtensionAmount = extension.ExtensionAmount,
            Status = extension.Status,
            Memo = extension.Memo,
            CreatedBy = extension.CreatedBy,
            CreatedAt = extension.CreatedAt,
            UpdatedBy = extension.UpdatedBy,
            UpdatedAt = extension.UpdatedAt
        };
    }
}

