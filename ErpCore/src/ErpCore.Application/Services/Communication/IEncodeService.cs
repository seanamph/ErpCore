using ErpCore.Application.DTOs.Communication;

namespace ErpCore.Application.Services.Communication;

/// <summary>
/// 編碼服務介面
/// </summary>
public interface IEncodeService
{
    /// <summary>
    /// Base64編碼
    /// </summary>
    Task<EncodeResultDto> Base64EncodeAsync(Base64EncodeRequestDto request);

    /// <summary>
    /// Base64解碼
    /// </summary>
    Task<EncodeResultDto> Base64DecodeAsync(Base64EncodeRequestDto request);

    /// <summary>
    /// 字串加密
    /// </summary>
    Task<EncodeResultDto> StringEncodeAsync(StringEncodeRequestDto request);

    /// <summary>
    /// 字串解密
    /// </summary>
    Task<EncodeResultDto> StringDecodeAsync(StringEncodeRequestDto request);

    /// <summary>
    /// 日期加密
    /// </summary>
    Task<EncodeResultDto> DateEncodeAsync(DateEncodeRequestDto request);

    /// <summary>
    /// 日期解密
    /// </summary>
    Task<EncodeResultDto> DateDecodeAsync(DateEncodeRequestDto request);
}

