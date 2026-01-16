using Dapper;
using ErpCore.Domain.Entities.BasicData;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.BasicData;

/// <summary>
/// 廠商 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class VendorRepository : BaseRepository, IVendorRepository
{
    public VendorRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Vendor?> GetByIdAsync(string vendorId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Vendors 
                WHERE VendorId = @VendorId";

            return await QueryFirstOrDefaultAsync<Vendor>(sql, new { VendorId = vendorId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢廠商失敗: {vendorId}", ex);
            throw;
        }
    }

    public async Task<Vendor?> GetByGuiIdAsync(string guiId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Vendors 
                WHERE GuiId = @GuiId";

            return await QueryFirstOrDefaultAsync<Vendor>(sql, new { GuiId = guiId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢廠商失敗: {guiId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<Vendor>> QueryAsync(VendorQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Vendors
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.VendorId))
            {
                sql += " AND VendorId LIKE @VendorId";
                parameters.Add("VendorId", $"%{query.VendorId}%");
            }

            if (!string.IsNullOrEmpty(query.GuiId))
            {
                sql += " AND GuiId LIKE @GuiId";
                parameters.Add("GuiId", $"%{query.GuiId}%");
            }

            if (!string.IsNullOrEmpty(query.VendorName))
            {
                sql += " AND VendorName LIKE @VendorName";
                parameters.Add("VendorName", $"%{query.VendorName}%");
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.SysId))
            {
                sql += " AND SysId = @SysId";
                parameters.Add("SysId", query.SysId);
            }

            if (!string.IsNullOrEmpty(query.OrgId))
            {
                sql += " AND OrgId = @OrgId";
                parameters.Add("OrgId", query.OrgId);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "VendorId" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<Vendor>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM Vendors
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.VendorId))
            {
                countSql += " AND VendorId LIKE @VendorId";
                countParameters.Add("VendorId", $"%{query.VendorId}%");
            }
            if (!string.IsNullOrEmpty(query.GuiId))
            {
                countSql += " AND GuiId LIKE @GuiId";
                countParameters.Add("GuiId", $"%{query.GuiId}%");
            }
            if (!string.IsNullOrEmpty(query.VendorName))
            {
                countSql += " AND VendorName LIKE @VendorName";
                countParameters.Add("VendorName", $"%{query.VendorName}%");
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }
            if (!string.IsNullOrEmpty(query.SysId))
            {
                countSql += " AND SysId = @SysId";
                countParameters.Add("SysId", query.SysId);
            }
            if (!string.IsNullOrEmpty(query.OrgId))
            {
                countSql += " AND OrgId = @OrgId";
                countParameters.Add("OrgId", query.OrgId);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<Vendor>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢廠商列表失敗", ex);
            throw;
        }
    }

    public async Task<Vendor> CreateAsync(Vendor vendor)
    {
        try
        {
            const string sql = @"
                INSERT INTO Vendors (
                    VendorId, GuiId, GuiType, VendorName, VendorNameE, VendorNameS, Mcode,
                    VendorRegAddr, TaxAddr, VendorRegTel, VendorFax, VendorEmail, InvEmail,
                    ChargeStaff, ChargeTitle, ChargePid, ChargeTel, ChargeAddr, ChargeEmail,
                    Status, SysId, PayType, SuplBankId, SuplBankAcct, SuplAcctName,
                    TicketBe, CheckTitle, OrgId, Notes,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                )
                OUTPUT INSERTED.*
                VALUES (
                    @VendorId, @GuiId, @GuiType, @VendorName, @VendorNameE, @VendorNameS, @Mcode,
                    @VendorRegAddr, @TaxAddr, @VendorRegTel, @VendorFax, @VendorEmail, @InvEmail,
                    @ChargeStaff, @ChargeTitle, @ChargePid, @ChargeTel, @ChargeAddr, @ChargeEmail,
                    @Status, @SysId, @PayType, @SuplBankId, @SuplBankAcct, @SuplAcctName,
                    @TicketBe, @CheckTitle, @OrgId, @Notes,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                )";

            var result = await QueryFirstOrDefaultAsync<Vendor>(sql, vendor);
            if (result == null)
            {
                throw new InvalidOperationException("新增廠商失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增廠商失敗: {vendor.VendorId}", ex);
            throw;
        }
    }

    public async Task<Vendor> UpdateAsync(Vendor vendor)
    {
        try
        {
            const string sql = @"
                UPDATE Vendors SET
                    VendorName = @VendorName,
                    VendorNameE = @VendorNameE,
                    VendorNameS = @VendorNameS,
                    Mcode = @Mcode,
                    VendorRegAddr = @VendorRegAddr,
                    TaxAddr = @TaxAddr,
                    VendorRegTel = @VendorRegTel,
                    VendorFax = @VendorFax,
                    VendorEmail = @VendorEmail,
                    InvEmail = @InvEmail,
                    ChargeStaff = @ChargeStaff,
                    ChargeTitle = @ChargeTitle,
                    ChargePid = @ChargePid,
                    ChargeTel = @ChargeTel,
                    ChargeAddr = @ChargeAddr,
                    ChargeEmail = @ChargeEmail,
                    Status = @Status,
                    SysId = @SysId,
                    PayType = @PayType,
                    SuplBankId = @SuplBankId,
                    SuplBankAcct = @SuplBankAcct,
                    SuplAcctName = @SuplAcctName,
                    TicketBe = @TicketBe,
                    CheckTitle = @CheckTitle,
                    OrgId = @OrgId,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                OUTPUT INSERTED.*
                WHERE VendorId = @VendorId";

            var result = await QueryFirstOrDefaultAsync<Vendor>(sql, vendor);
            if (result == null)
            {
                throw new InvalidOperationException($"廠商不存在: {vendor.VendorId}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改廠商失敗: {vendor.VendorId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string vendorId)
    {
        try
        {
            const string sql = @"
                DELETE FROM Vendors 
                WHERE VendorId = @VendorId";

            await ExecuteAsync(sql, new { VendorId = vendorId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除廠商失敗: {vendorId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsByGuiIdAsync(string guiId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Vendors 
                WHERE GuiId = @GuiId";

            var count = await QuerySingleAsync<int>(sql, new { GuiId = guiId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查統一編號是否存在失敗: {guiId}", ex);
            throw;
        }
    }

    public async Task<int> GetNextSequenceAsync(string guiId)
    {
        try
        {
            const string sql = @"
                SELECT ISNULL(MAX(CAST(SUBSTRING(VendorId, LEN(@GuiId) + 2, LEN(VendorId)) AS INT)), 0) + 1
                FROM Vendors
                WHERE GuiId = @GuiId AND VendorId LIKE @Pattern";

            var pattern = $"{guiId}-%";
            var sequence = await QuerySingleAsync<int>(sql, new { GuiId = guiId, Pattern = pattern });
            return sequence;
        }
        catch (Exception ex)
        {
            _logger.LogError($"取得下一個流水號失敗: {guiId}", ex);
            throw;
        }
    }
}
