using System.Data;
using Dapper;
using ErpCore.Domain.Entities.InvoiceSalesB2B;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.InvoiceSalesB2B;

/// <summary>
/// B2B電子發票列印設定 Repository 實作 (SYSG000_B2B - B2B電子發票列印)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class B2BElectronicInvoicePrintSettingRepository : BaseRepository, IB2BElectronicInvoicePrintSettingRepository
{
    public B2BElectronicInvoicePrintSettingRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<B2BElectronicInvoicePrintSetting?> GetByIdAsync(string settingId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM B2BElectronicInvoicePrintSettings 
                WHERE SettingId = @SettingId";

            return await QueryFirstOrDefaultAsync<B2BElectronicInvoicePrintSetting>(sql, new { SettingId = settingId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢B2B電子發票列印設定失敗: {settingId}", ex);
            throw;
        }
    }

    public async Task<List<B2BElectronicInvoicePrintSetting>> GetAllActiveAsync()
    {
        try
        {
            const string sql = @"
                SELECT * FROM B2BElectronicInvoicePrintSettings 
                WHERE Status = 'A' AND B2BFlag = 'Y'
                ORDER BY SettingId";

            var items = await QueryAsync<B2BElectronicInvoicePrintSetting>(sql);
            return items.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢B2B電子發票列印設定列表失敗", ex);
            throw;
        }
    }

    public async Task<int> CreateAsync(B2BElectronicInvoicePrintSetting setting)
    {
        try
        {
            const string sql = @"
                INSERT INTO B2BElectronicInvoicePrintSettings (
                    SettingId, PrintFormat, BarcodeType, BarcodeSize, BarcodeMargin,
                    ColCount, PageCount, B2BFlag, Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                )
                VALUES (
                    @SettingId, @PrintFormat, @BarcodeType, @BarcodeSize, @BarcodeMargin,
                    @ColCount, @PageCount, @B2BFlag, @Status, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                )";

            return await ExecuteAsync(sql, setting);
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增B2B電子發票列印設定失敗: {setting.SettingId}", ex);
            throw;
        }
    }

    public async Task<int> UpdateAsync(B2BElectronicInvoicePrintSetting setting)
    {
        try
        {
            const string sql = @"
                UPDATE B2BElectronicInvoicePrintSettings SET
                    PrintFormat = @PrintFormat,
                    BarcodeType = @BarcodeType,
                    BarcodeSize = @BarcodeSize,
                    BarcodeMargin = @BarcodeMargin,
                    ColCount = @ColCount,
                    PageCount = @PageCount,
                    Status = @Status,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE SettingId = @SettingId";

            return await ExecuteAsync(sql, setting);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改B2B電子發票列印設定失敗: {setting.SettingId}", ex);
            throw;
        }
    }

    public async Task<int> DeleteAsync(string settingId)
    {
        try
        {
            const string sql = @"
                DELETE FROM B2BElectronicInvoicePrintSettings 
                WHERE SettingId = @SettingId";

            return await ExecuteAsync(sql, new { SettingId = settingId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除B2B電子發票列印設定失敗: {settingId}", ex);
            throw;
        }
    }
}

