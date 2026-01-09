using ErpCore.Application.DTOs.SystemExtensionH;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.SystemExtensionH;
using ErpCore.Infrastructure.Repositories.SystemExtensionH;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.SystemExtensionH;

/// <summary>
/// 系統擴展PH服務實作 (SYSPH00 - 感應卡維護作業)
/// </summary>
public class SystemExtensionPHService : BaseService, ISystemExtensionPHService
{
    private readonly IEmpCardRepository _repository;

    public SystemExtensionPHService(
        IEmpCardRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    /// <summary>
    /// 查詢感應卡列表
    /// </summary>
    public async Task<PagedResult<EmpCardDto>> GetEmpCardsAsync(EmpCardQueryDto query)
    {
        try
        {
            var repositoryQuery = new EmpCardQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                CardNo = query.CardNo,
                EmpId = query.EmpId,
                CardStatus = query.CardStatus,
                BeginDateFrom = query.BeginDateFrom,
                BeginDateTo = query.BeginDateTo,
                EndDateFrom = query.EndDateFrom,
                EndDateTo = query.EndDateTo
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(MapToDto).ToList();

            return new PagedResult<EmpCardDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢感應卡列表失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 查詢單筆感應卡
    /// </summary>
    public async Task<EmpCardDto> GetEmpCardByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new Exception($"感應卡不存在: {tKey}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢感應卡失敗: {tKey}", ex);
            throw;
        }
    }

    /// <summary>
    /// 新增感應卡
    /// </summary>
    public async Task<long> CreateEmpCardAsync(CreateEmpCardDto dto)
    {
        try
        {
            // 檢查感應卡號是否已存在
            var exists = await _repository.ExistsByCardNoAsync(dto.CardNo);
            if (exists)
            {
                throw new Exception($"感應卡號已存在: {dto.CardNo}");
            }

            var entity = new EmpCard
            {
                CardNo = dto.CardNo,
                EmpId = dto.EmpId,
                BeginDate = dto.BeginDate,
                EndDate = dto.EndDate,
                CardStatus = dto.CardStatus,
                Notes = dto.Notes,
                BUser = GetCurrentUserId(),
                BTime = DateTime.Now,
                CUser = null,
                CTime = null,
                CPriority = null,
                CGroup = null
            };

            var tKey = await _repository.CreateAsync(entity);
            _logger.LogInfo($"新增感應卡成功: {tKey}, 卡號: {dto.CardNo}");

            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增感應卡失敗: {dto.CardNo}", ex);
            throw;
        }
    }

    /// <summary>
    /// 批量新增感應卡
    /// </summary>
    public async Task<int> CreateBatchEmpCardsAsync(CreateBatchEmpCardDto dto)
    {
        try
        {
            var entities = new List<EmpCard>();
            var currentUserId = GetCurrentUserId();
            var currentTime = DateTime.Now;

            foreach (var item in dto.Items)
            {
                // 檢查感應卡號是否已存在
                var exists = await _repository.ExistsByCardNoAsync(item.CardNo);
                if (exists)
                {
                    _logger.LogWarning($"感應卡號已存在，跳過: {item.CardNo}");
                    continue;
                }

                var entity = new EmpCard
                {
                    CardNo = item.CardNo,
                    EmpId = item.EmpId,
                    BeginDate = item.BeginDate,
                    EndDate = item.EndDate,
                    CardStatus = item.CardStatus,
                    Notes = item.Notes,
                    BUser = currentUserId,
                    BTime = currentTime,
                    CUser = null,
                    CTime = null,
                    CPriority = null,
                    CGroup = null
                };

                entities.Add(entity);
            }

            if (entities.Count == 0)
            {
                throw new Exception("沒有可新增的感應卡資料");
            }

            var count = await _repository.CreateBatchAsync(entities);
            _logger.LogInfo($"批量新增感應卡成功: {count} 筆");

            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError("批量新增感應卡失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 修改感應卡
    /// </summary>
    public async Task UpdateEmpCardAsync(long tKey, UpdateEmpCardDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new Exception($"感應卡不存在: {tKey}");
            }

            // 如果卡號有變更，檢查新卡號是否已存在
            if (entity.CardNo != dto.CardNo)
            {
                var exists = await _repository.ExistsByCardNoAsync(dto.CardNo);
                if (exists)
                {
                    throw new Exception($"感應卡號已存在: {dto.CardNo}");
                }
            }

            entity.CardNo = dto.CardNo;
            entity.EmpId = dto.EmpId;
            entity.BeginDate = dto.BeginDate;
            entity.EndDate = dto.EndDate;
            entity.CardStatus = dto.CardStatus;
            entity.Notes = dto.Notes;
            entity.CUser = GetCurrentUserId();
            entity.CTime = DateTime.Now;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"修改感應卡成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改感應卡失敗: {tKey}", ex);
            throw;
        }
    }

    /// <summary>
    /// 刪除感應卡
    /// </summary>
    public async Task DeleteEmpCardAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new Exception($"感應卡不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
            _logger.LogInfo($"刪除感應卡成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除感應卡失敗: {tKey}", ex);
            throw;
        }
    }

    /// <summary>
    /// 映射 Entity 到 DTO
    /// </summary>
    private EmpCardDto MapToDto(EmpCard entity)
    {
        return new EmpCardDto
        {
            TKey = entity.TKey,
            CardNo = entity.CardNo,
            EmpId = entity.EmpId,
            BeginDate = entity.BeginDate,
            EndDate = entity.EndDate,
            CardStatus = entity.CardStatus,
            Notes = entity.Notes,
            BUser = entity.BUser,
            BTime = entity.BTime,
            CUser = entity.CUser,
            CTime = entity.CTime,
            CPriority = entity.CPriority,
            CGroup = entity.CGroup
        };
    }
}

