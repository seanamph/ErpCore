using ErpCore.Application.DTOs.StoreFloor;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.StoreFloor;
using ErpCore.Infrastructure.Repositories.StoreFloor;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.StoreFloor;

/// <summary>
/// 類型代碼服務實作 (SYS6405-SYS6490 - 類型代碼維護)
/// </summary>
public class TypeCodeService : BaseService, ITypeCodeService
{
    private readonly ITypeCodeRepository _repository;

    public TypeCodeService(
        ITypeCodeRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<TypeCodeDto>> GetTypeCodesAsync(TypeCodeQueryDto query)
    {
        try
        {
            var repositoryQuery = new TypeCodeQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                TypeCode = query.TypeCode,
                TypeName = query.TypeName,
                Category = query.Category,
                Status = query.Status
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(MapToDto).ToList();

            return new PagedResult<TypeCodeDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢類型代碼列表失敗", ex);
            throw;
        }
    }

    public async Task<TypeCodeDto> GetTypeCodeByIdAsync(long tKey)
    {
        try
        {
            var typeCode = await _repository.GetByIdAsync(tKey);
            if (typeCode == null)
            {
                throw new KeyNotFoundException($"類型代碼不存在: {tKey}");
            }

            return MapToDto(typeCode);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢類型代碼失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<long> CreateTypeCodeAsync(CreateTypeCodeDto dto)
    {
        try
        {
            if (await _repository.ExistsAsync(dto.TypeCode, dto.Category))
            {
                throw new InvalidOperationException($"類型代碼已存在: {dto.TypeCode}/{dto.Category}");
            }

            var typeCode = new TypeCode
            {
                TypeCode = dto.TypeCode,
                TypeName = dto.TypeName,
                TypeNameEn = dto.TypeNameEn,
                Category = dto.Category,
                Description = dto.Description,
                SortOrder = dto.SortOrder,
                Status = dto.Status,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var result = await _repository.CreateAsync(typeCode);

            return result.TKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增類型代碼失敗: {dto.TypeCode}", ex);
            throw;
        }
    }

    public async Task UpdateTypeCodeAsync(long tKey, UpdateTypeCodeDto dto)
    {
        try
        {
            var typeCode = await _repository.GetByIdAsync(tKey);
            if (typeCode == null)
            {
                throw new KeyNotFoundException($"類型代碼不存在: {tKey}");
            }

            typeCode.TypeName = dto.TypeName;
            typeCode.TypeNameEn = dto.TypeNameEn;
            typeCode.Category = dto.Category;
            typeCode.Description = dto.Description;
            typeCode.SortOrder = dto.SortOrder;
            typeCode.Status = dto.Status;
            typeCode.UpdatedBy = GetCurrentUserId();
            typeCode.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(typeCode);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改類型代碼失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteTypeCodeAsync(long tKey)
    {
        try
        {
            var typeCode = await _repository.GetByIdAsync(tKey);
            if (typeCode == null)
            {
                throw new KeyNotFoundException($"類型代碼不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除類型代碼失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task BatchDeleteTypeCodesAsync(List<long> tKeys)
    {
        try
        {
            foreach (var tKey in tKeys)
            {
                await DeleteTypeCodeAsync(tKey);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"批次刪除類型代碼失敗", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string typeCode, string? category)
    {
        try
        {
            return await _repository.ExistsAsync(typeCode, category);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查類型代碼是否存在失敗: {typeCode}/{category}", ex);
            throw;
        }
    }

    /// <summary>
    /// 將 Entity 轉換為 DTO
    /// </summary>
    private TypeCodeDto MapToDto(ErpCore.Domain.Entities.StoreFloor.TypeCode typeCode)
    {
        return new TypeCodeDto
        {
            TKey = typeCode.TKey,
            TypeCode = typeCode.TypeCode,
            TypeName = typeCode.TypeName,
            TypeNameEn = typeCode.TypeNameEn,
            Category = typeCode.Category,
            Description = typeCode.Description,
            SortOrder = typeCode.SortOrder,
            Status = typeCode.Status,
            CreatedBy = typeCode.CreatedBy,
            CreatedAt = typeCode.CreatedAt,
            UpdatedBy = typeCode.UpdatedBy,
            UpdatedAt = typeCode.UpdatedAt
        };
    }
}

