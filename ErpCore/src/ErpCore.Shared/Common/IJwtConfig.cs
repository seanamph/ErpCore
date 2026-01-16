namespace ErpCore.Shared.Common;

/// <summary>
/// JWT 設定介面
/// </summary>
public interface IJwtConfig
{
    /// <summary>
    /// JWT 簽發者
    /// </summary>
    string Issuer { get; }

    /// <summary>
    /// JWT 受眾
    /// </summary>
    string Audience { get; }

    /// <summary>
    /// JWT 密鑰
    /// </summary>
    string SecretKey { get; }

    /// <summary>
    /// JWT 過期時間（分鐘）
    /// </summary>
    int ExpirationMinutes { get; }
}
