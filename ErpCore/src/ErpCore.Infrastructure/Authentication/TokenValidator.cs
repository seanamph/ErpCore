using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ErpCore.Infrastructure.Authentication;

/// <summary>
/// Token 驗證服務
/// 提供 Token 的驗證和解析功能
/// </summary>
public class TokenValidator
{
    private readonly IJwtConfig _jwtConfig;
    private readonly ILoggerService _logger;
    private readonly JwtTokenService _jwtTokenService;

    public TokenValidator(IJwtConfig jwtConfig, ILoggerService logger, JwtTokenService jwtTokenService)
    {
        _jwtConfig = jwtConfig;
        _logger = logger;
        _jwtTokenService = jwtTokenService;
    }

    /// <summary>
    /// 驗證 Token 是否有效
    /// </summary>
    /// <param name="token">JWT Token 字串</param>
    /// <returns>是否有效</returns>
    public bool IsValidToken(string token)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogWarning("Token 為空，無法驗證");
                return false;
            }

            var principal = _jwtTokenService.ValidateToken(token);
            return principal != null;
        }
        catch (Exception ex)
        {
            _logger.LogError("驗證 Token 時發生錯誤", ex);
            return false;
        }
    }

    /// <summary>
    /// 驗證 Token 是否過期
    /// </summary>
    /// <param name="token">JWT Token 字串</param>
    /// <returns>是否過期</returns>
    public bool IsTokenExpired(string token)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return true;
            }

            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token))
            {
                _logger.LogWarning("無法讀取 Token");
                return true;
            }

            var jwtToken = handler.ReadJwtToken(token);
            var expiration = jwtToken.ValidTo;
            
            return expiration < DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            _logger.LogError("檢查 Token 是否過期時發生錯誤", ex);
            return true;
        }
    }

    /// <summary>
    /// 從 Token 中取得 Claims
    /// </summary>
    /// <param name="token">JWT Token 字串</param>
    /// <returns>Claims 列表</returns>
    public IEnumerable<Claim>? GetClaims(string token)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            var principal = _jwtTokenService.ValidateToken(token);
            return principal?.Claims;
        }
        catch (Exception ex)
        {
            _logger.LogError("從 Token 取得 Claims 時發生錯誤", ex);
            return null;
        }
    }

    /// <summary>
    /// 從 Token 中取得特定 Claim 值
    /// </summary>
    /// <param name="token">JWT Token 字串</param>
    /// <param name="claimType">Claim 類型</param>
    /// <returns>Claim 值</returns>
    public string? GetClaimValue(string token, string claimType)
    {
        try
        {
            var claims = GetClaims(token);
            return claims?.FirstOrDefault(c => c.Type == claimType)?.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError($"從 Token 取得 Claim 值時發生錯誤: {claimType}", ex);
            return null;
        }
    }
}

