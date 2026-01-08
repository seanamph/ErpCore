using ErpCore.Application.DTOs.TaxAccounting;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.TaxAccounting;
using ErpCore.Infrastructure.Repositories.TaxAccounting;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.TaxAccounting;

/// <summary>
/// 傳票型態服務實作 (SYST121-SYST122)
/// </summary>
public class VoucherTypeService : BaseService, IVoucherTypeService
{
    private readonly IVoucherTypeRepository _repository;

    public VoucherTypeService(
        IVoucherTypeRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<VoucherTypeDto>> GetVoucherTypesAsync(VoucherTypeQueryDto query)
    {
        try
        {
            var repositoryQuery = new VoucherTypeQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                VoucherId = query.VoucherId,
                VoucherName = query.VoucherName,
                Status = query.Status
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<VoucherTypeDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢傳票型態列表失敗", ex);
            throw;
        }
    }

    public async Task<VoucherTypeDto> GetVoucherTypeByIdAsync(string voucherId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(voucherId);
            if (entity == null)
            {
                throw new InvalidOperationException($"傳票型態不存在: {voucherId}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢傳票型態失敗: {voucherId}", ex);
            throw;
        }
    }

    public async Task<string> CreateVoucherTypeAsync(CreateVoucherTypeDto dto)
    {
        try
        {
            // 驗證必填欄位
            if (string.IsNullOrWhiteSpace(dto.VoucherId))
            {
                throw new ArgumentException("型態代號不能為空");
            }

            if (string.IsNullOrWhiteSpace(dto.VoucherName))
            {
                throw new ArgumentException("型態名稱不能為空");
            }

            // 檢查型態代號是否已存在
            var exists = await _repository.ExistsAsync(dto.VoucherId);
            if (exists)
            {
                throw new InvalidOperationException($"型態代號已存在: {dto.VoucherId}");
            }

            var entity = MapToEntity(dto);
            entity.CreatedBy = GetCurrentUserId();
            entity.CreatedAt = DateTime.Now;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.CreateAsync(entity);

            _logger.LogInfo($"新增傳票型態成功: {dto.VoucherId}");

            return dto.VoucherId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增傳票型態失敗: {dto.VoucherId}", ex);
            throw;
        }
    }

    public async Task UpdateVoucherTypeAsync(string voucherId, UpdateVoucherTypeDto dto)
    {
        try
        {
            // 檢查型態是否存在
            var existing = await _repository.GetByIdAsync(voucherId);
            if (existing == null)
            {
                throw new InvalidOperationException($"傳票型態不存在: {voucherId}");
            }

            // 驗證必填欄位
            if (string.IsNullOrWhiteSpace(dto.VoucherName))
            {
                throw new ArgumentException("型態名稱不能為空");
            }

            existing.VoucherName = dto.VoucherName;
            existing.Status = dto.Status ?? existing.Status;
            existing.UpdatedBy = GetCurrentUserId();
            existing.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(existing);

            _logger.LogInfo($"修改傳票型態成功: {voucherId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改傳票型態失敗: {voucherId}", ex);
            throw;
        }
    }

    public async Task DeleteVoucherTypeAsync(string voucherId)
    {
        try
        {
            // 檢查型態是否存在
            var existing = await _repository.GetByIdAsync(voucherId);
            if (existing == null)
            {
                throw new InvalidOperationException($"傳票型態不存在: {voucherId}");
            }

            // 檢查是否有使用中的傳票
            var isInUse = await _repository.IsInUseAsync(voucherId);
            if (isInUse)
            {
                throw new InvalidOperationException($"傳票型態有使用中的傳票，無法刪除: {voucherId}");
            }

            await _repository.DeleteAsync(voucherId);

            _logger.LogInfo($"刪除傳票型態成功: {voucherId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除傳票型態失敗: {voucherId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string voucherId)
    {
        try
        {
            return await _repository.ExistsAsync(voucherId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查型態代號是否存在失敗: {voucherId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 將 Entity 轉換為 DTO
    /// </summary>
    private VoucherTypeDto MapToDto(VoucherType entity)
    {
        return new VoucherTypeDto
        {
            TKey = entity.TKey,
            VoucherId = entity.VoucherId,
            VoucherName = entity.VoucherName,
            Status = entity.Status,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }

    /// <summary>
    /// 將 Create DTO 轉換為 Entity
    /// </summary>
    private VoucherType MapToEntity(CreateVoucherTypeDto dto)
    {
        return new VoucherType
        {
            VoucherId = dto.VoucherId,
            VoucherName = dto.VoucherName,
            Status = dto.Status ?? "1"
        };
    }
}

