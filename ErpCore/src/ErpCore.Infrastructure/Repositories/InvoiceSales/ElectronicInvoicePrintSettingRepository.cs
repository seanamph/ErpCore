using System.Data;
using Dapper;
using ErpCore.Domain.Entities.InvoiceSales;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.InvoiceSales;

/// <summary>
/// 電子發票列印設定 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ElectronicInvoicePrintSettingRepository : BaseRepository, IElectronicInvoicePrintSettingRepository
{
    public ElectronicInvoicePrintSettingRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<ElectronicInvoicePrintSetting?> GetByIdAsync(string settingId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM ElectronicInvoicePrintSettings 
                WHERE SettingId = @SettingId";

            return await QueryFirstOrDefaultAsync<ElectronicInvoicePrintSetting>(sql, new { SettingId = settingId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢電子發票列印設定失敗: {settingId}", ex);
            throw;
        }
    }

    public async Task<List<ElectronicInvoicePrintSetting>> GetAllActiveAsync()
    {
        try
        {
            const string sql = @"
                SELECT * FROM ElectronicInvoicePrintSettings 
                WHERE Status = 'A'
                ORDER BY SettingId";

            var items = await QueryAsync<ElectronicInvoicePrintSetting>(sql);
            return items.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢電子發票列印設定列表失敗", ex);
            throw;
        }
    }

    public async Task<int> CreateAsync(ElectronicInvoicePrintSetting setting)
    {
        try
        {
            const string sql = @"
                INSERT INTO ElectronicInvoicePrintSettings (
                    SettingId, PrintFormat, BarcodeType, BarcodeSize, BarcodeMargin,
                    ColCount, PageCount, Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                )
                VALUES (
                    @SettingId, @PrintFormat, @BarcodeType, @BarcodeSize, @BarcodeMargin,
                    @ColCount, @PageCount, @Status, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                )";

            return await ExecuteAsync(sql, setting);
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增電子發票列印設定失敗: {setting.SettingId}", ex);
            throw;
        }
    }

    public async Task<int> UpdateAsync(ElectronicInvoicePrintSetting setting)
    {
        try
        {
            const string sql = @"
                UPDATE ElectronicInvoicePrintSettings SET
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
            _logger.LogError($"修改電子發票列印設定失敗: {setting.SettingId}", ex);
            throw;
        }
    }

    public async Task<int> DeleteAsync(string settingId)
    {
        try
        {
            const string sql = @"
                DELETE FROM ElectronicInvoicePrintSettings 
                WHERE SettingId = @SettingId";

            return await ExecuteAsync(sql, new { SettingId = settingId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除電子發票列印設定失敗: {settingId}", ex);
            throw;
        }
    }
}

