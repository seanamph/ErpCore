using Dapper;
using ErpCore.Domain.Entities.Accounting;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.TaxAccounting;

/// <summary>
/// 稅務報表查詢 Repository 實作 (SYST411-SYST452)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class TaxReportRepository : BaseRepository, ITaxReportRepository
{
    public TaxReportRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PagedResult<Voucher>> GetVouchersAsync(TaxReportVoucherQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Vouchers
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (query.DateFrom.HasValue)
            {
                sql += " AND VoucherDate >= @DateFrom";
                parameters.Add("DateFrom", query.DateFrom.Value);
            }

            if (query.DateTo.HasValue)
            {
                sql += " AND VoucherDate <= @DateTo";
                parameters.Add("DateTo", query.DateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.VoucherIdFrom))
            {
                sql += " AND VoucherId >= @VoucherIdFrom";
                parameters.Add("VoucherIdFrom", query.VoucherIdFrom);
            }

            if (!string.IsNullOrEmpty(query.VoucherIdTo))
            {
                sql += " AND VoucherId <= @VoucherIdTo";
                parameters.Add("VoucherIdTo", query.VoucherIdTo);
            }

            if (query.VoucherKinds != null && query.VoucherKinds.Count > 0)
            {
                sql += " AND VoucherKind IN @VoucherKinds";
                parameters.Add("VoucherKinds", query.VoucherKinds);
            }

            if (query.VoucherStatuses != null && query.VoucherStatuses.Count > 0)
            {
                sql += " AND VoucherStatus IN @VoucherStatuses";
                parameters.Add("VoucherStatuses", query.VoucherStatuses);
            }

            if (!string.IsNullOrEmpty(query.CreatedBy))
            {
                sql += " AND CreatedBy = @CreatedBy";
                parameters.Add("CreatedBy", query.CreatedBy);
            }

            if (query.CreatedDateFrom.HasValue)
            {
                sql += " AND CreatedAt >= @CreatedDateFrom";
                parameters.Add("CreatedDateFrom", query.CreatedDateFrom.Value);
            }

            if (query.CreatedDateTo.HasValue)
            {
                sql += " AND CreatedAt <= @CreatedDateTo";
                parameters.Add("CreatedDateTo", query.CreatedDateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.SiteId))
            {
                sql += " AND SiteId = @SiteId";
                parameters.Add("SiteId", query.SiteId);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "VoucherDate" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 總數查詢
            var countSql = sql.Replace("SELECT *", "SELECT COUNT(*)");
            var totalCount = await QuerySingleAsync<int>(countSql, parameters);

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<Voucher>(sql, parameters);

            return new PagedResult<Voucher>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢傳票列表失敗", ex);
            throw;
        }
    }

    public async Task<List<VoucherDetail>> GetVoucherDetailsAsync(string voucherId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM VoucherDetails
                WHERE VoucherId = @VoucherId
                ORDER BY SeqNo";

            return (await QueryAsync<VoucherDetail>(sql, new { VoucherId = voucherId })).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢傳票明細失敗: {voucherId}", ex);
            throw;
        }
    }
}

