using Dapper;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.Base;
using ErpCore.Infrastructure.Data;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 系統權限列表服務實作 (SYS0710)
/// </summary>
public class SystemPermissionService : BaseService, ISystemPermissionService
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ExportHelper _exportHelper;

    public SystemPermissionService(
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext,
        ExportHelper exportHelper) : base(logger, userContext)
    {
        _connectionFactory = connectionFactory;
        _exportHelper = exportHelper;
    }

    public async Task<SystemPermissionListResponseDto> GetSystemPermissionListAsync(SystemPermissionListRequestDto request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.UserId) && string.IsNullOrEmpty(request.RoleId))
            {
                throw new ArgumentException("請至少輸入使用者代碼或角色代碼");
            }

            var response = new SystemPermissionListResponseDto();
            var permissionDict = new Dictionary<string, SystemPermissionDto>();
            var programDict = new Dictionary<string, ProgramPermissionDto>();

            using var connection = _connectionFactory.CreateConnection();

            // 查詢使用者或角色的按鈕權限（包含 SubSystem 作為 Menu）
            var sql = @"
                SELECT DISTINCT
                    CS.SystemId,
                    CS.SystemName,
                    CSS.SubSystemId AS MenuId,
                    CSS.SubSystemName AS MenuName,
                    CP.ProgramId,
                    CP.ProgramName,
                    CB.ButtonId,
                    CB.ButtonName,
                    CB.PageId
                FROM ConfigButtons CB
                INNER JOIN ConfigPrograms CP ON CB.ProgramId = CP.ProgramId
                INNER JOIN ConfigSystems CS ON CP.SystemId = CS.SystemId
                LEFT JOIN ConfigSubSystems CSS ON CP.SubSystemId = CSS.SubSystemId
                WHERE CB.Status = '1' AND CP.Status = 'A' AND CS.Status = 'A'";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(request.UserId))
            {
                // 查詢使用者直接權限
                sql += @"
                    AND EXISTS (
                        SELECT 1 FROM UserButtons UB
                        WHERE UB.UserId = @UserId AND UB.ButtonId = CB.ButtonId
                    )";
                parameters.Add("UserId", request.UserId);

                // 查詢使用者資訊
                var userSql = "SELECT UserId, UserName FROM Users WHERE UserId = @UserId";
                var user = await connection.QueryFirstOrDefaultAsync<dynamic>(userSql, parameters);
                if (user != null)
                {
                    response.UserId = user.UserId;
                    response.UserName = user.UserName;
                }

                // 查詢使用者角色權限
                var roleSql = @"
                    SELECT DISTINCT
                        CS.SystemId,
                        CS.SystemName,
                        CSS.SubSystemId AS MenuId,
                        CSS.SubSystemName AS MenuName,
                        CP.ProgramId,
                        CP.ProgramName,
                        CB.ButtonId,
                        CB.ButtonName,
                        CB.PageId
                    FROM ConfigButtons CB
                    INNER JOIN ConfigPrograms CP ON CB.ProgramId = CP.ProgramId
                    INNER JOIN ConfigSystems CS ON CP.SystemId = CS.SystemId
                    LEFT JOIN ConfigSubSystems CSS ON CP.SubSystemId = CSS.SubSystemId
                    INNER JOIN RoleButtons RB ON CB.ButtonId = RB.ButtonId
                    INNER JOIN UserRoles UR ON RB.RoleId = UR.RoleId
                    WHERE CB.Status = '1' AND CP.Status = 'A' AND CS.Status = 'A'
                        AND UR.UserId = @UserId";
                
                var rolePermissions = await connection.QueryAsync<dynamic>(roleSql, parameters);
                foreach (var item in rolePermissions)
                {
                    AddPermissionItem(permissionDict, programDict, item);
                }
            }

            if (!string.IsNullOrEmpty(request.RoleId))
            {
                // 查詢角色權限
                sql += @"
                    AND EXISTS (
                        SELECT 1 FROM RoleButtons RB
                        WHERE RB.RoleId = @RoleId AND RB.ButtonId = CB.ButtonId
                    )";
                parameters.Add("RoleId", request.RoleId);

                // 查詢角色資訊
                var roleSql = "SELECT RoleId, RoleName FROM Roles WHERE RoleId = @RoleId";
                var role = await connection.QueryFirstOrDefaultAsync<dynamic>(roleSql, parameters);
                if (role != null)
                {
                    response.RoleId = role.RoleId;
                    response.RoleName = role.RoleName;
                }
            }

            // 篩選條件
            if (!string.IsNullOrEmpty(request.SystemId))
            {
                sql += " AND CS.SystemId = @SystemId";
                parameters.Add("SystemId", request.SystemId);
            }

            if (!string.IsNullOrEmpty(request.MenuId))
            {
                sql += " AND CSS.SubSystemId = @MenuId";
                parameters.Add("MenuId", request.MenuId);
            }

            if (!string.IsNullOrEmpty(request.ProgramId))
            {
                sql += " AND CP.ProgramId = @ProgramId";
                parameters.Add("ProgramId", request.ProgramId);
            }

            sql += " ORDER BY CS.SystemId, CSS.SubSystemId, CP.ProgramId, CB.ButtonId";

            var results = await connection.QueryAsync<dynamic>(sql, parameters);

            // 組織階層式結構
            foreach (var item in results)
            {
                AddPermissionItem(permissionDict, programDict, item);
            }

            response.Permissions = permissionDict.Values.ToList();
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢系統權限列表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportSystemPermissionReportAsync(SystemPermissionListRequestDto request, string exportFormat)
    {
        try
        {
            var data = await GetSystemPermissionListAsync(request);

            // 扁平化階層式結構為列表
            var exportData = new List<SystemPermissionExportItem>();
            foreach (var system in data.Permissions)
            {
                foreach (var menu in system.Menus)
                {
                    foreach (var program in menu.Programs)
                    {
                        foreach (var button in program.Buttons)
                        {
                            exportData.Add(new SystemPermissionExportItem
                            {
                                UserId = data.UserId ?? string.Empty,
                                UserName = data.UserName ?? string.Empty,
                                RoleId = data.RoleId ?? string.Empty,
                                RoleName = data.RoleName ?? string.Empty,
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
                new ExportColumn { PropertyName = "UserId", DisplayName = "使用者代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "UserName", DisplayName = "使用者名稱", DataType = ExportDataType.String },
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

            var title = "系統權限列表";
            if (!string.IsNullOrEmpty(data.UserId))
            {
                title = $"系統權限列表 - 使用者: {data.UserName} ({data.UserId})";
            }
            else if (!string.IsNullOrEmpty(data.RoleId))
            {
                title = $"系統權限列表 - 角色: {data.RoleName} ({data.RoleId})";
            }

            if (exportFormat == "Excel")
            {
                return _exportHelper.ExportToExcel(exportData, columns, "系統權限列表", title);
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
            _logger.LogError("匯出系統權限報表失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 系統權限匯出項目（扁平化結構）
    /// </summary>
    private class SystemPermissionExportItem
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
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

    /// <summary>
    /// 新增權限項目到階層式結構
    /// </summary>
    private void AddPermissionItem(
        Dictionary<string, SystemPermissionDto> permissionDict,
        Dictionary<string, ProgramPermissionDto> programDict,
        dynamic item)
    {
        var systemId = (string)item.SystemId;
        var systemName = (string)item.SystemName;
        var menuId = (string?)item.MenuId ?? string.Empty;
        var menuName = (string?)item.MenuName ?? string.Empty;
        var programId = (string)item.ProgramId;
        var programName = (string)item.ProgramName;
        var buttonId = (string)item.ButtonId;
        var buttonName = (string)item.ButtonName;
        var pageId = (string?)item.PageId;

        // 系統層級
        if (!permissionDict.ContainsKey(systemId))
        {
            permissionDict[systemId] = new SystemPermissionDto
            {
                SystemId = systemId,
                SystemName = systemName,
                Menus = new List<MenuPermissionDto>()
            };
        }

        // 選單層級（使用 SubSystem 作為 Menu）
        var menuKey = string.IsNullOrEmpty(menuId) ? $"{systemId}_NO_MENU" : $"{systemId}_{menuId}";
        var menuDict = new Dictionary<string, MenuPermissionDto>();
        foreach (var menu in permissionDict[systemId].Menus)
        {
            menuDict[menu.MenuId] = menu;
        }

        MenuPermissionDto menuDto;
        if (!menuDict.ContainsKey(menuId))
        {
            menuDto = new MenuPermissionDto
            {
                MenuId = menuId,
                MenuName = string.IsNullOrEmpty(menuName) ? "(無選單)" : menuName,
                Programs = new List<ProgramPermissionDto>()
            };
            permissionDict[systemId].Menus.Add(menuDto);
            menuDict[menuId] = menuDto;
        }
        else
        {
            menuDto = menuDict[menuId];
        }

        // 作業層級
        var programKey = $"{systemId}_{menuId}_{programId}";
        if (!programDict.ContainsKey(programKey))
        {
            var program = new ProgramPermissionDto
            {
                ProgramId = programId,
                ProgramName = programName,
                Buttons = new List<ButtonPermissionDto>()
            };
            programDict[programKey] = program;
            menuDto.Programs.Add(program);
        }

        // 按鈕層級
        var button = new ButtonPermissionDto
        {
            ButtonId = buttonId,
            ButtonName = buttonName,
            PageId = pageId
        };
        programDict[programKey].Buttons.Add(button);
    }
}

