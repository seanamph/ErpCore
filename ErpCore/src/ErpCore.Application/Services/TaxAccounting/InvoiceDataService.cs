using ErpCore.Application.DTOs.TaxAccounting;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Accounting;
using ErpCore.Domain.Entities.TaxAccounting;
using ErpCore.Infrastructure.Repositories.TaxAccounting;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using VoucherDetail = ErpCore.Domain.Entities.Accounting.VoucherDetail;

namespace ErpCore.Application.Services.TaxAccounting;

/// <summary>
/// 發票資料維護服務實作 (SYST211-SYST212)
/// </summary>
public class InvoiceDataService : BaseService, IInvoiceDataService
{
    private readonly IInvoiceDataRepository _repository;

    public InvoiceDataService(
        IInvoiceDataRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<InvoiceVoucherDto>> GetVouchersAsync(InvoiceVoucherQueryDto query)
    {
        try
        {
            var repositoryQuery = new InvoiceVoucherQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                VoucherId = query.VoucherId,
                VoucherDateFrom = query.VoucherDateFrom,
                VoucherDateTo = query.VoucherDateTo,
                VoucherStatus = query.VoucherStatus,
                VoucherKind = query.VoucherKind,
                TypeId = query.TypeId,
                SiteId = query.SiteId,
                VendorId = query.VendorId
            };

            var result = await _repository.QueryVouchersAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            // 載入發票傳票資料
            foreach (var dto in dtos)
            {
                var voucher = await _repository.GetVoucherByIdAsync(dto.VoucherId);
                if (voucher != null)
                {
                    var invoiceVouchers = await _repository.GetInvoiceVouchersByVoucherTKeyAsync(voucher.TKey);
                    dto.InvoiceVouchers = invoiceVouchers.Select(x => MapInvoiceVoucherToDto(x)).ToList();
                }
            }

            return new PagedResult<InvoiceVoucherDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢傳票列表失敗", ex);
            throw;
        }
    }

    public async Task<InvoiceVoucherDto?> GetVoucherByIdAsync(string voucherId)
    {
        try
        {
            var entity = await _repository.GetVoucherByIdAsync(voucherId);
            if (entity == null)
            {
                return null;
            }

            var dto = MapToDto(entity);

            // 載入發票傳票資料
            var invoiceVouchers = await _repository.GetInvoiceVouchersByVoucherTKeyAsync(entity.TKey);
            dto.InvoiceVouchers = invoiceVouchers.Select(x => MapInvoiceVoucherToDto(x)).ToList();

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢傳票失敗: {voucherId}", ex);
            throw;
        }
    }

    public async Task<string> CreateVoucherAsync(CreateInvoiceVoucherDto dto)
    {
        try
        {
            // 驗證借貸平衡
            if (dto.Details != null && dto.Details.Count > 0)
            {
                var totalDebit = dto.Details.Where(x => x.Dc == "D").Sum(x => x.Amount);
                var totalCredit = dto.Details.Where(x => x.Dc == "C").Sum(x => x.Amount);
                if (Math.Abs(totalDebit - totalCredit) > 0.01m)
                {
                    throw new InvalidOperationException($"借貸金額不平衡，借方總額: {totalDebit}, 貸方總額: {totalCredit}");
                }
            }

            // 產生傳票編號
            var voucherId = await GenerateVoucherIdAsync(dto.VoucherDate);

            var entity = new Voucher
            {
                VoucherId = voucherId,
                VoucherDate = dto.VoucherDate,
                VoucherTypeId = dto.TypeId,
                Description = dto.Notes,
                Status = "D",
                VoucherKind = dto.VoucherKind,
                VoucherStatus = "1",
                InvYn = dto.InvYn ?? "N",
                SiteId = dto.SiteId,
                VendorId = dto.VendorId,
                VendorName = dto.VendorName,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var created = await _repository.CreateVoucherAsync(entity);

            // 新增明細
            if (dto.Details != null && dto.Details.Count > 0)
            {
                foreach (var detailDto in dto.Details)
                {
                    var detail = new VoucherDetail
                    {
                        VoucherId = voucherId,
                        SeqNo = detailDto.SeqNo,
                        StypeId = detailDto.StypeId,
                        Dc = detailDto.Dc,
                        Amount = detailDto.Amount,
                        Description = detailDto.Description,
                        CreatedBy = GetCurrentUserId(),
                        CreatedAt = DateTime.Now,
                        UpdatedBy = GetCurrentUserId(),
                        UpdatedAt = DateTime.Now
                    };
                    await _repository.CreateVoucherDetailAsync(detail);
                }
            }

            // 新增發票傳票
            if (dto.InvoiceVouchers != null && dto.InvoiceVouchers.Count > 0)
            {
                foreach (var invoiceDto in dto.InvoiceVouchers)
                {
                    var invoiceVoucher = new InvoiceVoucher
                    {
                        VoucherTKey = created.TKey,
                        InvoiceType = invoiceDto.InvoiceType,
                        InvoiceNo = invoiceDto.InvoiceNo,
                        InvoiceDate = invoiceDto.InvoiceDate,
                        InvoiceFormat = invoiceDto.InvoiceFormat,
                        InvoiceAmount = invoiceDto.InvoiceAmount,
                        TaxAmount = invoiceDto.TaxAmount,
                        DeductCode = invoiceDto.DeductCode,
                        CategoryType = invoiceDto.CategoryType,
                        VoucherNo = invoiceDto.VoucherNo,
                        CreatedBy = GetCurrentUserId(),
                        CreatedAt = DateTime.Now,
                        UpdatedBy = GetCurrentUserId(),
                        UpdatedAt = DateTime.Now
                    };
                    await _repository.CreateInvoiceVoucherAsync(invoiceVoucher);
                }
            }

            _logger.LogInfo($"新增傳票成功: {voucherId}");

            return voucherId;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增傳票失敗", ex);
            throw;
        }
    }

    public async Task UpdateVoucherAsync(string voucherId, UpdateInvoiceVoucherDto dto)
    {
        try
        {
            var entity = await _repository.GetVoucherByIdAsync(voucherId);
            if (entity == null)
            {
                throw new InvalidOperationException($"傳票不存在: {voucherId}");
            }

            // 驗證借貸平衡
            if (dto.Details != null && dto.Details.Count > 0)
            {
                var totalDebit = dto.Details.Where(x => x.Dc == "D").Sum(x => x.Amount);
                var totalCredit = dto.Details.Where(x => x.Dc == "C").Sum(x => x.Amount);
                if (Math.Abs(totalDebit - totalCredit) > 0.01m)
                {
                    throw new InvalidOperationException($"借貸金額不平衡，借方總額: {totalDebit}, 貸方總額: {totalCredit}");
                }
            }

            entity.VoucherDate = dto.VoucherDate;
            entity.VoucherTypeId = dto.TypeId;
            entity.Description = dto.Notes;
            entity.VoucherKind = dto.VoucherKind;
            entity.InvYn = dto.InvYn ?? "N";
            entity.SiteId = dto.SiteId;
            entity.VendorId = dto.VendorId;
            entity.VendorName = dto.VendorName;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateVoucherAsync(entity);

            _logger.LogInfo($"修改傳票成功: {voucherId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改傳票失敗: {voucherId}", ex);
            throw;
        }
    }

    public async Task DeleteVoucherAsync(string voucherId)
    {
        try
        {
            var entity = await _repository.GetVoucherByIdAsync(voucherId);
            if (entity == null)
            {
                throw new InvalidOperationException($"傳票不存在: {voucherId}");
            }

            // 檢查傳票狀態
            if (entity.VoucherStatus == "3") // 已結帳
            {
                throw new InvalidOperationException("已結帳的傳票不可刪除");
            }

            await _repository.DeleteVoucherAsync(voucherId);

            _logger.LogInfo($"刪除傳票成功: {voucherId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除傳票失敗: {voucherId}", ex);
            throw;
        }
    }

    public async Task VoidVoucherAsync(string voucherId)
    {
        try
        {
            var entity = await _repository.GetVoucherByIdAsync(voucherId);
            if (entity == null)
            {
                throw new InvalidOperationException($"傳票不存在: {voucherId}");
            }

            entity.VoucherStatus = "2"; // 作廢
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateVoucherAsync(entity);

            _logger.LogInfo($"作廢傳票成功: {voucherId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"作廢傳票失敗: {voucherId}", ex);
            throw;
        }
    }

    public async Task<BalanceCheckDto> CheckBalanceAsync(string voucherId)
    {
        try
        {
            var result = await _repository.CheckBalanceAsync(voucherId);

            return new BalanceCheckDto
            {
                IsBalanced = result.IsBalanced,
                DebitAmount = result.DebitAmount,
                CreditAmount = result.CreditAmount,
                Difference = result.Difference
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查傳票借貸平衡失敗: {voucherId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<AllocationRatioDto>> GetAllocationRatiosAsync(AllocationRatioQueryDto query)
    {
        try
        {
            var repositoryQuery = new AllocationRatioQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                DisYm = query.DisYm,
                StypeId = query.StypeId,
                OrgId = query.OrgId
            };

            var result = await _repository.QueryAllocationRatiosAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapAllocationRatioToDto(x)).ToList();

            return new PagedResult<AllocationRatioDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢分攤比率列表失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateAllocationRatioAsync(CreateAllocationRatioDto dto)
    {
        try
        {
            var entity = new AllocationRatio
            {
                DisYm = dto.DisYm,
                StypeId = dto.StypeId,
                OrgId = dto.OrgId,
                Ratio = dto.Ratio,
                VoucherTKey = dto.VoucherTKey,
                VoucherDTKey = dto.VoucherDTKey,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var created = await _repository.CreateAllocationRatioAsync(entity);

            _logger.LogInfo($"新增分攤比率成功: {created.TKey}");

            return created.TKey;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增分攤比率失敗", ex);
            throw;
        }
    }

    public async Task UpdateAllocationRatioAsync(long tKey, UpdateAllocationRatioDto dto)
    {
        try
        {
            // 查詢現有分攤比率
            var ratios = await _repository.QueryAllocationRatiosAsync(new AllocationRatioQuery
            {
                PageIndex = 1,
                PageSize = 1
            });
            
            var existingRatio = ratios.Items.FirstOrDefault(x => x.TKey == tKey);
            if (existingRatio == null)
            {
                throw new InvalidOperationException($"分攤比率不存在: {tKey}");
            }

            // 更新分攤比率
            existingRatio.DisYm = dto.DisYm ?? existingRatio.DisYm;
            existingRatio.StypeId = dto.StypeId ?? existingRatio.StypeId;
            existingRatio.OrgId = dto.OrgId ?? existingRatio.OrgId;
            existingRatio.Ratio = dto.Ratio ?? existingRatio.Ratio;
            existingRatio.VoucherTKey = dto.VoucherTKey ?? existingRatio.VoucherTKey;
            existingRatio.VoucherDTKey = dto.VoucherDTKey ?? existingRatio.VoucherDTKey;
            existingRatio.UpdatedBy = GetCurrentUserId();
            existingRatio.UpdatedAt = DateTime.Now;

            await _repository.UpdateAllocationRatioAsync(existingRatio);

            _logger.LogInfo($"修改分攤比率成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改分攤比率失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteAllocationRatioAsync(long tKey)
    {
        try
        {
            await _repository.DeleteAllocationRatioAsync(tKey);

            _logger.LogInfo($"刪除分攤比率成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除分攤比率失敗: {tKey}", ex);
            throw;
        }
    }

    #region 私有方法

    private InvoiceVoucherDto MapToDto(Voucher entity)
    {
        return new InvoiceVoucherDto
        {
            TKey = entity.TKey,
            VoucherId = entity.VoucherId,
            VoucherDate = entity.VoucherDate,
            VoucherKind = entity.VoucherKind,
            VoucherStatus = entity.VoucherStatus,
            Notes = entity.Description,
            InvYn = entity.InvYn,
            TypeId = entity.VoucherTypeId,
            SiteId = entity.SiteId,
            VendorId = entity.VendorId,
            VendorName = entity.VendorName,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private InvoiceVoucherDetailDto MapInvoiceVoucherToDto(InvoiceVoucher entity)
    {
        return new InvoiceVoucherDetailDto
        {
            TKey = entity.TKey,
            VoucherTKey = entity.VoucherTKey,
            InvoiceType = entity.InvoiceType,
            InvoiceNo = entity.InvoiceNo,
            InvoiceDate = entity.InvoiceDate,
            InvoiceFormat = entity.InvoiceFormat,
            InvoiceAmount = entity.InvoiceAmount,
            TaxAmount = entity.TaxAmount,
            DeductCode = entity.DeductCode,
            CategoryType = entity.CategoryType,
            VoucherNo = entity.VoucherNo
        };
    }

    private AllocationRatioDto MapAllocationRatioToDto(AllocationRatio entity)
    {
        return new AllocationRatioDto
        {
            TKey = entity.TKey,
            DisYm = entity.DisYm,
            StypeId = entity.StypeId,
            OrgId = entity.OrgId,
            Ratio = entity.Ratio,
            VoucherTKey = entity.VoucherTKey,
            VoucherDTKey = entity.VoucherDTKey,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private async Task<string> GenerateVoucherIdAsync(DateTime voucherDate)
    {
        // 產生傳票編號：V + YYYYMMDD + 序號
        var prefix = $"V{voucherDate:yyyyMMdd}";
        var maxVoucherId = await _repository.QueryVouchersAsync(new InvoiceVoucherQuery
        {
            PageIndex = 1,
            PageSize = 1,
            VoucherId = prefix
        });

        int sequence = 1;
        if (maxVoucherId.Items.Count > 0)
        {
            var lastVoucher = maxVoucherId.Items.OrderByDescending(x => x.VoucherId).First();
            if (lastVoucher.VoucherId.StartsWith(prefix))
            {
                var lastSequence = lastVoucher.VoucherId.Substring(prefix.Length);
                if (int.TryParse(lastSequence, out int lastSeq))
                {
                    sequence = lastSeq + 1;
                }
            }
        }

        return $"{prefix}{sequence:D3}";
    }

    #endregion
}

