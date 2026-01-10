namespace ErpCore.Application.Handlers.Users;

using ErpCore.Application.Queries.Users;
using ErpCore.Application.DTOs.System;
using NLog;

/// <summary>
/// 取得使用者查詢處理器
/// 用於 CQRS 模式（如採用 CQRS）
/// </summary>
public class GetUserQueryHandler
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    // 注意：此專案目前不使用 CQRS 模式，此檔案作為模板參考
    // 如需使用 CQRS，請安裝 MediatR 套件並實作 IRequestHandler 介面

    /// <summary>
    /// 處理取得使用者查詢
    /// </summary>
    public async Task<UserDto?> Handle(GetUserQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            Logger.Info($"處理取得使用者查詢：UserId={query.UserId}");

            // TODO: 實作取得使用者的邏輯
            // 1. 驗證查詢資料
            // 2. 從資料庫查詢使用者
            // 3. 轉換為 DTO
            // 4. 返回結果

            await Task.CompletedTask;
            return null;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"處理取得使用者查詢時發生錯誤：UserId={query.UserId}");
            throw;
        }
    }
}

