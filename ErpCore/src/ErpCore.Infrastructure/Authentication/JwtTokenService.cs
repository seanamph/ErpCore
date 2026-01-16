using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using Microsoft.IdentityModel.Tokens;

namespace ErpCore.Infrastructure.Authentication;

/// <summary>
/// JWT Token 服務
/// 提供 JWT Token 的產生和驗證功能
/// </summary>
public class JwtTokenService
{
    private readonly IJwtConfig _jwtConfig;
    private readonly ILoggerService _logger;
    private readonly SymmetricSecurityKey _signingKey;

    public JwtTokenService(IJwtConfig jwtConfig, ILoggerService logger)
    {
        _jwtConfig = jwtConfig;
        _logger = logger;
        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecretKey));
    }

    /// <summary>
    /// 產生 JWT Token
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="userName">使用者名稱</param>
    /// <param name="roles">角色列表</param>
    /// <returns>JWT Token 字串</returns>
    public string GenerateToken(string userId, string userName, List<string>? roles = null)
    {
        try
        {
            _logger.LogInfo($"產生 JWT Token: {userId}");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, userName),
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // 加入角色聲明
            if (roles != null && roles.Any())
            {
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var credentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(_jwtConfig.ExpirationMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            _logger.LogInfo($"JWT Token 產生成功: {userId}");
            return tokenString;
        }
        catch (Exception ex)
        {
            _logger.LogError($"產生 JWT Token 時發生錯誤: {userId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 驗證 JWT Token
    /// </summary>
    /// <param name="token">JWT Token 字串</param>
    /// <returns>ClaimsPrincipal，驗證失敗則返回 null</returns>
    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogWarning("Token 為空，無法驗證");
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtConfig.Issuer,
                ValidAudience = _jwtConfig.Audience,
                IssuerSigningKey = _signingKey,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            _logger.LogDebug("JWT Token 驗證成功");
            return principal;
        }
        catch (Exception ex)
        {
            _logger.LogError("驗證 JWT Token 時發生錯誤", ex);
            return null;
        }
    }

    /// <summary>
    /// 從 Token 中取得使用者ID
    /// </summary>
    /// <param name="token">JWT Token 字串</param>
    /// <returns>使用者ID</returns>
    public string? GetUserIdFromToken(string token)
    {
        try
        {
            var principal = ValidateToken(token);
            return principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError("從 Token 取得使用者ID 時發生錯誤", ex);
            return null;
        }
    }

    /// <summary>
    /// 從 Token 中取得角色列表
    /// </summary>
    /// <param name="token">JWT Token 字串</param>
    /// <returns>角色列表</returns>
    public List<string> GetRolesFromToken(string token)
    {
        try
        {
            var principal = ValidateToken(token);
            if (principal == null)
            {
                return new List<string>();
            }

            return principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("從 Token 取得角色列表時發生錯誤", ex);
            return new List<string>();
        }
    }
}

