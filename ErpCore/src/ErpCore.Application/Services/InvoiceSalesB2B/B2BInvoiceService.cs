using ErpCore.Application.DTOs.InvoiceSalesB2B;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.InvoiceSalesB2B;
using ErpCore.Infrastructure.Repositories.InvoiceSalesB2B;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.InvoiceSalesB2B;

/// <summary>
/// B2B發票服務實作 (SYSG000_B2B - B2B發票資料維護)
/// </summary>
public class B2BInvoiceService : BaseService, IB2BInvoiceService
{
    private readonly IB2BInvoiceRepository _repository;

    public B2BInvoiceService(
        IB2BInvoiceRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<B2BInvoiceDto>> GetB2BInvoicesAsync(B2BInvoiceQueryDto query)
    {
        try
        {
            var repositoryQuery = new B2BInvoiceQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                InvoiceId = query.InvoiceId,
                InvoiceType = query.InvoiceType,
                InvoiceYm = query.InvoiceYm,
                TaxId = query.TaxId,
                SiteId = query.SiteId,
                Status = query.Status,
                B2BFlag = query.B2BFlag ?? "Y"
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(MapToDto).ToList();

            return new PagedResult<B2BInvoiceDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢B2B發票列表失敗", ex);
            throw;
        }
    }

    public async Task<B2BInvoiceDto> GetB2BInvoiceByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new Exception($"B2B發票不存在: {tKey}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢B2B發票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<long> CreateB2BInvoiceAsync(CreateB2BInvoiceDto dto)
    {
        try
        {
            // 檢查發票編號是否已存在
            var exists = await _repository.ExistsByInvoiceIdAsync(dto.InvoiceId);
            if (exists)
            {
                throw new Exception($"B2B發票編號已存在: {dto.InvoiceId}");
            }

            var entity = new B2BInvoice
            {
                InvoiceId = dto.InvoiceId,
                InvoiceType = dto.InvoiceType,
                InvoiceYear = dto.InvoiceYear,
                InvoiceMonth = dto.InvoiceMonth,
                InvoiceYm = dto.InvoiceYm,
                Track = dto.Track,
                InvoiceNoB = dto.InvoiceNoB,
                InvoiceNoE = dto.InvoiceNoE,
                InvoiceFormat = dto.InvoiceFormat,
                TaxId = dto.TaxId,
                CompanyName = dto.CompanyName,
                CompanyNameEn = dto.CompanyNameEn,
                Address = dto.Address,
                City = dto.City,
                Zone = dto.Zone,
                PostalCode = dto.PostalCode,
                Phone = dto.Phone,
                Fax = dto.Fax,
                Email = dto.Email,
                SiteId = dto.SiteId,
                SubCopy = dto.SubCopy,
                SubCopyValue = dto.SubCopyValue,
                B2BFlag = "Y",
                Status = dto.Status,
                Notes = dto.Notes,
                CreatedBy = _userContext.UserId,
                CreatedAt = DateTime.Now,
                UpdatedBy = _userContext.UserId,
                UpdatedAt = DateTime.Now
            };

            var tKey = await _repository.CreateAsync(entity);
            _logger.LogInfo($"新增B2B發票成功: {dto.InvoiceId}");
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增B2B發票失敗: {dto.InvoiceId}", ex);
            throw;
        }
    }

    public async Task UpdateB2BInvoiceAsync(UpdateB2BInvoiceDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(dto.TKey);
            if (entity == null)
            {
                throw new Exception($"B2B發票不存在: {dto.TKey}");
            }

            // 檢查發票編號是否已被其他記錄使用
            var exists = await _repository.ExistsByInvoiceIdAsync(dto.InvoiceId, dto.TKey);
            if (exists)
            {
                throw new Exception($"B2B發票編號已被使用: {dto.InvoiceId}");
            }

            entity.InvoiceId = dto.InvoiceId;
            entity.InvoiceType = dto.InvoiceType;
            entity.InvoiceYear = dto.InvoiceYear;
            entity.InvoiceMonth = dto.InvoiceMonth;
            entity.InvoiceYm = dto.InvoiceYm;
            entity.Track = dto.Track;
            entity.InvoiceNoB = dto.InvoiceNoB;
            entity.InvoiceNoE = dto.InvoiceNoE;
            entity.InvoiceFormat = dto.InvoiceFormat;
            entity.TaxId = dto.TaxId;
            entity.CompanyName = dto.CompanyName;
            entity.CompanyNameEn = dto.CompanyNameEn;
            entity.Address = dto.Address;
            entity.City = dto.City;
            entity.Zone = dto.Zone;
            entity.PostalCode = dto.PostalCode;
            entity.Phone = dto.Phone;
            entity.Fax = dto.Fax;
            entity.Email = dto.Email;
            entity.SiteId = dto.SiteId;
            entity.SubCopy = dto.SubCopy;
            entity.SubCopyValue = dto.SubCopyValue;
            entity.Status = dto.Status;
            entity.Notes = dto.Notes;
            entity.UpdatedBy = _userContext.UserId;
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"修改B2B發票成功: {dto.InvoiceId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改B2B發票失敗: {dto.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteB2BInvoiceAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new Exception($"B2B發票不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
            _logger.LogInfo($"刪除B2B發票成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除B2B發票失敗: {tKey}", ex);
            throw;
        }
    }

    private B2BInvoiceDto MapToDto(B2BInvoice entity)
    {
        return new B2BInvoiceDto
        {
            TKey = entity.TKey,
            InvoiceId = entity.InvoiceId,
            InvoiceType = entity.InvoiceType,
            InvoiceYear = entity.InvoiceYear,
            InvoiceMonth = entity.InvoiceMonth,
            InvoiceYm = entity.InvoiceYm,
            Track = entity.Track,
            InvoiceNoB = entity.InvoiceNoB,
            InvoiceNoE = entity.InvoiceNoE,
            InvoiceFormat = entity.InvoiceFormat,
            TaxId = entity.TaxId,
            CompanyName = entity.CompanyName,
            CompanyNameEn = entity.CompanyNameEn,
            Address = entity.Address,
            City = entity.City,
            Zone = entity.Zone,
            PostalCode = entity.PostalCode,
            Phone = entity.Phone,
            Fax = entity.Fax,
            Email = entity.Email,
            SiteId = entity.SiteId,
            SubCopy = entity.SubCopy,
            SubCopyValue = entity.SubCopyValue,
            B2BFlag = entity.B2BFlag,
            Status = entity.Status,
            Notes = entity.Notes,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

