using System.Data;
using Dapper;
using ErpCore.Domain.Entities.InvoiceSalesB2B;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.InvoiceSalesB2B;

/// <summary>
/// B2B電子發票 Repository 實作 (SYSG000_B2B - B2B電子發票列印)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class B2BElectronicInvoiceRepository : BaseRepository, IB2BElectronicInvoiceRepository
{
    public B2BElectronicInvoiceRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<B2BElectronicInvoice?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM B2BElectronicInvoices 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<B2BElectronicInvoice>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢B2B電子發票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<B2BElectronicInvoice?> GetByInvoiceIdAsync(string invoiceId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM B2BElectronicInvoices 
                WHERE InvoiceId = @InvoiceId";

            return await QueryFirstOrDefaultAsync<B2BElectronicInvoice>(sql, new { InvoiceId = invoiceId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢B2B電子發票失敗: {invoiceId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<B2BElectronicInvoice>> QueryAsync(B2BElectronicInvoiceQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM B2BElectronicInvoices
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.InvoiceId))
            {
                sql += " AND InvoiceId = @InvoiceId";
                parameters.Add("InvoiceId", query.InvoiceId);
            }

            if (!string.IsNullOrEmpty(query.PosId))
            {
                sql += " AND PosId = @PosId";
                parameters.Add("PosId", query.PosId);
            }

            if (!string.IsNullOrEmpty(query.InvYm))
            {
                sql += " AND InvYm = @InvYm";
                parameters.Add("InvYm", query.InvYm);
            }

            if (!string.IsNullOrEmpty(query.Track))
            {
                sql += " AND Track = @Track";
                parameters.Add("Track", query.Track);
            }

            if (!string.IsNullOrEmpty(query.PrizeType))
            {
                sql += " AND PrizeType = @PrizeType";
                parameters.Add("PrizeType", query.PrizeType);
            }

            if (!string.IsNullOrEmpty(query.TransferType))
            {
                sql += " AND TransferType = @TransferType";
                parameters.Add("TransferType", query.TransferType);
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

            var items = await QueryAsync<B2BElectronicInvoice>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM B2BElectronicInvoices
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.InvoiceId))
            {
                countSql += " AND InvoiceId = @InvoiceId";
                countParameters.Add("InvoiceId", query.InvoiceId);
            }
            if (!string.IsNullOrEmpty(query.PosId))
            {
                countSql += " AND PosId = @PosId";
                countParameters.Add("PosId", query.PosId);
            }
            if (!string.IsNullOrEmpty(query.InvYm))
            {
                countSql += " AND InvYm = @InvYm";
                countParameters.Add("InvYm", query.InvYm);
            }
            if (!string.IsNullOrEmpty(query.Track))
            {
                countSql += " AND Track = @Track";
                countParameters.Add("Track", query.Track);
            }
            if (!string.IsNullOrEmpty(query.PrizeType))
            {
                countSql += " AND PrizeType = @PrizeType";
                countParameters.Add("PrizeType", query.PrizeType);
            }
            if (!string.IsNullOrEmpty(query.TransferType))
            {
                countSql += " AND TransferType = @TransferType";
                countParameters.Add("TransferType", query.TransferType);
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

            return new PagedResult<B2BElectronicInvoice>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢B2B電子發票列表失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(B2BElectronicInvoice electronicInvoice)
    {
        try
        {
            const string sql = @"
                INSERT INTO B2BElectronicInvoices (
                    InvoiceId, PosId, InvYm, Track, InvNoB, InvNoE, PrintCode,
                    InvoiceDate, PrizeType, PrizeAmt, CarrierIdClear,
                    AwardPrint, AwardPos, AwardDate, B2BFlag, TransferType, TransferStatus, Status,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                )
                VALUES (
                    @InvoiceId, @PosId, @InvYm, @Track, @InvNoB, @InvNoE, @PrintCode,
                    @InvoiceDate, @PrizeType, @PrizeAmt, @CarrierIdClear,
                    @AwardPrint, @AwardPos, @AwardDate, @B2BFlag, @TransferType, @TransferStatus, @Status,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, electronicInvoice);
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增B2B電子發票失敗: {electronicInvoice.InvoiceId}", ex);
            throw;
        }
    }

    public async Task<int> UpdateAsync(B2BElectronicInvoice electronicInvoice)
    {
        try
        {
            const string sql = @"
                UPDATE B2BElectronicInvoices SET
                    InvoiceId = @InvoiceId,
                    PosId = @PosId,
                    InvYm = @InvYm,
                    Track = @Track,
                    InvNoB = @InvNoB,
                    InvNoE = @InvNoE,
                    PrintCode = @PrintCode,
                    InvoiceDate = @InvoiceDate,
                    PrizeType = @PrizeType,
                    PrizeAmt = @PrizeAmt,
                    CarrierIdClear = @CarrierIdClear,
                    AwardPrint = @AwardPrint,
                    AwardPos = @AwardPos,
                    AwardDate = @AwardDate,
                    B2BFlag = @B2BFlag,
                    TransferType = @TransferType,
                    TransferStatus = @TransferStatus,
                    Status = @Status,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            return await ExecuteAsync(sql, electronicInvoice);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改B2B電子發票失敗: {electronicInvoice.TKey}", ex);
            throw;
        }
    }

    public async Task<int> DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM B2BElectronicInvoices 
                WHERE TKey = @TKey";

            return await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除B2B電子發票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsByInvoiceIdAsync(string invoiceId, long? excludeTKey = null)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM B2BElectronicInvoices 
                WHERE InvoiceId = @InvoiceId";

            var parameters = new DynamicParameters();
            parameters.Add("InvoiceId", invoiceId);

            if (excludeTKey.HasValue)
            {
                sql += " AND TKey != @TKey";
                parameters.Add("TKey", excludeTKey.Value);
            }

            var count = await ExecuteScalarAsync<int>(sql, parameters);
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查B2B電子發票編號是否存在失敗: {invoiceId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<B2BElectronicInvoice>> QueryAwardListAsync(B2BAwardListQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM B2BElectronicInvoices
                WHERE PrizeType IS NOT NULL AND PrizeType != ''
                AND Status = 'A'";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.InvYm))
            {
                sql += " AND InvYm = @InvYm";
                parameters.Add("InvYm", query.InvYm);
            }

            if (!string.IsNullOrEmpty(query.PrizeType))
            {
                sql += " AND PrizeType = @PrizeType";
                parameters.Add("PrizeType", query.PrizeType);
            }

            if (!string.IsNullOrEmpty(query.B2BFlag))
            {
                sql += " AND B2BFlag = @B2BFlag";
                parameters.Add("B2BFlag", query.B2BFlag);
            }

            sql += " ORDER BY InvYm DESC, PrizeType, InvoiceDate DESC";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<B2BElectronicInvoice>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM B2BElectronicInvoices
                WHERE PrizeType IS NOT NULL AND PrizeType != ''
                AND Status = 'A'";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.InvYm))
            {
                countSql += " AND InvYm = @InvYm";
                countParameters.Add("InvYm", query.InvYm);
            }
            if (!string.IsNullOrEmpty(query.PrizeType))
            {
                countSql += " AND PrizeType = @PrizeType";
                countParameters.Add("PrizeType", query.PrizeType);
            }
            if (!string.IsNullOrEmpty(query.B2BFlag))
            {
                countSql += " AND B2BFlag = @B2BFlag";
                countParameters.Add("B2BFlag", query.B2BFlag);
            }

            var totalCount = await ExecuteScalarAsync<int>(countSql, countParameters);

            return new PagedResult<B2BElectronicInvoice>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢B2B中獎清冊失敗", ex);
            throw;
        }
    }
}

