using Dapper;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.Base;
using ErpCore.Infrastructure.Data;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 權限分類報表服務實作 (SYS0770)
/// </summary>
public class PermissionCategoryReportService : BaseService, IPermissionCategoryReportService
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ExportHelper _exportHelper;

    public PermissionCategoryReportService(
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext,
        ExportHelper exportHelper) : base(logger, userContext)
    {
        _connectionFactory = connectionFactory;
        _exportHelper = exportHelper;
    }

    /// <summary>
    /// 查詢權限分類報表
    /// </summary>
    public async Task<PermissionCategoryReportResponseDto> GetPermissionCategoryReportAsync(PermissionCategoryReportRequestDto request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.PermissionType))
            {
                throw new ArgumentException("授權型態不能為空");
            }

            if (request.PermissionType != "1" && request.PermissionType != "2")
            {
                throw new ArgumentException("授權型態必須為 1(角色) 或 2(使用者)");
            }

            var response = new PermissionCategoryReportResponseDto
            {
                PermissionType = request.PermissionType,
                PermissionTypeName = request.PermissionType == "1" ? "角色" : "使用者"
            };

            using var connection = _connectionFactory.CreateConnection();

            if (request.PermissionType == "1")
            {
                // 查詢角色權限分類報表
                var sql = @"
                    SELECT DISTINCT
                        U.UserId,
                        U.UserName,
                        R.RoleId,
                        R.RoleName
                    FROM RoleButtons RB
                    INNER JOIN UserRoles UR ON RB.RoleId = UR.RoleId
                    INNER JOIN Users U ON UR.UserId = U.UserId
                    INNER JOIN Roles R ON UR.RoleId = R.RoleId
                    WHERE RB.RoleId IS NOT NULL
                    ORDER BY U.UserId";

                var items = await connection.QueryAsync<dynamic>(sql);
                var seqNo = 1;
                foreach (var item in items)
                {
                    response.Items.Add(new PermissionCategoryReportItemDto
                    {
                        SeqNo = seqNo++,
                        UserId = item.UserId,
                        UserName = item.UserName ?? string.Empty,
                        RoleId = item.RoleId,
                        RoleName = item.RoleName ?? string.Empty
                    });
                }
            }
            else
            {
                // 查詢使用者權限分類報表
                var sql = @"
                    SELECT DISTINCT
                        U.UserId,
                        U.UserName
                    FROM UserButtons UB
                    INNER JOIN Users U ON UB.UserId = U.UserId
                    WHERE UB.UserId IS NOT NULL
                    ORDER BY U.UserId";

                var items = await connection.QueryAsync<dynamic>(sql);
                var seqNo = 1;
                foreach (var item in items)
                {
                    response.Items.Add(new PermissionCategoryReportItemDto
                    {
                        SeqNo = seqNo++,
                        UserId = item.UserId,
                        UserName = item.UserName ?? string.Empty
                    });
                }
            }

            _logger.LogInfo($"查詢權限分類報表成功，授權型態：{response.PermissionTypeName}，共 {response.Items.Count} 筆");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢權限分類報表失敗：{ex.Message}", ex);
            throw;
        }
    }

    /// <summary>
    /// 匯出權限分類報表
    /// </summary>
    public async Task<byte[]> ExportPermissionCategoryReportAsync(PermissionCategoryReportRequestDto request, string exportFormat)
    {
        try
        {
            var data = await GetPermissionCategoryReportAsync(request);

            // 定義匯出欄位
            var columns = new List<ExportColumn>();
            if (request.PermissionType == "1")
            {
                // 角色權限分類報表
                columns = new List<ExportColumn>
                {
                    new ExportColumn { PropertyName = "SeqNo", DisplayName = "序號", DataType = ExportDataType.Number },
                    new ExportColumn { PropertyName = "UserId", DisplayName = "使用者代碼", DataType = ExportDataType.String },
                    new ExportColumn { PropertyName = "UserName", DisplayName = "使用者名稱", DataType = ExportDataType.String },
                    new ExportColumn { PropertyName = "RoleId", DisplayName = "角色代碼", DataType = ExportDataType.String },
                    new ExportColumn { PropertyName = "RoleName", DisplayName = "角色名稱", DataType = ExportDataType.String }
                };
            }
            else
            {
                // 使用者權限分類報表
                columns = new List<ExportColumn>
                {
                    new ExportColumn { PropertyName = "SeqNo", DisplayName = "序號", DataType = ExportDataType.Number },
                    new ExportColumn { PropertyName = "UserId", DisplayName = "使用者代碼", DataType = ExportDataType.String },
                    new ExportColumn { PropertyName = "UserName", DisplayName = "使用者名稱", DataType = ExportDataType.String }
                };
            }

            var title = $"權限分類報表 - {data.PermissionTypeName}";

            if (exportFormat == "Excel")
            {
                return _exportHelper.ExportToExcel(data.Items, columns, "權限分類報表", title);
            }
            else if (exportFormat == "PDF")
            {
                return _exportHelper.ExportToPdf(data.Items, columns, title);
            }
            else
            {
                throw new ArgumentException($"不支援的匯出格式: {exportFormat}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"匯出權限分類報表失敗：{ex.Message}", ex);
            throw;
        }
    }
}

