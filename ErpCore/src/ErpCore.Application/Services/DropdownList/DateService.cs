using ErpCore.Application.DTOs.DropdownList;
using ErpCore.Application.Services.Base;
using ErpCore.Application.Services.BasicData;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.DropdownList;

/// <summary>
/// 日期服務實作
/// </summary>
public class DateService : BaseService, IDateService
{
    private readonly IParameterService _parameterService;

    public DateService(
        IParameterService parameterService,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _parameterService = parameterService;
    }

    public async Task<DateFormatDto> GetDateFormatAsync()
    {
        try
        {
            _logger.LogInfo("取得系統日期格式設定");

            // 從系統參數讀取日期格式，預設為 yyyy/MM/dd
            string dateFormat = "yyyy/MM/dd";
            string timeFormat = "HH:mm:ss";

            try
            {
                var dateFormatParam = await _parameterService.GetParameterValueAsync("DATE_FORMAT", "FORMAT");
                if (!string.IsNullOrEmpty(dateFormatParam))
                {
                    dateFormat = dateFormatParam;
                }
            }
            catch
            {
                // 如果參數不存在，使用預設值
                _logger.LogInfo("使用預設日期格式: yyyy/MM/dd");
            }

            try
            {
                var timeFormatParam = await _parameterService.GetParameterValueAsync("TIME_FORMAT", "FORMAT");
                if (!string.IsNullOrEmpty(timeFormatParam))
                {
                    timeFormat = timeFormatParam;
                }
            }
            catch
            {
                // 如果參數不存在，使用預設值
                _logger.LogInfo("使用預設時間格式: HH:mm:ss");
            }

            return new DateFormatDto
            {
                DateFormat = dateFormat,
                TimeFormat = timeFormat,
                DateTimeFormat = $"{dateFormat} {timeFormat}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("取得系統日期格式設定失敗", ex);
            throw;
        }
    }

    public async Task<DateValidationDto> ValidateDateAsync(string dateString, string? dateFormat = null)
    {
        try
        {
            _logger.LogInfo($"驗證日期格式: {dateString}");

            if (string.IsNullOrWhiteSpace(dateString))
            {
                return new DateValidationDto
                {
                    IsValid = false,
                    ErrorMessage = "日期字串不能為空"
                };
            }

            // 如果沒有指定格式，從系統參數取得
            if (string.IsNullOrEmpty(dateFormat))
            {
                var formatDto = await GetDateFormatAsync();
                dateFormat = formatDto.DateFormat;
            }

            // 嘗試解析日期
            var parsedDate = await ParseDateAsync(dateString, dateFormat);

            if (parsedDate == null)
            {
                return new DateValidationDto
                {
                    IsValid = false,
                    ErrorMessage = $"日期格式不正確，預期格式: {dateFormat}"
                };
            }

            // 驗證年份不能小於1582年（格里曆開始年份）
            if (parsedDate.Value.Year < 1582)
            {
                return new DateValidationDto
                {
                    IsValid = false,
                    ErrorMessage = "年份不能小於1582年"
                };
            }

            return new DateValidationDto
            {
                IsValid = true,
                ParsedDate = parsedDate
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"驗證日期格式失敗: {dateString}", ex);
            return new DateValidationDto
            {
                IsValid = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<DateTime?> ParseDateAsync(string dateString, string? dateFormat = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dateString))
            {
                return null;
            }

            // 如果沒有指定格式，從系統參數取得
            if (string.IsNullOrEmpty(dateFormat))
            {
                var formatDto = await GetDateFormatAsync();
                dateFormat = formatDto.DateFormat;
            }

            // 嘗試使用指定格式解析
            if (DateTime.TryParseExact(dateString, dateFormat, null, System.Globalization.DateTimeStyles.None, out var result))
            {
                return result;
            }

            // 如果指定格式解析失敗，嘗試使用系統預設格式解析
            if (DateTime.TryParse(dateString, out var defaultResult))
            {
                return defaultResult;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"解析日期字串失敗: {dateString}", ex);
            return null;
        }
    }
}

