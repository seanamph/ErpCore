// 注意：此 Repository 目前未被使用，Service 直接使用 Dapper 查詢資料庫
// 暫時註釋以避免編譯錯誤（Infrastructure 項目不能引用 Application 項目，會造成循環依賴）
// 如需使用此 Repository，請將 DTO 移到 Domain 或 Shared 項目中

/*
using ErpCore.Application.DTOs.System;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 系統作業與功能列表 Repository 介面 (SYS0810)
/// </summary>
public interface ISystemProgramButtonRepository
{
    /// <summary>
    /// 根據系統代碼查詢系統作業與功能列表
    /// </summary>
    Task<SystemProgramButtonQueryDto?> GetSystemProgramButtonsAsync(string systemId);
}
*/
