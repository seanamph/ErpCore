using Dapper;
using ErpCore.Domain.Entities.Accounting;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.TaxAccounting;

/// <summary>
/// 交易資料處理 Repository 實作 (SYST221, SYST311-SYST352)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class TransactionDataRepository : BaseRepository, ITransactionDataRepository
{
    public TransactionDataRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PagedResult<Voucher>> GetConfirmVouchersAsync(VoucherConfirmQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Vouchers
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.VoucherNoFrom))
            {
                sql += " AND VoucherId >= @VoucherNoFrom";
                parameters.Add("VoucherNoFrom", query.VoucherNoFrom);
            }

            if (!string.IsNullOrEmpty(query.VoucherNoTo))
            {
                sql += " AND VoucherId <= @VoucherNoTo";
                parameters.Add("VoucherNoTo", query.VoucherNoTo);
            }

            if (query.VoucherDateFrom.HasValue)
            {
                sql += " AND VoucherDate >= @VoucherDateFrom";
                parameters.Add("VoucherDateFrom", query.VoucherDateFrom.Value);
            }

            if (query.VoucherDateTo.HasValue)
            {
                sql += " AND VoucherDate <= @VoucherDateTo";
                parameters.Add("VoucherDateTo", query.VoucherDateTo.Value);
            }

            if (query.VoucherTypes != null && query.VoucherTypes.Count > 0)
            {
                sql += " AND VoucherTypeId IN @VoucherTypes";
                parameters.Add("VoucherTypes", query.VoucherTypes);
            }

            if (!string.IsNullOrEmpty(query.VoucherStatus))
            {
                sql += " AND VoucherStatus = @VoucherStatus";
                parameters.Add("VoucherStatus", query.VoucherStatus);
            }

            if (query.ConfirmDateFrom.HasValue)
            {
                sql += " AND ConfirmDate >= @ConfirmDateFrom";
                parameters.Add("ConfirmDateFrom", query.ConfirmDateFrom.Value);
            }

            if (query.ConfirmDateTo.HasValue)
            {
                sql += " AND ConfirmDate <= @ConfirmDateTo";
                parameters.Add("ConfirmDateTo", query.ConfirmDateTo.Value);
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
            _logger.LogError("查詢傳票確認列表失敗", ex);
            throw;
        }
    }

    public async Task<int> BatchConfirmVouchersAsync(List<string> voucherIds, DateTime confirmDate, string confirmBy)
    {
        try
        {
            const string sql = @"
                UPDATE Vouchers SET
                    VoucherStatus = '2',
                    ConfirmDate = @ConfirmDate,
                    ConfirmBy = @ConfirmBy,
                    UpdatedBy = @ConfirmBy,
                    UpdatedAt = GETDATE()
                WHERE VoucherId IN @VoucherIds
                AND VoucherStatus = '1'";

            var parameters = new { VoucherIds = voucherIds, ConfirmDate = confirmDate, ConfirmBy = confirmBy };
            return await ExecuteAsync(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("批次確認傳票失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<Voucher>> GetPostingVouchersAsync(VoucherPostingQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Vouchers
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.PostingYearMonth))
            {
                sql += " AND PostingYearMonth = @PostingYearMonth";
                parameters.Add("PostingYearMonth", query.PostingYearMonth);
            }

            if (query.VoucherDateFrom.HasValue)
            {
                sql += " AND VoucherDate >= @VoucherDateFrom";
                parameters.Add("VoucherDateFrom", query.VoucherDateFrom.Value);
            }

            if (query.VoucherDateTo.HasValue)
            {
                sql += " AND VoucherDate <= @VoucherDateTo";
                parameters.Add("VoucherDateTo", query.VoucherDateTo.Value);
            }

            if (query.VoucherTypes != null && query.VoucherTypes.Count > 0)
            {
                sql += " AND VoucherTypeId IN @VoucherTypes";
                parameters.Add("VoucherTypes", query.VoucherTypes);
            }

            if (!string.IsNullOrEmpty(query.VoucherStatus))
            {
                sql += " AND VoucherStatus = @VoucherStatus";
                parameters.Add("VoucherStatus", query.VoucherStatus);
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
            _logger.LogError("查詢傳票過帳列表失敗", ex);
            throw;
        }
    }

    public async Task<int> BatchPostingVouchersAsync(List<string> voucherIds, string postingYearMonth, DateTime postingDate, string postingBy)
    {
        try
        {
            const string sql = @"
                UPDATE Vouchers SET
                    VoucherStatus = '3',
                    PostingYearMonth = @PostingYearMonth,
                    PostingDate = @PostingDate,
                    PostingBy = @PostingBy,
                    UpdatedBy = @PostingBy,
                    UpdatedAt = GETDATE()
                WHERE VoucherId IN @VoucherIds
                AND VoucherStatus = '2'";

            var parameters = new { VoucherIds = voucherIds, PostingYearMonth = postingYearMonth, PostingDate = postingDate, PostingBy = postingBy };
            return await ExecuteAsync(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("批次過帳傳票失敗", ex);
            throw;
        }
    }

    public async Task<VoucherStatusCount> GetVoucherStatusCountAsync(string postingYearMonth)
    {
        try
        {
            const string sql = @"
                SELECT 
                    @PostingYearMonth AS PostingYearMonth,
                    COUNT(CASE WHEN VoucherStatus = '1' THEN 1 END) AS CreateCount,
                    COUNT(CASE WHEN VoucherStatus = '2' THEN 1 END) AS ConfirmCount,
                    COUNT(CASE WHEN VoucherStatus = '3' THEN 1 END) AS PostingCount,
                    ISNULL(SUM(CASE WHEN VoucherStatus = '1' THEN 1 ELSE 0 END), 0) AS CreateAmount,
                    ISNULL(SUM(CASE WHEN VoucherStatus = '2' THEN 1 ELSE 0 END), 0) AS ConfirmAmount,
                    ISNULL(SUM(CASE WHEN VoucherStatus = '3' THEN 1 ELSE 0 END), 0) AS PostingAmount
                FROM Vouchers
                WHERE PostingYearMonth = @PostingYearMonth
                AND Status = 'D'";

            var result = await QueryFirstOrDefaultAsync<VoucherStatusCount>(sql, new { PostingYearMonth = postingYearMonth });
            return result ?? new VoucherStatusCount { PostingYearMonth = postingYearMonth };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢傳票狀態統計失敗: {postingYearMonth}", ex);
            throw;
        }
    }

    public async Task ReversePostingVoucherAsync(string voucherId, DateTime reversePostingDate, string reversePostingBy)
    {
        try
        {
            const string sql = @"
                UPDATE Vouchers SET
                    VoucherStatus = '2',
                    ReversePostingDate = @ReversePostingDate,
                    ReversePostingBy = @ReversePostingBy,
                    UpdatedBy = @ReversePostingBy,
                    UpdatedAt = GETDATE()
                WHERE VoucherId = @VoucherId
                AND VoucherStatus = '3'";

            await ExecuteAsync(sql, new { VoucherId = voucherId, ReversePostingDate = reversePostingDate, ReversePostingBy = reversePostingBy });
        }
        catch (Exception ex)
        {
            _logger.LogError($"反過帳傳票失敗: {voucherId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<Voucher>> GetReverseYearEndVouchersAsync(ReverseYearEndQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Vouchers
                WHERE ReversePostingDate IS NOT NULL
                AND YEAR(ReversePostingDate) = @Year";

            var parameters = new DynamicParameters();
            parameters.Add("Year", query.Year);

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "ReversePostingDate" : query.SortField;
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
            _logger.LogError("查詢反過帳資料年結處理失敗", ex);
            throw;
        }
    }
}

