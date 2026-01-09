using ErpCore.Application.DTOs.InvoiceSales;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.InvoiceSales;
using ErpCore.Infrastructure.Repositories.InvoiceSales;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.InvoiceSales;

/// <summary>
/// 發票服務實作 (SYSG110-SYSG190 - 發票資料維護)
/// </summary>
public class InvoiceService : BaseService, IInvoiceService
{
    private readonly IInvoiceRepository _repository;

    public InvoiceService(
        IInvoiceRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<InvoiceDto>> GetInvoicesAsync(InvoiceQueryDto query)
    {
        try
        {
            var repositoryQuery = new InvoiceQuery
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
                Status = query.Status
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(MapToDto).ToList();

            return new PagedResult<InvoiceDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢發票列表失敗", ex);
            throw;
        }
    }

    public async Task<InvoiceDto> GetInvoiceByIdAsync(long tKey)
    {
        try
        {
            var invoice = await _repository.GetByIdAsync(tKey);
            if (invoice == null)
            {
                throw new KeyNotFoundException($"發票不存在: {tKey}");
            }

            return MapToDto(invoice);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢發票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<long> CreateInvoiceAsync(CreateInvoiceDto dto)
    {
        try
        {
            // 驗證發票編號唯一性
            var exists = await _repository.ExistsByInvoiceIdAsync(dto.InvoiceId);
            if (exists)
            {
                throw new InvalidOperationException($"發票編號已存在: {dto.InvoiceId}");
            }

            // 驗證發票年月格式
            var invoiceYm = $"{dto.InvoiceYear:0000}{dto.InvoiceMonth:00}";
            if (invoiceYm.Length != 6)
            {
                throw new ArgumentException("發票年月格式錯誤");
            }

            // 驗證發票號碼區間
            if (!string.IsNullOrEmpty(dto.InvoiceNoB) && !string.IsNullOrEmpty(dto.InvoiceNoE))
            {
                if (string.Compare(dto.InvoiceNoB, dto.InvoiceNoE) > 0)
                {
                    throw new ArgumentException("發票號碼起必須小於或等於發票號碼迄");
                }
            }

            // 驗證統一編號格式（8位數字）
            if (!string.IsNullOrEmpty(dto.TaxId) && (dto.TaxId.Length != 8 || !dto.TaxId.All(char.IsDigit)))
            {
                throw new ArgumentException("統一編號必須為8位數字");
            }

            var invoice = new Invoice
            {
                InvoiceId = dto.InvoiceId,
                InvoiceType = dto.InvoiceType,
                InvoiceYear = dto.InvoiceYear,
                InvoiceMonth = dto.InvoiceMonth,
                InvoiceYm = invoiceYm,
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
                Status = dto.Status,
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var tKey = await _repository.CreateAsync(invoice);
            _logger.LogInfo($"新增發票成功: {dto.InvoiceId} (TKey: {tKey})");
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增發票失敗: {dto.InvoiceId}", ex);
            throw;
        }
    }

    public async Task UpdateInvoiceAsync(UpdateInvoiceDto dto)
    {
        try
        {
            var invoice = await _repository.GetByIdAsync(dto.TKey);
            if (invoice == null)
            {
                throw new KeyNotFoundException($"發票不存在: {dto.TKey}");
            }

            // 驗證發票編號唯一性（排除自己）
            var exists = await _repository.ExistsByInvoiceIdAsync(dto.InvoiceId, dto.TKey);
            if (exists)
            {
                throw new InvalidOperationException($"發票編號已存在: {dto.InvoiceId}");
            }

            // 驗證發票年月格式
            var invoiceYm = $"{dto.InvoiceYear:0000}{dto.InvoiceMonth:00}";
            if (invoiceYm.Length != 6)
            {
                throw new ArgumentException("發票年月格式錯誤");
            }

            // 驗證發票號碼區間
            if (!string.IsNullOrEmpty(dto.InvoiceNoB) && !string.IsNullOrEmpty(dto.InvoiceNoE))
            {
                if (string.Compare(dto.InvoiceNoB, dto.InvoiceNoE) > 0)
                {
                    throw new ArgumentException("發票號碼起必須小於或等於發票號碼迄");
                }
            }

            // 驗證統一編號格式
            if (!string.IsNullOrEmpty(dto.TaxId) && (dto.TaxId.Length != 8 || !dto.TaxId.All(char.IsDigit)))
            {
                throw new ArgumentException("統一編號必須為8位數字");
            }

            invoice.InvoiceId = dto.InvoiceId;
            invoice.InvoiceType = dto.InvoiceType;
            invoice.InvoiceYear = dto.InvoiceYear;
            invoice.InvoiceMonth = dto.InvoiceMonth;
            invoice.InvoiceYm = invoiceYm;
            invoice.Track = dto.Track;
            invoice.InvoiceNoB = dto.InvoiceNoB;
            invoice.InvoiceNoE = dto.InvoiceNoE;
            invoice.InvoiceFormat = dto.InvoiceFormat;
            invoice.TaxId = dto.TaxId;
            invoice.CompanyName = dto.CompanyName;
            invoice.CompanyNameEn = dto.CompanyNameEn;
            invoice.Address = dto.Address;
            invoice.City = dto.City;
            invoice.Zone = dto.Zone;
            invoice.PostalCode = dto.PostalCode;
            invoice.Phone = dto.Phone;
            invoice.Fax = dto.Fax;
            invoice.Email = dto.Email;
            invoice.SiteId = dto.SiteId;
            invoice.SubCopy = dto.SubCopy;
            invoice.SubCopyValue = dto.SubCopyValue;
            invoice.Status = dto.Status;
            invoice.Notes = dto.Notes;
            invoice.UpdatedBy = GetCurrentUserId();
            invoice.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(invoice);
            _logger.LogInfo($"修改發票成功: {dto.InvoiceId} (TKey: {dto.TKey})");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改發票失敗: {dto.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteInvoiceAsync(long tKey)
    {
        try
        {
            var invoice = await _repository.GetByIdAsync(tKey);
            if (invoice == null)
            {
                throw new KeyNotFoundException($"發票不存在: {tKey}");
            }

            // TODO: 檢查是否有關聯的發票使用記錄
            // 如果有，則不允許刪除

            await _repository.DeleteAsync(tKey);
            _logger.LogInfo($"刪除發票成功: {invoice.InvoiceId} (TKey: {tKey})");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除發票失敗: {tKey}", ex);
            throw;
        }
    }

    /// <summary>
    /// 將 Entity 轉換為 DTO
    /// </summary>
    private InvoiceDto MapToDto(Invoice invoice)
    {
        return new InvoiceDto
        {
            TKey = invoice.TKey,
            InvoiceId = invoice.InvoiceId,
            InvoiceType = invoice.InvoiceType,
            InvoiceYear = invoice.InvoiceYear,
            InvoiceMonth = invoice.InvoiceMonth,
            InvoiceYm = invoice.InvoiceYm,
            Track = invoice.Track,
            InvoiceNoB = invoice.InvoiceNoB,
            InvoiceNoE = invoice.InvoiceNoE,
            InvoiceFormat = invoice.InvoiceFormat,
            TaxId = invoice.TaxId,
            CompanyName = invoice.CompanyName,
            CompanyNameEn = invoice.CompanyNameEn,
            Address = invoice.Address,
            City = invoice.City,
            Zone = invoice.Zone,
            PostalCode = invoice.PostalCode,
            Phone = invoice.Phone,
            Fax = invoice.Fax,
            Email = invoice.Email,
            SiteId = invoice.SiteId,
            SubCopy = invoice.SubCopy,
            SubCopyValue = invoice.SubCopyValue,
            Status = invoice.Status,
            Notes = invoice.Notes,
            CreatedBy = invoice.CreatedBy,
            CreatedAt = invoice.CreatedAt,
            UpdatedBy = invoice.UpdatedBy,
            UpdatedAt = invoice.UpdatedAt
        };
    }
}

