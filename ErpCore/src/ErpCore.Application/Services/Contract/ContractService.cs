using ErpCore.Application.DTOs.Contract;
using ErpCore.Application.Services.Base;
using ContractEntity = ErpCore.Domain.Entities.Contract.Contract;
using ErpCore.Infrastructure.Repositories.Contract;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Contract;

/// <summary>
/// 合同服務實作 (SYSF110-SYSF140)
/// </summary>
public class ContractService : BaseService, IContractService
{
    private readonly IContractRepository _repository;

    public ContractService(
        IContractRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<ContractDto>> GetContractsAsync(ContractQueryDto query)
    {
        try
        {
            var repositoryQuery = new ContractQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                ContractId = query.ContractId,
                ContractType = query.ContractType,
                VendorId = query.VendorId,
                Status = query.Status,
                EffectiveDateFrom = query.EffectiveDateFrom,
                EffectiveDateTo = query.EffectiveDateTo
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<ContractDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢合同列表失敗", ex);
            throw;
        }
    }

    public async Task<ContractDto> GetContractByIdAsync(string contractId, int version)
    {
        try
        {
            var contract = await _repository.GetByIdAsync(contractId, version);
            if (contract == null)
            {
                throw new InvalidOperationException($"合同不存在: {contractId}, Version: {version}");
            }

            return MapToDto(contract);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢合同失敗: {contractId}, Version: {version}", ex);
            throw;
        }
    }

    public async Task<ContractResultDto> CreateContractAsync(CreateContractDto dto)
    {
        try
        {
            // 檢查合同編號和版本是否已存在
            if (await _repository.ExistsAsync(dto.ContractId, dto.Version))
            {
                throw new InvalidOperationException($"合同編號和版本已存在: {dto.ContractId}, Version: {dto.Version}");
            }

            // 驗證日期範圍
            if (dto.EffectiveDate.HasValue && dto.ExpiryDate.HasValue && dto.EffectiveDate > dto.ExpiryDate)
            {
                throw new InvalidOperationException("生效日期不能晚於到期日期");
            }

            var contract = new Contract
            {
                ContractId = dto.ContractId,
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
                RecruitId = dto.RecruitId,
                Attorney = dto.Attorney,
                Salutation = dto.Salutation,
                VerStatus = dto.VerStatus,
                AgmStatus = dto.AgmStatus,
                Memo = dto.Memo,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(contract);

            _logger.LogInfo($"新增合同成功: {dto.ContractId}, Version: {dto.Version}");
            return new ContractResultDto
            {
                TKey = contract.TKey,
                ContractId = contract.ContractId,
                Version = contract.Version
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增合同失敗: {dto.ContractId}, Version: {dto.Version}", ex);
            throw;
        }
    }

    public async Task UpdateContractAsync(string contractId, int version, UpdateContractDto dto)
    {
        try
        {
            var contract = await _repository.GetByIdAsync(contractId, version);
            if (contract == null)
            {
                throw new InvalidOperationException($"合同不存在: {contractId}, Version: {version}");
            }

            // 驗證日期範圍
            if (dto.EffectiveDate.HasValue && dto.ExpiryDate.HasValue && dto.EffectiveDate > dto.ExpiryDate)
            {
                throw new InvalidOperationException("生效日期不能晚於到期日期");
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
            contract.RecruitId = dto.RecruitId;
            contract.Attorney = dto.Attorney;
            contract.Salutation = dto.Salutation;
            contract.VerStatus = dto.VerStatus;
            contract.AgmStatus = dto.AgmStatus;
            contract.Memo = dto.Memo;
            contract.UpdatedBy = GetCurrentUserId();
            contract.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(contract);

            _logger.LogInfo($"修改合同成功: {contractId}, Version: {version}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改合同失敗: {contractId}, Version: {version}", ex);
            throw;
        }
    }

    public async Task DeleteContractAsync(string contractId, int version)
    {
        try
        {
            var contract = await _repository.GetByIdAsync(contractId, version);
            if (contract == null)
            {
                throw new InvalidOperationException($"合同不存在: {contractId}, Version: {version}");
            }

            await _repository.DeleteAsync(contractId, version);

            _logger.LogInfo($"刪除合同成功: {contractId}, Version: {version}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除合同失敗: {contractId}, Version: {version}", ex);
            throw;
        }
    }

    public async Task ApproveContractAsync(string contractId, int version, ApproveContractDto dto)
    {
        try
        {
            var contract = await _repository.GetByIdAsync(contractId, version);
            if (contract == null)
            {
                throw new InvalidOperationException($"合同不存在: {contractId}, Version: {version}");
            }

            contract.Status = dto.Status;
            contract.UpdatedBy = dto.ApproveUserId;
            contract.UpdatedAt = dto.ApproveDate ?? DateTime.Now;

            await _repository.UpdateAsync(contract);

            _logger.LogInfo($"審核合同成功: {contractId}, Version: {version}, Status: {dto.Status}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"審核合同失敗: {contractId}, Version: {version}", ex);
            throw;
        }
    }

    public async Task<ContractResultDto> CreateNewVersionAsync(string contractId, int version, NewVersionDto dto)
    {
        try
        {
            var originalContract = await _repository.GetByIdAsync(contractId, version);
            if (originalContract == null)
            {
                throw new InvalidOperationException($"合同不存在: {contractId}, Version: {version}");
            }

            // 取得下一版本號
            var nextVersion = await _repository.GetNextVersionAsync(contractId);

            var newContract = new Contract
            {
                ContractId = contractId,
                ContractType = originalContract.ContractType,
                Version = nextVersion,
                VendorId = originalContract.VendorId,
                VendorName = originalContract.VendorName,
                SignDate = originalContract.SignDate,
                EffectiveDate = originalContract.EffectiveDate,
                ExpiryDate = originalContract.ExpiryDate,
                Status = "D", // 新版本預設為草稿
                TotalAmount = originalContract.TotalAmount,
                CurrencyId = originalContract.CurrencyId,
                ExchangeRate = originalContract.ExchangeRate,
                LocationId = originalContract.LocationId,
                RecruitId = originalContract.RecruitId,
                Attorney = originalContract.Attorney,
                Salutation = originalContract.Salutation,
                VerStatus = dto.VerStatus,
                AgmStatus = originalContract.AgmStatus,
                Memo = originalContract.Memo,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(newContract);

            _logger.LogInfo($"產生合同新版本成功: {contractId}, Version: {nextVersion}");
            return new ContractResultDto
            {
                TKey = newContract.TKey,
                ContractId = newContract.ContractId,
                Version = newContract.Version
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"產生合同新版本失敗: {contractId}, Version: {version}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string contractId, int version)
    {
        try
        {
            return await _repository.ExistsAsync(contractId, version);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查合同是否存在失敗: {contractId}, Version: {version}", ex);
            throw;
        }
    }

    private ContractDto MapToDto(ContractEntity contract)
    {
        return new ContractDto
        {
            TKey = contract.TKey,
            ContractId = contract.ContractId,
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
            RecruitId = contract.RecruitId,
            Attorney = contract.Attorney,
            Salutation = contract.Salutation,
            VerStatus = contract.VerStatus,
            AgmStatus = contract.AgmStatus,
            Memo = contract.Memo,
            CreatedBy = contract.CreatedBy,
            CreatedAt = contract.CreatedAt,
            UpdatedBy = contract.UpdatedBy,
            UpdatedAt = contract.UpdatedAt
        };
    }
}

