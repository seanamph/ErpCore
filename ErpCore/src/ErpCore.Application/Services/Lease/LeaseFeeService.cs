using ErpCore.Application.DTOs.Lease;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Lease;
using ErpCore.Infrastructure.Repositories.Lease;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Lease;

/// <summary>
/// 費用主檔服務實作 (SYSE310-SYSE430)
/// </summary>
public class LeaseFeeService : BaseService, ILeaseFeeService
{
    private readonly ILeaseFeeRepository _repository;

    public LeaseFeeService(
        ILeaseFeeRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<LeaseFeeDto>> GetLeaseFeesAsync(LeaseFeeQueryDto query)
    {
        try
        {
            var repositoryQuery = new LeaseFeeQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                FeeId = query.FeeId,
                LeaseId = query.LeaseId,
                FeeType = query.FeeType,
                Status = query.Status,
                FeeDateFrom = query.FeeDateFrom,
                FeeDateTo = query.FeeDateTo
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<LeaseFeeDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢費用列表失敗", ex);
            throw;
        }
    }

    public async Task<LeaseFeeDto> GetLeaseFeeByIdAsync(string feeId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(feeId);
            if (entity == null)
            {
                throw new InvalidOperationException($"費用不存在: {feeId}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢費用失敗: {feeId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<LeaseFeeDto>> GetLeaseFeesByLeaseIdAsync(string leaseId)
    {
        try
        {
            var items = await _repository.GetByLeaseIdAsync(leaseId);
            return items.Select(x => MapToDto(x)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢費用失敗: {leaseId}", ex);
            throw;
        }
    }

    public async Task<LeaseFeeDto> CreateLeaseFeeAsync(CreateLeaseFeeDto dto)
    {
        try
        {
            // 檢查費用編號是否已存在
            if (await _repository.ExistsAsync(dto.FeeId))
            {
                throw new InvalidOperationException($"費用編號已存在: {dto.FeeId}");
            }

            // 計算稅額和總額
            var taxAmount = dto.FeeAmount * dto.TaxRate / 100;
            var totalAmount = dto.FeeAmount + taxAmount;

            var entity = new LeaseFee
            {
                FeeId = dto.FeeId,
                LeaseId = dto.LeaseId,
                FeeType = dto.FeeType,
                FeeItemId = dto.FeeItemId,
                FeeItemName = dto.FeeItemName,
                FeeAmount = dto.FeeAmount,
                FeeDate = dto.FeeDate,
                DueDate = dto.DueDate,
                Status = dto.Status,
                CurrencyId = dto.CurrencyId ?? "TWD",
                ExchangeRate = dto.ExchangeRate,
                TaxRate = dto.TaxRate,
                TaxAmount = taxAmount,
                TotalAmount = totalAmount,
                Memo = dto.Memo,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var result = await _repository.CreateAsync(entity);
            _logger.LogInfo($"新增費用成功: {dto.FeeId}");

            return MapToDto(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增費用失敗: {dto.FeeId}", ex);
            throw;
        }
    }

    public async Task UpdateLeaseFeeAsync(string feeId, UpdateLeaseFeeDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(feeId);
            if (entity == null)
            {
                throw new InvalidOperationException($"費用不存在: {feeId}");
            }

            // 計算稅額和總額
            var taxAmount = dto.FeeAmount * dto.TaxRate / 100;
            var totalAmount = dto.FeeAmount + taxAmount;

            entity.FeeType = dto.FeeType;
            entity.FeeItemId = dto.FeeItemId;
            entity.FeeItemName = dto.FeeItemName;
            entity.FeeAmount = dto.FeeAmount;
            entity.FeeDate = dto.FeeDate;
            entity.DueDate = dto.DueDate;
            entity.Status = dto.Status;
            entity.CurrencyId = dto.CurrencyId ?? "TWD";
            entity.ExchangeRate = dto.ExchangeRate;
            entity.TaxRate = dto.TaxRate;
            entity.TaxAmount = taxAmount;
            entity.TotalAmount = totalAmount;
            entity.Memo = dto.Memo;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"修改費用成功: {feeId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改費用失敗: {feeId}", ex);
            throw;
        }
    }

    public async Task DeleteLeaseFeeAsync(string feeId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(feeId);
            if (entity == null)
            {
                throw new InvalidOperationException($"費用不存在: {feeId}");
            }

            await _repository.DeleteAsync(feeId);
            _logger.LogInfo($"刪除費用成功: {feeId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除費用失敗: {feeId}", ex);
            throw;
        }
    }

    public async Task UpdateLeaseFeeStatusAsync(string feeId, string status)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(feeId);
            if (entity == null)
            {
                throw new InvalidOperationException($"費用不存在: {feeId}");
            }

            await _repository.UpdateStatusAsync(feeId, status);
            _logger.LogInfo($"更新費用狀態成功: {feeId} -> {status}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新費用狀態失敗: {feeId}", ex);
            throw;
        }
    }

    public async Task UpdateLeaseFeePaidAmountAsync(string feeId, decimal paidAmount, DateTime? paidDate)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(feeId);
            if (entity == null)
            {
                throw new InvalidOperationException($"費用不存在: {feeId}");
            }

            await _repository.UpdatePaidAmountAsync(feeId, paidAmount, paidDate);
            _logger.LogInfo($"更新費用已付金額成功: {feeId} -> {paidAmount}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新費用已付金額失敗: {feeId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string feeId)
    {
        try
        {
            return await _repository.ExistsAsync(feeId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查費用是否存在失敗: {feeId}", ex);
            throw;
        }
    }

    private LeaseFeeDto MapToDto(LeaseFee entity)
    {
        return new LeaseFeeDto
        {
            TKey = entity.TKey,
            FeeId = entity.FeeId,
            LeaseId = entity.LeaseId,
            FeeType = entity.FeeType,
            FeeItemId = entity.FeeItemId,
            FeeItemName = entity.FeeItemName,
            FeeAmount = entity.FeeAmount,
            FeeDate = entity.FeeDate,
            DueDate = entity.DueDate,
            PaidDate = entity.PaidDate,
            PaidAmount = entity.PaidAmount,
            Status = entity.Status,
            StatusName = GetStatusName(entity.Status),
            CurrencyId = entity.CurrencyId,
            ExchangeRate = entity.ExchangeRate,
            TaxRate = entity.TaxRate,
            TaxAmount = entity.TaxAmount,
            TotalAmount = entity.TotalAmount,
            Memo = entity.Memo,
            SiteId = entity.SiteId,
            OrgId = entity.OrgId,
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
            "P" => "待付款",
            "P1" => "部分付款",
            "F" => "已付款",
            "C" => "已取消",
            _ => status
        };
    }
}

