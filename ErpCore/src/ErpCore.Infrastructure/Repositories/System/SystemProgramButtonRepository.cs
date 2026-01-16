// 注意：此 Repository 目前未被使用，Service 直接使用 Dapper 查詢資料庫
// 暫時註釋以避免編譯錯誤（Infrastructure 項目不能引用 Application 項目，會造成循環依賴）
// 如需使用此 Repository，請將 DTO 移到 Domain 或 Shared 項目中

/*
using Dapper;
using ErpCore.Application.DTOs.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 系統作業與功能列表 Repository 實作 (SYS0810)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class SystemProgramButtonRepository : BaseRepository, ISystemProgramButtonRepository
{
    public SystemProgramButtonRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<SystemProgramButtonQueryDto?> GetSystemProgramButtonsAsync(string systemId)
    {
        try
        {
            // 查詢系統資訊
            const string systemSql = @"
                SELECT SystemId, SystemName
                FROM Systems
                WHERE SystemId = @SystemId AND Status = 'A'";

            var system = await QueryFirstOrDefaultAsync<SystemProgramButtonQueryDto>(systemSql, new { SystemId = systemId });

            if (system == null)
            {
                return null;
            }

            // 查詢作業列表（透過 Menu 關聯到 System）
            const string programSql = @"
                SELECT 
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

            var programs = await QueryAsync<ProgramButtonDto>(programSql, new { SystemId = systemId });

            // 查詢每個作業的按鈕列表
            foreach (var program in programs)
            {
                const string buttonSql = @"
                    SELECT 
                        ButtonId,
                        ButtonName,
                        ButtonAttr AS ButtonType,
                        NULL AS SeqNo
                    FROM Buttons
                    WHERE ProgramId = @ProgramId 
                        AND Status = '1'
                    ORDER BY ButtonId";

                var buttons = await QueryAsync<ButtonInfoDto>(buttonSql, new { ProgramId = program.ProgramId });
                program.Buttons = buttons.ToList();
            }

            system.Programs = programs.ToList();

            return system;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢系統作業與功能列表失敗: {systemId}", ex);
            throw;
        }
    }
}
*/
