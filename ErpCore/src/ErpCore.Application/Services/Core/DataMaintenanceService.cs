using ErpCore.Application.DTOs.Core;
using ErpCore.Application.Services.Base;
using ErpCore.Infrastructure.Data;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using Dapper;
using System.Text.Json;

namespace ErpCore.Application.Services.Core;

/// <summary>
/// 資料維護服務實作 (IMS30系列)
/// </summary>
public class DataMaintenanceService : BaseService, IDataMaintenanceService
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DataMaintenanceService(
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _connectionFactory = connectionFactory;
    }

    #region IMS30_FB - 資料瀏覽功能

    public async Task<PagedResult<Dictionary<string, object>>> BrowseDataAsync(string moduleCode, DataBrowseQueryDto query)
    {
        try
        {
            _logger.LogInfo($"查詢資料列表: moduleCode={moduleCode}");
            
            // 取得設定
            var config = await GetBrowseConfigAsync(moduleCode);
            if (config == null)
            {
                throw new Exception($"模組 {moduleCode} 的瀏覽設定不存在");
            }

            // 構建查詢SQL
            var sql = BuildBrowseQuery(config, query);
            var countSql = BuildBrowseCountQuery(config, query);

            using var connection = _connectionFactory.CreateConnection();
            
            // 查詢總數
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql, query.Filters);
            
            // 查詢資料
            var items = await connection.QueryAsync(sql, query.Filters);
            var dataList = items.Select(x => (Dictionary<string, object>)x).ToList();

            return new PagedResult<Dictionary<string, object>>
            {
                Items = dataList,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢資料列表失敗", ex);
            throw;
        }
    }

    public async Task<DataBrowseConfigDto?> GetBrowseConfigAsync(string moduleCode)
    {
        try
        {
            _logger.LogInfo($"取得瀏覽設定: moduleCode={moduleCode}");
            
            var sql = @"
                SELECT ConfigId, ModuleCode, TableName, DisplayFields, FilterFields, SortFields, 
                       PageSize, DefaultSort, Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                FROM DataBrowseConfigs
                WHERE ModuleCode = @ModuleCode AND Status = '1'";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<DataBrowseConfigDto>(sql, new { ModuleCode = moduleCode });
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("取得瀏覽設定失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateBrowseConfigAsync(CreateDataBrowseConfigDto dto)
    {
        try
        {
            _logger.LogInfo($"新增瀏覽設定: moduleCode={dto.ModuleCode}");
            
            var sql = @"
                INSERT INTO DataBrowseConfigs (ModuleCode, TableName, DisplayFields, FilterFields, SortFields, 
                                                PageSize, DefaultSort, Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES (@ModuleCode, @TableName, @DisplayFields, @FilterFields, @SortFields, 
                        @PageSize, @DefaultSort, @Status, @CreatedBy, GETDATE(), @UpdatedBy, GETDATE());
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var userId = GetCurrentUserId();
            using var connection = _connectionFactory.CreateConnection();
            var configId = await connection.ExecuteScalarAsync<long>(sql, new
            {
                dto.ModuleCode,
                dto.TableName,
                dto.DisplayFields,
                dto.FilterFields,
                dto.SortFields,
                dto.PageSize,
                dto.DefaultSort,
                dto.Status,
                CreatedBy = userId,
                UpdatedBy = userId
            });

            return configId;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增瀏覽設定失敗", ex);
            throw;
        }
    }

    public async Task UpdateBrowseConfigAsync(long configId, UpdateDataBrowseConfigDto dto)
    {
        try
        {
            _logger.LogInfo($"修改瀏覽設定: configId={configId}");
            
            var sql = @"
                UPDATE DataBrowseConfigs
                SET DisplayFields = @DisplayFields,
                    FilterFields = @FilterFields,
                    SortFields = @SortFields,
                    PageSize = @PageSize,
                    DefaultSort = @DefaultSort,
                    Status = @Status,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = GETDATE()
                WHERE ConfigId = @ConfigId";

            var userId = GetCurrentUserId();
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                ConfigId = configId,
                dto.DisplayFields,
                dto.FilterFields,
                dto.SortFields,
                dto.PageSize,
                dto.DefaultSort,
                dto.Status,
                UpdatedBy = userId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("修改瀏覽設定失敗", ex);
            throw;
        }
    }

    public async Task DeleteBrowseConfigAsync(long configId)
    {
        try
        {
            _logger.LogInfo($"刪除瀏覽設定: configId={configId}");
            
            var sql = "UPDATE DataBrowseConfigs SET Status = '0' WHERE ConfigId = @ConfigId";
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(sql, new { ConfigId = configId });
        }
        catch (Exception ex)
        {
            _logger.LogError("刪除瀏覽設定失敗", ex);
            throw;
        }
    }

    #endregion

    #region IMS30_FI - 資料新增功能

    public async Task<DataInsertConfigDto?> GetInsertConfigAsync(string moduleCode)
    {
        try
        {
            _logger.LogInfo($"取得新增設定: moduleCode={moduleCode}");
            
            var sql = @"
                SELECT ConfigId, ModuleCode, TableName, FormFields, DefaultValues, ValidationRules, 
                       Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                FROM DataInsertConfigs
                WHERE ModuleCode = @ModuleCode AND Status = '1'";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<DataInsertConfigDto>(sql, new { ModuleCode = moduleCode });
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("取得新增設定失敗", ex);
            throw;
        }
    }

    public async Task<object> InsertDataAsync(string moduleCode, Dictionary<string, object> data)
    {
        try
        {
            _logger.LogInfo($"新增資料: moduleCode={moduleCode}");
            
            var config = await GetInsertConfigAsync(moduleCode);
            if (config == null)
            {
                throw new Exception($"模組 {moduleCode} 的新增設定不存在");
            }

            // 構建INSERT SQL
            var fields = data.Keys.ToList();
            var fieldNames = string.Join(", ", fields);
            var paramNames = string.Join(", ", fields.Select(f => "@" + f));
            
            var sql = $"INSERT INTO [{config.TableName}] ({fieldNames}) VALUES ({paramNames}); SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            using var connection = _connectionFactory.CreateConnection();
            var id = await connection.ExecuteScalarAsync<object>(sql, data);
            return id ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增資料失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateInsertConfigAsync(CreateDataInsertConfigDto dto)
    {
        try
        {
            _logger.LogInfo($"新增新增設定: moduleCode={dto.ModuleCode}");
            
            var sql = @"
                INSERT INTO DataInsertConfigs (ModuleCode, TableName, FormFields, DefaultValues, ValidationRules, 
                                                Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES (@ModuleCode, @TableName, @FormFields, @DefaultValues, @ValidationRules, 
                        @Status, @CreatedBy, GETDATE(), @UpdatedBy, GETDATE());
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var userId = GetCurrentUserId();
            using var connection = _connectionFactory.CreateConnection();
            var configId = await connection.ExecuteScalarAsync<long>(sql, new
            {
                dto.ModuleCode,
                dto.TableName,
                dto.FormFields,
                dto.DefaultValues,
                dto.ValidationRules,
                dto.Status,
                CreatedBy = userId,
                UpdatedBy = userId
            });

            return configId;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增新增設定失敗", ex);
            throw;
        }
    }

    public async Task UpdateInsertConfigAsync(long configId, CreateDataInsertConfigDto dto)
    {
        try
        {
            _logger.LogInfo($"修改新增設定: configId={configId}");
            
            var sql = @"
                UPDATE DataInsertConfigs
                SET FormFields = @FormFields,
                    DefaultValues = @DefaultValues,
                    ValidationRules = @ValidationRules,
                    Status = @Status,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = GETDATE()
                WHERE ConfigId = @ConfigId";

            var userId = GetCurrentUserId();
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                ConfigId = configId,
                dto.FormFields,
                dto.DefaultValues,
                dto.ValidationRules,
                dto.Status,
                UpdatedBy = userId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("修改新增設定失敗", ex);
            throw;
        }
    }

    public async Task DeleteInsertConfigAsync(long configId)
    {
        try
        {
            _logger.LogInfo($"刪除新增設定: configId={configId}");
            
            var sql = "UPDATE DataInsertConfigs SET Status = '0' WHERE ConfigId = @ConfigId";
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(sql, new { ConfigId = configId });
        }
        catch (Exception ex)
        {
            _logger.LogError("刪除新增設定失敗", ex);
            throw;
        }
    }

    #endregion

    #region IMS30_FQ - 資料查詢功能

    public async Task<PagedResult<Dictionary<string, object>>> QueryDataAsync(string moduleCode, DataBrowseQueryDto query)
    {
        // 與BrowseDataAsync類似，可共用邏輯
        return await BrowseDataAsync(moduleCode, query);
    }

    public async Task<DataQueryConfigDto?> GetQueryConfigAsync(string moduleCode)
    {
        try
        {
            _logger.LogInfo($"取得查詢設定: moduleCode={moduleCode}");
            
            var sql = @"
                SELECT ConfigId, ModuleCode, TableName, QueryFields, DisplayFields, SortFields, 
                       DefaultQuery, PageSize, Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                FROM DataQueryConfigs
                WHERE ModuleCode = @ModuleCode AND Status = '1'";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<DataQueryConfigDto>(sql, new { ModuleCode = moduleCode });
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("取得查詢設定失敗", ex);
            throw;
        }
    }

    public async Task<long> SaveQueryAsync(string moduleCode, string queryName, string queryConditions, bool isDefault)
    {
        try
        {
            _logger.LogInfo($"儲存查詢條件: moduleCode={moduleCode}, queryName={queryName}");
            
            var sql = @"
                INSERT INTO SavedQueries (ModuleCode, QueryName, QueryConditions, UserId, IsDefault, CreatedAt)
                VALUES (@ModuleCode, @QueryName, @QueryConditions, @UserId, @IsDefault, GETDATE());
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var userId = GetCurrentUserId();
            using var connection = _connectionFactory.CreateConnection();
            var queryId = await connection.ExecuteScalarAsync<long>(sql, new
            {
                ModuleCode = moduleCode,
                QueryName = queryName,
                QueryConditions = queryConditions,
                UserId = userId,
                IsDefault = isDefault
            });

            return queryId;
        }
        catch (Exception ex)
        {
            _logger.LogError("儲存查詢條件失敗", ex);
            throw;
        }
    }

    public async Task<List<SavedQueryDto>> GetSavedQueriesAsync(string moduleCode)
    {
        try
        {
            _logger.LogInfo($"取得儲存的查詢條件: moduleCode={moduleCode}");
            
            var sql = @"
                SELECT QueryId, ModuleCode, QueryName, QueryConditions, UserId, IsDefault, CreatedAt
                FROM SavedQueries
                WHERE ModuleCode = @ModuleCode AND UserId = @UserId
                ORDER BY IsDefault DESC, CreatedAt DESC";

            var userId = GetCurrentUserId();
            using var connection = _connectionFactory.CreateConnection();
            var results = await connection.QueryAsync<SavedQueryDto>(sql, new { ModuleCode = moduleCode, UserId = userId });
            return results.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("取得儲存的查詢條件失敗", ex);
            throw;
        }
    }

    public async Task DeleteSavedQueryAsync(long queryId)
    {
        try
        {
            _logger.LogInfo($"刪除儲存的查詢條件: queryId={queryId}");
            
            var sql = "DELETE FROM SavedQueries WHERE QueryId = @QueryId";
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(sql, new { QueryId = queryId });
        }
        catch (Exception ex)
        {
            _logger.LogError("刪除儲存的查詢條件失敗", ex);
            throw;
        }
    }

    #endregion

    #region IMS30_FS - 資料排序功能

    public async Task<DataSortConfigDto?> GetSortConfigAsync(string moduleCode)
    {
        try
        {
            _logger.LogInfo($"取得排序設定: moduleCode={moduleCode}");
            
            var sql = @"
                SELECT ConfigId, ModuleCode, TableName, SortFields, DefaultSort, 
                       Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                FROM DataSortConfigs
                WHERE ModuleCode = @ModuleCode AND Status = '1'";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<DataSortConfigDto>(sql, new { ModuleCode = moduleCode });
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("取得排序設定失敗", ex);
            throw;
        }
    }

    public async Task ApplySortAsync(string moduleCode, List<SortRuleDto> sortRules)
    {
        try
        {
            _logger.LogInfo($"套用排序: moduleCode={moduleCode}");
            // 排序邏輯在查詢時套用，這裡僅記錄
        }
        catch (Exception ex)
        {
            _logger.LogError("套用排序失敗", ex);
            throw;
        }
    }

    public async Task<long> SaveSortAsync(string moduleCode, string sortName, List<SortRuleDto> sortRules, bool isDefault)
    {
        try
        {
            _logger.LogInfo($"儲存排序規則: moduleCode={moduleCode}, sortName={sortName}");
            
            var sortRulesJson = JsonSerializer.Serialize(sortRules);
            var sql = @"
                INSERT INTO SavedSorts (ModuleCode, SortName, SortRules, UserId, IsDefault, CreatedAt)
                VALUES (@ModuleCode, @SortName, @SortRules, @UserId, @IsDefault, GETDATE());
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var userId = GetCurrentUserId();
            using var connection = _connectionFactory.CreateConnection();
            var sortId = await connection.ExecuteScalarAsync<long>(sql, new
            {
                ModuleCode = moduleCode,
                SortName = sortName,
                SortRules = sortRulesJson,
                UserId = userId,
                IsDefault = isDefault
            });

            return sortId;
        }
        catch (Exception ex)
        {
            _logger.LogError("儲存排序規則失敗", ex);
            throw;
        }
    }

    public async Task<List<SavedSortDto>> GetSavedSortsAsync(string moduleCode)
    {
        try
        {
            _logger.LogInfo($"取得儲存的排序規則: moduleCode={moduleCode}");
            
            var sql = @"
                SELECT SortId, ModuleCode, SortName, SortRules, UserId, IsDefault, CreatedAt
                FROM SavedSorts
                WHERE ModuleCode = @ModuleCode AND UserId = @UserId
                ORDER BY IsDefault DESC, CreatedAt DESC";

            var userId = GetCurrentUserId();
            using var connection = _connectionFactory.CreateConnection();
            var results = await connection.QueryAsync<SavedSortDto>(sql, new { ModuleCode = moduleCode, UserId = userId });
            return results.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("取得儲存的排序規則失敗", ex);
            throw;
        }
    }

    public async Task DeleteSavedSortAsync(long sortId)
    {
        try
        {
            _logger.LogInfo($"刪除儲存的排序規則: sortId={sortId}");
            
            var sql = "DELETE FROM SavedSorts WHERE SortId = @SortId";
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(sql, new { SortId = sortId });
        }
        catch (Exception ex)
        {
            _logger.LogError("刪除儲存的排序規則失敗", ex);
            throw;
        }
    }

    #endregion

    #region IMS30_FU - 資料修改功能

    public async Task<Dictionary<string, object>?> GetDataForUpdateAsync(string moduleCode, string id)
    {
        try
        {
            _logger.LogInfo($"查詢單筆資料: moduleCode={moduleCode}, id={id}");
            
            var config = await GetUpdateConfigAsync(moduleCode);
            if (config == null)
            {
                throw new Exception($"模組 {moduleCode} 的修改設定不存在");
            }

            // 需要根據實際表結構構建查詢，這裡簡化處理
            var sql = $"SELECT * FROM [{config.TableName}] WHERE Id = @Id";
            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync(sql, new { Id = id });
            return result as Dictionary<string, object>;
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢單筆資料失敗", ex);
            throw;
        }
    }

    public async Task<DataUpdateConfigDto?> GetUpdateConfigAsync(string moduleCode)
    {
        try
        {
            _logger.LogInfo($"取得修改設定: moduleCode={moduleCode}");
            
            var sql = @"
                SELECT ConfigId, ModuleCode, TableName, FormFields, ReadOnlyFields, ValidationRules, 
                       UseOptimisticLock, Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                FROM DataUpdateConfigs
                WHERE ModuleCode = @ModuleCode AND Status = '1'";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<DataUpdateConfigDto>(sql, new { ModuleCode = moduleCode });
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("取得修改設定失敗", ex);
            throw;
        }
    }

    public async Task UpdateDataAsync(string moduleCode, string id, Dictionary<string, object> data)
    {
        try
        {
            _logger.LogInfo($"修改資料: moduleCode={moduleCode}, id={id}");
            
            var config = await GetUpdateConfigAsync(moduleCode);
            if (config == null)
            {
                throw new Exception($"模組 {moduleCode} 的修改設定不存在");
            }

            // 構建UPDATE SQL
            var setClause = string.Join(", ", data.Keys.Select(k => $"[{k}] = @{k}"));
            var sql = $"UPDATE [{config.TableName}] SET {setClause}, UpdatedBy = @UpdatedBy, UpdatedAt = GETDATE() WHERE Id = @Id";

            var userId = GetCurrentUserId();
            data["Id"] = id;
            data["UpdatedBy"] = userId;

            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(sql, data);
        }
        catch (Exception ex)
        {
            _logger.LogError("修改資料失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateUpdateConfigAsync(DataUpdateConfigDto dto)
    {
        try
        {
            _logger.LogInfo($"新增修改設定: moduleCode={dto.ModuleCode}");
            
            var sql = @"
                INSERT INTO DataUpdateConfigs (ModuleCode, TableName, FormFields, ReadOnlyFields, ValidationRules, 
                                               UseOptimisticLock, Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES (@ModuleCode, @TableName, @FormFields, @ReadOnlyFields, @ValidationRules, 
                        @UseOptimisticLock, @Status, @CreatedBy, GETDATE(), @UpdatedBy, GETDATE());
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var userId = GetCurrentUserId();
            using var connection = _connectionFactory.CreateConnection();
            var configId = await connection.ExecuteScalarAsync<long>(sql, new
            {
                dto.ModuleCode,
                dto.TableName,
                dto.FormFields,
                dto.ReadOnlyFields,
                dto.ValidationRules,
                dto.UseOptimisticLock,
                dto.Status,
                CreatedBy = userId,
                UpdatedBy = userId
            });

            return configId;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增修改設定失敗", ex);
            throw;
        }
    }

    public async Task UpdateUpdateConfigAsync(long configId, DataUpdateConfigDto dto)
    {
        try
        {
            _logger.LogInfo($"修改修改設定: configId={configId}");
            
            var sql = @"
                UPDATE DataUpdateConfigs
                SET FormFields = @FormFields,
                    ReadOnlyFields = @ReadOnlyFields,
                    ValidationRules = @ValidationRules,
                    UseOptimisticLock = @UseOptimisticLock,
                    Status = @Status,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = GETDATE()
                WHERE ConfigId = @ConfigId";

            var userId = GetCurrentUserId();
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                ConfigId = configId,
                dto.FormFields,
                dto.ReadOnlyFields,
                dto.ValidationRules,
                dto.UseOptimisticLock,
                dto.Status,
                UpdatedBy = userId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("修改修改設定失敗", ex);
            throw;
        }
    }

    public async Task DeleteUpdateConfigAsync(long configId)
    {
        try
        {
            _logger.LogInfo($"刪除修改設定: configId={configId}");
            
            var sql = "UPDATE DataUpdateConfigs SET Status = '0' WHERE ConfigId = @ConfigId";
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(sql, new { ConfigId = configId });
        }
        catch (Exception ex)
        {
            _logger.LogError("刪除修改設定失敗", ex);
            throw;
        }
    }

    #endregion

    #region IMS30_PR - 資料列印功能

    public async Task<List<DataPrintConfigDto>> GetPrintConfigsAsync(string moduleCode)
    {
        try
        {
            _logger.LogInfo($"取得列印設定列表: moduleCode={moduleCode}");
            
            var sql = @"
                SELECT ConfigId, ModuleCode, ReportName, TemplatePath, TemplateType, 
                       PrintFields, PrintSettings, Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                FROM DataPrintConfigs
                WHERE ModuleCode = @ModuleCode AND Status = '1'
                ORDER BY ReportName";

            using var connection = _connectionFactory.CreateConnection();
            var results = await connection.QueryAsync<DataPrintConfigDto>(sql, new { ModuleCode = moduleCode });
            return results.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("取得列印設定列表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> PreviewPrintAsync(string moduleCode, long configId, Dictionary<string, object> data, Dictionary<string, object>? parameters)
    {
        try
        {
            _logger.LogInfo($"列印預覽: moduleCode={moduleCode}, configId={configId}");
            // 列印功能需要整合報表引擎，這裡先返回空陣列
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            _logger.LogError("列印預覽失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> PrintDataAsync(string moduleCode, long configId, Dictionary<string, object> data, Dictionary<string, object>? parameters)
    {
        // 與PreviewPrintAsync類似
        return await PreviewPrintAsync(moduleCode, configId, data, parameters);
    }

    public async Task<long> CreatePrintConfigAsync(DataPrintConfigDto dto)
    {
        try
        {
            _logger.LogInfo($"新增列印設定: moduleCode={dto.ModuleCode}");
            
            var sql = @"
                INSERT INTO DataPrintConfigs (ModuleCode, ReportName, TemplatePath, TemplateType, 
                                               PrintFields, PrintSettings, Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES (@ModuleCode, @ReportName, @TemplatePath, @TemplateType, 
                        @PrintFields, @PrintSettings, @Status, @CreatedBy, GETDATE(), @UpdatedBy, GETDATE());
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var userId = GetCurrentUserId();
            using var connection = _connectionFactory.CreateConnection();
            var configId = await connection.ExecuteScalarAsync<long>(sql, new
            {
                dto.ModuleCode,
                dto.ReportName,
                dto.TemplatePath,
                dto.TemplateType,
                dto.PrintFields,
                dto.PrintSettings,
                dto.Status,
                CreatedBy = userId,
                UpdatedBy = userId
            });

            return configId;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增列印設定失敗", ex);
            throw;
        }
    }

    public async Task UpdatePrintConfigAsync(long configId, DataPrintConfigDto dto)
    {
        try
        {
            _logger.LogInfo($"修改列印設定: configId={configId}");
            
            var sql = @"
                UPDATE DataPrintConfigs
                SET ReportName = @ReportName,
                    TemplatePath = @TemplatePath,
                    TemplateType = @TemplateType,
                    PrintFields = @PrintFields,
                    PrintSettings = @PrintSettings,
                    Status = @Status,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = GETDATE()
                WHERE ConfigId = @ConfigId";

            var userId = GetCurrentUserId();
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                ConfigId = configId,
                dto.ReportName,
                dto.TemplatePath,
                dto.TemplateType,
                dto.PrintFields,
                dto.PrintSettings,
                dto.Status,
                UpdatedBy = userId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("修改列印設定失敗", ex);
            throw;
        }
    }

    public async Task DeletePrintConfigAsync(long configId)
    {
        try
        {
            _logger.LogInfo($"刪除列印設定: configId={configId}");
            
            var sql = "UPDATE DataPrintConfigs SET Status = '0' WHERE ConfigId = @ConfigId";
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(sql, new { ConfigId = configId });
        }
        catch (Exception ex)
        {
            _logger.LogError("刪除列印設定失敗", ex);
            throw;
        }
    }

    #endregion

    #region 輔助方法

    private string BuildBrowseQuery(DataBrowseConfigDto config, DataBrowseQueryDto query)
    {
        // 簡化實現，實際應根據DisplayFields和FilterFields構建動態SQL
        var sql = $"SELECT TOP {query.PageSize} * FROM [{config.TableName}]";
        
        // 添加WHERE條件
        if (query.Filters != null && query.Filters.Count > 0)
        {
            var whereClause = string.Join(" AND ", query.Filters.Keys.Select(k => $"[{k}] = @{k}"));
            sql += " WHERE " + whereClause;
        }
        
        // 添加排序
        if (!string.IsNullOrEmpty(query.SortField))
        {
            sql += $" ORDER BY [{query.SortField}] {query.SortOrder ?? "ASC"}";
        }
        else if (!string.IsNullOrEmpty(config.DefaultSort))
        {
            sql += $" ORDER BY {config.DefaultSort}";
        }

        return sql;
    }

    private string BuildBrowseCountQuery(DataBrowseConfigDto config, DataBrowseQueryDto query)
    {
        var sql = $"SELECT COUNT(*) FROM [{config.TableName}]";
        
        if (query.Filters != null && query.Filters.Count > 0)
        {
            var whereClause = string.Join(" AND ", query.Filters.Keys.Select(k => $"[{k}] = @{k}"));
            sql += " WHERE " + whereClause;
        }

        return sql;
    }

    #endregion
}

