using System.Data;
using Dapper;
using ErpCore.Domain.Entities.CustomerInvoice;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.CustomerInvoice;

/// <summary>
/// 發票列印 Repository 實作 (SYS2000 - 發票列印作業)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class InvoicePrintRepository : BaseRepository, IInvoicePrintRepository
{
    public InvoicePrintRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Invoice?> GetByIdAsync(string invoiceNo)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Invoices 
                WHERE InvoiceNo = @InvoiceNo";

            return await QueryFirstOrDefaultAsync<Invoice>(sql, new { InvoiceNo = invoiceNo });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢發票失敗: {invoiceNo}", ex);
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

            if (!string.IsNullOrEmpty(query.InvoiceNo))
            {
                sql += " AND InvoiceNo LIKE @InvoiceNo";
                parameters.Add("InvoiceNo", $"%{query.InvoiceNo}%");
            }

            if (!string.IsNullOrEmpty(query.CustomerId))
            {
                sql += " AND CustomerId = @CustomerId";
                parameters.Add("CustomerId", query.CustomerId);
            }

            if (query.InvoiceDateFrom.HasValue)
            {
                sql += " AND InvoiceDate >= @InvoiceDateFrom";
                parameters.Add("InvoiceDateFrom", query.InvoiceDateFrom);
            }

            if (query.InvoiceDateTo.HasValue)
            {
                sql += " AND InvoiceDate <= @InvoiceDateTo";
                parameters.Add("InvoiceDateTo", query.InvoiceDateTo);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 排序
            var sortField = query.SortField ?? "InvoiceDate";
            var sortOrder = query.SortOrder ?? "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<Invoice>(sql, parameters);
            var totalCount = await GetCountAsync(query);

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

    public async Task<int> GetCountAsync(InvoiceQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM Invoices
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.InvoiceNo))
            {
                sql += " AND InvoiceNo LIKE @InvoiceNo";
                parameters.Add("InvoiceNo", $"%{query.InvoiceNo}%");
            }

            if (!string.IsNullOrEmpty(query.CustomerId))
            {
                sql += " AND CustomerId = @CustomerId";
                parameters.Add("CustomerId", query.CustomerId);
            }

            if (query.InvoiceDateFrom.HasValue)
            {
                sql += " AND InvoiceDate >= @InvoiceDateFrom";
                parameters.Add("InvoiceDateFrom", query.InvoiceDateFrom);
            }

            if (query.InvoiceDateTo.HasValue)
            {
                sql += " AND InvoiceDate <= @InvoiceDateTo";
                parameters.Add("InvoiceDateTo", query.InvoiceDateTo);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢發票總數失敗", ex);
            throw;
        }
    }

    public async Task<Invoice> CreateAsync(Invoice invoice, List<InvoiceDetail>? details = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            invoice.CreatedAt = DateTime.Now;
            invoice.UpdatedAt = DateTime.Now;

            const string insertSql = @"
                INSERT INTO Invoices 
                (InvoiceNo, InvoiceType, InvoiceDate, CustomerId, StoreId, TotalAmount, TaxAmount, Amount,
                 CurrencyId, Status, PrintCount, PrintFormat, Memo, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@InvoiceNo, @InvoiceType, @InvoiceDate, @CustomerId, @StoreId, @TotalAmount, @TaxAmount, @Amount,
                 @CurrencyId, @Status, @PrintCount, @PrintFormat, @Memo, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var tKey = await connection.QuerySingleAsync<long>(insertSql, invoice, transaction);
            invoice.TKey = tKey;

            // 新增明細
            if (details != null && details.Any())
            {
                foreach (var detail in details)
                {
                    detail.InvoiceNo = invoice.InvoiceNo;
                    detail.CreatedAt = DateTime.Now;

                    const string insertDetailSql = @"
                        INSERT INTO InvoiceDetails 
                        (InvoiceNo, LineNum, GoodsId, GoodsName, Qty, UnitPrice, Amount, TaxRate, TaxAmount, UnitId, Memo, CreatedBy, CreatedAt)
                        VALUES 
                        (@InvoiceNo, @LineNum, @GoodsId, @GoodsName, @Qty, @UnitPrice, @Amount, @TaxRate, @TaxAmount, @UnitId, @Memo, @CreatedBy, @CreatedAt);";

                    await connection.ExecuteAsync(insertDetailSql, detail, transaction);
                }
            }

            transaction.Commit();
            _logger.LogInfo($"建立發票成功: {invoice.InvoiceNo}");
            return invoice;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"建立發票失敗: {invoice.InvoiceNo}", ex);
            throw;
        }
    }

    public async Task<Invoice> UpdateAsync(Invoice invoice, List<InvoiceDetail>? details = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            invoice.UpdatedAt = DateTime.Now;

            const string updateSql = @"
                UPDATE Invoices SET
                    InvoiceType = @InvoiceType,
                    InvoiceDate = @InvoiceDate,
                    CustomerId = @CustomerId,
                    StoreId = @StoreId,
                    TotalAmount = @TotalAmount,
                    TaxAmount = @TaxAmount,
                    Amount = @Amount,
                    CurrencyId = @CurrencyId,
                    Status = @Status,
                    PrintFormat = @PrintFormat,
                    Memo = @Memo,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE InvoiceNo = @InvoiceNo";

            await connection.ExecuteAsync(updateSql, invoice, transaction);

            // 更新明細（先刪除再新增）
            if (details != null)
            {
                const string deleteDetailsSql = "DELETE FROM InvoiceDetails WHERE InvoiceNo = @InvoiceNo";
                await connection.ExecuteAsync(deleteDetailsSql, new { InvoiceNo = invoice.InvoiceNo }, transaction);

                foreach (var detail in details)
                {
                    detail.InvoiceNo = invoice.InvoiceNo;
                    detail.CreatedAt = DateTime.Now;

                    const string insertDetailSql = @"
                        INSERT INTO InvoiceDetails 
                        (InvoiceNo, LineNum, GoodsId, GoodsName, Qty, UnitPrice, Amount, TaxRate, TaxAmount, UnitId, Memo, CreatedBy, CreatedAt)
                        VALUES 
                        (@InvoiceNo, @LineNum, @GoodsId, @GoodsName, @Qty, @UnitPrice, @Amount, @TaxRate, @TaxAmount, @UnitId, @Memo, @CreatedBy, @CreatedAt);";

                    await connection.ExecuteAsync(insertDetailSql, detail, transaction);
                }
            }

            transaction.Commit();
            _logger.LogInfo($"更新發票成功: {invoice.InvoiceNo}");
            return invoice;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"更新發票失敗: {invoice.InvoiceNo}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string invoiceNo)
    {
        try
        {
            // 刪除明細
            const string deleteDetailsSql = "DELETE FROM InvoiceDetails WHERE InvoiceNo = @InvoiceNo";
            await ExecuteAsync(deleteDetailsSql, new { InvoiceNo = invoiceNo });

            // 刪除主檔
            const string deleteSql = "DELETE FROM Invoices WHERE InvoiceNo = @InvoiceNo";
            await ExecuteAsync(deleteSql, new { InvoiceNo = invoiceNo });

            _logger.LogInfo($"刪除發票成功: {invoiceNo}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除發票失敗: {invoiceNo}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string invoiceNo)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Invoices 
                WHERE InvoiceNo = @InvoiceNo";

            var count = await ExecuteScalarAsync<int>(sql, new { InvoiceNo = invoiceNo });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查發票號碼是否存在失敗: {invoiceNo}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<InvoiceDetail>> GetDetailsByInvoiceNoAsync(string invoiceNo)
    {
        try
        {
            const string sql = @"
                SELECT * FROM InvoiceDetails 
                WHERE InvoiceNo = @InvoiceNo 
                ORDER BY LineNum";

            return await QueryAsync<InvoiceDetail>(sql, new { InvoiceNo = invoiceNo });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢發票明細失敗: {invoiceNo}", ex);
            throw;
        }
    }

    public async Task<InvoicePrintLog> CreatePrintLogAsync(InvoicePrintLog printLog)
    {
        try
        {
            printLog.PrintDate = DateTime.Now;
            printLog.CreatedAt = DateTime.Now;

            const string insertSql = @"
                INSERT INTO InvoicePrintLogs 
                (InvoiceNo, PrintDate, PrintUser, PrintFormat, PrintType, PrinterName, PrintCount, Memo, CreatedBy, CreatedAt)
                VALUES 
                (@InvoiceNo, @PrintDate, @PrintUser, @PrintFormat, @PrintType, @PrinterName, @PrintCount, @Memo, @CreatedBy, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            using var connection = _connectionFactory.CreateConnection();
            var tKey = await connection.QuerySingleAsync<long>(insertSql, printLog);
            printLog.TKey = tKey;

            _logger.LogInfo($"建立發票列印記錄成功: InvoiceNo={printLog.InvoiceNo}");
            return printLog;
        }
        catch (Exception ex)
        {
            _logger.LogError($"建立發票列印記錄失敗: InvoiceNo={printLog.InvoiceNo}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<InvoicePrintLog>> GetPrintLogsByInvoiceNoAsync(string invoiceNo)
    {
        try
        {
            const string sql = @"
                SELECT * FROM InvoicePrintLogs 
                WHERE InvoiceNo = @InvoiceNo 
                ORDER BY PrintDate DESC";

            return await QueryAsync<InvoicePrintLog>(sql, new { InvoiceNo = invoiceNo });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢發票列印記錄失敗: {invoiceNo}", ex);
            throw;
        }
    }

    public async Task UpdatePrintInfoAsync(string invoiceNo, string printFormat, string printUser)
    {
        try
        {
            const string sql = @"
                UPDATE Invoices 
                SET PrintCount = PrintCount + 1,
                    LastPrintDate = GETDATE(),
                    LastPrintUser = @PrintUser,
                    PrintFormat = @PrintFormat,
                    UpdatedAt = GETDATE()
                WHERE InvoiceNo = @InvoiceNo";

            await ExecuteAsync(sql, new { InvoiceNo = invoiceNo, PrintFormat = printFormat, PrintUser = printUser });
            _logger.LogInfo($"更新發票列印資訊成功: {invoiceNo}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新發票列印資訊失敗: {invoiceNo}", ex);
            throw;
        }
    }
}

