using ErpCore.Application.DTOs.SapIntegration;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.SapIntegration;
using ErpCore.Infrastructure.Repositories.SapIntegration;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.SapIntegration;

/// <summary>
/// SAP整合服務實作 (TransSAP系列)
/// </summary>
public class TransSapService : BaseService, ITransSapService
{
    private readonly ITransSapRepository _repository;

    public TransSapService(
        ITransSapRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<TransSapDto>> GetTransSapListAsync(TransSapQueryDto query)
    {
        try
        {
            var repositoryQuery = new TransSapQuery
            {
                TransId = query.TransId,
                TransType = query.TransType,
                SapSystemCode = query.SapSystemCode,
                Status = query.Status,
                TransDateFrom = query.TransDateFrom,
                TransDateTo = query.TransDateTo,
                Keyword = query.Keyword,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(MapToDto).ToList();

            return new PagedResult<TransSapDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢SAP整合資料列表失敗", ex);
            throw;
        }
    }

    public async Task<TransSapDto?> GetTransSapByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢SAP整合資料失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<TransSapDto?> GetTransSapByTransIdAsync(string transId)
    {
        try
        {
            var entity = await _repository.GetByTransIdAsync(transId);
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢SAP整合資料失敗: {transId}", ex);
            throw;
        }
    }

    public async Task<long> CreateTransSapAsync(CreateTransSapDto dto)
    {
        try
        {
            // 檢查交易單號是否已存在
            var existing = await _repository.GetByTransIdAsync(dto.TransId);
            if (existing != null)
            {
                throw new InvalidOperationException($"交易單號已存在: {dto.TransId}");
            }

            var entity = new TransSap
            {
                TransId = dto.TransId,
                TransType = dto.TransType,
                SapSystemCode = dto.SapSystemCode,
                TransDate = dto.TransDate,
                Status = dto.Status,
                RequestData = dto.RequestData,
                ResponseData = dto.ResponseData,
                ErrorMessage = dto.ErrorMessage,
                RetryCount = dto.RetryCount,
                Memo = dto.Memo,
                CreatedBy = _userContext.UserId,
                CreatedAt = DateTime.Now,
                UpdatedBy = _userContext.UserId,
                UpdatedAt = DateTime.Now
            };

            var tKey = await _repository.CreateAsync(entity);
            _logger.LogInfo($"新增SAP整合資料成功: {dto.TransId}");
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增SAP整合資料失敗", ex);
            throw;
        }
    }

    public async Task UpdateTransSapAsync(long tKey, UpdateTransSapDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"資料不存在: {tKey}");
            }

            entity.TransType = dto.TransType;
            entity.SapSystemCode = dto.SapSystemCode;
            entity.TransDate = dto.TransDate;
            entity.Status = dto.Status;
            entity.RequestData = dto.RequestData;
            entity.ResponseData = dto.ResponseData;
            entity.ErrorMessage = dto.ErrorMessage;
            entity.RetryCount = dto.RetryCount;
            entity.Memo = dto.Memo;
            entity.UpdatedBy = _userContext.UserId;
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"修改SAP整合資料成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改SAP整合資料失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteTransSapAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"資料不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
            _logger.LogInfo($"刪除SAP整合資料成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除SAP整合資料失敗: {tKey}", ex);
            throw;
        }
    }

    private TransSapDto MapToDto(TransSap entity)
    {
        return new TransSapDto
        {
            TKey = entity.TKey,
            TransId = entity.TransId,
            TransType = entity.TransType,
            SapSystemCode = entity.SapSystemCode,
            TransDate = entity.TransDate,
            Status = entity.Status,
            RequestData = entity.RequestData,
            ResponseData = entity.ResponseData,
            ErrorMessage = entity.ErrorMessage,
            RetryCount = entity.RetryCount,
            Memo = entity.Memo,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

