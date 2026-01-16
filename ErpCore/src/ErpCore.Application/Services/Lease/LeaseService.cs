using ErpCore.Application.DTOs.Lease;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Lease;
using ErpCore.Infrastructure.Repositories.Lease;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Lease;

/// <summary>
/// 租賃服務實作 (SYS8110-SYS8220)
/// </summary>
public class LeaseService : BaseService, ILeaseService
{
    private readonly ILeaseRepository _repository;

    public LeaseService(
        ILeaseRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<LeaseDto>> GetLeasesAsync(LeaseQueryDto query)
    {
        try
        {
            var repositoryQuery = new LeaseQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                LeaseId = query.LeaseId,
                TenantId = query.TenantId,
                ShopId = query.ShopId,
                Status = query.Status,
                StartDateFrom = query.StartDateFrom,
                StartDateTo = query.StartDateTo,
                EndDateFrom = query.EndDateFrom,
                EndDateTo = query.EndDateTo
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<LeaseDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢租賃列表失敗", ex);
            throw;
        }
    }

    public async Task<LeaseDto> GetLeaseByIdAsync(string leaseId)
    {
        try
        {
            var lease = await _repository.GetByIdAsync(leaseId);
            if (lease == null)
            {
                throw new InvalidOperationException($"租賃不存在: {leaseId}");
            }

            return MapToDto(lease);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢租賃失敗: {leaseId}", ex);
            throw;
        }
    }

    public async Task<LeaseResultDto> CreateLeaseAsync(CreateLeaseDto dto)
    {
        try
        {
            // 檢查租賃編號是否已存在
            if (await _repository.ExistsAsync(dto.LeaseId))
            {
                throw new InvalidOperationException($"租賃編號已存在: {dto.LeaseId}");
            }

            // 驗證日期範圍
            if (dto.EndDate.HasValue && dto.StartDate > dto.EndDate.Value)
            {
                throw new InvalidOperationException("租期開始日不能晚於結束日");
            }

            var lease = new Lease
            {
                LeaseId = dto.LeaseId,
                TenantId = dto.TenantId,
                TenantName = dto.TenantName,
                ShopId = dto.ShopId,
                FloorId = dto.FloorId,
                LocationId = dto.LocationId,
                LeaseDate = dto.LeaseDate,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status,
                MonthlyRent = dto.MonthlyRent,
                TotalRent = dto.TotalRent,
                Deposit = dto.Deposit,
                CurrencyId = dto.CurrencyId,
                PaymentMethod = dto.PaymentMethod,
                PaymentDay = dto.PaymentDay,
                Memo = dto.Memo,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(lease);

            _logger.LogInfo($"新增租賃成功: {dto.LeaseId}");
            return new LeaseResultDto
            {
                TKey = lease.TKey,
                LeaseId = lease.LeaseId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增租賃失敗: {dto.LeaseId}", ex);
            throw;
        }
    }

    public async Task UpdateLeaseAsync(string leaseId, UpdateLeaseDto dto)
    {
        try
        {
            var lease = await _repository.GetByIdAsync(leaseId);
            if (lease == null)
            {
                throw new InvalidOperationException($"租賃不存在: {leaseId}");
            }

            // 驗證日期範圍
            if (dto.EndDate.HasValue && dto.StartDate > dto.EndDate.Value)
            {
                throw new InvalidOperationException("租期開始日不能晚於結束日");
            }

            lease.TenantId = dto.TenantId;
            lease.TenantName = dto.TenantName;
            lease.ShopId = dto.ShopId;
            lease.FloorId = dto.FloorId;
            lease.LocationId = dto.LocationId;
            lease.LeaseDate = dto.LeaseDate;
            lease.StartDate = dto.StartDate;
            lease.EndDate = dto.EndDate;
            lease.Status = dto.Status;
            lease.MonthlyRent = dto.MonthlyRent;
            lease.TotalRent = dto.TotalRent;
            lease.Deposit = dto.Deposit;
            lease.CurrencyId = dto.CurrencyId;
            lease.PaymentMethod = dto.PaymentMethod;
            lease.PaymentDay = dto.PaymentDay;
            lease.Memo = dto.Memo;
            lease.UpdatedBy = GetCurrentUserId();
            lease.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(lease);

            _logger.LogInfo($"修改租賃成功: {leaseId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改租賃失敗: {leaseId}", ex);
            throw;
        }
    }

    public async Task DeleteLeaseAsync(string leaseId)
    {
        try
        {
            var lease = await _repository.GetByIdAsync(leaseId);
            if (lease == null)
            {
                throw new InvalidOperationException($"租賃不存在: {leaseId}");
            }

            await _repository.DeleteAsync(leaseId);

            _logger.LogInfo($"刪除租賃成功: {leaseId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除租賃失敗: {leaseId}", ex);
            throw;
        }
    }

    public async Task BatchDeleteLeasesAsync(BatchDeleteLeaseDto dto)
    {
        try
        {
            foreach (var leaseId in dto.LeaseIds)
            {
                if (await _repository.ExistsAsync(leaseId))
                {
                    await _repository.DeleteAsync(leaseId);
                }
            }

            _logger.LogInfo($"批次刪除租賃成功: {dto.LeaseIds.Count} 筆");
        }
        catch (Exception ex)
        {
            _logger.LogError($"批次刪除租賃失敗", ex);
            throw;
        }
    }

    public async Task UpdateLeaseStatusAsync(string leaseId, UpdateLeaseStatusDto dto)
    {
        try
        {
            var lease = await _repository.GetByIdAsync(leaseId);
            if (lease == null)
            {
                throw new InvalidOperationException($"租賃不存在: {leaseId}");
            }

            await _repository.UpdateStatusAsync(leaseId, dto.Status);

            _logger.LogInfo($"更新租賃狀態成功: {leaseId}, Status: {dto.Status}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新租賃狀態失敗: {leaseId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string leaseId)
    {
        try
        {
            return await _repository.ExistsAsync(leaseId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查租賃是否存在失敗: {leaseId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 將 Entity 轉換為 DTO
    /// </summary>
    private LeaseDto MapToDto(ErpCore.Domain.Entities.Lease.Lease lease)
    {
        return new LeaseDto
        {
            TKey = lease.TKey,
            LeaseId = lease.LeaseId,
            TenantId = lease.TenantId,
            TenantName = lease.TenantName,
            ShopId = lease.ShopId,
            FloorId = lease.FloorId,
            LocationId = lease.LocationId,
            LeaseDate = lease.LeaseDate,
            StartDate = lease.StartDate,
            EndDate = lease.EndDate,
            Status = lease.Status,
            StatusName = GetStatusName(lease.Status),
            MonthlyRent = lease.MonthlyRent,
            TotalRent = lease.TotalRent,
            Deposit = lease.Deposit,
            CurrencyId = lease.CurrencyId,
            PaymentMethod = lease.PaymentMethod,
            PaymentDay = lease.PaymentDay,
            Memo = lease.Memo,
            SiteId = lease.SiteId,
            OrgId = lease.OrgId,
            CreatedBy = lease.CreatedBy,
            CreatedAt = lease.CreatedAt,
            UpdatedBy = lease.UpdatedBy,
            UpdatedAt = lease.UpdatedAt
        };
    }

    /// <summary>
    /// 取得狀態名稱
    /// </summary>
    private string GetStatusName(string status)
    {
        return status switch
        {
            "D" => "草稿",
            "S" => "已簽約",
            "E" => "已生效",
            "T" => "已終止",
            _ => status
        };
    }
}

