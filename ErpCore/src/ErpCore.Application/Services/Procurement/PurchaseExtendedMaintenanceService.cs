using ErpCore.Application.DTOs.Procurement;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Procurement;
using ErpCore.Infrastructure.Repositories.Procurement;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Procurement;

/// <summary>
/// 採購擴展維護服務實作 (SYSPA10-SYSPB60)
/// </summary>
public class PurchaseExtendedMaintenanceService : BaseService, IPurchaseExtendedMaintenanceService
{
    private readonly IPurchaseExtendedMaintenanceRepository _repository;

    public PurchaseExtendedMaintenanceService(
        IPurchaseExtendedMaintenanceRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<PurchaseExtendedMaintenanceDto>> GetPurchaseExtendedMaintenancesAsync(PurchaseExtendedMaintenanceQueryDto query)
    {
        try
        {
            var repositoryQuery = new PurchaseExtendedMaintenanceQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                MaintenanceId = query.MaintenanceId,
                MaintenanceName = query.MaintenanceName,
                MaintenanceType = query.MaintenanceType,
                Status = query.Status
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<PurchaseExtendedMaintenanceDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢採購擴展維護列表失敗", ex);
            throw;
        }
    }

    public async Task<PurchaseExtendedMaintenanceDto> GetPurchaseExtendedMaintenanceByTKeyAsync(long tKey)
    {
        try
        {
            var purchaseExtendedMaintenance = await _repository.GetByIdAsync(tKey);
            if (purchaseExtendedMaintenance == null)
            {
                throw new InvalidOperationException($"採購擴展維護不存在: {tKey}");
            }

            return MapToDto(purchaseExtendedMaintenance);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢採購擴展維護失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<PurchaseExtendedMaintenanceDto> GetPurchaseExtendedMaintenanceByMaintenanceIdAsync(string maintenanceId)
    {
        try
        {
            var purchaseExtendedMaintenance = await _repository.GetByMaintenanceIdAsync(maintenanceId);
            if (purchaseExtendedMaintenance == null)
            {
                throw new InvalidOperationException($"採購擴展維護不存在: {maintenanceId}");
            }

            return MapToDto(purchaseExtendedMaintenance);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢採購擴展維護失敗: {maintenanceId}", ex);
            throw;
        }
    }

    public async Task<long> CreatePurchaseExtendedMaintenanceAsync(CreatePurchaseExtendedMaintenanceDto dto)
    {
        try
        {
            // 檢查維護代碼是否已存在
            if (await _repository.ExistsAsync(dto.MaintenanceId))
            {
                throw new InvalidOperationException($"維護代碼已存在: {dto.MaintenanceId}");
            }

            var purchaseExtendedMaintenance = new PurchaseExtendedMaintenance
            {
                MaintenanceId = dto.MaintenanceId,
                MaintenanceName = dto.MaintenanceName,
                MaintenanceType = dto.MaintenanceType,
                MaintenanceDesc = dto.MaintenanceDesc,
                MaintenanceConfig = dto.MaintenanceConfig,
                ParameterConfig = dto.ParameterConfig,
                Status = dto.Status,
                SeqNo = dto.SeqNo,
                Memo = dto.Memo,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(purchaseExtendedMaintenance);

            _logger.LogInfo($"新增採購擴展維護成功: {dto.MaintenanceId}");
            return purchaseExtendedMaintenance.TKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增採購擴展維護失敗: {dto.MaintenanceId}", ex);
            throw;
        }
    }

    public async Task UpdatePurchaseExtendedMaintenanceAsync(long tKey, UpdatePurchaseExtendedMaintenanceDto dto)
    {
        try
        {
            var purchaseExtendedMaintenance = await _repository.GetByIdAsync(tKey);
            if (purchaseExtendedMaintenance == null)
            {
                throw new InvalidOperationException($"採購擴展維護不存在: {tKey}");
            }

            purchaseExtendedMaintenance.MaintenanceName = dto.MaintenanceName;
            purchaseExtendedMaintenance.MaintenanceType = dto.MaintenanceType;
            purchaseExtendedMaintenance.MaintenanceDesc = dto.MaintenanceDesc;
            purchaseExtendedMaintenance.MaintenanceConfig = dto.MaintenanceConfig;
            purchaseExtendedMaintenance.ParameterConfig = dto.ParameterConfig;
            purchaseExtendedMaintenance.Status = dto.Status;
            purchaseExtendedMaintenance.SeqNo = dto.SeqNo;
            purchaseExtendedMaintenance.Memo = dto.Memo;
            purchaseExtendedMaintenance.UpdatedBy = GetCurrentUserId();
            purchaseExtendedMaintenance.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(purchaseExtendedMaintenance);

            _logger.LogInfo($"修改採購擴展維護成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改採購擴展維護失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeletePurchaseExtendedMaintenanceAsync(long tKey)
    {
        try
        {
            var purchaseExtendedMaintenance = await _repository.GetByIdAsync(tKey);
            if (purchaseExtendedMaintenance == null)
            {
                throw new InvalidOperationException($"採購擴展維護不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);

            _logger.LogInfo($"刪除採購擴展維護成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除採購擴展維護失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string maintenanceId)
    {
        try
        {
            return await _repository.ExistsAsync(maintenanceId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查採購擴展維護是否存在失敗: {maintenanceId}", ex);
            throw;
        }
    }

    private PurchaseExtendedMaintenanceDto MapToDto(PurchaseExtendedMaintenance purchaseExtendedMaintenance)
    {
        return new PurchaseExtendedMaintenanceDto
        {
            TKey = purchaseExtendedMaintenance.TKey,
            MaintenanceId = purchaseExtendedMaintenance.MaintenanceId,
            MaintenanceName = purchaseExtendedMaintenance.MaintenanceName,
            MaintenanceType = purchaseExtendedMaintenance.MaintenanceType,
            MaintenanceDesc = purchaseExtendedMaintenance.MaintenanceDesc,
            MaintenanceConfig = purchaseExtendedMaintenance.MaintenanceConfig,
            ParameterConfig = purchaseExtendedMaintenance.ParameterConfig,
            Status = purchaseExtendedMaintenance.Status,
            SeqNo = purchaseExtendedMaintenance.SeqNo,
            Memo = purchaseExtendedMaintenance.Memo,
            CreatedBy = purchaseExtendedMaintenance.CreatedBy,
            CreatedAt = purchaseExtendedMaintenance.CreatedAt,
            UpdatedBy = purchaseExtendedMaintenance.UpdatedBy,
            UpdatedAt = purchaseExtendedMaintenance.UpdatedAt
        };
    }
}
