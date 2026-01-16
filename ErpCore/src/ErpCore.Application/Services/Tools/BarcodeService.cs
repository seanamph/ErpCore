using System.Drawing;
using System.Drawing.Imaging;
using ErpCore.Application.DTOs.Tools;
using ErpCore.Application.Services.Base;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using ZXing;
using ZXing.Common;

namespace ErpCore.Application.Services.Tools;

/// <summary>
/// 條碼服務實作
/// 使用 ZXing.Net 產生條碼
/// </summary>
public class BarcodeService : BaseService, IBarcodeService
{
    public BarcodeService(ILoggerService logger, IUserContext userContext) : base(logger, userContext)
    {
    }

    public async Task<byte[]> GenerateBarcodeAsync(BarcodeGenerateDto dto)
    {
        try
        {
            _logger.LogInfo($"產生條碼: {dto.Content}, 格式: {dto.Format}");

            BarcodeFormat format;
            switch (dto.Format.ToUpperInvariant())
            {
                case "CODE128":
                    format = BarcodeFormat.CODE_128;
                    break;
                case "EAN13":
                    format = BarcodeFormat.EAN_13;
                    break;
                case "QRCODE":
                    format = BarcodeFormat.QR_CODE;
                    break;
                default:
                    format = BarcodeFormat.CODE_128;
                    break;
            }

            var writer = new BarcodeWriterPixelData
            {
                Format = format,
                Options = new EncodingOptions
                {
                    Height = dto.Height,
                    Width = dto.Width,
                    Margin = 2
                }
            };

            var pixelData = writer.Write(dto.Content);

            // 轉換為 Bitmap
            using var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb);
            var bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, pixelData.Width, pixelData.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppRgb);

            try
            {
                System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            // 如果需要顯示文字，在圖片下方添加文字
            if (dto.ShowText && format != BarcodeFormat.QR_CODE)
            {
                using var graphics = Graphics.FromImage(bitmap);
                graphics.DrawString(dto.Content, new Font("Arial", 10), Brushes.Black, new PointF(5, dto.Height - 20));
            }

            // 轉換為 byte[]
            using var memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, ImageFormat.Png);
            return await Task.FromResult(memoryStream.ToArray());
        }
        catch (Exception ex)
        {
            _logger.LogError($"產生條碼失敗: {dto.Content}", ex);
            throw;
        }
    }

    public async Task<BarcodeValidationResultDto> ValidateBarcodeAsync(BarcodeValidateDto dto)
    {
        try
        {
            _logger.LogInfo($"驗證條碼: {dto.Content}, 格式: {dto.Format}");

            BarcodeFormat format;
            switch (dto.Format.ToUpperInvariant())
            {
                case "CODE128":
                    format = BarcodeFormat.CODE_128;
                    break;
                case "EAN13":
                    format = BarcodeFormat.EAN_13;
                    break;
                case "QRCODE":
                    format = BarcodeFormat.QR_CODE;
                    break;
                default:
                    return new BarcodeValidationResultDto
                    {
                        IsValid = false,
                        Format = dto.Format,
                        ErrorMessage = $"不支援的條碼格式: {dto.Format}"
                    };
            }

            // 基本驗證
            if (string.IsNullOrWhiteSpace(dto.Content))
            {
                return new BarcodeValidationResultDto
                {
                    IsValid = false,
                    Format = dto.Format,
                    ErrorMessage = "條碼內容不能為空"
                };
            }

            // EAN-13 必須是13位數字
            if (format == BarcodeFormat.EAN_13)
            {
                if (dto.Content.Length != 13 || !dto.Content.All(char.IsDigit))
                {
                    return new BarcodeValidationResultDto
                    {
                        IsValid = false,
                        Format = dto.Format,
                        ErrorMessage = "EAN-13 條碼必須是13位數字"
                    };
                }
            }

            return await Task.FromResult(new BarcodeValidationResultDto
            {
                IsValid = true,
                Format = dto.Format
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"驗證條碼失敗: {dto.Content}", ex);
            return new BarcodeValidationResultDto
            {
                IsValid = false,
                Format = dto.Format,
                ErrorMessage = ex.Message
            };
        }
    }
}

