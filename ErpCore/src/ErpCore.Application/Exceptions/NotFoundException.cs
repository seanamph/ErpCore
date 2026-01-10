namespace ErpCore.Application.Exceptions;

/// <summary>
/// 資源不存在例外
/// 用於處理查詢不到資源的情況
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>
    /// 資源類型名稱
    /// </summary>
    public string? ResourceType { get; set; }

    /// <summary>
    /// 資源識別碼
    /// </summary>
    public string? ResourceId { get; set; }

    /// <summary>
    /// 建構函式
    /// </summary>
    public NotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    /// 建構函式（含資源類型）
    /// </summary>
    public NotFoundException(string resourceType, string resourceId) 
        : base($"{resourceType} (ID: {resourceId}) 不存在")
    {
        ResourceType = resourceType;
        ResourceId = resourceId;
    }

    /// <summary>
    /// 建構函式（含自訂訊息）
    /// </summary>
    public NotFoundException(string message, string resourceType, string resourceId) : base(message)
    {
        ResourceType = resourceType;
        ResourceId = resourceId;
    }

    /// <summary>
    /// 建構函式（含內部例外）
    /// </summary>
    public NotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

