using ErpCore.Application.DTOs.TaxAccounting;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.TaxAccounting;
using ErpCore.Infrastructure.Repositories.TaxAccounting;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.TaxAccounting;

/// <summary>
/// 常用傳票服務實作 (SYST123)
/// </summary>
public class CommonVoucherService : BaseService, ICommonVoucherService
{
    private readonly ICommonVoucherRepository _repository;

    public CommonVoucherService(
        ICommonVoucherRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<CommonVoucherDto>> GetCommonVouchersAsync(CommonVoucherQueryDto query)
    {
        try
        {
            var repositoryQuery = new CommonVoucherQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                VoucherId = query.VoucherId,
                VoucherName = query.VoucherName,
                VoucherType = query.VoucherType,
                SiteId = query.SiteId
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<CommonVoucherDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢常用傳票列表失敗", ex);
            throw;
        }
    }

    public async Task<CommonVoucherDto> GetCommonVoucherByTKeyAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByTKeyAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"常用傳票不存在: {tKey}");
            }

            var dto = MapToDto(entity);

            // 載入明細
            var details = await _repository.GetDetailsAsync(tKey);
            dto.Details = details.Select(x => MapDetailToDto(x)).ToList();

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢常用傳票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<long> CreateCommonVoucherAsync(CreateCommonVoucherDto dto)
    {
        try
        {
            // 驗證必填欄位
            if (string.IsNullOrWhiteSpace(dto.VoucherId))
            {
                throw new ArgumentException("傳票代號不能為空");
            }

            if (string.IsNullOrWhiteSpace(dto.VoucherName))
            {
                throw new ArgumentException("傳票名稱不能為空");
            }

            // 檢查傳票代號是否已存在
            var exists = await _repository.ExistsAsync(dto.VoucherId);
            if (exists)
            {
                throw new InvalidOperationException($"傳票代號已存在: {dto.VoucherId}");
            }

            // 驗證借貸平衡
            if (dto.Details != null && dto.Details.Count > 0)
            {
                var totalDebit = dto.Details.Sum(x => x.DebitAmount);
                var totalCredit = dto.Details.Sum(x => x.CreditAmount);
                if (totalDebit != totalCredit)
                {
                    throw new InvalidOperationException($"借貸金額不平衡，借方總額: {totalDebit}, 貸方總額: {totalCredit}");
                }
            }

            var entity = MapToEntity(dto);
            entity.CreatedBy = GetCurrentUserId();
            entity.CreatedAt = DateTime.Now;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            var created = await _repository.CreateAsync(entity);

            // 新增明細
            if (dto.Details != null && dto.Details.Count > 0)
            {
                foreach (var detailDto in dto.Details)
                {
                    var detail = MapDetailToEntity(detailDto, created.TKey);
                    detail.CreatedBy = GetCurrentUserId();
                    detail.CreatedAt = DateTime.Now;
                    detail.UpdatedBy = GetCurrentUserId();
                    detail.UpdatedAt = DateTime.Now;
                    await _repository.CreateDetailAsync(detail);
                }
            }

            _logger.LogInfo($"新增常用傳票成功: {dto.VoucherId}");

            return created.TKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增常用傳票失敗: {dto.VoucherId}", ex);
            throw;
        }
    }

    public async Task UpdateCommonVoucherAsync(long tKey, UpdateCommonVoucherDto dto)
    {
        try
        {
            // 檢查傳票是否存在
            var existing = await _repository.GetByTKeyAsync(tKey);
            if (existing == null)
            {
                throw new InvalidOperationException($"常用傳票不存在: {tKey}");
            }

            // 驗證必填欄位
            if (string.IsNullOrWhiteSpace(dto.VoucherName))
            {
                throw new ArgumentException("傳票名稱不能為空");
            }

            // 驗證借貸平衡
            if (dto.Details != null && dto.Details.Count > 0)
            {
                var totalDebit = dto.Details.Sum(x => x.DebitAmount);
                var totalCredit = dto.Details.Sum(x => x.CreditAmount);
                if (totalDebit != totalCredit)
                {
                    throw new InvalidOperationException($"借貸金額不平衡，借方總額: {totalDebit}, 貸方總額: {totalCredit}");
                }
            }

            existing.VoucherName = dto.VoucherName;
            existing.VoucherType = dto.VoucherType;
            existing.SiteId = dto.SiteId;
            existing.VendorId = dto.VendorId;
            existing.VendorName = dto.VendorName;
            existing.Notes = dto.Notes;
            existing.CustomField1 = dto.CustomField1;
            existing.UpdatedBy = GetCurrentUserId();
            existing.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(existing);

            // 更新明細（先刪除舊的，再新增新的）
            await _repository.DeleteDetailsAsync(tKey);
            if (dto.Details != null && dto.Details.Count > 0)
            {
                foreach (var detailDto in dto.Details)
                {
                    var detail = MapDetailToEntity(detailDto, tKey);
                    detail.CreatedBy = GetCurrentUserId();
                    detail.CreatedAt = DateTime.Now;
                    detail.UpdatedBy = GetCurrentUserId();
                    detail.UpdatedAt = DateTime.Now;
                    await _repository.CreateDetailAsync(detail);
                }
            }

            _logger.LogInfo($"修改常用傳票成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改常用傳票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteCommonVoucherAsync(long tKey)
    {
        try
        {
            // 檢查傳票是否存在
            var existing = await _repository.GetByTKeyAsync(tKey);
            if (existing == null)
            {
                throw new InvalidOperationException($"常用傳票不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);

            _logger.LogInfo($"刪除常用傳票成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除常用傳票失敗: {tKey}", ex);
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
            _logger.LogError($"檢查傳票代號是否存在失敗: {voucherId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 將 Entity 轉換為 DTO
    /// </summary>
    private CommonVoucherDto MapToDto(CommonVoucher entity)
    {
        return new CommonVoucherDto
        {
            TKey = entity.TKey,
            VoucherId = entity.VoucherId,
            VoucherName = entity.VoucherName,
            VoucherType = entity.VoucherType,
            SiteId = entity.SiteId,
            VendorId = entity.VendorId,
            VendorName = entity.VendorName,
            Notes = entity.Notes,
            CustomField1 = entity.CustomField1,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }

    /// <summary>
    /// 將明細 Entity 轉換為 DTO
    /// </summary>
    private CommonVoucherDetailDto MapDetailToDto(CommonVoucherDetail entity)
    {
        return new CommonVoucherDetailDto
        {
            TKey = entity.TKey,
            VoucherTKey = entity.VoucherTKey,
            SeqNo = entity.SeqNo,
            StypeId = entity.StypeId,
            DebitAmount = entity.DebitAmount,
            CreditAmount = entity.CreditAmount,
            OrgId = entity.OrgId,
            Notes = entity.Notes,
            VendorId = entity.VendorId,
            CustomField1 = entity.CustomField1
        };
    }

    /// <summary>
    /// 將 Create DTO 轉換為 Entity
    /// </summary>
    private CommonVoucher MapToEntity(CreateCommonVoucherDto dto)
    {
        return new CommonVoucher
        {
            VoucherId = dto.VoucherId,
            VoucherName = dto.VoucherName,
            VoucherType = dto.VoucherType,
            SiteId = dto.SiteId,
            VendorId = dto.VendorId,
            VendorName = dto.VendorName,
            Notes = dto.Notes,
            CustomField1 = dto.CustomField1
        };
    }

    /// <summary>
    /// 將明細 DTO 轉換為 Entity
    /// </summary>
    private CommonVoucherDetail MapDetailToEntity(CreateCommonVoucherDetailDto dto, long voucherTKey)
    {
        return new CommonVoucherDetail
        {
            VoucherTKey = voucherTKey,
            SeqNo = dto.SeqNo,
            StypeId = dto.StypeId,
            DebitAmount = dto.DebitAmount,
            CreditAmount = dto.CreditAmount,
            OrgId = dto.OrgId,
            Notes = dto.Notes,
            VendorId = dto.VendorId,
            CustomField1 = dto.CustomField1
        };
    }

    /// <summary>
    /// 將明細 DTO 轉換為 Entity（用於更新）
    /// </summary>
    private CommonVoucherDetail MapDetailToEntity(UpdateCommonVoucherDetailDto dto, long voucherTKey)
    {
        return new CommonVoucherDetail
        {
            TKey = dto.TKey ?? 0,
            VoucherTKey = voucherTKey,
            SeqNo = dto.SeqNo,
            StypeId = dto.StypeId,
            DebitAmount = dto.DebitAmount,
            CreditAmount = dto.CreditAmount,
            OrgId = dto.OrgId,
            Notes = dto.Notes,
            VendorId = dto.VendorId,
            CustomField1 = dto.CustomField1
        };
    }
}

