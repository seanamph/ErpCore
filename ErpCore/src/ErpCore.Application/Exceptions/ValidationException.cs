namespace ErpCore.Application.Exceptions;

/// <summary>
/// 驗證例外
/// 用於處理資料驗證失敗的情況
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// 驗證錯誤字典（欄位名稱 -> 錯誤訊息列表）
    /// </summary>
    public Dictionary<string, List<string>> Errors { get; set; } = new();

    /// <summary>
    /// 建構函式
    /// </summary>
    public ValidationException(string message) : base(message)
    {
    }

    /// <summary>
    /// 建構函式（含驗證錯誤）
    /// </summary>
    public ValidationException(string message, Dictionary<string, List<string>> errors) : base(message)
    {
        Errors = errors;
    }

    /// <summary>
    /// 建構函式（含單一欄位錯誤）
    /// </summary>
    public ValidationException(string fieldName, string errorMessage) : base($"驗證失敗: {fieldName} - {errorMessage}")
    {
        Errors[fieldName] = new List<string> { errorMessage };
    }

    /// <summary>
    /// 建構函式（含內部例外）
    /// </summary>
    public ValidationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

