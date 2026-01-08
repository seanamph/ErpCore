using System.Data;

namespace ErpCore.Infrastructure.Data;

/// <summary>
/// 資料庫連線工廠介面
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// 建立資料庫連線
    /// </summary>
    IDbConnection CreateConnection();
}

