using ErpCore.Application.DTOs.Recruitment;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Recruitment;
using ErpCore.Infrastructure.Repositories.Recruitment;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Recruitment;

/// <summary>
/// 訪談服務實作 (SYSC222)
/// </summary>
public class InterviewService : BaseService, IInterviewService
{
    private readonly IInterviewRepository _repository;
    private readonly IProspectRepository _prospectRepository;

    public InterviewService(
        IInterviewRepository repository,
        IProspectRepository prospectRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _prospectRepository = prospectRepository;
    }

    public async Task<PagedResult<InterviewDto>> GetInterviewsAsync(InterviewQueryDto query)
    {
        try
        {
            var repositoryQuery = new InterviewQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                ProspectId = query.ProspectId,
                InterviewDateFrom = query.InterviewDateFrom,
                InterviewDateTo = query.InterviewDateTo,
                InterviewResult = query.InterviewResult,
                Status = query.Status,
                Interviewer = query.Interviewer
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            // 填充潛客名稱
            foreach (var dto in dtos)
            {
                if (!string.IsNullOrEmpty(dto.ProspectId))
                {
                    var prospect = await _prospectRepository.GetByIdAsync(dto.ProspectId);
                    if (prospect != null)
                    {
                        dto.ProspectName = prospect.ProspectName;
                    }
                }
            }

            return new PagedResult<InterviewDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢訪談列表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<InterviewDto>> GetInterviewsByProspectIdAsync(string prospectId, InterviewQueryDto query)
    {
        try
        {
            var repositoryQuery = new InterviewQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                ProspectId = prospectId,
                InterviewDateFrom = query.InterviewDateFrom,
                InterviewDateTo = query.InterviewDateTo,
                InterviewResult = query.InterviewResult,
                Status = query.Status,
                Interviewer = query.Interviewer
            };

            var result = await _repository.QueryByProspectIdAsync(prospectId, repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            // 填充潛客名稱
            var prospect = await _prospectRepository.GetByIdAsync(prospectId);
            if (prospect != null)
            {
                foreach (var dto in dtos)
                {
                    dto.ProspectName = prospect.ProspectName;
                }
            }

            return new PagedResult<InterviewDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"根據潛客查詢訪談列表失敗: {prospectId}", ex);
            throw;
        }
    }

    public async Task<InterviewDto> GetInterviewByIdAsync(long interviewId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(interviewId);
            if (entity == null)
            {
                throw new InvalidOperationException($"訪談不存在: {interviewId}");
            }

            var dto = MapToDto(entity);

            // 填充潛客名稱
            if (!string.IsNullOrEmpty(dto.ProspectId))
            {
                var prospect = await _prospectRepository.GetByIdAsync(dto.ProspectId);
                if (prospect != null)
                {
                    dto.ProspectName = prospect.ProspectName;
                }
            }

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢訪談失敗: {interviewId}", ex);
            throw;
        }
    }

    public async Task<long> CreateInterviewAsync(CreateInterviewDto dto)
    {
        try
        {
            // 驗證必填欄位
            if (string.IsNullOrWhiteSpace(dto.ProspectId))
            {
                throw new ArgumentException("潛客代碼不能為空");
            }

            // 檢查潛客是否存在
            var prospect = await _prospectRepository.GetByIdAsync(dto.ProspectId);
            if (prospect == null)
            {
                throw new InvalidOperationException($"潛客不存在: {dto.ProspectId}");
            }

            // 驗證訪談日期不能晚於今天
            if (dto.InterviewDate.Date > DateTime.Now.Date)
            {
                throw new ArgumentException("訪談日期不能晚於今天");
            }

            // 驗證後續行動日期必須晚於訪談日期
            if (dto.NextActionDate.HasValue && dto.InterviewDate.Date >= dto.NextActionDate.Value.Date)
            {
                throw new ArgumentException("後續行動日期必須晚於訪談日期");
            }

            // 驗證訪談結果值
            if (!string.IsNullOrEmpty(dto.InterviewResult) && 
                dto.InterviewResult != "SUCCESS" && dto.InterviewResult != "FOLLOW_UP" && 
                dto.InterviewResult != "CANCELLED" && dto.InterviewResult != "NO_SHOW")
            {
                throw new ArgumentException("訪談結果值必須為 SUCCESS、FOLLOW_UP、CANCELLED 或 NO_SHOW");
            }

            var entity = new Interview
            {
                ProspectId = dto.ProspectId,
                InterviewDate = dto.InterviewDate,
                InterviewTime = dto.InterviewTime,
                InterviewType = dto.InterviewType,
                Interviewer = dto.Interviewer,
                InterviewLocation = dto.InterviewLocation,
                InterviewContent = dto.InterviewContent,
                InterviewResult = dto.InterviewResult,
                NextAction = dto.NextAction,
                NextActionDate = dto.NextActionDate,
                FollowUpDate = dto.FollowUpDate,
                Notes = dto.Notes,
                Status = "ACTIVE",
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now,
                CreatedPriority = null,
                CreatedGroup = null
            };

            var result = await _repository.CreateAsync(entity);
            return result.InterviewId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增訪談失敗: {dto.ProspectId}", ex);
            throw;
        }
    }

    public async Task UpdateInterviewAsync(long interviewId, UpdateInterviewDto dto)
    {
        try
        {
            // 驗證必填欄位
            if (string.IsNullOrWhiteSpace(dto.ProspectId))
            {
                throw new ArgumentException("潛客代碼不能為空");
            }

            // 檢查訪談是否存在
            var existing = await _repository.GetByIdAsync(interviewId);
            if (existing == null)
            {
                throw new InvalidOperationException($"訪談不存在: {interviewId}");
            }

            // 檢查潛客是否存在
            var prospect = await _prospectRepository.GetByIdAsync(dto.ProspectId);
            if (prospect == null)
            {
                throw new InvalidOperationException($"潛客不存在: {dto.ProspectId}");
            }

            // 驗證訪談日期不能晚於今天
            if (dto.InterviewDate.Date > DateTime.Now.Date)
            {
                throw new ArgumentException("訪談日期不能晚於今天");
            }

            // 驗證後續行動日期必須晚於訪談日期
            if (dto.NextActionDate.HasValue && dto.InterviewDate.Date >= dto.NextActionDate.Value.Date)
            {
                throw new ArgumentException("後續行動日期必須晚於訪談日期");
            }

            // 驗證訪談結果值
            if (!string.IsNullOrEmpty(dto.InterviewResult) && 
                dto.InterviewResult != "SUCCESS" && dto.InterviewResult != "FOLLOW_UP" && 
                dto.InterviewResult != "CANCELLED" && dto.InterviewResult != "NO_SHOW")
            {
                throw new ArgumentException("訪談結果值必須為 SUCCESS、FOLLOW_UP、CANCELLED 或 NO_SHOW");
            }

            existing.ProspectId = dto.ProspectId;
            existing.InterviewDate = dto.InterviewDate;
            existing.InterviewTime = dto.InterviewTime;
            existing.InterviewType = dto.InterviewType;
            existing.Interviewer = dto.Interviewer;
            existing.InterviewLocation = dto.InterviewLocation;
            existing.InterviewContent = dto.InterviewContent;
            existing.InterviewResult = dto.InterviewResult;
            existing.NextAction = dto.NextAction;
            existing.NextActionDate = dto.NextActionDate;
            existing.FollowUpDate = dto.FollowUpDate;
            existing.Notes = dto.Notes;
            existing.UpdatedBy = GetCurrentUserId();
            existing.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(existing);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改訪談失敗: {interviewId}", ex);
            throw;
        }
    }

    public async Task DeleteInterviewAsync(long interviewId)
    {
        try
        {
            // 檢查訪談是否存在
            var existing = await _repository.GetByIdAsync(interviewId);
            if (existing == null)
            {
                throw new InvalidOperationException($"訪談不存在: {interviewId}");
            }

            await _repository.DeleteAsync(interviewId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除訪談失敗: {interviewId}", ex);
            throw;
        }
    }

    public async Task BatchDeleteInterviewsAsync(BatchDeleteInterviewDto dto)
    {
        try
        {
            if (dto.InterviewIds == null || dto.InterviewIds.Count == 0)
            {
                throw new ArgumentException("訪談ID列表不能為空");
            }

            foreach (var interviewId in dto.InterviewIds)
            {
                await DeleteInterviewAsync(interviewId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除訪談失敗", ex);
            throw;
        }
    }

    public async Task UpdateInterviewStatusAsync(long interviewId, UpdateInterviewStatusDto dto)
    {
        try
        {
            // 驗證狀態值
            if (string.IsNullOrEmpty(dto.Status) || 
                (dto.Status != "ACTIVE" && dto.Status != "CANCELLED"))
            {
                throw new ArgumentException("狀態值必須為 ACTIVE 或 CANCELLED");
            }

            // 檢查訪談是否存在
            var existing = await _repository.GetByIdAsync(interviewId);
            if (existing == null)
            {
                throw new InvalidOperationException($"訪談不存在: {interviewId}");
            }

            existing.Status = dto.Status;
            existing.UpdatedBy = GetCurrentUserId();
            existing.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(existing);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新訪談狀態失敗: {interviewId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 將 Entity 轉換為 DTO
    /// </summary>
    private static InterviewDto MapToDto(Interview entity)
    {
        return new InterviewDto
        {
            InterviewId = entity.InterviewId,
            ProspectId = entity.ProspectId,
            InterviewDate = entity.InterviewDate,
            InterviewTime = entity.InterviewTime,
            InterviewType = entity.InterviewType,
            Interviewer = entity.Interviewer,
            InterviewLocation = entity.InterviewLocation,
            InterviewContent = entity.InterviewContent,
            InterviewResult = entity.InterviewResult,
            NextAction = entity.NextAction,
            NextActionDate = entity.NextActionDate,
            FollowUpDate = entity.FollowUpDate,
            Notes = entity.Notes,
            Status = entity.Status,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

