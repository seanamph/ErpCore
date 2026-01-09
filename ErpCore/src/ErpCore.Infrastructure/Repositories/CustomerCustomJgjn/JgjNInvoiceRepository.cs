using System.Data;
using Dapper;
using ErpCore.Domain.Entities.CustomerCustomJgjn;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.CustomerCustomJgjn;

/// <summary>
/// JGJN發票 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class JgjNInvoiceRepository : BaseRepository, IJgjNInvoiceRepository
{
    public JgjNInvoiceRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<JgjNInvoice?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM JgjNInvoice 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<JgjNInvoice>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢JGJN發票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<JgjNInvoice?> GetByInvoiceIdAsync(string invoiceId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM JgjNInvoice 
                WHERE InvoiceId = @InvoiceId";

            return await QueryFirstOrDefaultAsync<JgjNInvoice>(sql, new { InvoiceId = invoiceId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢JGJN發票失敗: {invoiceId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<JgjNInvoice>> QueryAsync(JgjNInvoiceQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM JgjNInvoice
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.CustomerId))
            {
                sql += " AND CustomerId = @CustomerId";
                parameters.Add("CustomerId", query.CustomerId);
            }

            if (query.StartDate.HasValue)
            {
                sql += " AND InvoiceDate >= @StartDate";
                parameters.Add("StartDate", query.StartDate.Value);
            }

            if (query.EndDate.HasValue)
            {
                sql += " AND InvoiceDate <= @EndDate";
                parameters.Add("EndDate", query.EndDate.Value);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.PrintStatus))
            {
                sql += " AND PrintStatus = @PrintStatus";
                parameters.Add("PrintStatus", query.PrintStatus);
            }

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                sql += " AND (InvoiceId LIKE @Keyword OR InvoiceNo LIKE @Keyword)";
                parameters.Add("Keyword", $"%{query.Keyword}%");
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
            if (query.PageSize > 0)
            {
                var offset = (query.PageIndex - 1) * query.PageSize;
                sql += $" OFFSET {offset} ROWS FETCH NEXT {query.PageSize} ROWS ONLY";
            }

            return await QueryAsync<JgjNInvoice>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢JGJN發票列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(JgjNInvoiceQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM JgjNInvoice
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.CustomerId))
            {
                sql += " AND CustomerId = @CustomerId";
                parameters.Add("CustomerId", query.CustomerId);
            }

            if (query.StartDate.HasValue)
            {
                sql += " AND InvoiceDate >= @StartDate";
                parameters.Add("StartDate", query.StartDate.Value);
            }

            if (query.EndDate.HasValue)
            {
                sql += " AND InvoiceDate <= @EndDate";
                parameters.Add("EndDate", query.EndDate.Value);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.PrintStatus))
            {
                sql += " AND PrintStatus = @PrintStatus";
                parameters.Add("PrintStatus", query.PrintStatus);
            }

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                sql += " AND (InvoiceId LIKE @Keyword OR InvoiceNo LIKE @Keyword)";
                parameters.Add("Keyword", $"%{query.Keyword}%");
            }

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢JGJN發票數量失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(JgjNInvoice entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO JgjNInvoice 
                (InvoiceId, InvoiceNo, InvoiceDate, CustomerId, Amount, Currency, Status, PrintStatus, PrintDate, FilePath, Memo, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@InvoiceId, @InvoiceNo, @InvoiceDate, @CustomerId, @Amount, @Currency, @Status, @PrintStatus, @PrintDate, @FilePath, @Memo, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            return await ExecuteScalarAsync<long>(sql, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增JGJN發票失敗: {entity.InvoiceId}", ex);
            throw;
        }
    }

    public async Task UpdateAsync(JgjNInvoice entity)
    {
        try
        {
            const string sql = @"
                UPDATE JgjNInvoice 
                SET InvoiceNo = @InvoiceNo, InvoiceDate = @InvoiceDate, CustomerId = @CustomerId, 
                    Amount = @Amount, Currency = @Currency, Status = @Status, 
                    PrintStatus = @PrintStatus, PrintDate = @PrintDate, FilePath = @FilePath, Memo = @Memo, 
                    UpdatedBy = @UpdatedBy, UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新JGJN發票失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM JgjNInvoice 
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除JGJN發票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task UpdatePrintStatusAsync(long tKey, string printStatus, DateTime? printDate, string? filePath)
    {
        try
        {
            const string sql = @"
                UPDATE JgjNInvoice 
                SET PrintStatus = @PrintStatus, PrintDate = @PrintDate, FilePath = @FilePath, 
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new 
            { 
                TKey = tKey, 
                PrintStatus = printStatus, 
                PrintDate = printDate, 
                FilePath = filePath,
                UpdatedAt = DateTime.Now
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新JGJN發票列印狀態失敗: {tKey}", ex);
            throw;
        }
    }
}

