using System.Text;
using System.Xml.Linq;
using OfficeOpenXml;
using ErpCore.Domain.Entities.EInvoice;

namespace ErpCore.Shared.Common;

/// <summary>
/// 電子發票檔案解析器
/// 支援 Excel 和 XML 格式解析
/// </summary>
public static class EInvoiceFileParser
{
    /// <summary>
    /// 解析電子發票檔案
    /// </summary>
    /// <param name="fileStream">檔案串流</param>
    /// <param name="fileName">檔案名稱</param>
    /// <param name="uploadType">上傳類型 (ECA2050, ECA3010, ECA3030)</param>
    /// <returns>解析後的電子發票列表</returns>
    public static async Task<List<EInvoice>> ParseEInvoiceFileAsync(Stream fileStream, string fileName, string? uploadType = "ECA3010")
    {
        var extension = Path.GetExtension(fileName).ToLower();
        var result = new List<EInvoice>();

        try
        {
            switch (extension)
            {
                case ".xlsx":
                case ".xls":
                    result = await ParseExcelFileAsync(fileStream, uploadType);
                    break;
                case ".xml":
                    result = await ParseXmlFileAsync(fileStream, uploadType);
                    break;
                default:
                    throw new NotSupportedException($"不支援的檔案格式: {extension}");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"解析電子發票檔案失敗: {ex.Message}", ex);
        }

        return result;
    }

    /// <summary>
    /// 解析 Excel 檔案
    /// </summary>
    private static async Task<List<EInvoice>> ParseExcelFileAsync(Stream fileStream, string? uploadType)
    {
        var result = new List<EInvoice>();
        
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

            // 判斷第一行是否為標題行
            var firstRowValue = worksheet.Cells[startRow, startCol].Text?.Trim() ?? string.Empty;
            var hasHeader = firstRowValue.Contains("OrderNo", StringComparison.OrdinalIgnoreCase) ||
                           firstRowValue.Contains("訂單編號", StringComparison.OrdinalIgnoreCase) ||
                           firstRowValue.Contains("Order", StringComparison.OrdinalIgnoreCase);

            var dataStartRow = hasHeader ? startRow + 1 : startRow;

            // 從資料行開始解析
            for (int row = dataStartRow; row <= endRow; row++)
            {
                try
                {
                    // 讀取各欄位值（根據實際 Excel 格式調整欄位順序）
                    var orderNo = GetCellValue(worksheet, row, startCol);
                    var retailerOrderNo = GetCellValue(worksheet, row, startCol + 1);
                    var retailerOrderDetailNo = GetCellValue(worksheet, row, startCol + 2);
                    var orderDateStr = GetCellValue(worksheet, row, startCol + 3);
                    var storeId = GetCellValue(worksheet, row, startCol + 4);
                    var providerId = GetCellValue(worksheet, row, startCol + 5);
                    var ndType = GetCellValue(worksheet, row, startCol + 6);
                    var goodsId = GetCellValue(worksheet, row, startCol + 7);
                    var goodsName = GetCellValue(worksheet, row, startCol + 8);
                    var specId = GetCellValue(worksheet, row, startCol + 9);
                    var providerGoodsId = GetCellValue(worksheet, row, startCol + 10);
                    var specColor = GetCellValue(worksheet, row, startCol + 11);
                    var specSize = GetCellValue(worksheet, row, startCol + 12);
                    var suggestPrice = GetCellDecimalValue(worksheet, row, startCol + 13);
                    var internetPrice = GetCellDecimalValue(worksheet, row, startCol + 14);
                    var shippingType = GetCellValue(worksheet, row, startCol + 15);
                    var shippingFee = GetCellDecimalValue(worksheet, row, startCol + 16);
                    var orderQty = GetCellIntValue(worksheet, row, startCol + 17);
                    var orderShippingFee = GetCellDecimalValue(worksheet, row, startCol + 18);
                    var orderSubtotal = GetCellDecimalValue(worksheet, row, startCol + 19);
                    var orderTotal = GetCellDecimalValue(worksheet, row, startCol + 20);
                    var orderStatus = GetCellValue(worksheet, row, startCol + 21);

                    // 驗證必填欄位
                    if (string.IsNullOrWhiteSpace(orderNo))
                    {
                        continue; // 跳過訂單編號為空的行
                    }

                    // 解析訂單日期
                    DateTime? orderDate = null;
                    if (!string.IsNullOrWhiteSpace(orderDateStr))
                    {
                        if (DateTime.TryParse(orderDateStr, out var parsedDate))
                        {
                            orderDate = parsedDate;
                        }
                    }

                    var invoice = new EInvoice
                    {
                        OrderNo = orderNo,
                        RetailerOrderNo = retailerOrderNo,
                        RetailerOrderDetailNo = retailerOrderDetailNo,
                        OrderDate = orderDate,
                        StoreId = storeId,
                        ProviderId = providerId,
                        NdType = ndType,
                        GoodsId = goodsId,
                        GoodsName = goodsName,
                        SpecId = specId,
                        ProviderGoodsId = providerGoodsId,
                        SpecColor = specColor,
                        SpecSize = specSize,
                        SuggestPrice = suggestPrice,
                        InternetPrice = internetPrice,
                        ShippingType = shippingType,
                        ShippingFee = shippingFee,
                        OrderQty = orderQty,
                        OrderShippingFee = orderShippingFee,
                        OrderSubtotal = orderSubtotal,
                        OrderTotal = orderTotal,
                        OrderStatus = orderStatus,
                        ProcessStatus = "PENDING",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    result.Add(invoice);
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

    /// <summary>
    /// 解析 XML 檔案
    /// </summary>
    private static async Task<List<EInvoice>> ParseXmlFileAsync(Stream fileStream, string? uploadType)
    {
        var result = new List<EInvoice>();
        
        try
        {
            var xmlDoc = await XDocument.LoadAsync(fileStream, LoadOptions.None, CancellationToken.None);
            
            // 根據實際 XML 格式解析（這裡是範例，需要根據實際 XML 結構調整）
            var root = xmlDoc.Root;
            if (root == null)
            {
                return result;
            }

            // 假設 XML 格式為 <Invoices><Invoice>...</Invoice></Invoices>
            var invoiceElements = root.Descendants("Invoice");
            
            foreach (var invoiceElement in invoiceElements)
            {
                try
                {
                    var invoice = new EInvoice
                    {
                        OrderNo = invoiceElement.Element("OrderNo")?.Value,
                        RetailerOrderNo = invoiceElement.Element("RetailerOrderNo")?.Value,
                        RetailerOrderDetailNo = invoiceElement.Element("RetailerOrderDetailNo")?.Value,
                        OrderDate = ParseXmlDateTime(invoiceElement.Element("OrderDate")?.Value),
                        StoreId = invoiceElement.Element("StoreId")?.Value,
                        ProviderId = invoiceElement.Element("ProviderId")?.Value,
                        NdType = invoiceElement.Element("NdType")?.Value,
                        GoodsId = invoiceElement.Element("GoodsId")?.Value,
                        GoodsName = invoiceElement.Element("GoodsName")?.Value,
                        SpecId = invoiceElement.Element("SpecId")?.Value,
                        ProviderGoodsId = invoiceElement.Element("ProviderGoodsId")?.Value,
                        SpecColor = invoiceElement.Element("SpecColor")?.Value,
                        SpecSize = invoiceElement.Element("SpecSize")?.Value,
                        SuggestPrice = ParseXmlDecimal(invoiceElement.Element("SuggestPrice")?.Value),
                        InternetPrice = ParseXmlDecimal(invoiceElement.Element("InternetPrice")?.Value),
                        ShippingType = invoiceElement.Element("ShippingType")?.Value,
                        ShippingFee = ParseXmlDecimal(invoiceElement.Element("ShippingFee")?.Value),
                        OrderQty = ParseXmlInt(invoiceElement.Element("OrderQty")?.Value),
                        OrderShippingFee = ParseXmlDecimal(invoiceElement.Element("OrderShippingFee")?.Value),
                        OrderSubtotal = ParseXmlDecimal(invoiceElement.Element("OrderSubtotal")?.Value),
                        OrderTotal = ParseXmlDecimal(invoiceElement.Element("OrderTotal")?.Value),
                        OrderStatus = invoiceElement.Element("OrderStatus")?.Value,
                        ProcessStatus = "PENDING",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    // 驗證必填欄位
                    if (!string.IsNullOrWhiteSpace(invoice.OrderNo))
                    {
                        result.Add(invoice);
                    }
                }
                catch (Exception ex)
                {
                    // 記錄解析錯誤但繼續處理其他行
                    System.Diagnostics.Debug.WriteLine($"解析 XML Invoice 元素失敗: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"解析 XML 檔案失敗: {ex.Message}", ex);
        }

        return result;
    }

    /// <summary>
    /// 取得儲存格字串值
    /// </summary>
    private static string? GetCellValue(ExcelWorksheet worksheet, int row, int col)
    {
        try
        {
            var value = worksheet.Cells[row, col].Value;
            if (value == null)
                return null;
            
            return value.ToString()?.Trim();
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 取得儲存格小數值
    /// </summary>
    private static decimal? GetCellDecimalValue(ExcelWorksheet worksheet, int row, int col)
    {
        try
        {
            var value = worksheet.Cells[row, col].Value;
            if (value == null)
                return null;
            
            if (decimal.TryParse(value.ToString(), out var decimalValue))
                return decimalValue;
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 取得儲存格整數值
    /// </summary>
    private static int? GetCellIntValue(ExcelWorksheet worksheet, int row, int col)
    {
        try
        {
            var value = worksheet.Cells[row, col].Value;
            if (value == null)
                return null;
            
            if (int.TryParse(value.ToString(), out var intValue))
                return intValue;
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 解析 XML DateTime
    /// </summary>
    private static DateTime? ParseXmlDateTime(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        
        if (DateTime.TryParse(value, out var dateTime))
            return dateTime;
        
        return null;
    }

    /// <summary>
    /// 解析 XML Decimal
    /// </summary>
    private static decimal? ParseXmlDecimal(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        
        if (decimal.TryParse(value, out var decimalValue))
            return decimalValue;
        
        return null;
    }

    /// <summary>
    /// 解析 XML Int
    /// </summary>
    private static int? ParseXmlInt(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        
        if (int.TryParse(value, out var intValue))
            return intValue;
        
        return null;
    }
}

