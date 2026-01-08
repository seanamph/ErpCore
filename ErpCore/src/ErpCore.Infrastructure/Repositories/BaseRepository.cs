using System.Data;
using Dapper;
using ErpCore.Infrastructure.Data;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories;

/// <summary>
/// 基礎 Repository 類別
/// 使用 Dapper 進行資料庫存取
/// </summary>
public abstract class BaseRepository
{
    protected readonly IDbConnectionFactory _connectionFactory;
    protected readonly ILoggerService _logger;

    protected BaseRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    /// <summary>
    /// 執行查詢並返回單一結果
    /// </summary>
    protected async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null)
    {
        try
        {
            _logger.LogDebug($"執行查詢: {sql}");
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢失敗: {sql}", ex);
            throw;
        }
    }

    /// <summary>
    /// 執行查詢並返回多筆結果
    /// </summary>
    protected async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null)
    {
        try
        {
            _logger.LogDebug($"執行查詢: {sql}");
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<T>(sql, param, transaction);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢失敗: {sql}", ex);
            throw;
        }
    }

    /// <summary>
    /// 執行新增、修改、刪除操作
    /// </summary>
    protected async Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null)
    {
        try
        {
            _logger.LogDebug($"執行命令: {sql}");
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteAsync(sql, param, transaction);
        }
        catch (Exception ex)
        {
            _logger.LogError($"執行命令失敗: {sql}", ex);
            throw;
        }
    }

    /// <summary>
    /// 執行查詢並返回單一值
    /// </summary>
    protected async Task<T> ExecuteScalarAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null)
    {
        try
        {
            _logger.LogDebug($"執行查詢: {sql}");
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteScalarAsync<T>(sql, param, transaction) ?? default(T)!;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢失敗: {sql}", ex);
            throw;
        }
    }

    /// <summary>
    /// 執行查詢並返回單一值（用於 COUNT 等查詢）
    /// </summary>
    protected async Task<T> QuerySingleAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null)
    {
        try
        {
            _logger.LogDebug($"執行查詢: {sql}");
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QuerySingleAsync<T>(sql, param, transaction);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢失敗: {sql}", ex);
            throw;
        }
    }
}

