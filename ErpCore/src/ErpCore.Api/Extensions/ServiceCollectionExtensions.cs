using ErpCore.Api.Middleware;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Extensions;

/// <summary>
/// 服務集合擴充方法
/// 用於註冊應用程式服務
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 註冊中介軟體
    /// </summary>
    public static IServiceCollection AddErpCoreMiddleware(this IServiceCollection services)
    {
        // 註冊中介軟體（中介軟體本身不需要註冊，只需要在UseMiddleware中使用）
        return services;
    }

    /// <summary>
    /// 註冊過濾器
    /// </summary>
    public static IServiceCollection AddErpCoreFilters(this IServiceCollection services)
    {
        // 註冊全域過濾器
        services.AddControllers(options =>
        {
            // 可以在此處添加全域過濾器
            // options.Filters.Add<ValidateModelAttribute>();
            // options.Filters.Add<LogActionAttribute>();
        });

        return services;
    }

    /// <summary>
    /// 註冊CORS
    /// </summary>
    public static IServiceCollection AddErpCoreCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

        services.AddCors(options =>
        {
            options.AddPolicy("ErpCorePolicy", builder =>
            {
                builder.WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });

        return services;
    }
}

