using System.Data;
using Dapper;
using ErpCore.Domain.Entities.InvoiceSalesB2B;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.InvoiceSalesB2B;

/// <summary>
/// B2B發票 Repository 實作 (SYSG000_B2B - B2B發票資料維護)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class B2BInvoiceRepository : BaseRepository, IB2BInvoiceRepository
{
    public B2BInvoiceRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<B2BInvoice?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM B2BInvoices 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<B2BInvoice>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢B2B發票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<B2BInvoice?> GetByInvoiceIdAsync(string invoiceId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM B2BInvoices 
                WHERE InvoiceId = @InvoiceId";

            return await QueryFirstOrDefaultAsync<B2BInvoice>(sql, new { InvoiceId = invoiceId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢B2B發票失敗: {invoiceId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<B2BInvoice>> QueryAsync(B2BInvoiceQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM B2BInvoices
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

            if (!string.IsNullOrEmpty(query.B2BFlag))
            {
                sql += " AND B2BFlag = @B2BFlag";
                parameters.Add("B2BFlag", query.B2BFlag);
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

            var items = await QueryAsync<B2BInvoice>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM B2BInvoices
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
            if (!string.IsNullOrEmpty(query.B2BFlag))
            {
                countSql += " AND B2BFlag = @B2BFlag";
                countParameters.Add("B2BFlag", query.B2BFlag);
            }

            var totalCount = await ExecuteScalarAsync<int>(countSql, countParameters);

            return new PagedResult<B2BInvoice>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢B2B發票列表失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(B2BInvoice invoice)
    {
        try
        {
            const string sql = @"
                INSERT INTO B2BInvoices (
                    InvoiceId, InvoiceType, InvoiceYear, InvoiceMonth, InvoiceYm,
                    Track, InvoiceNoB, InvoiceNoE, InvoiceFormat, TaxId,
                    CompanyName, CompanyNameEn, Address, City, Zone, PostalCode,
                    Phone, Fax, Email, SiteId, SubCopy, SubCopyValue,
                    B2BFlag, Status, Notes, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                )
                VALUES (
                    @InvoiceId, @InvoiceType, @InvoiceYear, @InvoiceMonth, @InvoiceYm,
                    @Track, @InvoiceNoB, @InvoiceNoE, @InvoiceFormat, @TaxId,
                    @CompanyName, @CompanyNameEn, @Address, @City, @Zone, @PostalCode,
                    @Phone, @Fax, @Email, @SiteId, @SubCopy, @SubCopyValue,
                    @B2BFlag, @Status, @Notes, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, invoice);
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增B2B發票失敗: {invoice.InvoiceId}", ex);
            throw;
        }
    }

    public async Task<int> UpdateAsync(B2BInvoice invoice)
    {
        try
        {
            const string sql = @"
                UPDATE B2BInvoices SET
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
                    B2BFlag = @B2BFlag,
                    Status = @Status,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            return await ExecuteAsync(sql, invoice);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改B2B發票失敗: {invoice.TKey}", ex);
            throw;
        }
    }

    public async Task<int> DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM B2BInvoices 
                WHERE TKey = @TKey";

            return await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除B2B發票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsByInvoiceIdAsync(string invoiceId, long? excludeTKey = null)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM B2BInvoices 
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
            _logger.LogError($"檢查B2B發票編號是否存在失敗: {invoiceId}", ex);
            throw;
        }
    }
}

