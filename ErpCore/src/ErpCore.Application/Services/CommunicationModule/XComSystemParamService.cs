using ErpCore.Application.DTOs.CommunicationModule;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.CommunicationModule;
using ErpCore.Infrastructure.Repositories.CommunicationModule;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.CommunicationModule;

/// <summary>
/// XCOM系統參數服務實作
/// </summary>
public class XComSystemParamService : BaseService, IXComSystemParamService
{
    private readonly IXComSystemParamRepository _repository;

    public XComSystemParamService(
        IXComSystemParamRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<XComSystemParamDto>> GetSystemParamsAsync(XComSystemParamQueryDto query)
    {
        try
        {
            var repositoryQuery = new XComSystemParamQuery
            {
                ParamCode = query.ParamCode,
                ParamName = query.ParamName,
                ParamType = query.ParamType,
                Status = query.Status,
                SystemId = query.SystemId,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(MapToDto).ToList();

            return new PagedResult<XComSystemParamDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢XCOM系統參數列表失敗", ex);
            throw;
        }
    }

    public async Task<XComSystemParamDto?> GetSystemParamByIdAsync(string paramCode)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(paramCode);
            return entity == null ? null : MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢XCOM系統參數失敗: {paramCode}", ex);
            throw;
        }
    }

    public async Task CreateSystemParamAsync(CreateXComSystemParamDto dto)
    {
        try
        {
            var exists = await _repository.ExistsAsync(dto.ParamCode);
            if (exists)
            {
                throw new Exception($"參數代碼已存在: {dto.ParamCode}");
            }

            var entity = new XComSystemParam
            {
                ParamCode = dto.ParamCode,
                ParamName = dto.ParamName,
                ParamValue = dto.ParamValue,
                ParamType = dto.ParamType,
                Description = dto.Description,
                Status = dto.Status,
                SystemId = dto.SystemId,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(entity);
            _logger.LogInfo($"新增XCOM系統參數成功: {dto.ParamCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增XCOM系統參數失敗: {dto.ParamCode}", ex);
            throw;
        }
    }

    public async Task UpdateSystemParamAsync(string paramCode, UpdateXComSystemParamDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(paramCode);
            if (entity == null)
            {
                throw new Exception($"參數不存在: {paramCode}");
            }

            entity.ParamName = dto.ParamName;
            entity.ParamValue = dto.ParamValue;
            entity.ParamType = dto.ParamType;
            entity.Description = dto.Description;
            entity.Status = dto.Status;
            entity.SystemId = dto.SystemId;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"修改XCOM系統參數成功: {paramCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改XCOM系統參數失敗: {paramCode}", ex);
            throw;
        }
    }

    public async Task DeleteSystemParamAsync(string paramCode)
    {
        try
        {
            await _repository.DeleteAsync(paramCode);
            _logger.LogInfo($"刪除XCOM系統參數成功: {paramCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除XCOM系統參數失敗: {paramCode}", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(string paramCode, string status)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(paramCode);
            if (entity == null)
            {
                throw new Exception($"參數不存在: {paramCode}");
            }

            entity.Status = status;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"更新XCOM系統參數狀態成功: {paramCode}, {status}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新XCOM系統參數狀態失敗: {paramCode}", ex);
            throw;
        }
    }

    private static XComSystemParamDto MapToDto(XComSystemParam entity)
    {
        return new XComSystemParamDto
        {
            ParamCode = entity.ParamCode,
            ParamName = entity.ParamName,
            ParamValue = entity.ParamValue,
            ParamType = entity.ParamType,
            Description = entity.Description,
            Status = entity.Status,
            SystemId = entity.SystemId,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

