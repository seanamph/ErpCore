using ErpCore.Application.DTOs.CustomerCustomJgjn;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.CustomerCustomJgjn;
using ErpCore.Infrastructure.Repositories.CustomerCustomJgjn;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.CustomerCustomJgjn;

/// <summary>
/// JGJN客戶服務實作
/// </summary>
public class JgjNCustomerService : BaseService, IJgjNCustomerService
{
    private readonly IJgjNCustomerRepository _repository;

    public JgjNCustomerService(
        IJgjNCustomerRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<JgjNCustomerDto>> GetJgjNCustomerListAsync(JgjNCustomerQueryDto query)
    {
        try
        {
            var repositoryQuery = new JgjNCustomerQuery
            {
                CustomerType = query.CustomerType,
                Status = query.Status,
                Keyword = query.Keyword,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(MapToDto).ToList();

            return new PagedResult<JgjNCustomerDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢JGJN客戶列表失敗", ex);
            throw;
        }
    }

    public async Task<JgjNCustomerDto?> GetJgjNCustomerByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢JGJN客戶失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<JgjNCustomerDto?> GetJgjNCustomerByCustomerIdAsync(string customerId)
    {
        try
        {
            var entity = await _repository.GetByCustomerIdAsync(customerId);
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢JGJN客戶失敗: {customerId}", ex);
            throw;
        }
    }

    public async Task<long> CreateJgjNCustomerAsync(CreateJgjNCustomerDto dto)
    {
        try
        {
            // 檢查客戶代碼是否已存在
            var existing = await _repository.GetByCustomerIdAsync(dto.CustomerId);
            if (existing != null)
            {
                throw new InvalidOperationException($"客戶代碼已存在: {dto.CustomerId}");
            }

            var entity = new JgjNCustomer
            {
                CustomerId = dto.CustomerId,
                CustomerName = dto.CustomerName,
                CustomerType = dto.CustomerType,
                ContactPerson = dto.ContactPerson,
                Phone = dto.Phone,
                Email = dto.Email,
                Address = dto.Address,
                Status = dto.Status,
                Memo = dto.Memo,
                CreatedBy = _userContext.UserId,
                CreatedAt = DateTime.Now,
                UpdatedBy = _userContext.UserId,
                UpdatedAt = DateTime.Now
            };

            var tKey = await _repository.CreateAsync(entity);
            _logger.LogInfo($"新增JGJN客戶成功: {dto.CustomerId}");
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增JGJN客戶失敗", ex);
            throw;
        }
    }

    public async Task UpdateJgjNCustomerAsync(long tKey, UpdateJgjNCustomerDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"資料不存在: {tKey}");
            }

            entity.CustomerName = dto.CustomerName;
            entity.CustomerType = dto.CustomerType;
            entity.ContactPerson = dto.ContactPerson;
            entity.Phone = dto.Phone;
            entity.Email = dto.Email;
            entity.Address = dto.Address;
            entity.Status = dto.Status;
            entity.Memo = dto.Memo;
            entity.UpdatedBy = _userContext.UserId;
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"修改JGJN客戶成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改JGJN客戶失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteJgjNCustomerAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"資料不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
            _logger.LogInfo($"刪除JGJN客戶成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除JGJN客戶失敗: {tKey}", ex);
            throw;
        }
    }

    private JgjNCustomerDto MapToDto(JgjNCustomer entity)
    {
        return new JgjNCustomerDto
        {
            TKey = entity.TKey,
            CustomerId = entity.CustomerId,
            CustomerName = entity.CustomerName,
            CustomerType = entity.CustomerType,
            ContactPerson = entity.ContactPerson,
            Phone = entity.Phone,
            Email = entity.Email,
            Address = entity.Address,
            Status = entity.Status,
            Memo = entity.Memo,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

