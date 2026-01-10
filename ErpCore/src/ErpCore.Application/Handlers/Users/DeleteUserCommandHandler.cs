namespace ErpCore.Application.Handlers.Users;

using ErpCore.Application.Commands.Users;
using NLog;

/// <summary>
/// 刪除使用者命令處理器
/// 用於 CQRS 模式（如採用 CQRS）
/// </summary>
public class DeleteUserCommandHandler
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    // 注意：此專案目前不使用 CQRS 模式，此檔案作為模板參考
    // 如需使用 CQRS，請安裝 MediatR 套件並實作 IRequestHandler 介面

    /// <summary>
    /// 處理刪除使用者命令
    /// </summary>
    public async Task<bool> Handle(DeleteUserCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            Logger.Info($"處理刪除使用者命令：UserId={command.UserId}");

            // TODO: 實作刪除使用者的邏輯
            // 1. 驗證命令資料
            // 2. 檢查使用者是否存在
            // 3. 檢查是否有關聯資料
            // 4. 刪除使用者（軟刪除或硬刪除）
            // 5. 儲存到資料庫
            // 6. 返回結果

            await Task.CompletedTask;
            return true;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"處理刪除使用者命令時發生錯誤：UserId={command.UserId}");
            throw;
        }
    }
}

