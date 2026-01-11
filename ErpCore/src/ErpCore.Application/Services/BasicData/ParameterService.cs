using ErpCore.Application.DTOs.BasicData;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.BasicData;
using ErpCore.Infrastructure.Repositories.BasicData;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.BasicData;

/// <summary>
/// 參數服務實作
/// </summary>
public class ParameterService : BaseService, IParameterService
{
    private readonly IParameterRepository _repository;

    public ParameterService(
        IParameterRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<ParameterDto>> GetParametersAsync(ParameterQueryDto query)
    {
        try
        {
            var repositoryQuery = new ParameterQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                Title = query.Title,
                Tag = query.Tag,
                Status = query.Status,
                SystemId = query.SystemId
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => new ParameterDto
            {
                TKey = x.TKey,
                Title = x.Title,
                Tag = x.Tag,
                SeqNo = x.SeqNo,
                Content = x.Content,
                Content2 = x.Content2,
                Notes = x.Notes,
                Status = x.Status,
                ReadOnly = x.ReadOnly,
                SystemId = x.SystemId,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return new PagedResult<ParameterDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢參數列表失敗", ex);
            throw;
        }
    }

    public async Task<ParameterDto> GetParameterAsync(string title, string tag)
    {
        try
        {
            var entity = await _repository.GetByKeyAsync(title, tag);
            if (entity == null)
            {
                throw new InvalidOperationException($"參數不存在: {title}/{tag}");
            }

            return new ParameterDto
            {
                TKey = entity.TKey,
                Title = entity.Title,
                Tag = entity.Tag,
                SeqNo = entity.SeqNo,
                Content = entity.Content,
                Content2 = entity.Content2,
                Notes = entity.Notes,
                Status = entity.Status,
                ReadOnly = entity.ReadOnly,
                SystemId = entity.SystemId,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢參數失敗: {title}/{tag}", ex);
            throw;
        }
    }

    public async Task<List<ParameterDto>> GetParametersByTitleAsync(string title)
    {
        try
        {
            var entities = await _repository.GetByTitleAsync(title);
            return entities.Select(x => new ParameterDto
            {
                TKey = x.TKey,
                Title = x.Title,
                Tag = x.Tag,
                SeqNo = x.SeqNo,
                Content = x.Content,
                Content2 = x.Content2,
                Notes = x.Notes,
                Status = x.Status,
                ReadOnly = x.ReadOnly,
                SystemId = x.SystemId,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"根據標題查詢參數列表失敗: {title}", ex);
            throw;
        }
    }

    public async Task<ParameterKeyDto> CreateParameterAsync(CreateParameterDto dto)
    {
        try
        {
            // 驗證資料
            ValidateCreateDto(dto);

            // 檢查是否已存在
            var exists = await _repository.ExistsAsync(dto.Title, dto.Tag);
            if (exists)
            {
                throw new InvalidOperationException($"參數已存在: {dto.Title}/{dto.Tag}");
            }

            var entity = new Parameter
            {
                Title = dto.Title,
                Tag = dto.Tag,
                SeqNo = dto.SeqNo ?? 0,
                Content = dto.Content,
                Content2 = dto.Content2,
                Notes = dto.Notes,
                Status = dto.Status ?? "1",
                ReadOnly = dto.ReadOnly ?? "0",
                SystemId = dto.SystemId,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now,
                CreatedPriority = null,
                CreatedGroup = GetCurrentOrgId()
            };

            await _repository.CreateAsync(entity);

            return new ParameterKeyDto
            {
                Title = entity.Title,
                Tag = entity.Tag
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增參數失敗: {dto.Title}/{dto.Tag}", ex);
            throw;
        }
    }

    public async Task UpdateParameterAsync(string title, string tag, UpdateParameterDto dto)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByKeyAsync(title, tag);
            if (entity == null)
            {
                throw new InvalidOperationException($"參數不存在: {title}/{tag}");
            }

            // 檢查是否為只讀
            if (entity.ReadOnly == "1")
            {
                throw new InvalidOperationException($"參數為只讀，無法修改: {title}/{tag}");
            }

            entity.SeqNo = dto.SeqNo ?? 0;
            entity.Content = dto.Content;
            entity.Content2 = dto.Content2;
            entity.Notes = dto.Notes;
            entity.Status = dto.Status ?? "1";
            entity.ReadOnly = dto.ReadOnly ?? "0";
            entity.SystemId = dto.SystemId;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改參數失敗: {title}/{tag}", ex);
            throw;
        }
    }

    public async Task DeleteParameterAsync(string title, string tag)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByKeyAsync(title, tag);
            if (entity == null)
            {
                throw new InvalidOperationException($"參數不存在: {title}/{tag}");
            }

            // 檢查是否為只讀
            if (entity.ReadOnly == "1")
            {
                throw new InvalidOperationException($"參數為只讀，無法刪除: {title}/{tag}");
            }

            await _repository.DeleteAsync(title, tag);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除參數失敗: {title}/{tag}", ex);
            throw;
        }
    }

    public async Task DeleteParametersBatchAsync(BatchDeleteParameterDto dto)
    {
        try
        {
            foreach (var item in dto.Items)
            {
                await DeleteParameterAsync(item.Title, item.Tag);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除參數失敗", ex);
            throw;
        }
    }

    public async Task<string> GetParameterValueAsync(string title, string tag, string lang = null)
    {
        try
        {
            var entity = await _repository.GetByKeyAsync(title, tag);
            if (entity == null)
            {
                throw new InvalidOperationException($"參數不存在: {title}/{tag}");
            }

            // 根據語言選擇內容
            if (!string.IsNullOrEmpty(lang) && lang.ToUpper() == "EN" && !string.IsNullOrEmpty(entity.Content2))
            {
                return entity.Content2;
            }

            return entity.Content ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError($"取得參數值失敗: {title}/{tag}", ex);
            throw;
        }
    }

    private void ValidateCreateDto(CreateParameterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            throw new ArgumentException("參數標題不能為空");
        }

        if (string.IsNullOrWhiteSpace(dto.Tag))
        {
            throw new ArgumentException("參數標籤不能為空");
        }

        if (!string.IsNullOrEmpty(dto.Status) && dto.Status != "1" && dto.Status != "0")
        {
            throw new ArgumentException("狀態值必須為 1 (啟用) 或 0 (停用)");
        }

        if (!string.IsNullOrEmpty(dto.ReadOnly) && dto.ReadOnly != "1" && dto.ReadOnly != "0")
        {
            throw new ArgumentException("只讀標誌必須為 1 (只讀) 或 0 (可編輯)");
        }
    }
}
