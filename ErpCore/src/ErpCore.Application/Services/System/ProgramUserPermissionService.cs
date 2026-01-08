using Dapper;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.Base;
using ErpCore.Infrastructure.Data;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 作業權限之使用者列表服務實作 (SYS0720)
/// </summary>
public class ProgramUserPermissionService : BaseService, IProgramUserPermissionService
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ExportHelper _exportHelper;

    public ProgramUserPermissionService(
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext,
        ExportHelper exportHelper) : base(logger, userContext)
    {
        _connectionFactory = connectionFactory;
        _exportHelper = exportHelper;
    }

    /// <summary>
    /// 查詢作業權限之使用者列表
    /// </summary>
    public async Task<ProgramUserPermissionListResponseDto> GetProgramUserPermissionListAsync(
        ProgramUserPermissionListRequestDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.ProgramId))
            {
                throw new ArgumentException("作業代碼為必填");
            }

            using var connection = _connectionFactory.CreateConnection();

            // 驗證作業是否存在
            var programSql = @"
                SELECT ProgramId, ProgramName
                FROM ConfigPrograms
                WHERE ProgramId = @ProgramId AND Status = '1'";
            
            var program = await connection.QueryFirstOrDefaultAsync<dynamic>(
                programSql, 
                new { ProgramId = request.ProgramId });

            if (program == null)
            {
                throw new KeyNotFoundException($"作業 {request.ProgramId} 不存在或已停用");
            }

            // 查詢使用者權限（只查詢使用者直接權限，不包含角色權限）
            var sql = @"
                SELECT DISTINCT
                    U.UserId,
                    U.UserName,
                    CB.ButtonId,
                    CB.ButtonName,
                    CB.PageId
                FROM ConfigButtons CB
                INNER JOIN UserButtons UB ON CB.ButtonId = UB.ButtonId
                INNER JOIN Users U ON UB.UserId = U.UserId
                WHERE CB.ProgramId = @ProgramId
                    AND CB.Status = '1'
                    AND U.Status = '1'
                ORDER BY U.UserId, CB.PageId, CB.ButtonId";

            var results = await connection.QueryAsync<dynamic>(
                sql, 
                new { ProgramId = request.ProgramId });

            // 組織使用者與按鈕權限的對應關係
            var response = new ProgramUserPermissionListResponseDto
            {
                ProgramId = program.ProgramId,
                ProgramName = program.ProgramName,
                Users = new List<UserPermissionDto>()
            };

            var userDict = new Dictionary<string, UserPermissionDto>();

            foreach (var item in results)
            {
                var userId = (string)item.UserId;
                var userName = (string)item.UserName;
                var buttonId = (string)item.ButtonId;
                var buttonName = (string)item.ButtonName;
                var pageId = (string?)item.PageId;

                // 使用者層級
                if (!userDict.ContainsKey(userId))
                {
                    userDict[userId] = new UserPermissionDto
                    {
                        UserId = userId,
                        UserName = userName,
                        Buttons = new List<ButtonPermissionDto>()
                    };
                }

                // 按鈕層級
                var button = new ButtonPermissionDto
                {
                    ButtonId = buttonId,
                    ButtonName = buttonName,
                    PageId = pageId
                };
                userDict[userId].Buttons.Add(button);
            }

            response.Users = userDict.Values.OrderBy(x => x.UserId).ToList();
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢作業權限之使用者列表失敗: ProgramId={request.ProgramId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 匯出作業權限之使用者報表
    /// </summary>
    public async Task<byte[]> ExportProgramUserPermissionReportAsync(
        ProgramUserPermissionListRequestDto request,
        string exportFormat,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var data = await GetProgramUserPermissionListAsync(request, cancellationToken);

            // 扁平化結構為列表
            var exportData = new List<ProgramUserPermissionExportItem>();
            foreach (var user in data.Users)
            {
                foreach (var button in user.Buttons)
                {
                    exportData.Add(new ProgramUserPermissionExportItem
                    {
                        ProgramId = data.ProgramId,
                        ProgramName = data.ProgramName,
                        UserId = user.UserId,
                        UserName = user.UserName,
                        ButtonId = button.ButtonId,
                        ButtonName = button.ButtonName,
                        PageId = button.PageId ?? string.Empty
                    });
                }
            }

            // 定義匯出欄位
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "ProgramId", DisplayName = "作業代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ProgramName", DisplayName = "作業名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "UserId", DisplayName = "使用者代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "UserName", DisplayName = "使用者名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ButtonId", DisplayName = "按鈕代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ButtonName", DisplayName = "按鈕名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "PageId", DisplayName = "頁面代碼", DataType = ExportDataType.String }
            };

            var title = $"作業權限之使用者列表 - {data.ProgramName} ({data.ProgramId})";

            if (exportFormat == "Excel")
            {
                return _exportHelper.ExportToExcel(exportData, columns, "作業權限之使用者列表", title);
            }
            else if (exportFormat == "PDF")
            {
                return _exportHelper.ExportToPdf(exportData, columns, title);
            }
            else
            {
                throw new ArgumentException($"不支援的匯出格式: {exportFormat}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"匯出作業權限之使用者報表失敗: ProgramId={request.ProgramId}, Format={exportFormat}", ex);
            throw;
        }
    }

    /// <summary>
    /// 作業權限之使用者匯出項目（扁平化結構）
    /// </summary>
    private class ProgramUserPermissionExportItem
    {
        public string ProgramId { get; set; } = string.Empty;
        public string ProgramName { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string ButtonId { get; set; } = string.Empty;
        public string ButtonName { get; set; } = string.Empty;
        public string PageId { get; set; } = string.Empty;
    }
}

