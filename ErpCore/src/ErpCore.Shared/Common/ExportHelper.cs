using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using ErpCore.Shared.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ErpCore.Shared.Common;

/// <summary>
/// Excel/PDF 匯出工具類
/// 提供通用的資料匯出功能，支援 Excel 和 PDF 格式
/// </summary>
public class ExportHelper
{
    private readonly ILoggerService _logger;

    public ExportHelper(ILoggerService logger)
    {
        _logger = logger;
        // 設定 EPPlus 授權上下文（非商業使用）
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    /// <summary>
    /// 匯出資料到 Excel
    /// </summary>
    /// <typeparam name="T">資料類型</typeparam>
    /// <param name="data">資料列表</param>
    /// <param name="columns">欄位定義（欄位名稱、顯示名稱、資料類型）</param>
    /// <param name="sheetName">工作表名稱</param>
    /// <param name="title">報表標題（可選）</param>
    /// <returns>Excel 檔案位元組陣列</returns>
    public byte[] ExportToExcel<T>(
        IEnumerable<T> data,
        List<ExportColumn> columns,
        string sheetName = "Sheet1",
        string? title = null)
    {
        try
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add(sheetName);

            int currentRow = 1;

            // 如果有標題，先寫入標題
            if (!string.IsNullOrEmpty(title))
            {
                worksheet.Cells[currentRow, 1, currentRow, columns.Count].Merge = true;
                worksheet.Cells[currentRow, 1].Value = title;
                worksheet.Cells[currentRow, 1].Style.Font.Size = 16;
                worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                currentRow++;
            }

            // 寫入欄位標題
            for (int i = 0; i < columns.Count; i++)
            {
                var column = columns[i];
                worksheet.Cells[currentRow, i + 1].Value = column.DisplayName;
                worksheet.Cells[currentRow, i + 1].Style.Font.Bold = true;
                worksheet.Cells[currentRow, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[currentRow, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                worksheet.Cells[currentRow, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, i + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[currentRow, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            currentRow++;

            // 寫入資料
            var dataList = data.ToList();
            foreach (var item in dataList)
            {
                for (int i = 0; i < columns.Count; i++)
                {
                    var column = columns[i];
                    var value = GetPropertyValue(item, column.PropertyName);

                    // 根據資料類型格式化
                    var cell = worksheet.Cells[currentRow, i + 1];
                    cell.Value = value;

                    // 設定樣式
                    cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    // 根據資料類型設定格式
                    if (column.DataType == ExportDataType.Number || column.DataType == ExportDataType.Decimal)
                    {
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        if (column.DataType == ExportDataType.Decimal)
                        {
                            cell.Style.Numberformat.Format = "#,##0.00";
                        }
                        else
                        {
                            cell.Style.Numberformat.Format = "#,##0";
                        }
                    }
                    else if (column.DataType == ExportDataType.Date)
                    {
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        cell.Style.Numberformat.Format = "yyyy/mm/dd";
                    }
                    else if (column.DataType == ExportDataType.DateTime)
                    {
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        cell.Style.Numberformat.Format = "yyyy/mm/dd hh:mm:ss";
                    }
                    else
                    {
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }
                }
                currentRow++;
            }

            // 自動調整欄位寬度
            for (int i = 1; i <= columns.Count; i++)
            {
                worksheet.Column(i).AutoFit();
                // 設定最小寬度
                if (worksheet.Column(i).Width < 10)
                {
                    worksheet.Column(i).Width = 10;
                }
                // 設定最大寬度
                if (worksheet.Column(i).Width > 50)
                {
                    worksheet.Column(i).Width = 50;
                }
            }

            return package.GetAsByteArray();
        }
        catch (Exception ex)
        {
            _logger.LogError($"匯出 Excel 失敗: {ex.Message}", ex);
            throw;
        }
    }

    /// <summary>
    /// 匯出資料到 PDF
    /// 使用 QuestPDF 生成 PDF 文件
    /// </summary>
    /// <typeparam name="T">資料類型</typeparam>
    /// <param name="data">資料列表</param>
    /// <param name="columns">欄位定義</param>
    /// <param name="title">報表標題</param>
    /// <returns>PDF 檔案位元組陣列</returns>
    public byte[] ExportToPdf<T>(
        IEnumerable<T> data,
        List<ExportColumn> columns,
        string? title = null)
    {
        try
        {
            var dataList = data.ToList();
            
            // 設定 QuestPDF 授權（非商業使用）
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimeter);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Content()
                        .Column(column =>
                        {
                            // 標題
                            if (!string.IsNullOrEmpty(title))
                            {
                                column.Item()
                                    .PaddingBottom(10)
                                    .Text(title)
                                    .FontSize(16)
                                    .Bold()
                                    .AlignCenter();
                            }

                            // 表格
                            column.Item()
                                .Table(table =>
                                {
                                    // 定義欄位寬度（平均分配）
                                    table.ColumnsDefinition(columnsDef =>
                                    {
                                        for (int i = 0; i < columns.Count; i++)
                                        {
                                            columnsDef.RelativeColumn();
                                        }
                                    });

                                    // 標題行
                                    table.Header(header =>
                                    {
                                        foreach (var col in columns)
                                        {
                                            header.Cell()
                                                .Element(style =>
                                                {
                                                    style
                                                        .Background(Colors.Grey.Lighten3)
                                                        .Padding(8)
                                                        .Border(1)
                                                        .BorderColor(Colors.Grey.Lighten1);
                                                })
                                                .Text(col.DisplayName)
                                                .FontSize(10)
                                                .Bold()
                                                .AlignCenter();
                                        }
                                    });

                                    // 資料行
                                    foreach (var item in dataList)
                                    {
                                        foreach (var col in columns)
                                        {
                                            var value = GetPropertyValue(item, col.PropertyName);
                                            var displayValue = FormatValue(value, col.DataType);

                                            table.Cell()
                                                .Element(style =>
                                                {
                                                    style
                                                        .Padding(6)
                                                        .Border(1)
                                                        .BorderColor(Colors.Grey.Lighten2);
                                                })
                                                .Text(displayValue)
                                                .FontSize(9)
                                                .AlignLeft();
                                        }
                                    }
                                });
                        });
                });
            });

            return document.GeneratePdf();
        }
        catch (Exception ex)
        {
            _logger.LogError($"匯出 PDF 失敗: {ex.Message}", ex);
            throw;
        }
    }

    /// <summary>
    /// 格式化值
    /// </summary>
    private string FormatValue(object? value, ExportDataType dataType)
    {
        if (value == null)
            return string.Empty;

        return dataType switch
        {
            ExportDataType.Number => value is int intVal ? intVal.ToString("#,##0") : value.ToString() ?? string.Empty,
            ExportDataType.Decimal => value is decimal decVal ? decVal.ToString("#,##0.00") : value.ToString() ?? string.Empty,
            ExportDataType.Date => value is DateTime dateVal ? dateVal.ToString("yyyy/MM/dd") : value.ToString() ?? string.Empty,
            ExportDataType.DateTime => value is DateTime dateTimeVal ? dateTimeVal.ToString("yyyy/MM/dd HH:mm:ss") : value.ToString() ?? string.Empty,
            ExportDataType.Boolean => value is bool boolVal ? (boolVal ? "是" : "否") : value.ToString() ?? string.Empty,
            _ => value.ToString() ?? string.Empty
        };
    }

    /// <summary>
    /// 取得物件屬性值
    /// </summary>
    private object? GetPropertyValue<T>(T obj, string propertyName)
    {
        try
        {
            var property = typeof(T).GetProperty(propertyName);
            if (property == null)
            {
                return null;
            }

            var value = property.GetValue(obj);
            return value;
        }
        catch
        {
            return null;
        }
    }
}

/// <summary>
/// 匯出欄位定義
/// </summary>
public class ExportColumn
{
    /// <summary>
    /// 屬性名稱（對應物件的屬性名稱）
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;

    /// <summary>
    /// 顯示名稱（Excel 標題行顯示的名稱）
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// 資料類型
    /// </summary>
    public ExportDataType DataType { get; set; } = ExportDataType.String;
}

/// <summary>
/// 匯出資料類型
/// </summary>
public enum ExportDataType
{
    /// <summary>
    /// 字串
    /// </summary>
    String,

    /// <summary>
    /// 整數
    /// </summary>
    Number,

    /// <summary>
    /// 小數
    /// </summary>
    Decimal,

    /// <summary>
    /// 日期
    /// </summary>
    Date,

    /// <summary>
    /// 日期時間
    /// </summary>
    DateTime,

    /// <summary>
    /// 布林值
    /// </summary>
    Boolean
}

