using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.BusinessReport;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.BusinessReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using Dapper;

namespace ErpCore.Application.Services.BusinessReport;

/// <summary>
/// 員工餐卡申請服務實作 (SYSL130)
/// </summary>
public class EmployeeMealCardService : BaseService, IEmployeeMealCardService
{
    private readonly IEmployeeMealCardRepository _repository;
    private readonly IDbConnectionFactory _connectionFactory;

    public EmployeeMealCardService(
        IEmployeeMealCardRepository repository,
        ILoggerService logger,
        IUserContext userContext,
        IDbConnectionFactory connectionFactory) : base(logger, userContext)
    {
        _repository = repository;
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<EmployeeMealCardDto>> GetMealCardsAsync(EmployeeMealCardQueryDto query)
    {
        try
        {
            var repositoryQuery = new EmployeeMealCardQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                EmpId = query.EmpId,
                EmpName = query.EmpName,
                OrgId = query.OrgId,
                SiteId = query.SiteId,
                CardType = query.CardType,
                ActionType = query.ActionType,
                Status = query.Status,
                StartDateFrom = query.StartDateFrom,
                StartDateTo = query.StartDateTo,
                EndDateFrom = query.EndDateFrom,
                EndDateTo = query.EndDateTo
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<EmployeeMealCardDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢員工餐卡申請列表失敗", ex);
            throw;
        }
    }

    public async Task<EmployeeMealCardDto> GetMealCardByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"員工餐卡申請不存在: {tKey}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢員工餐卡申請失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<long> CreateMealCardAsync(CreateEmployeeMealCardDto dto)
    {
        try
        {
            // 驗證必填欄位
            if (string.IsNullOrWhiteSpace(dto.EmpId))
            {
                throw new ArgumentException("員工編號不能為空");
            }

            // 驗證日期
            if (dto.StartDate.HasValue && dto.EndDate.HasValue && dto.StartDate.Value > dto.EndDate.Value)
            {
                throw new ArgumentException("起始日期不能大於結束日期");
            }

            // 驗證起始日期必須是每月1日
            if (dto.StartDate.HasValue && dto.StartDate.Value.Day != 1)
            {
                throw new ArgumentException("起始日期必須是每月1日");
            }

            var entity = new EmployeeMealCard
            {
                EmpId = dto.EmpId,
                EmpName = dto.EmpName,
                OrgId = dto.OrgId,
                SiteId = dto.SiteId,
                CardType = dto.CardType,
                ActionType = dto.ActionType,
                ActionTypeD = dto.ActionTypeD,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = "P", // 預設為待審核
                Notes = dto.Notes,
                TxnNo = dto.TxnNo,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var result = await _repository.CreateAsync(entity);
            return result.TKey;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增員工餐卡申請失敗", ex);
            throw;
        }
    }

    public async Task UpdateMealCardAsync(long tKey, UpdateEmployeeMealCardDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"員工餐卡申請不存在: {tKey}");
            }

            // 檢查狀態，已審核的不可修改
            if (entity.Status == "A")
            {
                throw new InvalidOperationException("已審核的資料不可修改");
            }

            // 驗證日期
            if (dto.StartDate.HasValue && dto.EndDate.HasValue && dto.StartDate.Value > dto.EndDate.Value)
            {
                throw new ArgumentException("起始日期不能大於結束日期");
            }

            // 驗證起始日期必須是每月1日
            if (dto.StartDate.HasValue && dto.StartDate.Value.Day != 1)
            {
                throw new ArgumentException("起始日期必須是每月1日");
            }

            entity.EmpName = dto.EmpName;
            entity.OrgId = dto.OrgId;
            entity.SiteId = dto.SiteId;
            entity.CardType = dto.CardType;
            entity.ActionType = dto.ActionType;
            entity.ActionTypeD = dto.ActionTypeD;
            entity.StartDate = dto.StartDate;
            entity.EndDate = dto.EndDate;
            entity.Notes = dto.Notes;
            entity.TxnNo = dto.TxnNo;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改員工餐卡申請失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteMealCardAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"員工餐卡申請不存在: {tKey}");
            }

            // 檢查狀態，已審核的不可刪除
            if (entity.Status == "A")
            {
                throw new InvalidOperationException("已審核的資料不可刪除");
            }

            await _repository.DeleteAsync(tKey);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除員工餐卡申請失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<BatchVerifyResultDto> BatchVerifyAsync(BatchVerifyDto dto)
    {
        try
        {
            var result = new BatchVerifyResultDto();

            // 檢查所有選取的資料是否都是待審核狀態
            foreach (var tKey in dto.TKeys)
            {
                var entity = await _repository.GetByIdAsync(tKey);
                if (entity == null)
                {
                    result.FailCount++;
                    result.Errors.Add($"員工餐卡申請不存在: {tKey}");
                    continue;
                }

                if (entity.Status != "P")
                {
                    result.FailCount++;
                    result.Errors.Add($"員工餐卡申請 {tKey} 不是待審核狀態");
                    continue;
                }
            }

            // 批次審核
            var successCount = await _repository.BatchVerifyAsync(
                dto.TKeys,
                dto.Action,
                GetCurrentUserId(),
                dto.Notes);

            result.SuccessCount = successCount;
            result.FailCount = dto.TKeys.Count - successCount;

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("批次審核員工餐卡申請失敗", ex);
            throw;
        }
    }

    public async Task<MealCardDropdownsDto> GetDropdownsAsync()
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();

            // 查詢卡片類型
            const string cardTypeSql = "SELECT CardId, CardName FROM CardType WHERE Status = '1' ORDER BY CardId";
            var cardTypes = await connection.QueryAsync<CardTypeDto>(cardTypeSql);

            // 查詢動作類型
            const string actionTypeSql = "SELECT ActionId, ActionName FROM ActionType WHERE Status = '1' ORDER BY ActionId";
            var actionTypes = await connection.QueryAsync<ActionTypeDto>(actionTypeSql);

            // 查詢動作類型明細
            const string actionTypeDetailSql = "SELECT ActionId, ActionIdD, ActionNameD FROM ActionTypeDetail WHERE Status = '1' ORDER BY ActionId, ActionIdD";
            var actionTypeDetails = await connection.QueryAsync<ActionTypeDetailDto>(actionTypeDetailSql);

            return new MealCardDropdownsDto
            {
                CardTypes = cardTypes.ToList(),
                ActionTypes = actionTypes.ToList(),
                ActionTypeDetails = actionTypeDetails.ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("取得下拉選單資料失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 將 Entity 轉換為 DTO
    /// </summary>
    private EmployeeMealCardDto MapToDto(EmployeeMealCard entity)
    {
        return new EmployeeMealCardDto
        {
            TKey = entity.TKey,
            EmpId = entity.EmpId,
            EmpName = entity.EmpName,
            OrgId = entity.OrgId,
            SiteId = entity.SiteId,
            CardType = entity.CardType,
            ActionType = entity.ActionType,
            ActionTypeD = entity.ActionTypeD,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            Status = entity.Status,
            Verifier = entity.Verifier,
            VerifyDate = entity.VerifyDate,
            Notes = entity.Notes,
            TxnNo = entity.TxnNo,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

