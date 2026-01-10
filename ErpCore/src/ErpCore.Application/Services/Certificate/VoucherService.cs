using ErpCore.Application.DTOs.Certificate;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Certificate;
using ErpCore.Infrastructure.Repositories.Certificate;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using OfficeOpenXml;

namespace ErpCore.Application.Services.Certificate;

/// <summary>
/// 憑證服務實作 (SYSK110-SYSK150, SYSK210-SYSK230)
/// </summary>
public class VoucherService : BaseService, IVoucherService
{
    private readonly IVoucherRepository _voucherRepository;
    private readonly IVoucherDetailRepository _voucherDetailRepository;
    private readonly ExportHelper _exportHelper;

    public VoucherService(
        IVoucherRepository voucherRepository,
        IVoucherDetailRepository voucherDetailRepository,
        ExportHelper exportHelper,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _voucherRepository = voucherRepository;
        _voucherDetailRepository = voucherDetailRepository;
        _exportHelper = exportHelper;
    }

    public async Task<PagedResult<VoucherDto>> GetVouchersAsync(VoucherQueryDto query)
    {
        try
        {
            var entities = await _voucherRepository.GetPagedAsync(
                query.PageIndex,
                query.PageSize,
                query.VoucherId,
                query.VoucherType,
                query.ShopId,
                query.Status,
                query.VoucherDateFrom,
                query.VoucherDateTo);

            var totalCount = await _voucherRepository.GetCountAsync(
                query.VoucherId,
                query.VoucherType,
                query.ShopId,
                query.Status,
                query.VoucherDateFrom,
                query.VoucherDateTo);

            var items = entities.Select(MapToDto).ToList();

            return new PagedResult<VoucherDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢憑證列表失敗", ex);
            throw;
        }
    }

    public async Task<VoucherDto> GetVoucherByIdAsync(string voucherId)
    {
        try
        {
            var entity = await _voucherRepository.GetByVoucherIdAsync(voucherId);
            if (entity == null)
            {
                throw new InvalidOperationException($"憑證不存在: {voucherId}");
            }

            var dto = MapToDto(entity);

            // 載入明細
            var details = await _voucherDetailRepository.GetByVoucherIdAsync(voucherId);
            dto.Details = details.Select(MapDetailToDto).ToList();

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢憑證失敗: {voucherId}", ex);
            throw;
        }
    }

    public async Task<VoucherDto> CreateVoucherAsync(CreateVoucherDto dto)
    {
        try
        {
            // 檢查是否已存在
            var exists = await _voucherRepository.ExistsAsync(dto.VoucherId);
            if (exists)
            {
                throw new InvalidOperationException($"憑證編號已存在: {dto.VoucherId}");
            }

            // 驗證借方貸方平衡
            var totalDebit = dto.Details.Sum(d => d.DebitAmount);
            var totalCredit = dto.Details.Sum(d => d.CreditAmount);
            if (totalDebit != totalCredit)
            {
                throw new InvalidOperationException($"借方總額 ({totalDebit}) 與貸方總額 ({totalCredit}) 不平衡");
            }

            // 建立憑證主檔
            var entity = new Voucher
            {
                VoucherId = dto.VoucherId,
                VoucherDate = dto.VoucherDate,
                VoucherType = dto.VoucherType,
                ShopId = dto.ShopId,
                Status = dto.Status,
                TotalAmount = totalDebit,
                TotalDebitAmount = totalDebit,
                TotalCreditAmount = totalCredit,
                Memo = dto.Memo,
                SiteId = dto.SiteId,
                OrgId = dto.OrgId,
                CurrencyId = dto.CurrencyId,
                ExchangeRate = dto.ExchangeRate,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var result = await _voucherRepository.CreateAsync(entity);

            // 建立明細
            foreach (var detailDto in dto.Details)
            {
                var detail = new VoucherDetail
                {
                    VoucherId = result.VoucherId,
                    LineNum = detailDto.LineNum,
                    AccountId = detailDto.AccountId,
                    DebitAmount = detailDto.DebitAmount,
                    CreditAmount = detailDto.CreditAmount,
                    Description = detailDto.Description,
                    Memo = detailDto.Memo,
                    CreatedBy = GetCurrentUserId(),
                    CreatedAt = DateTime.Now,
                    UpdatedBy = GetCurrentUserId(),
                    UpdatedAt = DateTime.Now
                };

                await _voucherDetailRepository.CreateAsync(detail);
            }

            return await GetVoucherByIdAsync(result.VoucherId);
        }
        catch (Exception ex)
        {
            _logger.LogError("新增憑證失敗", ex);
            throw;
        }
    }

    public async Task<VoucherDto> UpdateVoucherAsync(string voucherId, UpdateVoucherDto dto)
    {
        try
        {
            var entity = await _voucherRepository.GetByVoucherIdAsync(voucherId);
            if (entity == null)
            {
                throw new InvalidOperationException($"憑證不存在: {voucherId}");
            }

            // 已審核的憑證不可修改
            if (entity.Status == "A")
            {
                throw new InvalidOperationException($"已審核的憑證不可修改: {voucherId}");
            }

            // 驗證借方貸方平衡
            var totalDebit = dto.Details.Sum(d => d.DebitAmount);
            var totalCredit = dto.Details.Sum(d => d.CreditAmount);
            if (totalDebit != totalCredit)
            {
                throw new InvalidOperationException($"借方總額 ({totalDebit}) 與貸方總額 ({totalCredit}) 不平衡");
            }

            // 更新憑證主檔
            entity.VoucherDate = dto.VoucherDate;
            entity.VoucherType = dto.VoucherType;
            entity.ShopId = dto.ShopId;
            entity.Status = dto.Status;
            entity.TotalAmount = totalDebit;
            entity.TotalDebitAmount = totalDebit;
            entity.TotalCreditAmount = totalCredit;
            entity.Memo = dto.Memo;
            entity.SiteId = dto.SiteId;
            entity.OrgId = dto.OrgId;
            entity.CurrencyId = dto.CurrencyId;
            entity.ExchangeRate = dto.ExchangeRate;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _voucherRepository.UpdateAsync(entity);

            // 刪除舊明細
            await _voucherDetailRepository.DeleteByVoucherIdAsync(voucherId);

            // 建立新明細
            foreach (var detailDto in dto.Details)
            {
                var detail = new VoucherDetail
                {
                    VoucherId = voucherId,
                    LineNum = detailDto.LineNum,
                    AccountId = detailDto.AccountId,
                    DebitAmount = detailDto.DebitAmount,
                    CreditAmount = detailDto.CreditAmount,
                    Description = detailDto.Description,
                    Memo = detailDto.Memo,
                    CreatedBy = GetCurrentUserId(),
                    CreatedAt = DateTime.Now,
                    UpdatedBy = GetCurrentUserId(),
                    UpdatedAt = DateTime.Now
                };

                await _voucherDetailRepository.CreateAsync(detail);
            }

            return await GetVoucherByIdAsync(voucherId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新憑證失敗: {voucherId}", ex);
            throw;
        }
    }

    public async Task DeleteVoucherAsync(string voucherId)
    {
        try
        {
            var entity = await _voucherRepository.GetByVoucherIdAsync(voucherId);
            if (entity == null)
            {
                throw new InvalidOperationException($"憑證不存在: {voucherId}");
            }

            // 已審核的憑證不可刪除
            if (entity.Status == "A")
            {
                throw new InvalidOperationException($"已審核的憑證不可刪除: {voucherId}");
            }

            await _voucherRepository.DeleteAsync(voucherId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除憑證失敗: {voucherId}", ex);
            throw;
        }
    }

    public async Task ApproveVoucherAsync(string voucherId, ApproveVoucherDto dto)
    {
        try
        {
            var entity = await _voucherRepository.GetByVoucherIdAsync(voucherId);
            if (entity == null)
            {
                throw new InvalidOperationException($"憑證不存在: {voucherId}");
            }

            if (entity.Status != "S")
            {
                throw new InvalidOperationException($"只有已送出的憑證才能審核: {voucherId}");
            }

            entity.Status = "A";
            entity.ApproveUserId = dto.ApproveUserId;
            entity.ApproveDate = DateTime.Now;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _voucherRepository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"審核憑證失敗: {voucherId}", ex);
            throw;
        }
    }

    public async Task CancelVoucherAsync(string voucherId, CancelVoucherDto dto)
    {
        try
        {
            var entity = await _voucherRepository.GetByVoucherIdAsync(voucherId);
            if (entity == null)
            {
                throw new InvalidOperationException($"憑證不存在: {voucherId}");
            }

            entity.Status = "X";
            entity.Memo = dto.Memo;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _voucherRepository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"取消憑證失敗: {voucherId}", ex);
            throw;
        }
    }

    public async Task<List<VoucherCheckResultDto>> CheckVouchersAsync(List<string> voucherIds)
    {
        try
        {
            var results = new List<VoucherCheckResultDto>();

            foreach (var voucherId in voucherIds)
            {
                var result = new VoucherCheckResultDto
                {
                    VoucherId = voucherId,
                    Status = "SUCCESS",
                    Message = "檢查通過",
                    Errors = new List<string>()
                };

                try
                {
                    var voucher = await GetVoucherByIdAsync(voucherId);

                    // 檢查借方貸方平衡
                    if (voucher.TotalDebitAmount != voucher.TotalCreditAmount)
                    {
                        result.Status = "FAILED";
                        result.Message = "借方貸方不平衡";
                        result.Errors.Add($"借方總額 ({voucher.TotalDebitAmount}) 與貸方總額 ({voucher.TotalCreditAmount}) 不一致");
                    }

                    // 檢查明細是否為空
                    if (voucher.Details == null || !voucher.Details.Any())
                    {
                        result.Status = "FAILED";
                        result.Message = "憑證明細為空";
                        result.Errors.Add("憑證明細至少需有一筆");
                    }
                }
                catch (Exception ex)
                {
                    result.Status = "FAILED";
                    result.Message = $"檢查失敗: {ex.Message}";
                    result.Errors.Add(ex.Message);
                }

                results.Add(result);
            }

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError("檢查憑證失敗", ex);
            throw;
        }
    }

    private VoucherDto MapToDto(Voucher entity)
    {
        return new VoucherDto
        {
            TKey = entity.TKey,
            VoucherId = entity.VoucherId,
            VoucherDate = entity.VoucherDate,
            VoucherType = entity.VoucherType,
            ShopId = entity.ShopId,
            Status = entity.Status,
            ApplyUserId = entity.ApplyUserId,
            ApplyDate = entity.ApplyDate,
            ApproveUserId = entity.ApproveUserId,
            ApproveDate = entity.ApproveDate,
            TotalAmount = entity.TotalAmount,
            TotalDebitAmount = entity.TotalDebitAmount,
            TotalCreditAmount = entity.TotalCreditAmount,
            Memo = entity.Memo,
            SiteId = entity.SiteId,
            OrgId = entity.OrgId,
            CurrencyId = entity.CurrencyId,
            ExchangeRate = entity.ExchangeRate,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private VoucherDetailDto MapDetailToDto(VoucherDetail entity)
    {
        return new VoucherDetailDto
        {
            TKey = entity.TKey,
            VoucherId = entity.VoucherId,
            LineNum = entity.LineNum,
            AccountId = entity.AccountId,
            DebitAmount = entity.DebitAmount,
            CreditAmount = entity.CreditAmount,
            Description = entity.Description,
            Memo = entity.Memo,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }

    // SYSK210-SYSK230: 憑證處理作業

    public async Task<VoucherProcessResultDto> ImportVouchersAsync(byte[] fileData, string fileName)
    {
        try
        {
            _logger.LogInfo($"導入憑證檔案: {fileName}");

            var result = new VoucherProcessResultDto
            {
                TotalCount = 0,
                SuccessCount = 0,
                FailedCount = 0,
                Results = new List<VoucherProcessItemDto>()
            };

            // 解析 Excel 檔案（使用 EPPlus）
            using var package = new OfficeOpenXml.ExcelPackage(new MemoryStream(fileData));
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension?.Rows ?? 0;

            if (rowCount < 2)
            {
                throw new InvalidOperationException("Excel 檔案格式錯誤：至少需要標題列和一行資料");
            }

            result.TotalCount = rowCount - 1;

            // 讀取標題列（第一列）
            var headers = new Dictionary<int, string>();
            for (int col = 1; col <= worksheet.Dimension?.Columns; col++)
            {
                var headerValue = worksheet.Cells[1, col].Value?.ToString();
                if (!string.IsNullOrEmpty(headerValue))
                {
                    headers[col] = headerValue;
                }
            }

            // 讀取資料列（從第二列開始）
            for (int row = 2; row <= rowCount; row++)
            {
                var item = new VoucherProcessItemDto
                {
                    RowNum = row - 1,
                    VoucherId = string.Empty,
                    Status = "FAILED",
                    Message = string.Empty
                };

                try
                {
                    // 讀取憑證資料（根據實際 Excel 格式調整）
                    var voucherId = worksheet.Cells[row, GetColumnIndex(headers, "憑證編號")].Value?.ToString() ?? 
                                   worksheet.Cells[row, GetColumnIndex(headers, "VoucherId")].Value?.ToString() ?? 
                                   string.Empty;

                    if (string.IsNullOrEmpty(voucherId))
                    {
                        item.Message = "憑證編號不能為空";
                        result.FailedCount++;
                        result.Results.Add(item);
                        continue;
                    }

                    item.VoucherId = voucherId;

                    // 檢查是否已存在
                    var exists = await _voucherRepository.ExistsAsync(voucherId);
                    if (exists)
                    {
                        item.Message = $"憑證編號已存在: {voucherId}";
                        result.FailedCount++;
                        result.Results.Add(item);
                        continue;
                    }

                    // 讀取其他欄位並建立憑證（簡化版本，實際應根據 Excel 格式完整讀取）
                    var voucherDateStr = worksheet.Cells[row, GetColumnIndex(headers, "憑證日期")].Value?.ToString() ?? 
                                       worksheet.Cells[row, GetColumnIndex(headers, "VoucherDate")].Value?.ToString() ?? 
                                       DateTime.Now.ToString("yyyy-MM-dd");
                    
                    if (!DateTime.TryParse(voucherDateStr, out var voucherDate))
                    {
                        voucherDate = DateTime.Now;
                    }

                    // 建立憑證（簡化版本，實際應完整讀取所有欄位）
                    var createDto = new CreateVoucherDto
                    {
                        VoucherId = voucherId,
                        VoucherDate = voucherDate,
                        VoucherType = worksheet.Cells[row, GetColumnIndex(headers, "憑證類型")].Value?.ToString() ?? 
                                     worksheet.Cells[row, GetColumnIndex(headers, "VoucherType")].Value?.ToString() ?? 
                                     string.Empty,
                        ShopId = worksheet.Cells[row, GetColumnIndex(headers, "店別")].Value?.ToString() ?? 
                                worksheet.Cells[row, GetColumnIndex(headers, "ShopId")].Value?.ToString() ?? 
                                string.Empty,
                        Status = "D",
                        Details = new List<CreateVoucherDetailDto>()
                    };

                    // 建立憑證
                    await CreateVoucherAsync(createDto);

                    item.Status = "SUCCESS";
                    item.Message = "導入成功";
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    item.Message = $"導入失敗: {ex.Message}";
                    result.FailedCount++;
                    _logger.LogError($"導入憑證失敗: Row={row}, VoucherId={item.VoucherId}", ex);
                }

                result.Results.Add(item);
            }

            _logger.LogInfo($"導入憑證完成: 總數={result.TotalCount}, 成功={result.SuccessCount}, 失敗={result.FailedCount}");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"導入憑證檔案失敗: {fileName}", ex);
            throw;
        }
    }

    private int GetColumnIndex(Dictionary<int, string> headers, string headerName)
    {
        return headers.FirstOrDefault(h => h.Value.Contains(headerName)).Key;
    }

    public async Task<PrintResultDto> PrintVouchersAsync(PrintVoucherDto dto)
    {
        try
        {
            _logger.LogInfo($"列印憑證: {string.Join(",", dto.VoucherIds)}");

            // 查詢憑證資料
            var vouchers = new List<VoucherDto>();
            foreach (var voucherId in dto.VoucherIds)
            {
                try
                {
                    var voucher = await GetVoucherByIdAsync(voucherId);
                    vouchers.Add(voucher);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"查詢憑證失敗: {voucherId}", ex);
                }
            }

            if (!vouchers.Any())
            {
                throw new InvalidOperationException("找不到要列印的憑證");
            }

            // 定義列印欄位
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "VoucherId", DisplayName = "憑證編號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "VoucherDate", DisplayName = "憑證日期", DataType = ExportDataType.Date },
                new ExportColumn { PropertyName = "VoucherType", DisplayName = "憑證類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ShopId", DisplayName = "店別", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Status", DisplayName = "狀態", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "TotalAmount", DisplayName = "總金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "Memo", DisplayName = "備註", DataType = ExportDataType.String }
            };

            // 產生 PDF
            byte[] fileData;
            string fileExtension;
            if (dto.PrintFormat.ToUpper() == "PDF")
            {
                fileData = _exportHelper.ExportToPdf(vouchers, columns, "憑證列印");
                fileExtension = ".pdf";
            }
            else
            {
                fileData = _exportHelper.ExportToExcel(vouchers, columns, "憑證列印", "憑證列印");
                fileExtension = ".xlsx";
            }

            // 儲存檔案（實際應儲存到檔案系統或雲端儲存）
            var fileName = $"vouchers_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";
            var fileUrl = $"/api/v1/files/print/{fileName}";

            // 這裡應該實際儲存檔案，目前僅返回 URL
            // await _fileStorageService.SaveFileAsync(fileName, fileData);

            return new PrintResultDto
            {
                FileName = fileName,
                FileUrl = fileUrl,
                FileSize = fileData.Length
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("列印憑證失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportVouchersAsync(ExportVoucherDto dto)
    {
        try
        {
            _logger.LogInfo($"匯出憑證: {string.Join(",", dto.VoucherIds)}, 格式: {dto.ExportFormat}");

            // 查詢憑證資料
            var vouchers = new List<VoucherDto>();
            foreach (var voucherId in dto.VoucherIds)
            {
                try
                {
                    var voucher = await GetVoucherByIdAsync(voucherId);
                    vouchers.Add(voucher);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"查詢憑證失敗: {voucherId}", ex);
                }
            }

            if (!vouchers.Any())
            {
                throw new InvalidOperationException("找不到要匯出的憑證");
            }

            // 定義匯出欄位
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "VoucherId", DisplayName = "憑證編號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "VoucherDate", DisplayName = "憑證日期", DataType = ExportDataType.Date },
                new ExportColumn { PropertyName = "VoucherType", DisplayName = "憑證類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ShopId", DisplayName = "店別", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Status", DisplayName = "狀態", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "TotalAmount", DisplayName = "總金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "TotalDebitAmount", DisplayName = "借方總額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "TotalCreditAmount", DisplayName = "貸方總額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "Memo", DisplayName = "備註", DataType = ExportDataType.String }
            };

            // 匯出
            if (dto.ExportFormat.ToUpper() == "PDF")
            {
                return _exportHelper.ExportToPdf(vouchers, columns, "憑證匯出");
            }
            else
            {
                return _exportHelper.ExportToExcel(vouchers, columns, "憑證匯出", "憑證匯出");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出憑證失敗", ex);
            throw;
        }
    }

    public async Task BatchUpdateVoucherStatusAsync(BatchUpdateVoucherStatusDto dto)
    {
        try
        {
            _logger.LogInfo($"批量更新憑證狀態: {dto.Status}, 數量: {dto.VoucherIds.Count}");

            foreach (var voucherId in dto.VoucherIds)
            {
                try
                {
                    var entity = await _voucherRepository.GetByVoucherIdAsync(voucherId);
                    if (entity == null)
                    {
                        _logger.LogWarning($"憑證不存在: {voucherId}");
                        continue;
                    }

                    // 已審核的憑證不可修改狀態
                    if (entity.Status == "A" && dto.Status != "A")
                    {
                        _logger.LogWarning($"已審核的憑證不可修改狀態: {voucherId}");
                        continue;
                    }

                    entity.Status = dto.Status;
                    if (!string.IsNullOrEmpty(dto.Memo))
                    {
                        entity.Memo = dto.Memo;
                    }
                    entity.UpdatedBy = GetCurrentUserId();
                    entity.UpdatedAt = DateTime.Now;

                    await _voucherRepository.UpdateAsync(entity);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"批量更新憑證狀態失敗: {voucherId}", ex);
                }
            }

            _logger.LogInfo($"批量更新憑證狀態完成: {dto.VoucherIds.Count} 筆");
        }
        catch (Exception ex)
        {
            _logger.LogError("批量更新憑證狀態失敗", ex);
            throw;
        }
    }
}

