using ErpCore.Application.DTOs.Certificate;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Certificate;
using ErpCore.Infrastructure.Repositories.Certificate;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Certificate;

/// <summary>
/// 憑證服務實作 (SYSK110-SYSK150)
/// </summary>
public class VoucherService : BaseService, IVoucherService
{
    private readonly IVoucherRepository _voucherRepository;
    private readonly IVoucherDetailRepository _voucherDetailRepository;

    public VoucherService(
        IVoucherRepository voucherRepository,
        IVoucherDetailRepository voucherDetailRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _voucherRepository = voucherRepository;
        _voucherDetailRepository = voucherDetailRepository;
    }

    public async Task<PagedResult<VoucherDto>> GetVouchersAsync(VoucherQueryDto query)
    {
        try
        {
            var entities = await _voucherRepository.GetPagedAsync(
                query.PageIndex,
                query.PageSize,
                query.VoucherId,
                query.VoucherType,
                query.ShopId,
                query.Status,
                query.VoucherDateFrom,
                query.VoucherDateTo);

            var totalCount = await _voucherRepository.GetCountAsync(
                query.VoucherId,
                query.VoucherType,
                query.ShopId,
                query.Status,
                query.VoucherDateFrom,
                query.VoucherDateTo);

            var items = entities.Select(MapToDto).ToList();

            return new PagedResult<VoucherDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢憑證列表失敗", ex);
            throw;
        }
    }

    public async Task<VoucherDto> GetVoucherByIdAsync(string voucherId)
    {
        try
        {
            var entity = await _voucherRepository.GetByVoucherIdAsync(voucherId);
            if (entity == null)
            {
                throw new InvalidOperationException($"憑證不存在: {voucherId}");
            }

            var dto = MapToDto(entity);

            // 載入明細
            var details = await _voucherDetailRepository.GetByVoucherIdAsync(voucherId);
            dto.Details = details.Select(MapDetailToDto).ToList();

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢憑證失敗: {voucherId}", ex);
            throw;
        }
    }

    public async Task<VoucherDto> CreateVoucherAsync(CreateVoucherDto dto)
    {
        try
        {
            // 檢查是否已存在
            var exists = await _voucherRepository.ExistsAsync(dto.VoucherId);
            if (exists)
            {
                throw new InvalidOperationException($"憑證編號已存在: {dto.VoucherId}");
            }

            // 驗證借方貸方平衡
            var totalDebit = dto.Details.Sum(d => d.DebitAmount);
            var totalCredit = dto.Details.Sum(d => d.CreditAmount);
            if (totalDebit != totalCredit)
            {
                throw new InvalidOperationException($"借方總額 ({totalDebit}) 與貸方總額 ({totalCredit}) 不平衡");
            }

            // 建立憑證主檔
            var entity = new Voucher
            {
                VoucherId = dto.VoucherId,
                VoucherDate = dto.VoucherDate,
                VoucherType = dto.VoucherType,
                ShopId = dto.ShopId,
                Status = dto.Status,
                TotalAmount = totalDebit,
                TotalDebitAmount = totalDebit,
                TotalCreditAmount = totalCredit,
                Memo = dto.Memo,
                SiteId = dto.SiteId,
                OrgId = dto.OrgId,
                CurrencyId = dto.CurrencyId,
                ExchangeRate = dto.ExchangeRate,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var result = await _voucherRepository.CreateAsync(entity);

            // 建立明細
            foreach (var detailDto in dto.Details)
            {
                var detail = new VoucherDetail
                {
                    VoucherId = result.VoucherId,
                    LineNum = detailDto.LineNum,
                    AccountId = detailDto.AccountId,
                    DebitAmount = detailDto.DebitAmount,
                    CreditAmount = detailDto.CreditAmount,
                    Description = detailDto.Description,
                    Memo = detailDto.Memo,
                    CreatedBy = GetCurrentUserId(),
                    CreatedAt = DateTime.Now,
                    UpdatedBy = GetCurrentUserId(),
                    UpdatedAt = DateTime.Now
                };

                await _voucherDetailRepository.CreateAsync(detail);
            }

            return await GetVoucherByIdAsync(result.VoucherId);
        }
        catch (Exception ex)
        {
            _logger.LogError("新增憑證失敗", ex);
            throw;
        }
    }

    public async Task<VoucherDto> UpdateVoucherAsync(string voucherId, UpdateVoucherDto dto)
    {
        try
        {
            var entity = await _voucherRepository.GetByVoucherIdAsync(voucherId);
            if (entity == null)
            {
                throw new InvalidOperationException($"憑證不存在: {voucherId}");
            }

            // 已審核的憑證不可修改
            if (entity.Status == "A")
            {
                throw new InvalidOperationException($"已審核的憑證不可修改: {voucherId}");
            }

            // 驗證借方貸方平衡
            var totalDebit = dto.Details.Sum(d => d.DebitAmount);
            var totalCredit = dto.Details.Sum(d => d.CreditAmount);
            if (totalDebit != totalCredit)
            {
                throw new InvalidOperationException($"借方總額 ({totalDebit}) 與貸方總額 ({totalCredit}) 不平衡");
            }

            // 更新憑證主檔
            entity.VoucherDate = dto.VoucherDate;
            entity.VoucherType = dto.VoucherType;
            entity.ShopId = dto.ShopId;
            entity.Status = dto.Status;
            entity.TotalAmount = totalDebit;
            entity.TotalDebitAmount = totalDebit;
            entity.TotalCreditAmount = totalCredit;
            entity.Memo = dto.Memo;
            entity.SiteId = dto.SiteId;
            entity.OrgId = dto.OrgId;
            entity.CurrencyId = dto.CurrencyId;
            entity.ExchangeRate = dto.ExchangeRate;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _voucherRepository.UpdateAsync(entity);

            // 刪除舊明細
            await _voucherDetailRepository.DeleteByVoucherIdAsync(voucherId);

            // 建立新明細
            foreach (var detailDto in dto.Details)
            {
                var detail = new VoucherDetail
                {
                    VoucherId = voucherId,
                    LineNum = detailDto.LineNum,
                    AccountId = detailDto.AccountId,
                    DebitAmount = detailDto.DebitAmount,
                    CreditAmount = detailDto.CreditAmount,
                    Description = detailDto.Description,
                    Memo = detailDto.Memo,
                    CreatedBy = GetCurrentUserId(),
                    CreatedAt = DateTime.Now,
                    UpdatedBy = GetCurrentUserId(),
                    UpdatedAt = DateTime.Now
                };

                await _voucherDetailRepository.CreateAsync(detail);
            }

            return await GetVoucherByIdAsync(voucherId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新憑證失敗: {voucherId}", ex);
            throw;
        }
    }

    public async Task DeleteVoucherAsync(string voucherId)
    {
        try
        {
            var entity = await _voucherRepository.GetByVoucherIdAsync(voucherId);
            if (entity == null)
            {
                throw new InvalidOperationException($"憑證不存在: {voucherId}");
            }

            // 已審核的憑證不可刪除
            if (entity.Status == "A")
            {
                throw new InvalidOperationException($"已審核的憑證不可刪除: {voucherId}");
            }

            await _voucherRepository.DeleteAsync(voucherId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除憑證失敗: {voucherId}", ex);
            throw;
        }
    }

    public async Task ApproveVoucherAsync(string voucherId, ApproveVoucherDto dto)
    {
        try
        {
            var entity = await _voucherRepository.GetByVoucherIdAsync(voucherId);
            if (entity == null)
            {
                throw new InvalidOperationException($"憑證不存在: {voucherId}");
            }

            if (entity.Status != "S")
            {
                throw new InvalidOperationException($"只有已送出的憑證才能審核: {voucherId}");
            }

            entity.Status = "A";
            entity.ApproveUserId = dto.ApproveUserId;
            entity.ApproveDate = DateTime.Now;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _voucherRepository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"審核憑證失敗: {voucherId}", ex);
            throw;
        }
    }

    public async Task CancelVoucherAsync(string voucherId, CancelVoucherDto dto)
    {
        try
        {
            var entity = await _voucherRepository.GetByVoucherIdAsync(voucherId);
            if (entity == null)
            {
                throw new InvalidOperationException($"憑證不存在: {voucherId}");
            }

            entity.Status = "X";
            entity.Memo = dto.Memo;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _voucherRepository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"取消憑證失敗: {voucherId}", ex);
            throw;
        }
    }

    public async Task<List<VoucherCheckResultDto>> CheckVouchersAsync(List<string> voucherIds)
    {
        try
        {
            var results = new List<VoucherCheckResultDto>();

            foreach (var voucherId in voucherIds)
            {
                var result = new VoucherCheckResultDto
                {
                    VoucherId = voucherId,
                    Status = "SUCCESS",
                    Message = "檢查通過",
                    Errors = new List<string>()
                };

                try
                {
                    var voucher = await GetVoucherByIdAsync(voucherId);

                    // 檢查借方貸方平衡
                    if (voucher.TotalDebitAmount != voucher.TotalCreditAmount)
                    {
                        result.Status = "FAILED";
                        result.Message = "借方貸方不平衡";
                        result.Errors.Add($"借方總額 ({voucher.TotalDebitAmount}) 與貸方總額 ({voucher.TotalCreditAmount}) 不一致");
                    }

                    // 檢查明細是否為空
                    if (voucher.Details == null || !voucher.Details.Any())
                    {
                        result.Status = "FAILED";
                        result.Message = "憑證明細為空";
                        result.Errors.Add("憑證明細至少需有一筆");
                    }
                }
                catch (Exception ex)
                {
                    result.Status = "FAILED";
                    result.Message = $"檢查失敗: {ex.Message}";
                    result.Errors.Add(ex.Message);
                }

                results.Add(result);
            }

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError("檢查憑證失敗", ex);
            throw;
        }
    }

    private VoucherDto MapToDto(Voucher entity)
    {
        return new VoucherDto
        {
            TKey = entity.TKey,
            VoucherId = entity.VoucherId,
            VoucherDate = entity.VoucherDate,
            VoucherType = entity.VoucherType,
            ShopId = entity.ShopId,
            Status = entity.Status,
            ApplyUserId = entity.ApplyUserId,
            ApplyDate = entity.ApplyDate,
            ApproveUserId = entity.ApproveUserId,
            ApproveDate = entity.ApproveDate,
            TotalAmount = entity.TotalAmount,
            TotalDebitAmount = entity.TotalDebitAmount,
            TotalCreditAmount = entity.TotalCreditAmount,
            Memo = entity.Memo,
            SiteId = entity.SiteId,
            OrgId = entity.OrgId,
            CurrencyId = entity.CurrencyId,
            ExchangeRate = entity.ExchangeRate,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private VoucherDetailDto MapDetailToDto(VoucherDetail entity)
    {
        return new VoucherDetailDto
        {
            TKey = entity.TKey,
            VoucherId = entity.VoucherId,
            LineNum = entity.LineNum,
            AccountId = entity.AccountId,
            DebitAmount = entity.DebitAmount,
            CreditAmount = entity.CreditAmount,
            Description = entity.Description,
            Memo = entity.Memo,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

