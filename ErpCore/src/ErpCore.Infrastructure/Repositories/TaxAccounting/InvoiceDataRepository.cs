using Dapper;
using ErpCore.Domain.Entities.Accounting;
using ErpCore.Domain.Entities.TaxAccounting;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.TaxAccounting;

/// <summary>
/// 發票資料維護 Repository 實作 (SYST211-SYST212)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class InvoiceDataRepository : BaseRepository, IInvoiceDataRepository
{
    public InvoiceDataRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Voucher?> GetVoucherByIdAsync(string voucherId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Vouchers 
                WHERE VoucherId = @VoucherId";

            return await QueryFirstOrDefaultAsync<Voucher>(sql, new { VoucherId = voucherId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢傳票失敗: {voucherId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<Voucher>> QueryVouchersAsync(InvoiceVoucherQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Vouchers
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.VoucherId))
            {
                sql += " AND VoucherId LIKE @VoucherId";
                parameters.Add("VoucherId", $"%{query.VoucherId}%");
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

            if (!string.IsNullOrEmpty(query.VoucherStatus))
            {
                sql += " AND VoucherStatus = @VoucherStatus";
                parameters.Add("VoucherStatus", query.VoucherStatus);
            }

            if (!string.IsNullOrEmpty(query.VoucherKind))
            {
                sql += " AND VoucherKind = @VoucherKind";
                parameters.Add("VoucherKind", query.VoucherKind);
            }

            if (!string.IsNullOrEmpty(query.TypeId))
            {
                sql += " AND VoucherTypeId = @TypeId";
                parameters.Add("TypeId", query.TypeId);
            }

            if (!string.IsNullOrEmpty(query.SiteId))
            {
                sql += " AND SiteId = @SiteId";
                parameters.Add("SiteId", query.SiteId);
            }

            if (!string.IsNullOrEmpty(query.VendorId))
            {
                sql += " AND VendorId = @VendorId";
                parameters.Add("VendorId", query.VendorId);
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

    public async Task<Voucher> CreateVoucherAsync(Voucher voucher)
    {
        try
        {
            const string sql = @"
                INSERT INTO Vouchers (
                    VoucherId, VoucherDate, VoucherTypeId, Description, Status,
                    VoucherKind, VoucherStatus, InvYn, SiteId, VendorId, VendorName,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                )
                OUTPUT INSERTED.*
                VALUES (
                    @VoucherId, @VoucherDate, @VoucherTypeId, @Description, @Status,
                    @VoucherKind, @VoucherStatus, @InvYn, @SiteId, @VendorId, @VendorName,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                )";

            return await QuerySingleAsync<Voucher>(sql, voucher);
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增傳票失敗: {voucher.VoucherId}", ex);
            throw;
        }
    }

    public async Task<Voucher> UpdateVoucherAsync(Voucher voucher)
    {
        try
        {
            const string sql = @"
                UPDATE Vouchers SET
                    VoucherDate = @VoucherDate,
                    VoucherTypeId = @VoucherTypeId,
                    Description = @Description,
                    Status = @Status,
                    VoucherKind = @VoucherKind,
                    VoucherStatus = @VoucherStatus,
                    InvYn = @InvYn,
                    SiteId = @SiteId,
                    VendorId = @VendorId,
                    VendorName = @VendorName,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                OUTPUT INSERTED.*
                WHERE VoucherId = @VoucherId";

            return await QuerySingleAsync<Voucher>(sql, voucher);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改傳票失敗: {voucher.VoucherId}", ex);
            throw;
        }
    }

    public async Task DeleteVoucherAsync(string voucherId)
    {
        try
        {
            const string sql = @"
                DELETE FROM Vouchers 
                WHERE VoucherId = @VoucherId";

            await ExecuteAsync(sql, new { VoucherId = voucherId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除傳票失敗: {voucherId}", ex);
            throw;
        }
    }

    public async Task<bool> VoucherExistsAsync(string voucherId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Vouchers 
                WHERE VoucherId = @VoucherId";

            var count = await QuerySingleAsync<int>(sql, new { VoucherId = voucherId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查傳票是否存在失敗: {voucherId}", ex);
            throw;
        }
    }

    public async Task<BalanceCheckResult> CheckBalanceAsync(string voucherId)
    {
        try
        {
            const string sql = @"
                SELECT 
                    SUM(CASE WHEN DC = 'D' THEN Amount ELSE 0 END) AS DebitAmount,
                    SUM(CASE WHEN DC = 'C' THEN Amount ELSE 0 END) AS CreditAmount
                FROM VoucherDetails
                WHERE VoucherId = @VoucherId";

            var result = await QueryFirstOrDefaultAsync<dynamic>(sql, new { VoucherId = voucherId });

            if (result == null)
            {
                return new BalanceCheckResult
                {
                    IsBalanced = false,
                    DebitAmount = 0,
                    CreditAmount = 0,
                    Difference = 0
                };
            }

            var debitAmount = (decimal)(result.DebitAmount ?? 0);
            var creditAmount = (decimal)(result.CreditAmount ?? 0);
            var difference = Math.Abs(debitAmount - creditAmount);

            return new BalanceCheckResult
            {
                IsBalanced = difference < 0.01m, // 允許0.01的誤差
                DebitAmount = debitAmount,
                CreditAmount = creditAmount,
                Difference = difference
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查傳票借貸平衡失敗: {voucherId}", ex);
            throw;
        }
    }

    public async Task<List<InvoiceVoucher>> GetInvoiceVouchersByVoucherTKeyAsync(long voucherTKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM InvoiceVouchers 
                WHERE VoucherTKey = @VoucherTKey
                ORDER BY InvoiceType, InvoiceDate";

            return (await QueryAsync<InvoiceVoucher>(sql, new { VoucherTKey = voucherTKey })).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢發票傳票失敗: {voucherTKey}", ex);
            throw;
        }
    }

    public async Task<InvoiceVoucher> CreateInvoiceVoucherAsync(InvoiceVoucher invoiceVoucher)
    {
        try
        {
            const string sql = @"
                INSERT INTO InvoiceVouchers (
                    VoucherTKey, InvoiceType, InvoiceNo, InvoiceDate, InvoiceFormat,
                    InvoiceAmount, TaxAmount, DeductCode, CategoryType, VoucherNo,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                )
                OUTPUT INSERTED.*
                VALUES (
                    @VoucherTKey, @InvoiceType, @InvoiceNo, @InvoiceDate, @InvoiceFormat,
                    @InvoiceAmount, @TaxAmount, @DeductCode, @CategoryType, @VoucherNo,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                )";

            return await QuerySingleAsync<InvoiceVoucher>(sql, invoiceVoucher);
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增發票傳票失敗", ex);
            throw;
        }
    }

    public async Task<InvoiceVoucher> UpdateInvoiceVoucherAsync(InvoiceVoucher invoiceVoucher)
    {
        try
        {
            const string sql = @"
                UPDATE InvoiceVouchers SET
                    InvoiceType = @InvoiceType,
                    InvoiceNo = @InvoiceNo,
                    InvoiceDate = @InvoiceDate,
                    InvoiceFormat = @InvoiceFormat,
                    InvoiceAmount = @InvoiceAmount,
                    TaxAmount = @TaxAmount,
                    DeductCode = @DeductCode,
                    CategoryType = @CategoryType,
                    VoucherNo = @VoucherNo,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                OUTPUT INSERTED.*
                WHERE TKey = @TKey";

            return await QuerySingleAsync<InvoiceVoucher>(sql, invoiceVoucher);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改發票傳票失敗: {invoiceVoucher.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteInvoiceVoucherAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM InvoiceVouchers 
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除發票傳票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<PagedResult<AllocationRatio>> QueryAllocationRatiosAsync(AllocationRatioQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM AllocationRatios
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.DisYm))
            {
                sql += " AND DisYm = @DisYm";
                parameters.Add("DisYm", query.DisYm);
            }

            if (!string.IsNullOrEmpty(query.StypeId))
            {
                sql += " AND StypeId = @StypeId";
                parameters.Add("StypeId", query.StypeId);
            }

            if (!string.IsNullOrEmpty(query.OrgId))
            {
                sql += " AND OrgId = @OrgId";
                parameters.Add("OrgId", query.OrgId);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "DisYm" : query.SortField;
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

            var items = await QueryAsync<AllocationRatio>(sql, parameters);

            return new PagedResult<AllocationRatio>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢分攤比率列表失敗", ex);
            throw;
        }
    }

    public async Task<AllocationRatio> CreateAllocationRatioAsync(AllocationRatio allocationRatio)
    {
        try
        {
            const string sql = @"
                INSERT INTO AllocationRatios (
                    DisYm, StypeId, OrgId, Ratio, VoucherTKey, VoucherDTKey,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                )
                OUTPUT INSERTED.*
                VALUES (
                    @DisYm, @StypeId, @OrgId, @Ratio, @VoucherTKey, @VoucherDTKey,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                )";

            return await QuerySingleAsync<AllocationRatio>(sql, allocationRatio);
        }
        catch (Exception ex)
        {
            _logger.LogError("新增分攤比率失敗", ex);
            throw;
        }
    }

    public async Task<AllocationRatio> UpdateAllocationRatioAsync(AllocationRatio allocationRatio)
    {
        try
        {
            const string sql = @"
                UPDATE AllocationRatios SET
                    DisYm = @DisYm,
                    StypeId = @StypeId,
                    OrgId = @OrgId,
                    Ratio = @Ratio,
                    VoucherTKey = @VoucherTKey,
                    VoucherDTKey = @VoucherDTKey,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                OUTPUT INSERTED.*
                WHERE TKey = @TKey";

            return await QuerySingleAsync<AllocationRatio>(sql, allocationRatio);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改分攤比率失敗: {allocationRatio.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAllocationRatioAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM AllocationRatios 
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除分攤比率失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<VoucherDetail> CreateVoucherDetailAsync(VoucherDetail detail)
    {
        try
        {
            const string sql = @"
                INSERT INTO VoucherDetails (
                    VoucherId, SeqNo, StypeId, Dc, Amount, Description,
                    VoucherTKey, OrgId, ActId, AbatId, VendorId, CustomField1,
                    DAmt, CAmt,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                )
                OUTPUT INSERTED.*
                VALUES (
                    @VoucherId, @SeqNo, @StypeId, @Dc, @Amount, @Description,
                    @VoucherTKey, @OrgId, @ActId, @AbatId, @VendorId, @CustomField1,
                    @DAmt, @CAmt,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                )";

            // 根據 Dc 設定 DAmt 和 CAmt
            var dAmt = detail.Dc == "D" ? detail.Amount : 0;
            var cAmt = detail.Dc == "C" ? detail.Amount : 0;

            var parameters = new
            {
                detail.VoucherId,
                detail.SeqNo,
                detail.StypeId,
                detail.Dc,
                detail.Amount,
                detail.Description,
                VoucherTKey = (long?)null, // 如果需要可以從傳票取得
                OrgId = (string?)null,
                ActId = (string?)null,
                AbatId = (string?)null,
                VendorId = (string?)null,
                CustomField1 = (string?)null,
                DAmt = dAmt,
                CAmt = cAmt,
                detail.CreatedBy,
                detail.CreatedAt,
                detail.UpdatedBy,
                detail.UpdatedAt
            };

            return await QuerySingleAsync<VoucherDetail>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增傳票明細失敗: {detail.VoucherId}, SeqNo: {detail.SeqNo}", ex);
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

