using System.Text.Json;
using ErpCore.Application.DTOs.Query;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Query;
using ErpCore.Infrastructure.Repositories.Query;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Query;

/// <summary>
/// 零用金請款檔服務實作 (SYSQ220)
/// </summary>
public class PcCashRequestService : BaseService, IPcCashRequestService
{
    private readonly IPcCashRequestRepository _repository;
    private readonly IPcCashRepository _pcCashRepository;

    public PcCashRequestService(
        IPcCashRequestRepository repository,
        IPcCashRepository pcCashRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _pcCashRepository = pcCashRepository;
    }

    public async Task<PagedResult<PcCashRequestDto>> QueryAsync(PcCashRequestQueryDto query)
    {
        try
        {
            return await _repository.QueryAsync(query);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢零用金請款檔列表失敗", ex);
            throw;
        }
    }

    public async Task<PcCashRequestDto> GetByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"零用金請款檔不存在: {tKey}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢零用金請款檔失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<PcCashRequestDto> GetByRequestIdAsync(string requestId)
    {
        try
        {
            var entity = await _repository.GetByRequestIdAsync(requestId);
            if (entity == null)
            {
                throw new InvalidOperationException($"零用金請款檔不存在: {requestId}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢零用金請款檔失敗: {requestId}", ex);
            throw;
        }
    }

    public async Task<PcCashRequestDto> CreateAsync(CreatePcCashRequestDto dto)
    {
        try
        {
            // 驗證零用金單號是否存在
            decimal totalAmount = 0;
            foreach (var cashId in dto.CashIds)
            {
                var cash = await _pcCashRepository.GetByCashIdAsync(cashId);
                if (cash == null)
                {
                    throw new InvalidOperationException($"零用金單號不存在: {cashId}");
                }
                totalAmount += cash.CashAmount;
            }

            var requestId = await _repository.GenerateRequestIdAsync(dto.SiteId);
            var cashIdsJson = JsonSerializer.Serialize(dto.CashIds);

            var entity = new PcCashRequest
            {
                RequestId = requestId,
                SiteId = dto.SiteId,
                RequestDate = dto.RequestDate,
                CashIds = cashIdsJson,
                RequestAmount = totalAmount,
                RequestStatus = "DRAFT",
                Notes = dto.Notes,
                BUser = GetCurrentUserId(),
                BTime = DateTime.Now,
                CUser = GetCurrentUserId(),
                CTime = DateTime.Now
            };

            var result = await _repository.CreateAsync(entity);
            return MapToDto(result);
        }
        catch (Exception ex)
        {
            _logger.LogError("新增零用金請款檔失敗", ex);
            throw;
        }
    }

    public async Task<PcCashRequestDto> UpdateAsync(long tKey, UpdatePcCashRequestDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"零用金請款檔不存在: {tKey}");
            }

            // 驗證零用金單號是否存在
            decimal totalAmount = 0;
            foreach (var cashId in dto.CashIds)
            {
                var cash = await _pcCashRepository.GetByCashIdAsync(cashId);
                if (cash == null)
                {
                    throw new InvalidOperationException($"零用金單號不存在: {cashId}");
                }
                totalAmount += cash.CashAmount;
            }

            var cashIdsJson = JsonSerializer.Serialize(dto.CashIds);

            entity.RequestDate = dto.RequestDate;
            entity.CashIds = cashIdsJson;
            entity.RequestAmount = totalAmount;
            entity.Notes = dto.Notes;
            entity.CUser = GetCurrentUserId();
            entity.CTime = DateTime.Now;

            var result = await _repository.UpdateAsync(entity);
            return MapToDto(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改零用金請款檔失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"零用金請款檔不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除零用金請款檔失敗: {tKey}", ex);
            throw;
        }
    }

    private PcCashRequestDto MapToDto(PcCashRequest entity)
    {
        var dto = new PcCashRequestDto
        {
            TKey = entity.TKey,
            RequestId = entity.RequestId,
            SiteId = entity.SiteId,
            RequestDate = entity.RequestDate,
            CashIds = entity.CashIds,
            RequestAmount = entity.RequestAmount,
            RequestStatus = entity.RequestStatus,
            Notes = entity.Notes,
            BUser = entity.BUser,
            BTime = entity.BTime,
            CUser = entity.CUser,
            CTime = entity.CTime
        };

        // 解析 CashIds JSON
        if (!string.IsNullOrEmpty(entity.CashIds))
        {
            try
            {
                dto.CashIdList = JsonSerializer.Deserialize<List<string>>(entity.CashIds);
            }
            catch
            {
                dto.CashIdList = new List<string>();
            }
        }

        return dto;
    }
}

