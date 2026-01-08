namespace ErpCore.Application.DTOs.Communication;

/// <summary>
/// 編碼結果 DTO
/// </summary>
public class EncodeResultDto
{
    public string OriginalData { get; set; } = string.Empty;
    public string EncodedData { get; set; } = string.Empty;
    public string EncodeType { get; set; } = string.Empty;
}

/// <summary>
/// Base64編碼請求 DTO
/// </summary>
public class Base64EncodeRequestDto
{
    public string Data { get; set; } = string.Empty;
    public bool SaveLog { get; set; } = false;
}

/// <summary>
/// 字串加密請求 DTO
/// </summary>
public class StringEncodeRequestDto
{
    public string Data { get; set; } = string.Empty;
    public string KeyKind { get; set; } = "1";
    public bool SaveLog { get; set; } = false;
}

/// <summary>
/// 日期加密請求 DTO
/// </summary>
public class DateEncodeRequestDto
{
    public string Data { get; set; } = string.Empty;
    public bool SaveLog { get; set; } = false;
}

/// <summary>
/// 資料編碼請求 DTO
/// </summary>
public class DataEncodeRequestDto
{
    public Dictionary<string, object> Data { get; set; } = new();
    public string UseDownGo { get; set; } = "0";
    public bool SaveLog { get; set; } = false;
}

