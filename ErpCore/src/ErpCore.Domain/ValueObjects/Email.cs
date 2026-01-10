namespace ErpCore.Domain.ValueObjects;

/// <summary>
/// 電子郵件值物件
/// </summary>
public class Email
{
    /// <summary>
    /// 電子郵件地址
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// 建構函式
    /// </summary>
    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("電子郵件地址不能為空", nameof(value));
        }

        if (!IsValidEmail(value))
        {
            throw new ArgumentException("無效的電子郵件地址格式", nameof(value));
        }

        Value = value;
    }

    /// <summary>
    /// 驗證電子郵件格式
    /// </summary>
    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public override string ToString() => Value;
}

