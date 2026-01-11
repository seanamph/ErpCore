using Dapper;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.Base;
using ErpCore.Infrastructure.Data;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 角色系統權限列表服務實作 (SYS0731)
/// </summary>
public class RoleSystemPermissionService : BaseService, IRoleSystemPermissionService
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ExportHelper _exportHelper;

    public RoleSystemPermissionService(
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext,
        ExportHelper exportHelper) : base(logger, userContext)
    {
        _connectionFactory = connectionFactory;
        _exportHelper = exportHelper;
    }

    public async Task<RoleSystemPermissionListResponseDto> GetRoleSystemPermissionListAsync(RoleSystemPermissionListRequestDto request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.RoleId))
            {
                throw new ArgumentException("角色代碼為必填");
            }

            using var connection = _connectionFactory.CreateConnection();

            // 查詢角色資訊
            var roleSql = "SELECT RoleId, RoleName FROM Roles WHERE RoleId = @RoleId";
            var role = await connection.QueryFirstOrDefaultAsync<dynamic>(roleSql, new { RoleId = request.RoleId });
            if (role == null)
            {
                throw new KeyNotFoundException($"角色 {request.RoleId} 不存在");
            }

            var response = new RoleSystemPermissionListResponseDto
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName
            };

            // 查詢角色按鈕權限（只查詢角色直接權限，不包含使用者權限）
            // 使用視圖 V_RoleSystemPermission 或直接查詢
            var sql = @"
                SELECT DISTINCT
                    CS.SystemId AS SYS_ID,
                    CS.SystemName AS SYS_NAME,
                    ISNULL(CSS.SubSystemId, '') AS MENU_ID,
                    ISNULL(CSS.SubSystemName, '') AS MENU_NAME,
                    CP.ProgramId AS PROG_ID,
                    CP.ProgramName AS PROG_NAME,
                    CB.ButtonId AS BUTTON_ID,
                    CB.ButtonName AS BUTTON_NAME,
                    CB.PageId AS PAGE_ID
                FROM ConfigButtons CB
                INNER JOIN ConfigPrograms CP ON CB.ProgramId = CP.ProgramId
                LEFT JOIN ConfigSubSystems CSS ON CP.SubSystemId = CSS.SubSystemId
                INNER JOIN ConfigSystems CS ON CP.SystemId = CS.SystemId
                INNER JOIN RoleButtons RB ON CB.ButtonId = RB.ButtonId
                WHERE CB.Status = '1' 
                    AND CP.Status = 'A' 
                    AND CS.Status = 'A'
                    AND (CSS.SubSystemId IS NULL OR CSS.Status = 'A')
                    AND RB.RoleId = @RoleId
                    AND RB.UserId IS NULL";

            var parameters = new DynamicParameters();
            parameters.Add("RoleId", request.RoleId);

            // 篩選條件
            if (!string.IsNullOrEmpty(request.SystemId))
            {
                sql += " AND CS.SystemId = @SystemId";
                parameters.Add("SystemId", request.SystemId);
            }

            if (!string.IsNullOrEmpty(request.MenuId))
            {
                sql += " AND (CSS.SubSystemId = @MenuId OR (CSS.SubSystemId IS NULL AND @MenuId = ''))";
                parameters.Add("MenuId", request.MenuId);
            }

            if (!string.IsNullOrEmpty(request.ProgramId))
            {
                sql += " AND CP.ProgramId = @ProgramId";
                parameters.Add("ProgramId", request.ProgramId);
            }

            sql += " ORDER BY CS.SystemId, ISNULL(CSS.SubSystemId, ''), CP.ProgramId, CB.PageId, CB.ButtonId";

            var results = await connection.QueryAsync<dynamic>(sql, parameters);

            // 組織階層式結構（系統→選單→作業→按鈕）
            var systemDict = new Dictionary<string, RoleSystemPermissionDto>();
            var menuDict = new Dictionary<string, RoleMenuPermissionDto>();
            var programDict = new Dictionary<string, RoleProgramPermissionDto>();

            foreach (var item in results)
            {
                var systemId = (string)item.SYS_ID;
                var systemName = (string)item.SYS_NAME;
                var menuId = (string)item.MENU_ID ?? string.Empty;
                var menuName = (string)item.MENU_NAME ?? string.Empty;
                var programId = (string)item.PROG_ID;
                var programName = (string)item.PROG_NAME;
                var buttonId = (string)item.BUTTON_ID;
                var buttonName = (string)item.BUTTON_NAME;
                var pageId = (string?)item.PAGE_ID;

                // 系統層級
                if (!systemDict.ContainsKey(systemId))
                {
                    systemDict[systemId] = new RoleSystemPermissionDto
                    {
                        SystemId = systemId,
                        SystemName = systemName,
                        Menus = new List<RoleMenuPermissionDto>()
                    };
                }

                // 選單層級（使用 SubSystem，如果沒有則使用空字串作為預設選單）
                var menuKey = string.IsNullOrEmpty(menuId) ? $"{systemId}_DEFAULT" : $"{systemId}_{menuId}";
                if (!menuDict.ContainsKey(menuKey))
                {
                    var menu = new RoleMenuPermissionDto
                    {
                        MenuId = menuId,
                        MenuName = string.IsNullOrEmpty(menuName) ? "(未分類)" : menuName,
                        Programs = new List<RoleProgramPermissionDto>()
                    };
                    menuDict[menuKey] = menu;
                    systemDict[systemId].Menus.Add(menu);
                }

                // 作業層級
                var programKey = $"{systemId}_{menuId}_{programId}";
                if (!programDict.ContainsKey(programKey))
                {
                    var program = new RoleProgramPermissionDto
                    {
                        ProgramId = programId,
                        ProgramName = programName,
                        Buttons = new List<RoleButtonPermissionDto>()
                    };
                    programDict[programKey] = program;
                    menuDict[menuKey].Programs.Add(program);
                }

                // 按鈕層級
                var button = new RoleButtonPermissionDto
                {
                    ButtonId = buttonId,
                    ButtonName = buttonName,
                    PageId = pageId
                };
                programDict[programKey].Buttons.Add(button);
            }

            response.Permissions = systemDict.Values.OrderBy(x => x.SystemId).ToList();
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢角色系統權限列表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportRoleSystemPermissionReportAsync(RoleSystemPermissionListRequestDto request, string exportFormat)
    {
        try
        {
            var data = await GetRoleSystemPermissionListAsync(request);

            // 扁平化階層式結構為列表
            var exportData = new List<RoleSystemPermissionExportItem>();
            foreach (var system in data.Permissions)
            {
                foreach (var menu in system.Menus)
                {
                    foreach (var program in menu.Programs)
                    {
                        foreach (var button in program.Buttons)
                        {
                            exportData.Add(new RoleSystemPermissionExportItem
                            {
                                RoleId = data.RoleId,
                                RoleName = data.RoleName,
                                SystemId = system.SystemId,
                                SystemName = system.SystemName,
                                MenuId = menu.MenuId,
                                MenuName = menu.MenuName,
                                ProgramId = program.ProgramId,
                                ProgramName = program.ProgramName,
                                ButtonId = button.ButtonId,
                                ButtonName = button.ButtonName,
                                PageId = button.PageId ?? string.Empty
                            });
                        }
                    }
                }
            }

            // 定義匯出欄位
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "RoleId", DisplayName = "角色代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "RoleName", DisplayName = "角色名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SystemId", DisplayName = "系統代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SystemName", DisplayName = "系統名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "MenuId", DisplayName = "選單代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "MenuName", DisplayName = "選單名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ProgramId", DisplayName = "作業代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ProgramName", DisplayName = "作業名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ButtonId", DisplayName = "按鈕代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ButtonName", DisplayName = "按鈕名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "PageId", DisplayName = "頁面代碼", DataType = ExportDataType.String }
            };

            var title = $"角色系統權限列表 - {data.RoleName} ({data.RoleId})";

            if (exportFormat == "Excel")
            {
                return _exportHelper.ExportToExcel(exportData, columns, "角色系統權限列表", title);
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
            _logger.LogError("匯出角色系統權限報表失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 角色系統權限匯出項目（扁平化結構）
    /// </summary>
    private class RoleSystemPermissionExportItem
    {
        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string SystemId { get; set; } = string.Empty;
        public string SystemName { get; set; } = string.Empty;
        public string MenuId { get; set; } = string.Empty;
        public string MenuName { get; set; } = string.Empty;
        public string ProgramId { get; set; } = string.Empty;
        public string ProgramName { get; set; } = string.Empty;
        public string ButtonId { get; set; } = string.Empty;
        public string ButtonName { get; set; } = string.Empty;
        public string PageId { get; set; } = string.Empty;
    }
}

