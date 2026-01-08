using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.BusinessReport;
using ErpCore.Infrastructure.Repositories.BusinessReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.BusinessReport;

/// <summary>
/// 員餐卡欄位服務實作 (SYSL206/SYSL207)
/// </summary>
public class EmployeeMealCardFieldService : BaseService, IEmployeeMealCardFieldService
{
    private readonly IEmployeeMealCardFieldRepository _repository;

    public EmployeeMealCardFieldService(
        IEmployeeMealCardFieldRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<EmployeeMealCardFieldDto>> GetFieldsAsync(EmployeeMealCardFieldQueryDto query)
    {
        try
        {
            var repositoryQuery = new EmployeeMealCardFieldQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                FieldId = query.FieldId,
                FieldName = query.FieldName,
                CardType = query.CardType,
                ActionType = query.ActionType,
                OtherType = query.OtherType,
                Status = query.Status
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<EmployeeMealCardFieldDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢員餐卡欄位列表失敗", ex);
            throw;
        }
    }

    public async Task<EmployeeMealCardFieldDto?> GetFieldAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                return null;
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢員餐卡欄位失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<long> CreateFieldAsync(CreateEmployeeMealCardFieldDto dto)
    {
        try
        {
            // 檢查欄位ID是否已存在
            var existing = await _repository.GetByFieldIdAsync(dto.FieldId);
            if (existing != null)
            {
                throw new Exception($"欄位ID已存在: {dto.FieldId}");
            }

            var entity = new EmployeeMealCardField
            {
                FieldId = dto.FieldId,
                FieldName = dto.FieldName,
                CardType = dto.CardType,
                ActionType = dto.ActionType,
                OtherType = dto.OtherType,
                MustKeyinYn = dto.MustKeyinYn ?? "N",
                ReadonlyYn = dto.ReadonlyYn ?? "N",
                BtnEtekYn = dto.BtnEtekYn ?? "N",
                SeqNo = dto.SeqNo ?? 0,
                Status = dto.Status ?? "1",
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var tKey = await _repository.CreateAsync(entity);
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增員餐卡欄位失敗", ex);
            throw;
        }
    }

    public async Task<bool> UpdateFieldAsync(long tKey, UpdateEmployeeMealCardFieldDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new Exception($"找不到員餐卡欄位資料: {tKey}");
            }

            entity.FieldName = dto.FieldName ?? entity.FieldName;
            entity.CardType = dto.CardType ?? entity.CardType;
            entity.ActionType = dto.ActionType ?? entity.ActionType;
            entity.OtherType = dto.OtherType ?? entity.OtherType;
            entity.MustKeyinYn = dto.MustKeyinYn ?? entity.MustKeyinYn;
            entity.ReadonlyYn = dto.ReadonlyYn ?? entity.ReadonlyYn;
            entity.BtnEtekYn = dto.BtnEtekYn ?? entity.BtnEtekYn;
            entity.SeqNo = dto.SeqNo ?? entity.SeqNo;
            entity.Status = dto.Status ?? entity.Status;
            entity.Notes = dto.Notes ?? entity.Notes;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            var result = await _repository.UpdateAsync(entity);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改員餐卡欄位失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<bool> DeleteFieldAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new Exception($"找不到員餐卡欄位資料: {tKey}");
            }

            var result = await _repository.DeleteAsync(tKey);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除員餐卡欄位失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<EmployeeMealCardFieldDto?> GetPreviousFieldAsync(string fieldId)
    {
        try
        {
            var entity = await _repository.GetPreviousAsync(fieldId);
            if (entity == null)
            {
                return null;
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"載入上一筆員餐卡欄位失敗: {fieldId}", ex);
            throw;
        }
    }

    public async Task<EmployeeMealCardFieldDto> ToggleYnAsync(long tKey, ToggleYnDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new Exception($"找不到員餐卡欄位資料: {tKey}");
            }

            // 切換Y/N值
            var newValue = dto.CurrentValue == "Y" ? "N" : "Y";
            switch (dto.FieldType.ToLower())
            {
                case "mustkeyinyn":
                    entity.MustKeyinYn = newValue;
                    break;
                case "readonlyyn":
                    entity.ReadonlyYn = newValue;
                    break;
                case "btnetekyn":
                    entity.BtnEtekYn = newValue;
                    break;
                default:
                    throw new Exception($"不支援的欄位類型: {dto.FieldType}");
            }

            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"切換Y/N值失敗: {tKey}", ex);
            throw;
        }
    }

    /// <summary>
    /// 將 Entity 轉換為 DTO
    /// </summary>
    private EmployeeMealCardFieldDto MapToDto(EmployeeMealCardField entity)
    {
        return new EmployeeMealCardFieldDto
        {
            TKey = entity.TKey,
            FieldId = entity.FieldId,
            FieldName = entity.FieldName,
            CardType = entity.CardType,
            ActionType = entity.ActionType,
            OtherType = entity.OtherType,
            MustKeyinYn = entity.MustKeyinYn,
            ReadonlyYn = entity.ReadonlyYn,
            BtnEtekYn = entity.BtnEtekYn,
            SeqNo = entity.SeqNo,
            Status = entity.Status,
            Notes = entity.Notes,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

