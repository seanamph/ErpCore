using ErpCore.Application.DTOs.Procurement;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Procurement;
using ErpCore.Infrastructure.Repositories.Procurement;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Procurement;

/// <summary>
/// 供應商服務實作 (SYSP210-SYSP260)
/// </summary>
public class SupplierService : BaseService, ISupplierService
{
    private readonly ISupplierRepository _repository;

    public SupplierService(
        ISupplierRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<SupplierDto>> GetSuppliersAsync(SupplierQueryDto query)
    {
        try
        {
            var repositoryQuery = new SupplierQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SupplierId = query.SupplierId,
                SupplierName = query.SupplierName,
                Status = query.Status,
                Rating = query.Rating
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<SupplierDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢供應商列表失敗", ex);
            throw;
        }
    }

    public async Task<SupplierDto> GetSupplierByIdAsync(string supplierId)
    {
        try
        {
            var supplier = await _repository.GetByIdAsync(supplierId);
            if (supplier == null)
            {
                throw new InvalidOperationException($"供應商不存在: {supplierId}");
            }

            return MapToDto(supplier);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢供應商失敗: {supplierId}", ex);
            throw;
        }
    }

    public async Task<string> CreateSupplierAsync(CreateSupplierDto dto)
    {
        try
        {
            // 檢查供應商編號是否已存在
            if (await _repository.ExistsAsync(dto.SupplierId))
            {
                throw new InvalidOperationException($"供應商編號已存在: {dto.SupplierId}");
            }

            var supplier = new Supplier
            {
                SupplierId = dto.SupplierId,
                SupplierName = dto.SupplierName,
                SupplierNameE = dto.SupplierNameE,
                ContactPerson = dto.ContactPerson,
                Phone = dto.Phone,
                Fax = dto.Fax,
                Email = dto.Email,
                Address = dto.Address,
                PaymentTerms = dto.PaymentTerms,
                TaxId = dto.TaxId,
                Status = dto.Status,
                Rating = dto.Rating,
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(supplier);

            _logger.LogInfo($"新增供應商成功: {dto.SupplierId}");
            return supplier.SupplierId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增供應商失敗: {dto.SupplierId}", ex);
            throw;
        }
    }

    public async Task UpdateSupplierAsync(string supplierId, UpdateSupplierDto dto)
    {
        try
        {
            var supplier = await _repository.GetByIdAsync(supplierId);
            if (supplier == null)
            {
                throw new InvalidOperationException($"供應商不存在: {supplierId}");
            }

            supplier.SupplierName = dto.SupplierName;
            supplier.SupplierNameE = dto.SupplierNameE;
            supplier.ContactPerson = dto.ContactPerson;
            supplier.Phone = dto.Phone;
            supplier.Fax = dto.Fax;
            supplier.Email = dto.Email;
            supplier.Address = dto.Address;
            supplier.PaymentTerms = dto.PaymentTerms;
            supplier.TaxId = dto.TaxId;
            supplier.Status = dto.Status;
            supplier.Rating = dto.Rating;
            supplier.Notes = dto.Notes;
            supplier.UpdatedBy = GetCurrentUserId();
            supplier.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(supplier);

            _logger.LogInfo($"修改供應商成功: {supplierId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改供應商失敗: {supplierId}", ex);
            throw;
        }
    }

    public async Task DeleteSupplierAsync(string supplierId)
    {
        try
        {
            var supplier = await _repository.GetByIdAsync(supplierId);
            if (supplier == null)
            {
                throw new InvalidOperationException($"供應商不存在: {supplierId}");
            }

            await _repository.DeleteAsync(supplierId);

            _logger.LogInfo($"刪除供應商成功: {supplierId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除供應商失敗: {supplierId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string supplierId)
    {
        try
        {
            return await _repository.ExistsAsync(supplierId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查供應商是否存在失敗: {supplierId}", ex);
            throw;
        }
    }

    private SupplierDto MapToDto(Supplier supplier)
    {
        return new SupplierDto
        {
            TKey = supplier.TKey,
            SupplierId = supplier.SupplierId,
            SupplierName = supplier.SupplierName,
            SupplierNameE = supplier.SupplierNameE,
            ContactPerson = supplier.ContactPerson,
            Phone = supplier.Phone,
            Fax = supplier.Fax,
            Email = supplier.Email,
            Address = supplier.Address,
            PaymentTerms = supplier.PaymentTerms,
            TaxId = supplier.TaxId,
            Status = supplier.Status,
            Rating = supplier.Rating,
            Notes = supplier.Notes,
            CreatedBy = supplier.CreatedBy,
            CreatedAt = supplier.CreatedAt,
            UpdatedBy = supplier.UpdatedBy,
            UpdatedAt = supplier.UpdatedAt
        };
    }
}

