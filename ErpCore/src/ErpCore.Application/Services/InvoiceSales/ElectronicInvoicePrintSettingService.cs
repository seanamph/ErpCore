using ErpCore.Application.DTOs.InvoiceSales;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.InvoiceSales;
using ErpCore.Infrastructure.Repositories.InvoiceSales;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.InvoiceSales;

/// <summary>
/// 電子發票列印設定服務實作
/// </summary>
public class ElectronicInvoicePrintSettingService : BaseService, IElectronicInvoicePrintSettingService
{
    private readonly IElectronicInvoicePrintSettingRepository _repository;

    public ElectronicInvoicePrintSettingService(
        IElectronicInvoicePrintSettingRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<ElectronicInvoicePrintSettingDto?> GetSettingAsync(string settingId)
    {
        try
        {
            var setting = await _repository.GetByIdAsync(settingId);
            if (setting == null)
            {
                return null;
            }

            return MapToDto(setting);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢電子發票列印設定失敗: {settingId}", ex);
            throw;
        }
    }

    public async Task<List<ElectronicInvoicePrintSettingDto>> GetAllActiveSettingsAsync()
    {
        try
        {
            var settings = await _repository.GetAllActiveAsync();
            return settings.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢電子發票列印設定列表失敗", ex);
            throw;
        }
    }

    public async Task UpdateSettingAsync(UpdateElectronicInvoicePrintSettingDto dto)
    {
        try
        {
            var setting = await _repository.GetByIdAsync(dto.SettingId);
            if (setting == null)
            {
                throw new KeyNotFoundException($"電子發票列印設定不存在: {dto.SettingId}");
            }

            setting.PrintFormat = dto.PrintFormat;
            setting.BarcodeType = dto.BarcodeType;
            setting.BarcodeSize = dto.BarcodeSize;
            setting.BarcodeMargin = dto.BarcodeMargin;
            setting.ColCount = dto.ColCount;
            setting.PageCount = dto.PageCount;
            setting.Status = dto.Status;
            setting.UpdatedBy = GetCurrentUserId();
            setting.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(setting);
            _logger.LogInfo($"更新電子發票列印設定成功: {dto.SettingId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新電子發票列印設定失敗: {dto.SettingId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 將 Entity 轉換為 DTO
    /// </summary>
    private ElectronicInvoicePrintSettingDto MapToDto(ElectronicInvoicePrintSetting setting)
    {
        return new ElectronicInvoicePrintSettingDto
        {
            SettingId = setting.SettingId,
            PrintFormat = setting.PrintFormat,
            BarcodeType = setting.BarcodeType,
            BarcodeSize = setting.BarcodeSize,
            BarcodeMargin = setting.BarcodeMargin,
            ColCount = setting.ColCount,
            PageCount = setting.PageCount,
            Status = setting.Status,
            CreatedBy = setting.CreatedBy,
            CreatedAt = setting.CreatedAt,
            UpdatedBy = setting.UpdatedBy,
            UpdatedAt = setting.UpdatedAt
        };
    }
}

