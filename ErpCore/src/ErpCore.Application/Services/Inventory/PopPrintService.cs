using OfficeOpenXml;
using ZXing;
using ErpCore.Application.DTOs.Inventory;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Inventory;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.Inventory;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Inventory;

/// <summary>
/// POP列印服務實作
/// </summary>
public class PopPrintService : BaseService, IPopPrintService
{
    private readonly IPopPrintRepository _repository;
    private readonly IDbConnectionFactory _connectionFactory;

    public PopPrintService(
        IPopPrintRepository repository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<PopPrintProductDto>> GetProductsAsync(PopPrintProductQueryDto query)
    {
        try
        {
            var repositoryQuery = new PopPrintProductQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                GoodsId = query.GoodsId,
                GoodsName = query.GoodsName,
                BarCode = query.BarCode,
                VendorGoodsId = query.VendorGoodsId,
                LogoId = query.LogoId,
                BClassId = query.BClassId,
                MClassId = query.MClassId,
                SClassId = query.SClassId
            };

            var result = await _repository.GetProductsAsync(repositoryQuery);

            var dtos = result.Items.Select(x => new PopPrintProductDto
            {
                GoodsId = x.GoodsId,
                GoodsName = x.GoodsName,
                BarCode = x.BarCode,
                VendorGoodsId = x.VendorGoodsId,
                LogoId = x.LogoId,
                Price = x.Price,
                Mprc = x.Mprc,
                Unit = x.Unit,
                UnitName = x.UnitName
            }).ToList();

            return new PagedResult<PopPrintProductDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢商品列表失敗", ex);
            throw;
        }
    }

    public async Task<PopPrintDataDto> GeneratePrintDataAsync(GeneratePrintDataDto dto)
    {
        try
        {
            var printData = new List<PopPrintDataItemDto>();

            foreach (var goodsId in dto.GoodsIds)
            {
                var product = await _repository.GetProductByIdAsync(goodsId);
                if (product == null)
                {
                    _logger.LogWarning($"商品不存在: {goodsId}");
                    continue;
                }

                var item = new PopPrintDataItemDto
                {
                    GoodsId = product.GoodsId,
                    GoodsName = product.GoodsName,
                    BarCode = product.BarCode,
                    BarCodeText = product.BarCode,
                    Price = product.Price ?? product.Mprc, // 使用 Price 或 Mprc 作为价格
                    Unit = product.Unit,
                    PrintFormat = dto.PrintFormat
                };

                // 根據選項設定資料
                if (dto.Options != null)
                {
                    if (!dto.Options.IncludeBarcode)
                    {
                        item.BarCode = null;
                        item.BarCodeText = null;
                        item.BarCodeImageBase64 = null;
                    }
                    else if (!string.IsNullOrWhiteSpace(product.BarCode))
                    {
                        // 生成條碼圖片（Base64）
                        try
                        {
                            var barcodeFormat = BarcodeHelper.DetectBarcodeFormat(product.BarCode);
                            item.BarCodeImageBase64 = BarcodeHelper.GenerateBarcodeBase64(
                                product.BarCode,
                                width: 300,
                                height: 100,
                                format: barcodeFormat);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning($"生成條碼圖片失敗: {product.BarCode}, {ex.Message}");
                            // 條碼圖片生成失敗不影響其他功能，繼續處理
                        }
                    }
                    if (!dto.Options.IncludePrice)
                    {
                        item.Price = null;
                    }
                }
                else if (!string.IsNullOrWhiteSpace(product.BarCode))
                {
                    // 預設包含條碼，生成條碼圖片
                    try
                    {
                        var barcodeFormat = BarcodeHelper.DetectBarcodeFormat(product.BarCode);
                        item.BarCodeImageBase64 = BarcodeHelper.GenerateBarcodeBase64(
                            product.BarCode,
                            width: 300,
                            height: 100,
                            format: barcodeFormat);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"生成條碼圖片失敗: {product.BarCode}, {ex.Message}");
                        // 條碼圖片生成失敗不影響其他功能，繼續處理
                    }
                }

                printData.Add(item);
            }

            return new PopPrintDataDto
            {
                PrintData = printData,
                TotalCount = printData.Count
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("產生列印資料失敗", ex);
            throw;
        }
    }

    public async Task<PrintJobDto> PrintAsync(PrintRequestDto dto)
    {
        try
        {
            var printJobId = Guid.NewGuid().ToString();
            var printedCount = 0;

            foreach (var goodsId in dto.GoodsIds)
            {
                for (int i = 0; i < dto.PrintCount; i++)
                {
                    var log = new PopPrintLog
                    {
                        GoodsId = goodsId,
                        PrintType = dto.PrintType,
                        PrintFormat = dto.PrintFormat,
                        Version = dto.Version ?? "STANDARD",
                        PrintCount = 1,
                        PrintDate = DateTime.Now,
                        PrintedBy = GetCurrentUserId(),
                        ShopId = dto.ShopId
                    };

                    await _repository.CreateLogAsync(log);
                    printedCount++;
                }
            }

            return new PrintJobDto
            {
                PrintJobId = printJobId,
                PrintedCount = printedCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("執行列印失敗", ex);
            throw;
        }
    }

    public async Task<PopPrintSettingDto?> GetSettingsAsync(string? shopId, string? version = null)
    {
        try
        {
            var setting = await _repository.GetSettingAsync(shopId, version);
            if (setting == null)
            {
                return null;
            }

            return new PopPrintSettingDto
            {
                SettingId = setting.SettingId,
                ShopId = setting.ShopId,
                Ip = setting.Ip,
                TypeId = setting.TypeId,
                Version = setting.Version,
                DebugMode = setting.DebugMode,
                HeaderHeightPadding = setting.HeaderHeightPadding,
                HeaderHeightPaddingRemain = setting.HeaderHeightPaddingRemain,
                PageHeaderHeightPadding = setting.PageHeaderHeightPadding,
                PagePadding = setting.PagePadding,
                PageSize = setting.PageSize,
                ApSpecificSettings = setting.ApSpecificSettings
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"取得列印設定失敗: {shopId}, {version}", ex);
            throw;
        }
    }

    public async Task UpdateSettingsAsync(string? shopId, UpdatePopPrintSettingDto dto)
    {
        try
        {
            var setting = new PopPrintSetting
            {
                ShopId = shopId,
                Ip = dto.Ip,
                TypeId = dto.TypeId,
                Version = dto.Version ?? "STANDARD",
                DebugMode = dto.DebugMode,
                HeaderHeightPadding = dto.HeaderHeightPadding,
                HeaderHeightPaddingRemain = dto.HeaderHeightPaddingRemain,
                PageHeaderHeightPadding = dto.PageHeaderHeightPadding,
                PagePadding = dto.PagePadding,
                PageSize = dto.PageSize,
                ApSpecificSettings = dto.ApSpecificSettings,
                UpdatedBy = GetCurrentUserId()
            };

            await _repository.CreateOrUpdateSettingAsync(setting);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新列印設定失敗: {shopId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<PopPrintLogDto>> GetLogsAsync(PopPrintLogQueryDto query)
    {
        try
        {
            var repositoryQuery = new PopPrintLogQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                GoodsId = query.GoodsId,
                PrintType = query.PrintType,
                PrintFormat = query.PrintFormat,
                Version = query.Version,
                ShopId = query.ShopId,
                PrintDateFrom = query.PrintDateFrom,
                PrintDateTo = query.PrintDateTo
            };

            var result = await _repository.GetLogsAsync(repositoryQuery);

            var dtos = result.Items.Select(x => new PopPrintLogDto
            {
                LogId = x.LogId,
                GoodsId = x.GoodsId,
                PrintType = x.PrintType,
                PrintFormat = x.PrintFormat,
                Version = x.Version,
                PrintCount = x.PrintCount,
                PrintDate = x.PrintDate,
                PrintedBy = x.PrintedBy,
                ShopId = x.ShopId
            }).ToList();

            return new PagedResult<PopPrintLogDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢列印記錄列表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportExcelAsync(GeneratePrintDataDto dto)
    {
        try
        {
            // 設定 EPPlus 授權上下文（非商業使用）
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("POP列印資料");

            // 設定標題行
            worksheet.Cells[1, 1].Value = "商品編號";
            worksheet.Cells[1, 2].Value = "商品名稱";
            worksheet.Cells[1, 3].Value = "條碼";
            worksheet.Cells[1, 4].Value = "供應商商品編號";
            worksheet.Cells[1, 5].Value = "品牌編號";
            worksheet.Cells[1, 6].Value = "價格";
            worksheet.Cells[1, 7].Value = "售價";
            worksheet.Cells[1, 8].Value = "單位";
            worksheet.Cells[1, 9].Value = "單位名稱";
            worksheet.Cells[1, 10].Value = "列印格式";

            // 設定標題行樣式
            using (var range = worksheet.Cells[1, 1, 1, 10])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            }

            // 查詢商品資料
            var row = 2;
            foreach (var goodsId in dto.GoodsIds)
            {
                var product = await _repository.GetProductByIdAsync(goodsId);
                if (product == null)
                {
                    _logger.LogWarning($"商品不存在: {goodsId}");
                    continue;
                }

                // 根據選項決定是否包含某些欄位
                bool includeBarcode = dto.Options?.IncludeBarcode ?? true;
                bool includePrice = dto.Options?.IncludePrice ?? true;

                worksheet.Cells[row, 1].Value = product.GoodsId;
                worksheet.Cells[row, 2].Value = product.GoodsName;
                worksheet.Cells[row, 3].Value = includeBarcode ? product.BarCode : string.Empty;
                worksheet.Cells[row, 4].Value = product.VendorGoodsId;
                worksheet.Cells[row, 5].Value = product.LogoId;
                worksheet.Cells[row, 6].Value = includePrice ? product.Price : null;
                worksheet.Cells[row, 7].Value = includePrice ? product.Mprc : null;
                worksheet.Cells[row, 8].Value = product.Unit;
                worksheet.Cells[row, 9].Value = product.UnitName;
                worksheet.Cells[row, 10].Value = dto.PrintFormat;

                row++;
            }

            // 自動調整欄位寬度
            worksheet.Cells.AutoFitColumns();

            // 設定數值欄位格式
            if (dto.Options?.IncludePrice ?? true)
            {
                worksheet.Cells[2, 6, row - 1, 7].Style.Numberformat.Format = "#,##0.00";
            }

            // 轉換為位元組陣列
            return await Task.FromResult(package.GetAsByteArray());
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出Excel失敗", ex);
            throw;
        }
    }
}

