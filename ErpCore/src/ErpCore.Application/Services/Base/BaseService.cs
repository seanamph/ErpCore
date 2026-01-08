using ErpCore.Shared.Logging;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Base;

/// <summary>
/// 基礎服務類別
/// 提供統一的日誌記錄和使用者上下文
/// </summary>
public abstract class BaseService
{
    protected readonly ILoggerService _logger;
    protected readonly IUserContext _userContext;

    protected BaseService(ILoggerService logger, IUserContext userContext)
    {
        _logger = logger;
        _userContext = userContext;
    }

    /// <summary>
    /// 取得當前使用者 ID
    /// </summary>
    protected string GetCurrentUserId() => _userContext.GetUserId() ?? "SYSTEM";

    /// <summary>
    /// 取得當前使用者名稱
    /// </summary>
    protected string GetCurrentUserName() => _userContext.GetUserName() ?? "SYSTEM";

    /// <summary>
    /// 取得當前使用者組織 ID
    /// </summary>
    protected string? GetCurrentOrgId() => _userContext.GetOrgId();
}

