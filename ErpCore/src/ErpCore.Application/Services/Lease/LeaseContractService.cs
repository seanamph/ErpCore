using ErpCore.Application.DTOs.Lease;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Lease;
using ErpCore.Infrastructure.Repositories.Lease;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Lease;

/// <summary>
/// 租賃合同資料服務實作 (SYSM111-SYSM138)
/// </summary>
public class LeaseContractService : BaseService, ILeaseContractService
{
    private readonly ILeaseContractRepository _repository;

    public LeaseContractService(
        ILeaseContractRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<LeaseContractDto>> GetLeaseContractsAsync(LeaseContractQueryDto query)
    {
        try
        {
            var repositoryQuery = new LeaseContractQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                ContractNo = query.ContractNo,
                LeaseId = query.LeaseId,
                ContractType = query.ContractType,
                Status = query.Status,
                ContractDateFrom = query.ContractDateFrom,
                ContractDateTo = query.ContractDateTo
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<LeaseContractDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢租賃合同列表失敗", ex);
            throw;
        }
    }

    public async Task<LeaseContractDto> GetLeaseContractByIdAsync(string contractNo)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(contractNo);
            if (entity == null)
            {
                throw new InvalidOperationException($"租賃合同不存在: {contractNo}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢租賃合同失敗: {contractNo}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<LeaseContractDto>> GetLeaseContractsByLeaseIdAsync(string leaseId)
    {
        try
        {
            var items = await _repository.GetByLeaseIdAsync(leaseId);
            return items.Select(x => MapToDto(x)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢租賃合同失敗: {leaseId}", ex);
            throw;
        }
    }

    public async Task<LeaseContractDto> CreateLeaseContractAsync(CreateLeaseContractDto dto)
    {
        try
        {
            // 檢查合同編號是否已存在
            if (await _repository.ExistsAsync(dto.ContractNo))
            {
                throw new InvalidOperationException($"合同編號已存在: {dto.ContractNo}");
            }

            var entity = new LeaseContract
            {
                ContractNo = dto.ContractNo,
                LeaseId = dto.LeaseId,
                ContractDate = dto.ContractDate,
                ContractType = dto.ContractType,
                ContractContent = dto.ContractContent,
                Status = dto.Status,
                SignedBy = dto.SignedBy,
                SignedDate = dto.SignedDate,
                Memo = dto.Memo,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var result = await _repository.CreateAsync(entity);
            _logger.LogInfo($"新增租賃合同成功: {dto.ContractNo}");

            return MapToDto(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增租賃合同失敗: {dto.ContractNo}", ex);
            throw;
        }
    }

    public async Task UpdateLeaseContractAsync(string contractNo, UpdateLeaseContractDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(contractNo);
            if (entity == null)
            {
                throw new InvalidOperationException($"租賃合同不存在: {contractNo}");
            }

            entity.ContractDate = dto.ContractDate;
            entity.ContractType = dto.ContractType;
            entity.ContractContent = dto.ContractContent;
            entity.Status = dto.Status;
            entity.SignedBy = dto.SignedBy;
            entity.SignedDate = dto.SignedDate;
            entity.Memo = dto.Memo;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"修改租賃合同成功: {contractNo}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改租賃合同失敗: {contractNo}", ex);
            throw;
        }
    }

    public async Task DeleteLeaseContractAsync(string contractNo)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(contractNo);
            if (entity == null)
            {
                throw new InvalidOperationException($"租賃合同不存在: {contractNo}");
            }

            await _repository.DeleteAsync(contractNo);
            _logger.LogInfo($"刪除租賃合同成功: {contractNo}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除租賃合同失敗: {contractNo}", ex);
            throw;
        }
    }

    public async Task UpdateLeaseContractStatusAsync(string contractNo, string status)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(contractNo);
            if (entity == null)
            {
                throw new InvalidOperationException($"租賃合同不存在: {contractNo}");
            }

            await _repository.UpdateStatusAsync(contractNo, status);
            _logger.LogInfo($"更新租賃合同狀態成功: {contractNo} -> {status}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新租賃合同狀態失敗: {contractNo}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string contractNo)
    {
        try
        {
            return await _repository.ExistsAsync(contractNo);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查租賃合同是否存在失敗: {contractNo}", ex);
            throw;
        }
    }

    private LeaseContractDto MapToDto(LeaseContract entity)
    {
        return new LeaseContractDto
        {
            TKey = entity.TKey,
            ContractNo = entity.ContractNo,
            LeaseId = entity.LeaseId,
            ContractDate = entity.ContractDate,
            ContractType = entity.ContractType,
            ContractContent = entity.ContractContent,
            Status = entity.Status,
            StatusName = GetStatusName(entity.Status),
            SignedBy = entity.SignedBy,
            SignedDate = entity.SignedDate,
            Memo = entity.Memo,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private string? GetStatusName(string status)
    {
        return status switch
        {
            "A" => "有效",
            "I" => "無效",
            _ => status
        };
    }
}

