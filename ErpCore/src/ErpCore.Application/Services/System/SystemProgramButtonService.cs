using Dapper;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.Base;
using ErpCore.Infrastructure.Data;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 系統作業與功能列表服務實作 (SYS0810, SYS0780, SYS0999)
/// </summary>
public class SystemProgramButtonService : BaseService, ISystemProgramButtonService
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ExportHelper _exportHelper;

    public SystemProgramButtonService(
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext,
        ExportHelper exportHelper) : base(logger, userContext)
    {
        _connectionFactory = connectionFactory;
        _exportHelper = exportHelper;
    }

    /// <summary>
    /// 查詢系統作業與功能列表
    /// </summary>
    public async Task<SystemProgramButtonResponseDto> GetSystemProgramButtonsAsync(string systemId)
    {
        try
        {
            if (string.IsNullOrEmpty(systemId))
            {
                throw new ArgumentException("系統代碼不能為空");
            }

            using var connection = _connectionFactory.CreateConnection();

            // 查詢系統資訊
            var systemSql = @"
                SELECT SystemId, SystemName
                FROM Systems
                WHERE SystemId = @SystemId AND Status = 'A'";

            var system = await connection.QueryFirstOrDefaultAsync<dynamic>(systemSql, new { SystemId = systemId });
            if (system == null)
            {
                throw new ArgumentException($"系統代碼 {systemId} 不存在或已停用");
            }

            var response = new SystemProgramButtonResponseDto
            {
                SystemId = system.SystemId,
                SystemName = system.SystemName ?? string.Empty
            };

            // 查詢作業列表（透過 Menu 關聯到 System）
            var programSql = @"
                SELECT DISTINCT
                    p.ProgramId,
                    p.ProgramName,
                    p.ProgramType,
                    p.SeqNo
                FROM Programs p
                INNER JOIN Menus m ON p.MenuId = m.MenuId
                WHERE m.SystemId = @SystemId
                    AND p.Status = '1'
                    AND m.Status = '1'
                ORDER BY p.SeqNo, p.ProgramId";

            var programs = await connection.QueryAsync<dynamic>(programSql, new { SystemId = systemId });

            var programDict = new Dictionary<string, ProgramButtonDto>();

            foreach (var program in programs)
            {
                var programDto = new ProgramButtonDto
                {
                    ProgramId = program.ProgramId,
                    ProgramName = program.ProgramName ?? string.Empty,
                    ProgramType = program.ProgramType,
                    SeqNo = program.SeqNo ?? 0
                };

                // 查詢該作業的功能按鈕列表
                var buttonSql = @"
                    SELECT
                        ButtonId,
                        ButtonName,
                        ButtonAttr AS ButtonType,
                        NULL AS SeqNo
                    FROM Buttons
                    WHERE ProgramId = @ProgramId
                        AND Status = '1'
                    ORDER BY ButtonId";

                var buttons = await connection.QueryAsync<dynamic>(buttonSql, new { ProgramId = program.ProgramId });

                foreach (var button in buttons)
                {
                    programDto.Buttons.Add(new ButtonDto
                    {
                        ButtonId = button.ButtonId,
                        ButtonName = button.ButtonName ?? string.Empty,
                        ButtonType = button.ButtonType,
                        PageId = null,
                        SeqNo = button.SeqNo ?? 0
                    });
                }

                response.Programs.Add(programDto);
            }

            _logger.LogInfo($"查詢系統作業與功能列表成功，系統代碼：{systemId}，共 {response.Programs.Count} 個作業");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢系統作業與功能列表失敗：{ex.Message}", ex);
            throw;
        }
    }

    /// <summary>
    /// 匯出系統作業與功能列表報表
    /// </summary>
    public async Task<byte[]> ExportSystemProgramButtonsAsync(string systemId, string exportFormat)
    {
        try
        {
            var data = await GetSystemProgramButtonsAsync(systemId);

            // 扁平化階層式結構為列表
            var exportData = new List<SystemProgramButtonExportItem>();
            foreach (var program in data.Programs)
            {
                foreach (var button in program.Buttons)
                {
                    exportData.Add(new SystemProgramButtonExportItem
                    {
                        SystemId = data.SystemId,
                        SystemName = data.SystemName,
                        ProgramId = program.ProgramId,
                        ProgramName = program.ProgramName,
                        ProgramType = program.ProgramType ?? string.Empty,
                        SeqNo = program.SeqNo,
                        ButtonId = button.ButtonId,
                        ButtonName = button.ButtonName,
                        ButtonType = button.ButtonType ?? string.Empty,
                        PageId = button.PageId ?? string.Empty,
                        ButtonSeqNo = button.SeqNo
                    });
                }
            }

            // 定義匯出欄位
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SystemId", DisplayName = "系統代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SystemName", DisplayName = "系統名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ProgramId", DisplayName = "作業代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ProgramName", DisplayName = "作業名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ProgramType", DisplayName = "作業類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SeqNo", DisplayName = "作業順序", DataType = ExportDataType.Number },
                new ExportColumn { PropertyName = "ButtonId", DisplayName = "按鈕代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ButtonName", DisplayName = "按鈕名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ButtonType", DisplayName = "按鈕類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "PageId", DisplayName = "頁面代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ButtonSeqNo", DisplayName = "按鈕順序", DataType = ExportDataType.Number }
            };

            var title = $"系統作業與功能列表 - {data.SystemName} ({data.SystemId})";

            if (exportFormat == "Excel")
            {
                return _exportHelper.ExportToExcel(exportData, columns, "系統作業與功能列表", title);
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
            _logger.LogError($"匯出系統作業與功能列表報表失敗：{ex.Message}", ex);
            throw;
        }
    }

    /// <summary>
    /// 查詢系統作業與功能列表（出庫用）(SYS0999)
    /// </summary>
    public async Task<SystemProgramButtonResponseDto> GetSystemProgramButtonsForExportAsync(string systemId)
    {
        // 出庫用查詢邏輯與一般查詢相同，但可針對出庫用途優化
        return await GetSystemProgramButtonsAsync(systemId);
    }

    /// <summary>
    /// 匯出系統作業與功能列表報表（出庫用）(SYS0999)
    /// </summary>
    public async Task<byte[]> ExportSystemProgramButtonsReportAsync(string systemId, string exportFormat)
    {
        try
        {
            var data = await GetSystemProgramButtonsForExportAsync(systemId);

            // 扁平化階層式結構為列表（針對出庫用途優化）
            var exportData = new List<SystemProgramButtonExportItem>();
            foreach (var program in data.Programs)
            {
                foreach (var button in program.Buttons)
                {
                    exportData.Add(new SystemProgramButtonExportItem
                    {
                        SystemId = data.SystemId,
                        SystemName = data.SystemName,
                        ProgramId = program.ProgramId,
                        ProgramName = program.ProgramName,
                        ProgramType = program.ProgramType ?? string.Empty,
                        SeqNo = program.SeqNo,
                        ButtonId = button.ButtonId,
                        ButtonName = button.ButtonName,
                        ButtonType = button.ButtonType ?? string.Empty,
                        PageId = button.PageId ?? string.Empty,
                        ButtonSeqNo = button.SeqNo
                    });
                }
            }

            // 定義匯出欄位（針對出庫用途）
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SystemId", DisplayName = "系統代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SystemName", DisplayName = "系統名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ProgramId", DisplayName = "作業代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ProgramName", DisplayName = "作業名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ProgramType", DisplayName = "作業類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SeqNo", DisplayName = "作業順序", DataType = ExportDataType.Number },
                new ExportColumn { PropertyName = "ButtonId", DisplayName = "按鈕代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ButtonName", DisplayName = "按鈕名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ButtonType", DisplayName = "按鈕類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "PageId", DisplayName = "頁面代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ButtonSeqNo", DisplayName = "按鈕順序", DataType = ExportDataType.Number }
            };

            var title = $"系統作業與功能列表（出庫用） - {data.SystemName} ({data.SystemId})";

            if (exportFormat == "Excel")
            {
                return _exportHelper.ExportToExcel(exportData, columns, "系統作業與功能列表（出庫用）", title);
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
            _logger.LogError($"匯出系統作業與功能列表報表（出庫用）失敗：{ex.Message}", ex);
            throw;
        }
    }

    /// <summary>
    /// 系統作業與功能匯出項目（扁平化結構）
    /// </summary>
    private class SystemProgramButtonExportItem
    {
        public string SystemId { get; set; } = string.Empty;
        public string SystemName { get; set; } = string.Empty;
        public string ProgramId { get; set; } = string.Empty;
        public string ProgramName { get; set; } = string.Empty;
        public string ProgramType { get; set; } = string.Empty;
        public int SeqNo { get; set; }
        public string ButtonId { get; set; } = string.Empty;
        public string ButtonName { get; set; } = string.Empty;
        public string ButtonType { get; set; } = string.Empty;
        public string PageId { get; set; } = string.Empty;
        public int ButtonSeqNo { get; set; }
    }
}

