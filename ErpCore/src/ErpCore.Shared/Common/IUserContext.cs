namespace ErpCore.Shared.Common;

/// <summary>
/// 使用者上下文介面
/// 提供取得當前使用者資訊的功能
/// </summary>
public interface IUserContext
{
    /// <summary>
    /// 取得當前使用者 ID
    /// </summary>
    string? GetUserId();

    /// <summary>
    /// 取得當前使用者名稱
    /// </summary>
    string? GetUserName();

    /// <summary>
    /// 取得當前使用者組織 ID
    /// </summary>
    string? GetOrgId();
}

