using System.Data;
using System.Linq;
using Dapper;
using ErpCore.Domain.Entities.InvoiceSales;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.InvoiceSales;

/// <summary>
/// 發票 Repository 實作 (SYSG110-SYSG190 - 發票資料維護)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class InvoiceRepository : BaseRepository, IInvoiceRepository
{
    public InvoiceRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Invoice?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Invoices 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<Invoice>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢發票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<Invoice?> GetByInvoiceIdAsync(string invoiceId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Invoices 
                WHERE InvoiceId = @InvoiceId";

            return await QueryFirstOrDefaultAsync<Invoice>(sql, new { InvoiceId = invoiceId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢發票失敗: {invoiceId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<Invoice>> QueryAsync(InvoiceQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Invoices
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.InvoiceId))
            {
                sql += " AND InvoiceId LIKE @InvoiceId";
                parameters.Add("InvoiceId", $"%{query.InvoiceId}%");
            }

            if (!string.IsNullOrEmpty(query.InvoiceType))
            {
                sql += " AND InvoiceType = @InvoiceType";
                parameters.Add("InvoiceType", query.InvoiceType);
            }

            if (!string.IsNullOrEmpty(query.InvoiceYm))
            {
                sql += " AND InvoiceYm = @InvoiceYm";
                parameters.Add("InvoiceYm", query.InvoiceYm);
            }

            if (!string.IsNullOrEmpty(query.TaxId))
            {
                sql += " AND TaxId LIKE @TaxId";
                parameters.Add("TaxId", $"%{query.TaxId}%");
            }

            if (!string.IsNullOrEmpty(query.SiteId))
            {
                sql += " AND SiteId = @SiteId";
                parameters.Add("SiteId", query.SiteId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 排序
            if (!string.IsNullOrEmpty(query.SortField))
            {
                var sortOrder = query.SortOrder == "DESC" ? "DESC" : "ASC";
                sql += $" ORDER BY {query.SortField} {sortOrder}";
            }
            else
            {
                sql += " ORDER BY CreatedAt DESC";
            }

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<Invoice>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM Invoices
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.InvoiceId))
            {
                countSql += " AND InvoiceId LIKE @InvoiceId";
                countParameters.Add("InvoiceId", $"%{query.InvoiceId}%");
            }
            if (!string.IsNullOrEmpty(query.InvoiceType))
            {
                countSql += " AND InvoiceType = @InvoiceType";
                countParameters.Add("InvoiceType", query.InvoiceType);
            }
            if (!string.IsNullOrEmpty(query.InvoiceYm))
            {
                countSql += " AND InvoiceYm = @InvoiceYm";
                countParameters.Add("InvoiceYm", query.InvoiceYm);
            }
            if (!string.IsNullOrEmpty(query.TaxId))
            {
                countSql += " AND TaxId LIKE @TaxId";
                countParameters.Add("TaxId", $"%{query.TaxId}%");
            }
            if (!string.IsNullOrEmpty(query.SiteId))
            {
                countSql += " AND SiteId = @SiteId";
                countParameters.Add("SiteId", query.SiteId);
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }

            var totalCount = await ExecuteScalarAsync<int>(countSql, countParameters);

            return new PagedResult<Invoice>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢發票列表失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(Invoice invoice)
    {
        try
        {
            const string sql = @"
                INSERT INTO Invoices (
                    InvoiceId, InvoiceType, InvoiceYear, InvoiceMonth, InvoiceYm,
                    Track, InvoiceNoB, InvoiceNoE, InvoiceFormat, TaxId,
                    CompanyName, CompanyNameEn, Address, City, Zone, PostalCode,
                    Phone, Fax, Email, SiteId, SubCopy, SubCopyValue,
                    Status, Notes, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                )
                VALUES (
                    @InvoiceId, @InvoiceType, @InvoiceYear, @InvoiceMonth, @InvoiceYm,
                    @Track, @InvoiceNoB, @InvoiceNoE, @InvoiceFormat, @TaxId,
                    @CompanyName, @CompanyNameEn, @Address, @City, @Zone, @PostalCode,
                    @Phone, @Fax, @Email, @SiteId, @SubCopy, @SubCopyValue,
                    @Status, @Notes, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, invoice);
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增發票失敗: {invoice.InvoiceId}", ex);
            throw;
        }
    }

    public async Task<int> UpdateAsync(Invoice invoice)
    {
        try
        {
            const string sql = @"
                UPDATE Invoices SET
                    InvoiceId = @InvoiceId,
                    InvoiceType = @InvoiceType,
                    InvoiceYear = @InvoiceYear,
                    InvoiceMonth = @InvoiceMonth,
                    InvoiceYm = @InvoiceYm,
                    Track = @Track,
                    InvoiceNoB = @InvoiceNoB,
                    InvoiceNoE = @InvoiceNoE,
                    InvoiceFormat = @InvoiceFormat,
                    TaxId = @TaxId,
                    CompanyName = @CompanyName,
                    CompanyNameEn = @CompanyNameEn,
                    Address = @Address,
                    City = @City,
                    Zone = @Zone,
                    PostalCode = @PostalCode,
                    Phone = @Phone,
                    Fax = @Fax,
                    Email = @Email,
                    SiteId = @SiteId,
                    SubCopy = @SubCopy,
                    SubCopyValue = @SubCopyValue,
                    Status = @Status,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            return await ExecuteAsync(sql, invoice);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改發票失敗: {invoice.TKey}", ex);
            throw;
        }
    }

    public async Task<int> DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM Invoices 
                WHERE TKey = @TKey";

            return await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除發票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsByInvoiceIdAsync(string invoiceId, long? excludeTKey = null)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM Invoices 
                WHERE InvoiceId = @InvoiceId";

            var parameters = new DynamicParameters();
            parameters.Add("InvoiceId", invoiceId);

            if (excludeTKey.HasValue)
            {
                sql += " AND TKey != @ExcludeTKey";
                parameters.Add("ExcludeTKey", excludeTKey.Value);
            }

            var count = await ExecuteScalarAsync<int>(sql, parameters);
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查發票編號是否存在失敗: {invoiceId}", ex);
            throw;
        }
    }
}

