using System.Data;
using Dapper;
using ErpCore.Domain.Entities.InvoiceSales;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.InvoiceSales;

/// <summary>
/// 電子發票 Repository 實作 (SYSG210-SYSG2B0 - 電子發票列印)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ElectronicInvoiceRepository : BaseRepository, IElectronicInvoiceRepository
{
    public ElectronicInvoiceRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<ElectronicInvoice?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM ElectronicInvoices 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<ElectronicInvoice>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢電子發票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<PagedResult<ElectronicInvoice>> QueryAsync(ElectronicInvoiceQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM ElectronicInvoices
                WHERE 1=1";

            var parameters = new DynamicParameters();

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

            if (!string.IsNullOrEmpty(query.PosId))
            {
                sql += " AND PosId = @PosId";
                parameters.Add("PosId", query.PosId);
            }

            if (!string.IsNullOrEmpty(query.PrizeType))
            {
                sql += " AND PrizeType = @PrizeType";
                parameters.Add("PrizeType", query.PrizeType);
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

            var items = await QueryAsync<ElectronicInvoice>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM ElectronicInvoices
                WHERE 1=1";

            var countParameters = new DynamicParameters();
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
            if (!string.IsNullOrEmpty(query.PosId))
            {
                countSql += " AND PosId = @PosId";
                countParameters.Add("PosId", query.PosId);
            }
            if (!string.IsNullOrEmpty(query.PrizeType))
            {
                countSql += " AND PrizeType = @PrizeType";
                countParameters.Add("PrizeType", query.PrizeType);
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }

            var totalCount = await ExecuteScalarAsync<int>(countSql, countParameters);

            return new PagedResult<ElectronicInvoice>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢電子發票列表失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(ElectronicInvoice electronicInvoice)
    {
        try
        {
            const string sql = @"
                INSERT INTO ElectronicInvoices (
                    PosId, InvYm, Track, InvNoB, InvNoE, PrintCode,
                    InvoiceDate, PrizeType, PrizeAmt, CarrierIdClear,
                    AwardPrint, AwardPos, AwardDate, Status,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                )
                VALUES (
                    @PosId, @InvYm, @Track, @InvNoB, @InvNoE, @PrintCode,
                    @InvoiceDate, @PrizeType, @PrizeAmt, @CarrierIdClear,
                    @AwardPrint, @AwardPos, @AwardDate, @Status,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, electronicInvoice);
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增電子發票失敗: {electronicInvoice.InvYm}", ex);
            throw;
        }
    }

    public async Task<int> UpdateAsync(ElectronicInvoice electronicInvoice)
    {
        try
        {
            const string sql = @"
                UPDATE ElectronicInvoices SET
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
                    Status = @Status,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            return await ExecuteAsync(sql, electronicInvoice);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改電子發票失敗: {electronicInvoice.TKey}", ex);
            throw;
        }
    }

    public async Task<int> DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM ElectronicInvoices 
                WHERE TKey = @TKey";

            return await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除電子發票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<PagedResult<ElectronicInvoice>> QueryAwardListAsync(AwardListQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM ElectronicInvoices
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

            sql += " ORDER BY InvYm DESC, PrizeType, InvoiceDate DESC";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<ElectronicInvoice>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM ElectronicInvoices
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

            var totalCount = await ExecuteScalarAsync<int>(countSql, countParameters);

            return new PagedResult<ElectronicInvoice>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢中獎清冊失敗", ex);
            throw;
        }
    }
}

