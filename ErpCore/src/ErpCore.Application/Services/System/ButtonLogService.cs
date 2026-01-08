using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using Dapper;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 按鈕操作記錄服務實作 (SYS0790)
/// </summary>
public class ButtonLogService : BaseService, IButtonLogService
{
    private readonly IButtonLogRepository _repository;
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ExportHelper _exportHelper;

    public ButtonLogService(
        IButtonLogRepository repository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext,
        ExportHelper exportHelper) : base(logger, userContext)
    {
        _repository = repository;
        _connectionFactory = connectionFactory;
        _exportHelper = exportHelper;
    }

    public async Task<PagedResult<ButtonLogDto>> GetButtonLogsAsync(ButtonLogQueryDto query)
    {
        try
        {
            // 處理日期時間範圍
            DateTime? startDateTime = null;
            DateTime? endDateTime = null;

            if (query.Filters != null)
            {
                if (query.Filters.StartDate.HasValue)
                {
                    var startDate = query.Filters.StartDate.Value.Date;
                    if (!string.IsNullOrEmpty(query.Filters.StartTime) && query.Filters.StartTime.Length == 4)
                    {
                        var hour = int.Parse(query.Filters.StartTime.Substring(0, 2));
                        var minute = int.Parse(query.Filters.StartTime.Substring(2, 2));
                        startDateTime = startDate.AddHours(hour).AddMinutes(minute);
                    }
                    else
                    {
                        startDateTime = startDate;
                    }
                }

                if (query.Filters.EndDate.HasValue)
                {
                    var endDate = query.Filters.EndDate.Value.Date;
                    if (!string.IsNullOrEmpty(query.Filters.EndTime) && query.Filters.EndTime.Length == 4)
                    {
                        var hour = int.Parse(query.Filters.EndTime.Substring(0, 2));
                        var minute = int.Parse(query.Filters.EndTime.Substring(2, 2));
                        endDateTime = endDate.AddHours(hour).AddMinutes(minute);
                    }
                    else
                    {
                        endDateTime = endDate.AddDays(1).AddSeconds(-1);
                    }
                }
            }

            var repositoryQuery = new ButtonLogQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                UserIds = query.Filters?.UserIds,
                ProgId = query.Filters?.ProgId,
                StartDateTime = startDateTime,
                EndDateTime = endDateTime
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            // 查詢使用者名稱
            var userIds = result.Items.Select(x => x.BUser).Distinct().ToList();
            var userNames = new Dictionary<string, string>();

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

            var dtos = result.Items.Select(x => new ButtonLogDto
            {
                TKey = x.TKey,
                BUser = x.BUser,
                UserName = userNames.ContainsKey(x.BUser) ? userNames[x.BUser] : null,
                BTime = x.BTime,
                ProgId = x.ProgId,
                ProgName = x.ProgName,
                ButtonName = x.ButtonName,
                Url = x.Url,
                FrameName = x.FrameName
            }).ToList();

            return new PagedResult<ButtonLogDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢按鈕操作記錄失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateButtonLogAsync(CreateButtonLogDto dto)
    {
        try
        {
            // 從使用者上下文取得當前使用者
            var currentUserId = _userContext.GetUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                throw new InvalidOperationException("無法取得當前使用者資訊");
            }

            // 查詢作業名稱（如果有作業代碼）
            string? progName = null;
            if (!string.IsNullOrEmpty(dto.ProgId))
            {
                using var connection = _connectionFactory.CreateConnection();
                var sql = @"
                    SELECT ProgramName
                    FROM ConfigPrograms
                    WHERE ProgramId = @ProgramId";
                progName = await connection.QueryFirstOrDefaultAsync<string>(sql, new { ProgramId = dto.ProgId });
            }

            var buttonLog = new ButtonLog
            {
                BUser = currentUserId,
                BTime = DateTime.Now,
                ProgId = dto.ProgId,
                ProgName = progName ?? dto.ProgName,
                ButtonName = dto.ButtonName,
                Url = dto.Url,
                FrameName = dto.FrameName
            };

            var result = await _repository.CreateAsync(buttonLog);
            return result.TKey;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增按鈕操作記錄失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportButtonLogReportAsync(ButtonLogQueryDto query, string format)
    {
        try
        {
            // 查詢所有資料（不分頁）
            var allDataQuery = new ButtonLogQueryDto
            {
                PageIndex = 1,
                PageSize = int.MaxValue,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                Filters = query.Filters
            };

            var result = await GetButtonLogsAsync(allDataQuery);

            // 定義匯出欄位
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "BUser", DisplayName = "使用者代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "UserName", DisplayName = "使用者名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "BTime", DisplayName = "操作時間", DataType = ExportDataType.DateTime },
                new ExportColumn { PropertyName = "ProgId", DisplayName = "作業代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ProgName", DisplayName = "作業名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ButtonName", DisplayName = "按鈕名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Url", DisplayName = "URL", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "FrameName", DisplayName = "框架名稱", DataType = ExportDataType.String }
            };

            var title = "按鈕操作記錄報表";

            if (format == "Excel")
            {
                return _exportHelper.ExportToExcel(result.Items, columns, "按鈕操作記錄報表", title);
            }
            else if (format == "PDF")
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
            _logger.LogError("匯出按鈕操作記錄報表失敗", ex);
            throw;
        }
    }
}

