using System.Drawing;
using System.Drawing.Imaging;
using ZXing;
using ZXing.Common;

namespace ErpCore.Shared.Common;

/// <summary>
/// 條碼生成工具類
/// 使用 ZXing.Net 生成條碼圖片
/// </summary>
public static class BarcodeHelper
{
    /// <summary>
    /// 生成條碼圖片（Base64 字串）
    /// </summary>
    /// <param name="barcodeText">條碼文字</param>
    /// <param name="width">圖片寬度（預設 300）</param>
    /// <param name="height">圖片高度（預設 100）</param>
    /// <param name="format">條碼格式（預設 CODE_128）</param>
    /// <returns>Base64 編碼的圖片字串</returns>
    public static string GenerateBarcodeBase64(
        string barcodeText,
        int width = 300,
        int height = 100,
        BarcodeFormat format = BarcodeFormat.CODE_128)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(barcodeText))
            {
                return string.Empty;
            }

            var writer = new BarcodeWriterPixelData
            {
                Format = format,
                Options = new EncodingOptions
                {
                    Height = height,
                    Width = width,
                    Margin = 2
                }
            };

            var pixelData = writer.Write(barcodeText);

            // 將像素資料轉換為 Bitmap
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

            // 轉換為 Base64 字串
            using var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            var imageBytes = ms.ToArray();
            return Convert.ToBase64String(imageBytes);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"生成條碼失敗: {ex.Message}");
            return string.Empty;
        }
    }

    /// <summary>
    /// 生成條碼圖片（位元組陣列）
    /// </summary>
    /// <param name="barcodeText">條碼文字</param>
    /// <param name="width">圖片寬度（預設 300）</param>
    /// <param name="height">圖片高度（預設 100）</param>
    /// <param name="format">條碼格式（預設 CODE_128）</param>
    /// <returns>PNG 格式的圖片位元組陣列</returns>
    public static byte[] GenerateBarcodeBytes(
        string barcodeText,
        int width = 300,
        int height = 100,
        BarcodeFormat format = BarcodeFormat.CODE_128)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(barcodeText))
            {
                return Array.Empty<byte>();
            }

            var writer = new BarcodeWriterPixelData
            {
                Format = format,
                Options = new EncodingOptions
                {
                    Height = height,
                    Width = width,
                    Margin = 2
                }
            };

            var pixelData = writer.Write(barcodeText);

            // 將像素資料轉換為 Bitmap
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

            // 轉換為位元組陣列
            using var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"生成條碼失敗: {ex.Message}");
            return Array.Empty<byte>();
        }
    }

    /// <summary>
    /// 根據條碼文字自動判斷條碼格式
    /// </summary>
    /// <param name="barcodeText">條碼文字</param>
    /// <returns>條碼格式</returns>
    public static BarcodeFormat DetectBarcodeFormat(string barcodeText)
    {
        if (string.IsNullOrWhiteSpace(barcodeText))
        {
            return BarcodeFormat.CODE_128;
        }

        // 根據條碼長度和格式判斷
        // EAN-13: 13 位數字
        if (barcodeText.Length == 13 && barcodeText.All(char.IsDigit))
        {
            return BarcodeFormat.EAN_13;
        }

        // EAN-8: 8 位數字
        if (barcodeText.Length == 8 && barcodeText.All(char.IsDigit))
        {
            return BarcodeFormat.EAN_8;
        }

        // UPC-A: 12 位數字
        if (barcodeText.Length == 12 && barcodeText.All(char.IsDigit))
        {
            return BarcodeFormat.UPC_A;
        }

        // CODE 39: 通常包含字母和數字
        if (barcodeText.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '.' || c == ' ' || c == '$' || c == '/' || c == '+' || c == '%'))
        {
            return BarcodeFormat.CODE_39;
        }

        // 預設使用 CODE_128（支援 ASCII 字元）
        return BarcodeFormat.CODE_128;
    }
}

