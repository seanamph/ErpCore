namespace ErpCore.Api.ViewModels.Response;

/// <summary>
/// API回應模型
/// 與 ErpCore.Shared.Common.ApiResponse 保持一致
/// </summary>
public class ApiResponse<T>
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 回應訊息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 回應資料
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// 錯誤代碼
    /// </summary>
    public string? ErrorCode { get; set; }
}

