using ErpCore.Api.Middleware;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Extensions;

/// <summary>
/// 應用程式建構器擴充方法
/// 用於配置中介軟體管道
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// 使用ErpCore中介軟體
    /// </summary>
    public static IApplicationBuilder UseErpCoreMiddleware(this IApplicationBuilder app)
    {
        // 使用日誌記錄中介軟體（最先執行）
        app.UseMiddleware<LoggingMiddleware>();

        // 使用例外處理中介軟體（應該在其他中介軟體之前）
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        // 使用身份驗證中介軟體
        app.UseMiddleware<AuthenticationMiddleware>();

        // 使用授權中介軟體
        app.UseMiddleware<AuthorizationMiddleware>();

        return app;
    }

    /// <summary>
    /// 使用ErpCore CORS
    /// </summary>
    public static IApplicationBuilder UseErpCoreCors(this IApplicationBuilder app)
    {
        app.UseCors("ErpCorePolicy");
        return app;
    }
}

