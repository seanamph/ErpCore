namespace ErpCore.Application.Exceptions;

/// <summary>
/// 業務邏輯例外
/// 用於處理業務規則違反的情況
/// </summary>
public class BusinessException : Exception
{
    /// <summary>
    /// 錯誤代碼
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// 建構函式
    /// </summary>
    public BusinessException(string message) : base(message)
    {
    }

    /// <summary>
    /// 建構函式（含錯誤代碼）
    /// </summary>
    public BusinessException(string message, string errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// 建構函式（含內部例外）
    /// </summary>
    public BusinessException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// 建構函式（含錯誤代碼和內部例外）
    /// </summary>
    public BusinessException(string message, string errorCode, Exception innerException) : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}

