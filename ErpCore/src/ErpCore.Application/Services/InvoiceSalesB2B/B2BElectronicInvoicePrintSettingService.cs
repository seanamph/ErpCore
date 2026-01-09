using ErpCore.Application.DTOs.InvoiceSalesB2B;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.InvoiceSalesB2B;
using ErpCore.Infrastructure.Repositories.InvoiceSalesB2B;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.InvoiceSalesB2B;

/// <summary>
/// B2B電子發票列印設定服務實作 (SYSG000_B2B - B2B電子發票列印)
/// </summary>
public class B2BElectronicInvoicePrintSettingService : BaseService, IB2BElectronicInvoicePrintSettingService
{
    private readonly IB2BElectronicInvoicePrintSettingRepository _repository;

    public B2BElectronicInvoicePrintSettingService(
        IB2BElectronicInvoicePrintSettingRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<B2BElectronicInvoicePrintSettingDto?> GetSettingAsync(string settingId)
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
            _logger.LogError($"查詢B2B電子發票列印設定失敗: {settingId}", ex);
            throw;
        }
    }

    public async Task<List<B2BElectronicInvoicePrintSettingDto>> GetAllActiveSettingsAsync()
    {
        try
        {
            var settings = await _repository.GetAllActiveAsync();
            return settings.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢B2B電子發票列印設定列表失敗", ex);
            throw;
        }
    }

    public async Task UpdateSettingAsync(UpdateB2BElectronicInvoicePrintSettingDto dto)
    {
        try
        {
            var setting = await _repository.GetByIdAsync(dto.SettingId);
            if (setting == null)
            {
                throw new KeyNotFoundException($"B2B電子發票列印設定不存在: {dto.SettingId}");
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
            _logger.LogInfo($"更新B2B電子發票列印設定成功: {dto.SettingId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新B2B電子發票列印設定失敗: {dto.SettingId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 將 Entity 轉換為 DTO
    /// </summary>
    private B2BElectronicInvoicePrintSettingDto MapToDto(B2BElectronicInvoicePrintSetting setting)
    {
        return new B2BElectronicInvoicePrintSettingDto
        {
            SettingId = setting.SettingId,
            PrintFormat = setting.PrintFormat,
            BarcodeType = setting.BarcodeType,
            BarcodeSize = setting.BarcodeSize,
            BarcodeMargin = setting.BarcodeMargin,
            ColCount = setting.ColCount,
            PageCount = setting.PageCount,
            B2BFlag = setting.B2BFlag,
            Status = setting.Status,
            CreatedBy = setting.CreatedBy,
            CreatedAt = setting.CreatedAt,
            UpdatedBy = setting.UpdatedBy,
            UpdatedAt = setting.UpdatedAt
        };
    }
}

