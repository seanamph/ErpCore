using Dapper;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.Base;
using ErpCore.Infrastructure.Data;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 作業權限之角色列表服務實作 (SYS0740)
/// </summary>
public class ProgramRolePermissionService : BaseService, IProgramRolePermissionService
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ExportHelper _exportHelper;

    public ProgramRolePermissionService(
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext,
        ExportHelper exportHelper) : base(logger, userContext)
    {
        _connectionFactory = connectionFactory;
        _exportHelper = exportHelper;
    }

    public async Task<ProgramRolePermissionListResponseDto> GetProgramRolePermissionListAsync(ProgramRolePermissionListRequestDto request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.ProgramId))
            {
                throw new ArgumentException("作業代碼為必填");
            }

            using var connection = _connectionFactory.CreateConnection();

            // 查詢作業資訊
            var programSql = "SELECT ProgramId, ProgramName FROM ConfigPrograms WHERE ProgramId = @ProgramId AND Status = '1'";
            var program = await connection.QueryFirstOrDefaultAsync<dynamic>(programSql, new { ProgramId = request.ProgramId });
            if (program == null)
            {
                throw new KeyNotFoundException($"作業 {request.ProgramId} 不存在");
            }

            var response = new ProgramRolePermissionListResponseDto
            {
                ProgramId = program.ProgramId,
                ProgramName = program.ProgramName
            };

            // 查詢作業權限之角色列表（只查詢角色直接權限，不包含使用者權限）
            var sql = @"
                SELECT DISTINCT
                    R.RoleId,
                    R.RoleName,
                    CB.ButtonId,
                    CB.ButtonName,
                    CB.PageId
                FROM ConfigButtons CB
                INNER JOIN ConfigPrograms CP ON CB.ProgramId = CP.ProgramId
                INNER JOIN RoleButtons RB ON CB.ButtonId = RB.ButtonId
                INNER JOIN Roles R ON RB.RoleId = R.RoleId
                WHERE CB.Status = '1' 
                    AND CP.Status = '1'
                    AND CP.ProgramId = @ProgramId
                    AND RB.RoleId IS NOT NULL
                    AND RB.UserId IS NULL
                ORDER BY R.RoleId, CB.PageId, CB.ButtonId";

            var results = await connection.QueryAsync<dynamic>(sql, new { ProgramId = request.ProgramId });

            // 組織角色與按鈕權限的對應關係
            var roleDict = new Dictionary<string, ProgramRolePermissionDto>();

            foreach (var item in results)
            {
                var roleId = (string)item.RoleId;
                var roleName = (string)item.RoleName;
                var buttonId = (string)item.ButtonId;
                var buttonName = (string)item.ButtonName;
                var pageId = (string?)item.PageId;

                // 角色層級
                if (!roleDict.ContainsKey(roleId))
                {
                    roleDict[roleId] = new ProgramRolePermissionDto
                    {
                        RoleId = roleId,
                        RoleName = roleName,
                        Buttons = new List<ProgramButtonPermissionDto>()
                    };
                }

                // 按鈕層級
                var button = new ProgramButtonPermissionDto
                {
                    ButtonId = buttonId,
                    ButtonName = buttonName,
                    PageId = pageId
                };
                roleDict[roleId].Buttons.Add(button);
            }

            response.Roles = roleDict.Values.OrderBy(x => x.RoleId).ToList();
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢作業權限之角色列表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportProgramRolePermissionReportAsync(ProgramRolePermissionListRequestDto request, string exportFormat)
    {
        try
        {
            var data = await GetProgramRolePermissionListAsync(request);

            // 扁平化結構為列表
            var exportData = new List<ProgramRolePermissionExportItem>();
            foreach (var role in data.Roles)
            {
                foreach (var button in role.Buttons)
                {
                    exportData.Add(new ProgramRolePermissionExportItem
                    {
                        ProgramId = data.ProgramId,
                        ProgramName = data.ProgramName,
                        RoleId = role.RoleId,
                        RoleName = role.RoleName,
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
                new ExportColumn { PropertyName = "RoleId", DisplayName = "角色代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "RoleName", DisplayName = "角色名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ButtonId", DisplayName = "按鈕代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ButtonName", DisplayName = "按鈕名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "PageId", DisplayName = "頁面代碼", DataType = ExportDataType.String }
            };

            var title = $"作業權限之角色列表 - {data.ProgramName} ({data.ProgramId})";

            if (exportFormat == "Excel")
            {
                return _exportHelper.ExportToExcel(exportData, columns, "作業權限之角色列表", title);
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
            _logger.LogError("匯出作業權限之角色報表失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 作業權限之角色匯出項目（扁平化結構）
    /// </summary>
    private class ProgramRolePermissionExportItem
    {
        public string ProgramId { get; set; } = string.Empty;
        public string ProgramName { get; set; } = string.Empty;
        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string ButtonId { get; set; } = string.Empty;
        public string ButtonName { get; set; } = string.Empty;
        public string PageId { get; set; } = string.Empty;
    }
}

