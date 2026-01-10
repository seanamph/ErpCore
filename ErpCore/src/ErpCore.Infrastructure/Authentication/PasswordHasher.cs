using System.Security.Cryptography;
using System.Text;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Authentication;

/// <summary>
/// 密碼雜湊服務
/// 提供密碼的雜湊和驗證功能
/// </summary>
public class PasswordHasher
{
    private readonly ILoggerService _logger;
    private const int SaltSize = 16; // 128 bits
    private const int HashSize = 32; // 256 bits
    private const int Iterations = 10000; // PBKDF2 迭代次數

    public PasswordHasher(ILoggerService logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 雜湊密碼
    /// </summary>
    /// <param name="password">原始密碼</param>
    /// <returns>雜湊後的密碼字串（包含 Salt 和 Hash）</returns>
    public string HashPassword(string password)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                _logger.LogWarning("密碼為空，無法進行雜湊");
                throw new ArgumentException("密碼不能為空", nameof(password));
            }

            // 產生隨機 Salt
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);

            // 使用 PBKDF2 進行雜湊
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(HashSize);

            // 組合 Salt 和 Hash
            var hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            // 轉換為 Base64 字串
            var hashString = Convert.ToBase64String(hashBytes);
            _logger.LogDebug("密碼雜湊成功");
            return hashString;
        }
        catch (Exception ex)
        {
            _logger.LogError("雜湊密碼時發生錯誤", ex);
            throw;
        }
    }

    /// <summary>
    /// 驗證密碼
    /// </summary>
    /// <param name="password">原始密碼</param>
    /// <param name="hashedPassword">雜湊後的密碼字串</param>
    /// <returns>是否驗證成功</returns>
    public bool VerifyPassword(string password, string hashedPassword)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                _logger.LogWarning("密碼為空，無法驗證");
                return false;
            }

            if (string.IsNullOrWhiteSpace(hashedPassword))
            {
                _logger.LogWarning("雜湊密碼為空，無法驗證");
                return false;
            }

            // 從 Base64 字串還原 Salt 和 Hash
            var hashBytes = Convert.FromBase64String(hashedPassword);
            
            if (hashBytes.Length != SaltSize + HashSize)
            {
                _logger.LogWarning("雜湊密碼格式錯誤");
                return false;
            }

            var salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            // 使用相同的 Salt 和迭代次數重新計算 Hash
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(HashSize);

            // 比較 Hash 是否相同
            for (int i = 0; i < HashSize; i++)
            {
                if (hashBytes[i + SaltSize] != hash[i])
                {
                    _logger.LogDebug("密碼驗證失敗");
                    return false;
                }
            }

            _logger.LogDebug("密碼驗證成功");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("驗證密碼時發生錯誤", ex);
            return false;
        }
    }
}

