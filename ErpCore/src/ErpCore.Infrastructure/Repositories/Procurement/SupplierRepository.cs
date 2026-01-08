using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Procurement;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Procurement;

/// <summary>
/// 供應商 Repository 實作 (SYSP210-SYSP260)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class SupplierRepository : BaseRepository, ISupplierRepository
{
    public SupplierRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Supplier?> GetByIdAsync(string supplierId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Suppliers 
                WHERE SupplierId = @SupplierId";

            return await QueryFirstOrDefaultAsync<Supplier>(sql, new { SupplierId = supplierId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢供應商失敗: {supplierId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<Supplier>> QueryAsync(SupplierQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Suppliers 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.SupplierId))
            {
                sql += " AND SupplierId LIKE @SupplierId";
                parameters.Add("SupplierId", $"%{query.SupplierId}%");
            }

            if (!string.IsNullOrEmpty(query.SupplierName))
            {
                sql += " AND SupplierName LIKE @SupplierName";
                parameters.Add("SupplierName", $"%{query.SupplierName}%");
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.Rating))
            {
                sql += " AND Rating = @Rating";
                parameters.Add("Rating", query.Rating);
            }

            sql += " ORDER BY SupplierId";
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<Supplier>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢供應商列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(SupplierQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM Suppliers 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.SupplierId))
            {
                sql += " AND SupplierId LIKE @SupplierId";
                parameters.Add("SupplierId", $"%{query.SupplierId}%");
            }

            if (!string.IsNullOrEmpty(query.SupplierName))
            {
                sql += " AND SupplierName LIKE @SupplierName";
                parameters.Add("SupplierName", $"%{query.SupplierName}%");
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.Rating))
            {
                sql += " AND Rating = @Rating";
                parameters.Add("Rating", query.Rating);
            }

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢供應商數量失敗", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string supplierId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Suppliers 
                WHERE SupplierId = @SupplierId";

            var count = await ExecuteScalarAsync<int>(sql, new { SupplierId = supplierId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查供應商是否存在失敗: {supplierId}", ex);
            throw;
        }
    }

    public async Task<Supplier> CreateAsync(Supplier supplier)
    {
        try
        {
            const string sql = @"
                INSERT INTO Suppliers (
                    SupplierId, SupplierName, SupplierNameE, ContactPerson, Phone, Fax, Email,
                    Address, PaymentTerms, TaxId, Status, Rating, Notes,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                ) VALUES (
                    @SupplierId, @SupplierName, @SupplierNameE, @ContactPerson, @Phone, @Fax, @Email,
                    @Address, @PaymentTerms, @TaxId, @Status, @Rating, @Notes,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                supplier.SupplierId,
                supplier.SupplierName,
                supplier.SupplierNameE,
                supplier.ContactPerson,
                supplier.Phone,
                supplier.Fax,
                supplier.Email,
                supplier.Address,
                supplier.PaymentTerms,
                supplier.TaxId,
                supplier.Status,
                supplier.Rating,
                supplier.Notes,
                supplier.CreatedBy,
                supplier.CreatedAt,
                supplier.UpdatedBy,
                supplier.UpdatedAt
            });

            supplier.TKey = tKey;
            return supplier;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增供應商失敗: {supplier.SupplierId}", ex);
            throw;
        }
    }

    public async Task<Supplier> UpdateAsync(Supplier supplier)
    {
        try
        {
            const string sql = @"
                UPDATE Suppliers SET
                    SupplierName = @SupplierName,
                    SupplierNameE = @SupplierNameE,
                    ContactPerson = @ContactPerson,
                    Phone = @Phone,
                    Fax = @Fax,
                    Email = @Email,
                    Address = @Address,
                    PaymentTerms = @PaymentTerms,
                    TaxId = @TaxId,
                    Status = @Status,
                    Rating = @Rating,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE SupplierId = @SupplierId";

            await ExecuteAsync(sql, new
            {
                supplier.SupplierId,
                supplier.SupplierName,
                supplier.SupplierNameE,
                supplier.ContactPerson,
                supplier.Phone,
                supplier.Fax,
                supplier.Email,
                supplier.Address,
                supplier.PaymentTerms,
                supplier.TaxId,
                supplier.Status,
                supplier.Rating,
                supplier.Notes,
                supplier.UpdatedBy,
                supplier.UpdatedAt
            });

            return supplier;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改供應商失敗: {supplier.SupplierId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string supplierId)
    {
        try
        {
            const string sql = @"
                DELETE FROM Suppliers 
                WHERE SupplierId = @SupplierId";

            await ExecuteAsync(sql, new { SupplierId = supplierId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除供應商失敗: {supplierId}", ex);
            throw;
        }
    }
}

