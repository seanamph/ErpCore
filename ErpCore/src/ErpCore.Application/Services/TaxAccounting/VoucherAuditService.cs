using ErpCore.Application.DTOs.TaxAccounting;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Accounting;
using ErpCore.Domain.Entities.TaxAccounting;
using ErpCore.Infrastructure.Repositories.Accounting;
using ErpCore.Infrastructure.Repositories.TaxAccounting;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.TaxAccounting;

/// <summary>
/// 暫存傳票審核服務實作 (SYSTA00-SYSTA70)
/// </summary>
public class VoucherAuditService : BaseService, IVoucherAuditService
{
    private readonly IVoucherAuditRepository _repository;
    private readonly IVoucherRepository _voucherRepository;

    public VoucherAuditService(
        IVoucherAuditRepository repository,
        IVoucherRepository voucherRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _voucherRepository = voucherRepository;
    }

    public async Task<List<SystemVoucherCountDto>> GetSystemListAsync()
    {
        try
        {
            var results = await _repository.GetSystemVoucherCountsAsync();
            return results.Select(x => new SystemVoucherCountDto
            {
                SysId = x.SysId,
                SysName = x.SysName,
                ProgId = x.ProgId,
                UnreviewedCount = x.UnreviewedCount
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢前端系統列表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<TmpVoucherDto>> GetTmpVouchersAsync(TmpVoucherQueryDto query)
    {
        try
        {
            var repositoryQuery = new TmpVoucherQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                TypeId = query.TypeId,
                SysId = query.SysId,
                Status = query.Status,
                VoucherDateFrom = query.VoucherDateFrom,
                VoucherDateTo = query.VoucherDateTo,
                SlipType = query.SlipType,
                VendorId = query.VendorId,
                StoreId = query.StoreId,
                SiteId = query.SiteId
            };

            var result = await _repository.GetTmpVouchersPagedAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<TmpVoucherDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢暫存傳票列表失敗", ex);
            throw;
        }
    }

    public async Task<TmpVoucherDetailDto> GetTmpVoucherByIdAsync(long tKey)
    {
        try
        {
            var voucher = await _repository.GetTmpVoucherByIdAsync(tKey);
            if (voucher == null)
            {
                throw new InvalidOperationException($"暫存傳票不存在: {tKey}");
            }

            var details = await _repository.GetTmpVoucherDetailsAsync(tKey);

            var dto = MapToDetailDto(voucher);
            dto.Details = details.Select(x => MapToDetailItemDto(x)).ToList();

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢暫存傳票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<TmpVoucherDetailDto> UpdateTmpVoucherAsync(long tKey, UpdateTmpVoucherDto dto)
    {
        try
        {
            var voucher = await _repository.GetTmpVoucherByIdAsync(tKey);
            if (voucher == null)
            {
                throw new InvalidOperationException($"暫存傳票不存在: {tKey}");
            }

            if (voucher.Status != "1")
            {
                throw new InvalidOperationException("只有未審核的傳票可以修改");
            }

            // 更新主檔
            voucher.Notes = dto.Notes;
            voucher.UpdatedBy = GetCurrentUserId();
            voucher.UpdatedAt = DateTime.Now;

            await _repository.UpdateTmpVoucherAsync(voucher);

            // 刪除舊明細
            await _repository.DeleteTmpVoucherDetailsAsync(tKey);

            // 新增新明細
            foreach (var detailDto in dto.Details)
            {
                var detail = new TmpVoucherD
                {
                    VoucherTKey = tKey,
                    Sn = detailDto.Sn,
                    Dc = detailDto.Dc,
                    SubN = detailDto.SubN,
                    OrgId = detailDto.OrgId,
                    ActId = detailDto.ActId,
                    Notes = detailDto.Notes,
                    Val0 = detailDto.Val0,
                    Val1 = detailDto.Val1,
                    VendorId = detailDto.VendorId,
                    AbatId = detailDto.AbatId,
                    ObjectId = detailDto.ObjectId,
                    CreatedBy = GetCurrentUserId(),
                    CreatedAt = DateTime.Now
                };

                await _repository.CreateTmpVoucherDetailAsync(detail);
            }

            _logger.LogInfo($"修改暫存傳票成功: {tKey}");

            return await GetTmpVoucherByIdAsync(tKey);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改暫存傳票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteTmpVoucherAsync(long tKey)
    {
        try
        {
            var voucher = await _repository.GetTmpVoucherByIdAsync(tKey);
            if (voucher == null)
            {
                throw new InvalidOperationException($"暫存傳票不存在: {tKey}");
            }

            if (voucher.Status != "1")
            {
                throw new InvalidOperationException("只有未審核的傳票可以刪除");
            }

            await _repository.DeleteTmpVoucherAsync(tKey);

            _logger.LogInfo($"刪除暫存傳票成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除暫存傳票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<TransferVoucherResultDto> TransferTmpVoucherAsync(long tKey, TransferVoucherDto dto)
    {
        try
        {
            var tmpVoucher = await _repository.GetTmpVoucherByIdAsync(tKey);
            if (tmpVoucher == null)
            {
                throw new InvalidOperationException($"暫存傳票不存在: {tKey}");
            }

            if (tmpVoucher.Status == "3")
            {
                throw new InvalidOperationException("傳票已拋轉，無法再次拋轉");
            }

            var voucherDate = dto.VoucherDate ?? tmpVoucher.VoucherDate ?? DateTime.Now;

            // 驗證傳票日期
            if (dto.ValidateCloseDate && !await _repository.IsVoucherDateValidAsync(voucherDate))
            {
                throw new InvalidOperationException("傳票日期必須大於關帳年月");
            }

            // 檢查借貸平衡
            var details = await _repository.GetTmpVoucherDetailsAsync(tKey);
            var debitTotal = details.Sum(x => x.Val0);
            var creditTotal = details.Sum(x => x.Val1);

            if (debitTotal != creditTotal)
            {
                throw new InvalidOperationException("傳票借貸不平衡");
            }

            // 產生傳票編號
            var voucherId = await _repository.GenerateVoucherIdAsync(voucherDate);

            // 建立正式傳票
            var voucher = new Voucher
            {
                VoucherId = voucherId,
                VoucherTypeId = tmpVoucher.TypeId ?? string.Empty,
                Description = tmpVoucher.Notes ?? string.Empty,
                Status = "P", // 已過帳
                PostedBy = GetCurrentUserId(),
                PostedAt = DateTime.Now
            };

            var voucherTKey = await _voucherRepository.CreateVoucherAsync(voucher);

            // 建立傳票明細
            foreach (var detail in details)
            {
                var voucherDetail = new VoucherDetail
                {
                    VoucherTKey = voucherTKey,
                    AccountSubjectId = detail.SubN ?? string.Empty,
                    DepartmentId = detail.OrgId,
                    ProjectId = detail.ActId,
                    Description = detail.Notes,
                    DebitAmount = detail.Val0,
                    CreditAmount = detail.Val1
                };

                await _voucherRepository.CreateVoucherDetailAsync(voucherDetail);
            }

            // 更新暫存傳票狀態
            tmpVoucher.Status = "3";
            tmpVoucher.UpFlag = "1";
            tmpVoucher.UpdatedBy = GetCurrentUserId();
            tmpVoucher.UpdatedAt = DateTime.Now;

            await _repository.UpdateTmpVoucherAsync(tmpVoucher);

            _logger.LogInfo($"拋轉暫存傳票成功: {tKey} -> {voucherId}");

            return new TransferVoucherResultDto
            {
                TKey = tKey,
                VoucherId = voucherId,
                TransferDate = DateTime.Now
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"拋轉暫存傳票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<BatchTransferResultDto> BatchTransferTmpVouchersAsync(BatchTransferVoucherDto dto)
    {
        try
        {
            var result = new BatchTransferResultDto
            {
                TotalCount = dto.TKeys.Count,
                SuccessCount = 0,
                FailCount = 0,
                ErrorMessages = new List<string>()
            };

            foreach (var tKey in dto.TKeys)
            {
                try
                {
                    var transferDto = new TransferVoucherDto
                    {
                        ValidateCloseDate = dto.ValidateCloseDate
                    };

                    await TransferTmpVoucherAsync(tKey, transferDto);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.FailCount++;
                    result.ErrorMessages.Add($"傳票 {tKey}: {ex.Message}");
                }
            }

            _logger.LogInfo($"批次拋轉暫存傳票完成: 成功 {result.SuccessCount}, 失敗 {result.FailCount}");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("批次拋轉暫存傳票失敗", ex);
            throw;
        }
    }

    public async Task<UnreviewedCountDto> GetUnreviewedCountAsync(string? typeId = null, string? sysId = null)
    {
        try
        {
            var totalCount = await _repository.GetUnreviewedCountAsync(typeId, sysId);
            var bySystem = await _repository.GetSystemVoucherCountsAsync();

            return new UnreviewedCountDto
            {
                TotalCount = totalCount,
                BySystem = bySystem.Select(x => new SystemVoucherCountDto
                {
                    SysId = x.SysId,
                    SysName = x.SysName,
                    ProgId = x.ProgId,
                    UnreviewedCount = x.UnreviewedCount
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢未審核筆數失敗", ex);
            throw;
        }
    }

    #region 私有方法

    private TmpVoucherDto MapToDto(TmpVoucherM entity)
    {
        return new TmpVoucherDto
        {
            TKey = entity.TKey,
            VoucherId = entity.VoucherId,
            VoucherDate = entity.VoucherDate,
            TypeId = entity.TypeId,
            SysId = entity.SysId,
            Status = entity.Status,
            UpFlag = entity.UpFlag,
            Notes = entity.Notes,
            VendorId = entity.VendorId,
            StoreId = entity.StoreId,
            SiteId = entity.SiteId,
            SlipType = entity.SlipType,
            SlipNo = entity.SlipNo
        };
    }

    private TmpVoucherDetailDto MapToDetailDto(TmpVoucherM entity)
    {
        return new TmpVoucherDetailDto
        {
            TKey = entity.TKey,
            VoucherId = entity.VoucherId,
            VoucherDate = entity.VoucherDate,
            TypeId = entity.TypeId,
            SysId = entity.SysId,
            Status = entity.Status,
            UpFlag = entity.UpFlag,
            Notes = entity.Notes,
            VendorId = entity.VendorId,
            StoreId = entity.StoreId,
            SiteId = entity.SiteId,
            SlipType = entity.SlipType,
            SlipNo = entity.SlipNo,
            Details = new List<TmpVoucherDetailItemDto>()
        };
    }

    private TmpVoucherDetailItemDto MapToDetailItemDto(TmpVoucherD entity)
    {
        return new TmpVoucherDetailItemDto
        {
            TKey = entity.TKey,
            Sn = entity.Sn,
            Dc = entity.Dc,
            SubN = entity.SubN,
            OrgId = entity.OrgId,
            ActId = entity.ActId,
            Notes = entity.Notes,
            Val0 = entity.Val0,
            Val1 = entity.Val1,
            VendorId = entity.VendorId,
            AbatId = entity.AbatId,
            ObjectId = entity.ObjectId
        };
    }

    #endregion
}

