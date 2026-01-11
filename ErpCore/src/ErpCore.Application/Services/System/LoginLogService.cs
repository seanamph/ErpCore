using Dapper;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 使用者異常登入記錄服務實作 (SYS0760)
/// </summary>
public class LoginLogService : BaseService, ILoginLogService
{
    private readonly ILoginLogRepository _repository;
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ExportHelper _exportHelper;

    public LoginLogService(
        ILoginLogRepository repository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext,
        ExportHelper exportHelper) : base(logger, userContext)
    {
        _repository = repository;
        _connectionFactory = connectionFactory;
        _exportHelper = exportHelper;
    }

    public async Task<PagedResult<LoginLogDto>> GetLoginLogsAsync(LoginLogQueryDto query)
    {
        try
        {
            // 驗證查詢條件
            ValidateQuery(query);

            var repositoryQuery = new LoginLogQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortBy1 = query.SortBy1,
                SortOrder1 = query.SortOrder1,
                SortBy2 = query.SortBy2,
                SortOrder2 = query.SortOrder2,
                SortBy3 = query.SortBy3,
                SortOrder3 = query.SortOrder3,
                EventIds = query.EventIds,
                UserId = query.UserId,
                EventTimeFrom = query.EventTimeFrom,
                EventTimeTo = query.EventTimeTo
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            // 查詢使用者名稱和異常事件代碼名稱
            var userIds = result.Items.Where(x => !string.IsNullOrEmpty(x.UserId)).Select(x => x.UserId!).Distinct().ToList();
            var userNames = new Dictionary<string, string>();
            var eventTypeNames = new Dictionary<string, string>();

            if (userIds.Any())
            {
                using var connection = _connectionFactory.CreateConnection();
                var sql = @"
                    SELECT UserId, UserName
                    FROM Users
                    WHERE UserId IN @UserIds";
                var users = await connection.QueryAsync<(string UserId, string UserName)>(sql, new { UserIds = userIds });
                foreach (var user in users)
                {
                    userNames[user.UserId] = user.UserName;
                }
            }

            // 查詢異常事件代碼名稱
            var eventTypes = await _repository.GetEventTypesAsync();
            foreach (var eventType in eventTypes)
            {
                eventTypeNames[eventType.Tag] = eventType.Content;
            }

            var dtos = result.Items.Select(x => new LoginLogDto
            {
                TKey = x.TKey,
                EventId = x.EventId,
                EventIdName = eventTypeNames.ContainsKey(x.EventId) ? eventTypeNames[x.EventId] : null,
                UserId = x.UserId,
                UserName = !string.IsNullOrEmpty(x.UserId) && userNames.ContainsKey(x.UserId) ? userNames[x.UserId] : null,
                LoginIp = x.LoginIp,
                EventTime = x.EventTime,
                BUser = x.BUser,
                BTime = x.BTime,
                CUser = x.CUser,
                CTime = x.CTime,
                CPriority = x.CPriority,
                CGroup = x.CGroup
            }).ToList();

            return new PagedResult<LoginLogDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢異常登入記錄失敗", ex);
            throw;
        }
    }

    public async Task<List<EventTypeDto>> GetEventTypesAsync()
    {
        try
        {
            var eventTypes = await _repository.GetEventTypesAsync();
            return eventTypes.Select(x => new EventTypeDto
            {
                Tag = x.Tag,
                Content = x.Content
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢異常事件代碼選項失敗", ex);
            throw;
        }
    }

    public async Task<int> DeleteLoginLogsAsync(List<long> tKeys, string currentUserId)
    {
        try
        {
            if (tKeys == null || !tKeys.Any())
                throw new ArgumentException("刪除的記錄主鍵不能為空");

            // 記錄操作日誌
            _logger.LogInformation(
                "使用者 {UserId} 刪除異常登入記錄: {TKeys}",
                currentUserId,
                string.Join(",", tKeys));

            return await _repository.DeleteAsync(tKeys);
        }
        catch (Exception ex)
        {
            _logger.LogError("刪除異常登入記錄失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> GenerateReportAsync(LoginLogReportDto reportDto, string format)
    {
        try
        {
            // 查詢所有資料（不分頁）
            var query = new LoginLogQueryDto
            {
                PageIndex = 1,
                PageSize = int.MaxValue,
                EventIds = reportDto.EventIds,
                UserId = reportDto.UserId,
                EventTimeFrom = reportDto.EventTimeFrom,
                EventTimeTo = reportDto.EventTimeTo
            };

            var result = await GetLoginLogsAsync(query);

            // 定義匯出欄位
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "EventIdName", DisplayName = "異常事件代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "UserId", DisplayName = "使用者代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "UserName", DisplayName = "使用者名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "LoginIp", DisplayName = "登入IP位址", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "EventTime", DisplayName = "事件發生時間", DataType = ExportDataType.DateTime }
            };

            var title = "使用者異常登入報表";

            if (format.ToUpper() == "EXCEL")
            {
                return _exportHelper.ExportToExcel(result.Items, columns, "使用者異常登入報表", title);
            }
            else if (format.ToUpper() == "PDF")
            {
                return _exportHelper.ExportToPdf(result.Items, columns, title);
            }
            else
            {
                throw new ArgumentException($"不支援的匯出格式: {format}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("產生異常登入報表失敗", ex);
            throw;
        }
    }

    private void ValidateQuery(LoginLogQueryDto query)
    {
        if (query.EventTimeFrom.HasValue && query.EventTimeTo.HasValue)
        {
            if (query.EventTimeFrom.Value > query.EventTimeTo.Value)
            {
                throw new ArgumentException("事件時間起不能大於事件時間迄");
            }
        }
    }
}
