using System.Text;
using OfficeOpenXml;
using ErpCore.Application.DTOs.InventoryCheck;

namespace ErpCore.Shared.Common;

/// <summary>
/// 檔案解析工具類
/// 支援 CSV 和 Excel 檔案解析
/// </summary>
public static class FileParser
{
    /// <summary>
    /// 解析盤點資料檔案
    /// 支援格式：CSV (使用 | 分隔符)、Excel (.xls, .xlsx)
    /// </summary>
    /// <param name="file">上傳的檔案</param>
    /// <returns>解析後的盤點資料列表</returns>
    public static async Task<List<StocktakingTempDto>> ParseStocktakingFileAsync(Stream fileStream, string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLower();
        var result = new List<StocktakingTempDto>();

        switch (extension)
        {
            case ".csv":
            case ".txt":
                result = await ParseCsvFileAsync(fileStream);
                break;
            case ".xls":
            case ".xlsx":
                result = await ParseExcelFileAsync(fileStream);
                break;
            default:
                throw new NotSupportedException($"不支援的檔案格式: {extension}");
        }

        return result;
    }

    /// <summary>
    /// 解析 CSV 檔案（使用 | 分隔符）
    /// 檔案格式：PlanId|ShopId|GoodsId|Kind|ShelfNo|SerialNo|Qty|IQty
    /// </summary>
    private static async Task<List<StocktakingTempDto>> ParseCsvFileAsync(Stream fileStream)
    {
        var result = new List<StocktakingTempDto>();
        using var reader = new StreamReader(fileStream, Encoding.Default);
        string? line;
        int lineNumber = 0;

        while ((line = await reader.ReadLineAsync()) != null)
        {
            lineNumber++;

            // 跳過空行
            if (string.IsNullOrWhiteSpace(line))
                continue;

            // 跳過 EOF 標記
            if (line.Trim().Equals("EOF", StringComparison.OrdinalIgnoreCase))
                break;

            try
            {
                var fields = line.Split('|', StringSplitOptions.RemoveEmptyEntries);
                
                // 至少需要 3 個欄位：ShopId, GoodsId, Qty
                if (fields.Length < 3)
                {
                    continue; // 跳過格式不正確的行
                }

                var dto = new StocktakingTempDto
                {
                    ShopId = fields.Length > 0 ? fields[0].Trim() : string.Empty,
                    GoodsId = fields.Length > 1 ? fields[1].Trim() : string.Empty,
                    Qty = fields.Length > 2 && decimal.TryParse(fields[2].Trim(), out var qty) ? qty : 0,
                    IQty = fields.Length > 3 && decimal.TryParse(fields[3].Trim(), out var iqty) ? iqty : 0,
                    Kind = fields.Length > 4 ? fields[4].Trim() : null,
                    ShelfNo = fields.Length > 5 ? fields[5].Trim() : null,
                    SerialNo = fields.Length > 6 && int.TryParse(fields[6].Trim(), out var serialNo) ? serialNo : null
                };

                // 驗證必填欄位
                if (string.IsNullOrWhiteSpace(dto.ShopId) || string.IsNullOrWhiteSpace(dto.GoodsId))
                {
                    continue; // 跳過必填欄位為空的行
                }

                result.Add(dto);
            }
            catch (Exception ex)
            {
                // 記錄解析錯誤但繼續處理其他行
                // 可以考慮記錄到日誌
                System.Diagnostics.Debug.WriteLine($"解析第 {lineNumber} 行失敗: {ex.Message}");
            }
        }

        return result;
    }

    /// <summary>
    /// 解析 Excel 檔案
    /// 使用 EPPlus 套件解析 .xlsx 檔案
    /// Excel 格式：第一行為標題行（可選），資料從第二行開始
    /// 欄位順序：ShopId, GoodsId, Qty, IQty, Kind, ShelfNo, SerialNo
    /// </summary>
    private static async Task<List<StocktakingTempDto>> ParseExcelFileAsync(Stream fileStream)
    {
        var result = new List<StocktakingTempDto>();
        
        try
        {
            // 設定 EPPlus 授權上下文（非商業使用）
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            
            using var package = new ExcelPackage(fileStream);
            var worksheet = package.Workbook.Worksheets[0];
            
            if (worksheet == null || worksheet.Dimension == null)
            {
                return result;
            }

            var startRow = worksheet.Dimension.Start.Row;
            var endRow = worksheet.Dimension.End.Row;
            var startCol = worksheet.Dimension.Start.Column;
            var endCol = worksheet.Dimension.End.Column;

            // 判斷第一行是否為標題行（檢查是否包含 "ShopId" 或 "店別" 等關鍵字）
            var firstRowValue = worksheet.Cells[startRow, startCol].Text?.Trim() ?? string.Empty;
            var hasHeader = firstRowValue.Contains("ShopId", StringComparison.OrdinalIgnoreCase) ||
                           firstRowValue.Contains("店別", StringComparison.OrdinalIgnoreCase) ||
                           firstRowValue.Contains("店舖", StringComparison.OrdinalIgnoreCase);

            var dataStartRow = hasHeader ? startRow + 1 : startRow;

            // 從資料行開始解析
            for (int row = dataStartRow; row <= endRow; row++)
            {
                try
                {
                    // 讀取各欄位值
                    var shopId = worksheet.Cells[row, startCol].Text?.Trim() ?? string.Empty;
                    var goodsId = row <= endRow && startCol + 1 <= endCol 
                        ? worksheet.Cells[row, startCol + 1].Text?.Trim() ?? string.Empty 
                        : string.Empty;
                    
                    // 驗證必填欄位
                    if (string.IsNullOrWhiteSpace(shopId) || string.IsNullOrWhiteSpace(goodsId))
                    {
                        continue; // 跳過必填欄位為空的行
                    }

                    // 解析數值欄位
                    decimal qty = 0;
                    if (row <= endRow && startCol + 2 <= endCol)
                    {
                        var qtyValue = worksheet.Cells[row, startCol + 2].Value;
                        if (qtyValue != null)
                        {
                            decimal.TryParse(qtyValue.ToString(), out qty);
                        }
                    }

                    decimal iqty = 0;
                    if (row <= endRow && startCol + 3 <= endCol)
                    {
                        var iqtyValue = worksheet.Cells[row, startCol + 3].Value;
                        if (iqtyValue != null)
                        {
                            decimal.TryParse(iqtyValue.ToString(), out iqty);
                        }
                    }

                    // 解析其他可選欄位
                    string? kind = null;
                    if (row <= endRow && startCol + 4 <= endCol)
                    {
                        kind = worksheet.Cells[row, startCol + 4].Text?.Trim();
                        if (string.IsNullOrWhiteSpace(kind))
                            kind = null;
                    }

                    string? shelfNo = null;
                    if (row <= endRow && startCol + 5 <= endCol)
                    {
                        shelfNo = worksheet.Cells[row, startCol + 5].Text?.Trim();
                        if (string.IsNullOrWhiteSpace(shelfNo))
                            shelfNo = null;
                    }

                    int? serialNo = null;
                    if (row <= endRow && startCol + 6 <= endCol)
                    {
                        var serialNoValue = worksheet.Cells[row, startCol + 6].Value;
                        if (serialNoValue != null && int.TryParse(serialNoValue.ToString(), out var serial))
                        {
                            serialNo = serial;
                        }
                    }

                    var dto = new StocktakingTempDto
                    {
                        ShopId = shopId,
                        GoodsId = goodsId,
                        Qty = qty,
                        IQty = iqty,
                        Kind = kind,
                        ShelfNo = shelfNo,
                        SerialNo = serialNo
                    };

                    result.Add(dto);
                }
                catch (Exception ex)
                {
                    // 記錄解析錯誤但繼續處理其他行
                    System.Diagnostics.Debug.WriteLine($"解析 Excel 第 {row} 行失敗: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"解析 Excel 檔案失敗: {ex.Message}", ex);
        }

        return result;
    }
}

