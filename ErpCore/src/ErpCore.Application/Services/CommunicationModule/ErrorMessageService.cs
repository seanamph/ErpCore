using ErpCore.Application.DTOs.CommunicationModule;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.CommunicationModule;
using ErpCore.Infrastructure.Repositories.CommunicationModule;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.CommunicationModule;

/// <summary>
/// 錯誤訊息服務實作
/// </summary>
public class ErrorMessageService : BaseService, IErrorMessageService
{
    private readonly IErrorMessageRepository _repository;

    public ErrorMessageService(
        IErrorMessageRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<ErrorMessageDto>> GetErrorMessagesAsync(ErrorMessageQueryDto query)
    {
        try
        {
            var repositoryQuery = new ErrorMessageQuery
            {
                ErrorCode = query.ErrorCode,
                ErrorType = query.ErrorType,
                HttpStatusCode = query.HttpStatusCode,
                UserId = query.UserId,
                StartDate = query.StartDate,
                EndDate = query.EndDate,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder
            };

            var (items, totalCount) = await _repository.QueryAsync(repositoryQuery);

            var dtos = items.Select(MapToDto).ToList();

            return new PagedResult<ErrorMessageDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢錯誤訊息列表失敗", ex);
            throw;
        }
    }

    public async Task<ErrorMessageDto?> GetErrorMessageByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            return entity == null ? null : MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢錯誤訊息失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<long> CreateErrorMessageAsync(CreateErrorMessageDto dto)
    {
        try
        {
            var entity = new ErrorMessage
            {
                ErrorCode = dto.ErrorCode,
                ErrorType = dto.ErrorType,
                HttpStatusCode = dto.HttpStatusCode,
                ErrorMessageText = dto.ErrorMessageText,
                ErrorDetail = dto.ErrorDetail,
                RequestUrl = dto.RequestUrl,
                RequestMethod = dto.RequestMethod,
                UserId = dto.UserId ?? _userContext.UserId,
                UserIp = dto.UserIp,
                UserAgent = dto.UserAgent,
                StackTrace = dto.StackTrace,
                CreatedAt = DateTime.Now
            };

            var tKey = await _repository.CreateAsync(entity);
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增錯誤訊息失敗", ex);
            throw;
        }
    }

    public async Task<ErrorPageDto> GetErrorPageAsync(int statusCode, string language = "zh-TW")
    {
        try
        {
            var errorCode = statusCode.ToString();
            var template = await _repository.GetTemplateByErrorCodeAsync(errorCode, language);

            if (template != null)
            {
                return new ErrorPageDto
                {
                    StatusCode = statusCode,
                    Title = template.Title,
                    Message = template.Message,
                    Description = template.Description,
                    Suggestions = template.Solution?.Split('\n').Where(s => !string.IsNullOrWhiteSpace(s)).ToList()
                };
            }

            // 預設錯誤頁面資訊
            return GetDefaultErrorPage(statusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError($"取得HTTP錯誤頁面資訊失敗: {statusCode}", ex);
            return GetDefaultErrorPage(statusCode);
        }
    }

    public async Task<WarningPageDto> GetWarningPageAsync(string warningCode, string language = "zh-TW")
    {
        try
        {
            var template = await _repository.GetTemplateByErrorCodeAsync(warningCode, language);

            if (template != null)
            {
                return new WarningPageDto
                {
                    WarningCode = warningCode,
                    Title = template.Title,
                    Message = template.Message,
                    Type = "WARNING",
                    ConfirmText = "確定",
                    CancelText = "取消"
                };
            }

            // 預設警告頁面資訊
            return new WarningPageDto
            {
                WarningCode = warningCode,
                Title = "操作警告",
                Message = "此操作可能會有風險，請確認是否繼續？",
                Type = "WARNING",
                ConfirmText = "確定",
                CancelText = "取消"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"取得警告頁面資訊失敗: {warningCode}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<ErrorMessageTemplateDto>> GetTemplatesAsync(string? errorCode = null, string? language = null)
    {
        try
        {
            var templates = await _repository.GetTemplatesAsync(errorCode, language);
            return templates.Select(MapTemplateToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢錯誤訊息模板列表失敗", ex);
            throw;
        }
    }

    private ErrorMessageDto MapToDto(ErrorMessage entity)
    {
        return new ErrorMessageDto
        {
            TKey = entity.TKey,
            ErrorCode = entity.ErrorCode,
            ErrorType = entity.ErrorType,
            HttpStatusCode = entity.HttpStatusCode,
            ErrorMessageText = entity.ErrorMessageText,
            ErrorDetail = entity.ErrorDetail,
            RequestUrl = entity.RequestUrl,
            RequestMethod = entity.RequestMethod,
            UserId = entity.UserId,
            UserIp = entity.UserIp,
            UserAgent = entity.UserAgent,
            StackTrace = entity.StackTrace,
            CreatedAt = entity.CreatedAt
        };
    }

    private ErrorMessageTemplateDto MapTemplateToDto(ErrorMessageTemplate entity)
    {
        return new ErrorMessageTemplateDto
        {
            TKey = entity.TKey,
            ErrorCode = entity.ErrorCode,
            Language = entity.Language,
            Title = entity.Title,
            Message = entity.Message,
            Description = entity.Description,
            Solution = entity.Solution,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private ErrorPageDto GetDefaultErrorPage(int statusCode)
    {
        return statusCode switch
        {
            401 => new ErrorPageDto
            {
                StatusCode = 401,
                Title = "未授權",
                Message = "抱歉，您沒有權限存取此資源",
                Description = "請確認您已登入系統，或聯繫系統管理員",
                Suggestions = new List<string> { "確認是否已登入", "檢查帳號權限", "聯繫系統管理員" }
            },
            404 => new ErrorPageDto
            {
                StatusCode = 404,
                Title = "找不到頁面",
                Message = "抱歉，您要查找的頁面不存在",
                Description = "請檢查網址是否正確，或返回首頁",
                Suggestions = new List<string> { "檢查網址是否正確", "返回首頁", "聯繫系統管理員" }
            },
            500 => new ErrorPageDto
            {
                StatusCode = 500,
                Title = "伺服器錯誤",
                Message = "抱歉，伺服器發生錯誤",
                Description = "我們正在處理此問題，請稍後再試",
                Suggestions = new List<string> { "稍後再試", "返回首頁", "聯繫系統管理員" }
            },
            _ => new ErrorPageDto
            {
                StatusCode = statusCode,
                Title = "發生錯誤",
                Message = "抱歉，發生未預期的錯誤",
                Description = "請聯繫系統管理員",
                Suggestions = new List<string> { "返回首頁", "聯繫系統管理員" }
            }
        };
    }
}

