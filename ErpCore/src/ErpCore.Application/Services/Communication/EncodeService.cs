using ErpCore.Application.DTOs.Communication;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Communication;
using ErpCore.Infrastructure.Repositories.Communication;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using System.Text;

namespace ErpCore.Application.Services.Communication;

/// <summary>
/// 編碼服務實作
/// </summary>
public class EncodeService : BaseService, IEncodeService
{
    private readonly IEncodeLogRepository _encodeLogRepository;

    public EncodeService(
        IEncodeLogRepository encodeLogRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _encodeLogRepository = encodeLogRepository;
    }

    public async Task<EncodeResultDto> Base64EncodeAsync(Base64EncodeRequestDto request)
    {
        try
        {
            var encodedData = Convert.ToBase64String(Encoding.UTF8.GetBytes(request.Data));

            if (request.SaveLog)
            {
                await SaveEncodeLogAsync("Base64", request.Data, encodedData, null);
            }

            return new EncodeResultDto
            {
                OriginalData = request.Data,
                EncodedData = encodedData,
                EncodeType = "Base64"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("Base64編碼失敗", ex);
            throw;
        }
    }

    public async Task<EncodeResultDto> Base64DecodeAsync(Base64EncodeRequestDto request)
    {
        try
        {
            var decodedData = Encoding.UTF8.GetString(Convert.FromBase64String(request.Data));

            if (request.SaveLog)
            {
                await SaveEncodeLogAsync("Base64", decodedData, request.Data, null);
            }

            return new EncodeResultDto
            {
                OriginalData = decodedData,
                EncodedData = request.Data,
                EncodeType = "Base64"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("Base64解碼失敗", ex);
            throw;
        }
    }

    public async Task<EncodeResultDto> StringEncodeAsync(StringEncodeRequestDto request)
    {
        try
        {
            // 字元轉3位數字
            var charEncoded = CharEncGen(request.Data, request.KeyKind);
            
            // Base64編碼
            var encodedData = Convert.ToBase64String(Encoding.UTF8.GetBytes(charEncoded));

            if (request.SaveLog)
            {
                await SaveEncodeLogAsync("String", request.Data, encodedData, request.KeyKind);
            }

            return new EncodeResultDto
            {
                OriginalData = request.Data,
                EncodedData = encodedData,
                EncodeType = "String"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("字串加密失敗", ex);
            throw;
        }
    }

    public async Task<EncodeResultDto> StringDecodeAsync(StringEncodeRequestDto request)
    {
        try
        {
            // Base64解碼
            var base64Decoded = Encoding.UTF8.GetString(Convert.FromBase64String(request.Data));
            
            // 3位數字轉字元
            var decodedData = CharDecGen(base64Decoded, request.KeyKind);

            if (request.SaveLog)
            {
                await SaveEncodeLogAsync("String", decodedData, request.Data, request.KeyKind);
            }

            return new EncodeResultDto
            {
                OriginalData = decodedData,
                EncodedData = request.Data,
                EncodeType = "String"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("字串解密失敗", ex);
            throw;
        }
    }

    public async Task<EncodeResultDto> DateEncodeAsync(DateEncodeRequestDto request)
    {
        try
        {
            var encodedData = DateEncGen(request.Data);

            if (request.SaveLog)
            {
                await SaveEncodeLogAsync("Date", request.Data, encodedData, null);
            }

            return new EncodeResultDto
            {
                OriginalData = request.Data,
                EncodedData = encodedData,
                EncodeType = "Date"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("日期加密失敗", ex);
            throw;
        }
    }

    public async Task<EncodeResultDto> DateDecodeAsync(DateEncodeRequestDto request)
    {
        try
        {
            var decodedData = DateDecGen(request.Data);

            if (request.SaveLog)
            {
                await SaveEncodeLogAsync("Date", decodedData, request.Data, null);
            }

            return new EncodeResultDto
            {
                OriginalData = decodedData,
                EncodedData = request.Data,
                EncodeType = "Date"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("日期解密失敗", ex);
            throw;
        }
    }

    private string CharEncGen(string source, string keyKind = "1")
    {
        var result = "";
        foreach (var ch in source)
        {
            var charCode = (int)ch;
            result += charCode.ToString("D3");
        }
        return result;
    }

    private string CharDecGen(string source, string keyKind = "1")
    {
        var result = "";
        for (int i = 0; i < source.Length; i += 3)
        {
            if (i + 3 <= source.Length)
            {
                var charCode = int.Parse(source.Substring(i, 3));
                result += (char)charCode;
            }
        }
        return result;
    }

    private string DateEncGen(string dateStr)
    {
        var result = "";
        foreach (var ch in dateStr)
        {
            var j = (255 - (int)ch) - 180;
            result += j.ToString("D2");
        }
        return result;
    }

    private string DateDecGen(string encodedStr)
    {
        var result = "";
        for (int i = 0; i < encodedStr.Length; i += 2)
        {
            if (i + 2 <= encodedStr.Length)
            {
                var j = int.Parse(encodedStr.Substring(i, 2)) + 180;
                result += (char)(255 - j);
            }
        }
        return result;
    }

    private async Task SaveEncodeLogAsync(string encodeType, string originalData, string encodedData, string? keyKind)
    {
        try
        {
            var log = new EncodeLog
            {
                EncodeType = encodeType,
                OriginalData = originalData,
                EncodedData = encodedData,
                KeyKind = keyKind,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now
            };
            await _encodeLogRepository.CreateAsync(log);
        }
        catch (Exception ex)
        {
            _logger.LogError("儲存編碼記錄失敗", ex);
            // 不拋出異常，避免影響主要功能
        }
    }
}

