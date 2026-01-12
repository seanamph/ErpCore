using Dapper;
using ErpCore.Domain.Entities.AnalysisReport;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using System.Data;

namespace ErpCore.Infrastructure.Repositories.AnalysisReport;

/// <summary>
/// 單位領用申請單 Repository 實作 (SYSA210)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class MaterialApplyRepository : BaseRepository, IMaterialApplyRepository
{
    public MaterialApplyRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PagedResult<MaterialApplyMaster>> GetListAsync(MaterialApplyQuery query)
    {
        try
        {
            var sql = @"
                SELECT 
                    m.TKey, m.ApplyId, m.EmpId, m.OrgId, m.SiteId, m.ApplyDate, m.ApplyStatus,
                    m.Amount, m.AprvEmpId, m.AprvDate, m.CheckDate, m.WhId, m.StoreId, m.Notes,
                    m.CreatedBy, m.CreatedAt, m.UpdatedBy, m.UpdatedAt, m.CreatedPriority, m.CreatedGroup
                FROM MaterialApplyMasters m
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ApplyId))
            {
                sql += " AND m.ApplyId LIKE @ApplyId";
                parameters.Add("ApplyId", $"%{query.ApplyId}%");
            }

            if (!string.IsNullOrEmpty(query.EmpId))
            {
                sql += " AND m.EmpId = @EmpId";
                parameters.Add("EmpId", query.EmpId);
            }

            if (!string.IsNullOrEmpty(query.OrgId))
            {
                sql += " AND m.OrgId = @OrgId";
                parameters.Add("OrgId", query.OrgId);
            }

            if (!string.IsNullOrEmpty(query.SiteId))
            {
                sql += " AND m.SiteId = @SiteId";
                parameters.Add("SiteId", query.SiteId);
            }

            if (query.ApplyDateFrom.HasValue)
            {
                sql += " AND m.ApplyDate >= @ApplyDateFrom";
                parameters.Add("ApplyDateFrom", query.ApplyDateFrom.Value);
            }

            if (query.ApplyDateTo.HasValue)
            {
                sql += " AND m.ApplyDate <= @ApplyDateTo";
                parameters.Add("ApplyDateTo", query.ApplyDateTo.Value);
            }

            if (query.AprvDateFrom.HasValue)
            {
                sql += " AND m.AprvDate >= @AprvDateFrom";
                parameters.Add("AprvDateFrom", query.AprvDateFrom.Value);
            }

            if (query.AprvDateTo.HasValue)
            {
                sql += " AND m.AprvDate <= @AprvDateTo";
                parameters.Add("AprvDateTo", query.AprvDateTo.Value);
            }

            if (query.CheckDate.HasValue)
            {
                sql += " AND m.CheckDate = @CheckDate";
                parameters.Add("CheckDate", query.CheckDate.Value);
            }

            if (!string.IsNullOrEmpty(query.ApplyStatus))
            {
                sql += " AND m.ApplyStatus = @ApplyStatus";
                parameters.Add("ApplyStatus", query.ApplyStatus);
            }

            if (!string.IsNullOrEmpty(query.WhId))
            {
                sql += " AND m.WhId = @WhId";
                parameters.Add("WhId", query.WhId);
            }

            if (!string.IsNullOrEmpty(query.StoreId))
            {
                sql += " AND m.StoreId = @StoreId";
                parameters.Add("StoreId", query.StoreId);
            }

            // 如果有品項編號查詢，需要 JOIN 明細表
            if (!string.IsNullOrEmpty(query.GoodsId))
            {
                sql += " AND EXISTS (SELECT 1 FROM MaterialApplyDetails d WHERE d.ApplyId = m.ApplyId AND d.GoodsId = @GoodsId)";
                parameters.Add("GoodsId", query.GoodsId);
            }

            // 排序
            var sortField = query.SortField ?? "ApplyDate";
            var sortOrder = query.SortOrder ?? "DESC";
            sql += $" ORDER BY m.{sortField} {sortOrder}";

            // 查詢總數
            var countSql = sql.Replace("SELECT m.TKey, m.ApplyId", "SELECT COUNT(*)").Split("ORDER BY")[0];
            var totalCount = await QuerySingleAsync<int>(countSql, parameters);

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            using var connection = _connectionFactory.CreateConnection();
            var items = await connection.QueryAsync<MaterialApplyMaster>(sql, parameters);

            return new PagedResult<MaterialApplyMaster>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢單位領用申請單列表失敗", ex);
            throw;
        }
    }

    public async Task<MaterialApplyMaster?> GetByApplyIdAsync(string applyId)
    {
        try
        {
            var sql = @"
                SELECT 
                    m.TKey, m.ApplyId, m.EmpId, m.OrgId, m.SiteId, m.ApplyDate, m.ApplyStatus,
                    m.Amount, m.AprvEmpId, m.AprvDate, m.CheckDate, m.WhId, m.StoreId, m.Notes,
                    m.CreatedBy, m.CreatedAt, m.UpdatedBy, m.UpdatedAt, m.CreatedPriority, m.CreatedGroup
                FROM MaterialApplyMasters m
                WHERE m.ApplyId = @ApplyId";

            var parameters = new DynamicParameters();
            parameters.Add("ApplyId", applyId);

            using var connection = _connectionFactory.CreateConnection();
            var master = await connection.QueryFirstOrDefaultAsync<MaterialApplyMaster>(sql, parameters);

            if (master != null)
            {
                // 查詢明細
                var detailSql = @"
                    SELECT 
                        d.TKey, d.ApplyMasterKey, d.ApplyId, d.GoodsTKey, d.GoodsId,
                        d.ApplyQty, d.IssueQty, d.Unit, d.UnitPrice, d.Amount, d.Notes, d.SeqNo,
                        d.CreatedBy, d.CreatedAt, d.UpdatedBy, d.UpdatedAt
                    FROM MaterialApplyDetails d
                    WHERE d.ApplyId = @ApplyId
                    ORDER BY d.SeqNo";

                var details = await connection.QueryAsync<MaterialApplyDetail>(detailSql, parameters);
                master.Details = details.ToList();
            }

            return master;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢單位領用申請單失敗: {applyId}", ex);
            throw;
        }
    }

    public async Task<MaterialApplyMaster> CreateAsync(MaterialApplyMaster master)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                // 插入主檔
                var mainSql = @"
                    INSERT INTO MaterialApplyMasters (
                        ApplyId, EmpId, OrgId, SiteId, ApplyDate, ApplyStatus,
                        Amount, AprvEmpId, AprvDate, CheckDate, WhId, StoreId, Notes,
                        CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                    ) VALUES (
                        @ApplyId, @EmpId, @OrgId, @SiteId, @ApplyDate, @ApplyStatus,
                        @Amount, @AprvEmpId, @AprvDate, @CheckDate, @WhId, @StoreId, @Notes,
                        @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                    );
                    SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

                var mainParams = new DynamicParameters();
                mainParams.Add("ApplyId", master.ApplyId);
                mainParams.Add("EmpId", master.EmpId);
                mainParams.Add("OrgId", master.OrgId);
                mainParams.Add("SiteId", master.SiteId);
                mainParams.Add("ApplyDate", master.ApplyDate);
                mainParams.Add("ApplyStatus", master.ApplyStatus);
                mainParams.Add("Amount", master.Amount);
                mainParams.Add("AprvEmpId", master.AprvEmpId);
                mainParams.Add("AprvDate", master.AprvDate);
                mainParams.Add("CheckDate", master.CheckDate);
                mainParams.Add("WhId", master.WhId);
                mainParams.Add("StoreId", master.StoreId);
                mainParams.Add("Notes", master.Notes);
                mainParams.Add("CreatedBy", master.CreatedBy);
                mainParams.Add("CreatedAt", master.CreatedAt);
                mainParams.Add("UpdatedBy", master.UpdatedBy);
                mainParams.Add("UpdatedAt", master.UpdatedAt);
                mainParams.Add("CreatedPriority", master.CreatedPriority);
                mainParams.Add("CreatedGroup", master.CreatedGroup);

                var tKey = await connection.ExecuteScalarAsync<long>(mainSql, mainParams, transaction);
                master.TKey = tKey;

                // 插入明細
                if (master.Details != null && master.Details.Any())
                {
                    var detailSql = @"
                        INSERT INTO MaterialApplyDetails (
                            ApplyMasterKey, ApplyId, GoodsTKey, GoodsId, ApplyQty, IssueQty,
                            Unit, UnitPrice, Amount, Notes, SeqNo,
                            CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                        ) VALUES (
                            @ApplyMasterKey, @ApplyId, @GoodsTKey, @GoodsId, @ApplyQty, @IssueQty,
                            @Unit, @UnitPrice, @Amount, @Notes, @SeqNo,
                            @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                        );
                        SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

                    foreach (var detail in master.Details)
                    {
                        var detailParams = new DynamicParameters();
                        detailParams.Add("ApplyMasterKey", master.TKey);
                        detailParams.Add("ApplyId", master.ApplyId);
                        detailParams.Add("GoodsTKey", detail.GoodsTKey);
                        detailParams.Add("GoodsId", detail.GoodsId);
                        detailParams.Add("ApplyQty", detail.ApplyQty);
                        detailParams.Add("IssueQty", detail.IssueQty);
                        detailParams.Add("Unit", detail.Unit);
                        detailParams.Add("UnitPrice", detail.UnitPrice);
                        detailParams.Add("Amount", detail.Amount);
                        detailParams.Add("Notes", detail.Notes);
                        detailParams.Add("SeqNo", detail.SeqNo);
                        detailParams.Add("CreatedBy", detail.CreatedBy);
                        detailParams.Add("CreatedAt", detail.CreatedAt);
                        detailParams.Add("UpdatedBy", detail.UpdatedBy);
                        detailParams.Add("UpdatedAt", detail.UpdatedAt);

                        var detailTKey = await connection.ExecuteScalarAsync<long>(detailSql, detailParams, transaction);
                        detail.TKey = detailTKey;
                    }
                }

                transaction.Commit();
                return master;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增單位領用申請單失敗: {master.ApplyId}", ex);
            throw;
        }
    }

    public async Task<MaterialApplyMaster> UpdateAsync(MaterialApplyMaster master)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                // 更新主檔
                var mainSql = @"
                    UPDATE MaterialApplyMasters SET
                        EmpId = @EmpId,
                        OrgId = @OrgId,
                        SiteId = @SiteId,
                        ApplyDate = @ApplyDate,
                        ApplyStatus = @ApplyStatus,
                        Amount = @Amount,
                        AprvEmpId = @AprvEmpId,
                        AprvDate = @AprvDate,
                        CheckDate = @CheckDate,
                        WhId = @WhId,
                        StoreId = @StoreId,
                        Notes = @Notes,
                        UpdatedBy = @UpdatedBy,
                        UpdatedAt = @UpdatedAt
                    WHERE ApplyId = @ApplyId";

                var mainParams = new DynamicParameters();
                mainParams.Add("ApplyId", master.ApplyId);
                mainParams.Add("EmpId", master.EmpId);
                mainParams.Add("OrgId", master.OrgId);
                mainParams.Add("SiteId", master.SiteId);
                mainParams.Add("ApplyDate", master.ApplyDate);
                mainParams.Add("ApplyStatus", master.ApplyStatus);
                mainParams.Add("Amount", master.Amount);
                mainParams.Add("AprvEmpId", master.AprvEmpId);
                mainParams.Add("AprvDate", master.AprvDate);
                mainParams.Add("CheckDate", master.CheckDate);
                mainParams.Add("WhId", master.WhId);
                mainParams.Add("StoreId", master.StoreId);
                mainParams.Add("Notes", master.Notes);
                mainParams.Add("UpdatedBy", master.UpdatedBy);
                mainParams.Add("UpdatedAt", master.UpdatedAt);

                await connection.ExecuteAsync(mainSql, mainParams, transaction);

                // 刪除舊明細
                var deleteDetailSql = "DELETE FROM MaterialApplyDetails WHERE ApplyId = @ApplyId";
                await connection.ExecuteAsync(deleteDetailSql, new { ApplyId = master.ApplyId }, transaction);

                // 插入新明細
                if (master.Details != null && master.Details.Any())
                {
                    var detailSql = @"
                        INSERT INTO MaterialApplyDetails (
                            ApplyMasterKey, ApplyId, GoodsTKey, GoodsId, ApplyQty, IssueQty,
                            Unit, UnitPrice, Amount, Notes, SeqNo,
                            CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                        ) VALUES (
                            @ApplyMasterKey, @ApplyId, @GoodsTKey, @GoodsId, @ApplyQty, @IssueQty,
                            @Unit, @UnitPrice, @Amount, @Notes, @SeqNo,
                            @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                        );
                        SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

                    foreach (var detail in master.Details)
                    {
                        var detailParams = new DynamicParameters();
                        detailParams.Add("ApplyMasterKey", master.TKey);
                        detailParams.Add("ApplyId", master.ApplyId);
                        detailParams.Add("GoodsTKey", detail.GoodsTKey);
                        detailParams.Add("GoodsId", detail.GoodsId);
                        detailParams.Add("ApplyQty", detail.ApplyQty);
                        detailParams.Add("IssueQty", detail.IssueQty);
                        detailParams.Add("Unit", detail.Unit);
                        detailParams.Add("UnitPrice", detail.UnitPrice);
                        detailParams.Add("Amount", detail.Amount);
                        detailParams.Add("Notes", detail.Notes);
                        detailParams.Add("SeqNo", detail.SeqNo);
                        detailParams.Add("CreatedBy", detail.CreatedBy);
                        detailParams.Add("CreatedAt", detail.CreatedAt);
                        detailParams.Add("UpdatedBy", detail.UpdatedBy);
                        detailParams.Add("UpdatedAt", detail.UpdatedAt);

                        var detailTKey = await connection.ExecuteScalarAsync<long>(detailSql, detailParams, transaction);
                        detail.TKey = detailTKey;
                    }
                }

                transaction.Commit();
                return master;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新單位領用申請單失敗: {master.ApplyId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string applyId)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                // 刪除明細（外鍵約束會自動處理）
                var deleteDetailSql = "DELETE FROM MaterialApplyDetails WHERE ApplyId = @ApplyId";
                await connection.ExecuteAsync(deleteDetailSql, new { ApplyId = applyId }, transaction);

                // 刪除主檔
                var deleteMainSql = "DELETE FROM MaterialApplyMasters WHERE ApplyId = @ApplyId";
                await connection.ExecuteAsync(deleteMainSql, new { ApplyId = applyId }, transaction);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除單位領用申請單失敗: {applyId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string applyId)
    {
        try
        {
            var sql = "SELECT COUNT(*) FROM MaterialApplyMasters WHERE ApplyId = @ApplyId";
            var count = await QuerySingleAsync<int>(sql, new { ApplyId = applyId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查領用單號是否存在失敗: {applyId}", ex);
            throw;
        }
    }

    public async Task<string> GenerateApplyIdAsync()
    {
        try
        {
            var dateStr = DateTime.Now.ToString("yyyyMMdd");
            var prefix = $"MA{dateStr}";

            var sql = @"
                SELECT ISNULL(MAX(CAST(SUBSTRING(ApplyId, LEN(@Prefix) + 1, LEN(ApplyId)) AS INT)), 0) + 1
                FROM MaterialApplyMasters
                WHERE ApplyId LIKE @Prefix + '%'";

            var parameters = new DynamicParameters();
            parameters.Add("Prefix", prefix);

            var seqNo = await QuerySingleAsync<int>(sql, parameters);
            return $"{prefix}{seqNo:D3}";
        }
        catch (Exception ex)
        {
            _logger.LogError("生成領用單號失敗", ex);
            throw;
        }
    }
}
