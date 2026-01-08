using System.Text;
using System.Text.Json;

namespace ErpCore.Shared.Common;

/// <summary>
/// HT680格式文本文件解析器
/// 支援多種文本文件格式：退貨檔、盤點檔、訂貨檔、POP卡製作檔、商品卡檔
/// </summary>
public static class HT680TextFileParser
{
    /// <summary>
    /// 解析退貨檔 (TXT_BACK)
    /// 格式：分店代號(4) + 貨號/店內碼(8) + 訂貨數量(8)
    /// </summary>
    public static List<HT680BackRecord> ParseBackFile(string content, string? shopId = null)
    {
        var records = new List<HT680BackRecord>();
        var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line) || line.Equals("EOF", StringComparison.OrdinalIgnoreCase))
                continue;

            try
            {
                if (line.Length < 20) // 至少需要20個字元
                    continue;

                var record = new HT680BackRecord
                {
                    LineNumber = i + 1,
                    ShopId = line.Substring(0, Math.Min(4, line.Length)).Trim(),
                    GoodsId = line.Length > 4 ? line.Substring(4, Math.Min(8, line.Length - 4)).Trim() : string.Empty,
                    Qty = line.Length > 12 && decimal.TryParse(line.Substring(12, Math.Min(8, line.Length - 12)).Trim(), out var qty) ? qty : 0
                };

                if (!string.IsNullOrWhiteSpace(record.ShopId) && !string.IsNullOrWhiteSpace(record.GoodsId))
                {
                    records.Add(record);
                }
            }
            catch (Exception)
            {
                // 跳過解析失敗的行
                continue;
            }
        }

        return records;
    }

    /// <summary>
    /// 解析盤點檔 (TXT_INV)
    /// 格式：貨架(2) + 盤點序號(4) + 貨號/店內碼(8) + 盤點數量(8)
    /// </summary>
    public static List<HT680InvRecord> ParseInvFile(string content, string? shopId = null)
    {
        var records = new List<HT680InvRecord>();
        var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line) || line.Equals("EOF", StringComparison.OrdinalIgnoreCase))
                continue;

            try
            {
                if (line.Length < 22) // 至少需要22個字元
                    continue;

                var record = new HT680InvRecord
                {
                    LineNumber = i + 1,
                    ShelfNo = line.Substring(0, Math.Min(2, line.Length)).Trim(),
                    SerialNo = line.Length > 2 && int.TryParse(line.Substring(2, Math.Min(4, line.Length - 2)).Trim(), out var serial) ? serial : 0,
                    GoodsId = line.Length > 6 ? line.Substring(6, Math.Min(8, line.Length - 6)).Trim() : string.Empty,
                    Qty = line.Length > 14 && decimal.TryParse(line.Substring(14, Math.Min(8, line.Length - 14)).Trim(), out var qty) ? qty : 0
                };

                if (!string.IsNullOrWhiteSpace(record.GoodsId))
                {
                    records.Add(record);
                }
            }
            catch (Exception)
            {
                // 跳過解析失敗的行
                continue;
            }
        }

        return records;
    }

    /// <summary>
    /// 解析訂貨檔 (TXT_ORDER)
    /// 格式：分店代號(4) + 貨號/店內碼(8) + 訂貨數量(8)
    /// </summary>
    public static List<HT680OrderRecord> ParseOrderFile(string content, string? shopId = null)
    {
        var records = new List<HT680OrderRecord>();
        var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line) || line.Equals("EOF", StringComparison.OrdinalIgnoreCase))
                continue;

            try
            {
                if (line.Length < 20) // 至少需要20個字元
                    continue;

                var record = new HT680OrderRecord
                {
                    LineNumber = i + 1,
                    ShopId = line.Substring(0, Math.Min(4, line.Length)).Trim(),
                    GoodsId = line.Length > 4 ? line.Substring(4, Math.Min(8, line.Length - 4)).Trim() : string.Empty,
                    Qty = line.Length > 12 && decimal.TryParse(line.Substring(12, Math.Min(8, line.Length - 12)).Trim(), out var qty) ? qty : 0
                };

                if (!string.IsNullOrWhiteSpace(record.ShopId) && !string.IsNullOrWhiteSpace(record.GoodsId))
                {
                    records.Add(record);
                }
            }
            catch (Exception)
            {
                // 跳過解析失敗的行
                continue;
            }
        }

        return records;
    }

    /// <summary>
    /// 解析訂貨檔版本六 (TXT_ORDER_6)
    /// 格式：分店代號(6) + 貨號/店內碼(8) + 訂貨數量(8)
    /// </summary>
    public static List<HT680Order6Record> ParseOrder6File(string content, string? shopId = null)
    {
        var records = new List<HT680Order6Record>();
        var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line) || line.Equals("EOF", StringComparison.OrdinalIgnoreCase))
                continue;

            try
            {
                if (line.Length < 22) // 至少需要22個字元
                    continue;

                var record = new HT680Order6Record
                {
                    LineNumber = i + 1,
                    ShopId = line.Substring(0, Math.Min(6, line.Length)).Trim(),
                    GoodsId = line.Length > 6 ? line.Substring(6, Math.Min(8, line.Length - 6)).Trim() : string.Empty,
                    Qty = line.Length > 14 && decimal.TryParse(line.Substring(14, Math.Min(8, line.Length - 14)).Trim(), out var qty) ? qty : 0
                };

                if (!string.IsNullOrWhiteSpace(record.ShopId) && !string.IsNullOrWhiteSpace(record.GoodsId))
                {
                    records.Add(record);
                }
            }
            catch (Exception)
            {
                // 跳過解析失敗的行
                continue;
            }
        }

        return records;
    }

    /// <summary>
    /// 解析POP卡製作檔 (TXT_POP)
    /// 格式：分店代號(4) + 貨號/店內碼(8)
    /// </summary>
    public static List<HT680PopRecord> ParsePopFile(string content, string? shopId = null)
    {
        var records = new List<HT680PopRecord>();
        var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line) || line.Equals("EOF", StringComparison.OrdinalIgnoreCase))
                continue;

            try
            {
                if (line.Length < 12) // 至少需要12個字元
                    continue;

                var record = new HT680PopRecord
                {
                    LineNumber = i + 1,
                    ShopId = line.Substring(0, Math.Min(4, line.Length)).Trim(),
                    GoodsId = line.Length > 4 ? line.Substring(4, Math.Min(8, line.Length - 4)).Trim() : string.Empty
                };

                if (!string.IsNullOrWhiteSpace(record.ShopId) && !string.IsNullOrWhiteSpace(record.GoodsId))
                {
                    records.Add(record);
                }
            }
            catch (Exception)
            {
                // 跳過解析失敗的行
                continue;
            }
        }

        return records;
    }

    /// <summary>
    /// 解析商品卡檔 (TXT_PRIC)
    /// 格式：貨號/店內碼(8)
    /// </summary>
    public static List<HT680PricRecord> ParsePricFile(string content, string? shopId = null)
    {
        var records = new List<HT680PricRecord>();
        var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line) || line.Equals("EOF", StringComparison.OrdinalIgnoreCase))
                continue;

            try
            {
                if (line.Length < 8) // 至少需要8個字元
                    continue;

                var record = new HT680PricRecord
                {
                    LineNumber = i + 1,
                    GoodsId = line.Substring(0, Math.Min(8, line.Length)).Trim()
                };

                if (!string.IsNullOrWhiteSpace(record.GoodsId))
                {
                    records.Add(record);
                }
            }
            catch (Exception)
            {
                // 跳過解析失敗的行
                continue;
            }
        }

        return records;
    }
}

/// <summary>
/// HT680退貨檔記錄
/// </summary>
public class HT680BackRecord
{
    public int LineNumber { get; set; }
    public string ShopId { get; set; } = string.Empty;
    public string GoodsId { get; set; } = string.Empty;
    public decimal Qty { get; set; }
}

/// <summary>
/// HT680盤點檔記錄
/// </summary>
public class HT680InvRecord
{
    public int LineNumber { get; set; }
    public string ShelfNo { get; set; } = string.Empty;
    public int SerialNo { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public decimal Qty { get; set; }
}

/// <summary>
/// HT680訂貨檔記錄
/// </summary>
public class HT680OrderRecord
{
    public int LineNumber { get; set; }
    public string ShopId { get; set; } = string.Empty;
    public string GoodsId { get; set; } = string.Empty;
    public decimal Qty { get; set; }
}

/// <summary>
/// HT680訂貨檔版本六記錄
/// </summary>
public class HT680Order6Record
{
    public int LineNumber { get; set; }
    public string ShopId { get; set; } = string.Empty;
    public string GoodsId { get; set; } = string.Empty;
    public decimal Qty { get; set; }
}

/// <summary>
/// HT680 POP卡製作檔記錄
/// </summary>
public class HT680PopRecord
{
    public int LineNumber { get; set; }
    public string ShopId { get; set; } = string.Empty;
    public string GoodsId { get; set; } = string.Empty;
}

/// <summary>
/// HT680商品卡檔記錄
/// </summary>
public class HT680PricRecord
{
    public int LineNumber { get; set; }
    public string GoodsId { get; set; } = string.Empty;
}

