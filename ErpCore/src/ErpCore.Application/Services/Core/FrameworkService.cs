using ErpCore.Application.DTOs.Core;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.SystemConfig;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.SystemConfig;
using ErpCore.Infrastructure.Repositories.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Text;
using Dapper;

namespace ErpCore.Application.Services.Core;

/// <summary>
/// 框架功能服務實作
/// </summary>
public class FrameworkService : BaseService, IFrameworkService
{
    private readonly IConfigSystemRepository _configSystemRepository;
    private readonly IConfigSubSystemRepository _configSubSystemRepository;
    private readonly IConfigProgramRepository _configProgramRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IConfiguration _configuration;

    public FrameworkService(
        IConfigSystemRepository configSystemRepository,
        IConfigSubSystemRepository configSubSystemRepository,
        IConfigProgramRepository configProgramRepository,
        IUserRepository userRepository,
        IDbConnectionFactory connectionFactory,
        IConfiguration configuration,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _configSystemRepository = configSystemRepository;
        _configSubSystemRepository = configSubSystemRepository;
        _configProgramRepository = configProgramRepository;
        _userRepository = userRepository;
        _connectionFactory = connectionFactory;
        _configuration = configuration;
    }

    public async Task<MenuTreeDto> GetMenuTreeAsync(string? sysId, string? searchKeyword)
    {
        try
        {
            _logger.LogInfo($"查詢選單樹: sysId={sysId}, searchKeyword={searchKeyword}");
            var userId = GetCurrentUserId();
            var menus = new List<MenuNodeDto>();

            // 查詢系統列表
            var systemQuery = new ConfigSystemQuery
            {
                PageIndex = 1,
                PageSize = 1000,
                Status = "A"
            };
            if (!string.IsNullOrEmpty(sysId))
            {
                systemQuery.SystemId = sysId;
            }
            var systemsResult = await _configSystemRepository.QueryAsync(systemQuery);
            var systems = systemsResult.Items.ToList();

            foreach (var system in systems.OrderBy(s => s.SeqNo))
            {
                // 檢查使用者權限（簡化處理，實際應根據權限表查詢）
                var hasPermission = await CheckSystemPermissionAsync(userId, system.SystemId);
                if (!hasPermission) continue;

                var systemNode = new MenuNodeDto
                {
                    Id = system.SystemId,
                    Name = system.SystemName,
                    Type = "SYSTEM",
                    Icon = "system",
                    Children = new List<MenuNodeDto>()
                };

                // 查詢子系統
                var subSystemQuery = new ConfigSubSystemQuery
                {
                    PageIndex = 1,
                    PageSize = 1000,
                    SystemId = system.SystemId,
                    Status = "A"
                };
                var subSystemsResult = await _configSubSystemRepository.QueryAsync(subSystemQuery);
                var subSystems = subSystemsResult.Items.ToList();
                foreach (var subSystem in subSystems.OrderBy(s => s.SeqNo))
                {
                    var subSystemNode = new MenuNodeDto
                    {
                        Id = subSystem.SubSystemId ?? string.Empty,
                        Name = subSystem.SubSystemName ?? string.Empty,
                        Type = "SUBSYSTEM",
                        Icon = "folder",
                        Children = new List<MenuNodeDto>()
                    };

                    // 查詢作業
                    var programQuery = new ConfigProgramQuery
                    {
                        PageIndex = 1,
                        PageSize = 1000,
                        SystemId = system.SystemId,
                        SubSystemId = subSystem.SubSystemId,
                        Status = "A"
                    };
                    var programsResult = await _configProgramRepository.QueryAsync(programQuery);
                    var programs = programsResult.Items.ToList();
                    foreach (var program in programs.OrderBy(p => p.SeqNo))
                    {
                        var programNode = new MenuNodeDto
                        {
                            Id = program.ProgramId,
                            Name = program.ProgramName,
                            Type = "PROGRAM",
                            Icon = "file",
                            Url = $"/system/programs/{program.ProgramId}"
                        };

                        // 搜尋關鍵字過濾
                        if (string.IsNullOrEmpty(searchKeyword) ||
                            program.ProgramName.Contains(searchKeyword, StringComparison.OrdinalIgnoreCase) ||
                            program.ProgramId.Contains(searchKeyword, StringComparison.OrdinalIgnoreCase))
                        {
                            subSystemNode.Children.Add(programNode);
                        }
                    }

                    if (subSystemNode.Children.Count > 0 || string.IsNullOrEmpty(searchKeyword))
                    {
                        systemNode.Children.Add(subSystemNode);
                    }
                }

                if (systemNode.Children.Count > 0 || string.IsNullOrEmpty(searchKeyword))
                {
                    menus.Add(systemNode);
                }
            }

            return new MenuTreeDto { Menus = menus };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢選單樹失敗", ex);
            throw;
        }
    }

    public async Task<MenuSearchResultDto> SearchMenusAsync(string keyword, string? sysId)
    {
        try
        {
            _logger.LogInfo($"搜尋選單: keyword={keyword}, sysId={sysId}");
            var results = new List<MenuSearchItemDto>();

            // 查詢作業
            var programQuery = new ConfigProgramQuery
            {
                PageIndex = 1,
                PageSize = 1000,
                Status = "A"
            };
            if (!string.IsNullOrEmpty(sysId))
            {
                programQuery.SystemId = sysId;
            }
            var programsResult = await _configProgramRepository.QueryAsync(programQuery);
            var programs = programsResult.Items.ToList();

            foreach (var program in programs)
            {
                if (program.ProgramName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    program.ProgramId.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    var system = await _configSystemRepository.GetByIdAsync(program.SystemId);
                    var subSystem = program.SubSystemId != null
                        ? await _configSubSystemRepository.GetByIdAsync(program.SubSystemId)
                        : null;

                    var path = $"{system?.SystemName ?? ""} > {subSystem?.SubSystemName ?? ""} > {program.ProgramName}";

                    results.Add(new MenuSearchItemDto
                    {
                        Id = program.ProgramId,
                        Name = program.ProgramName,
                        Type = "PROGRAM",
                        Url = $"/system/programs/{program.ProgramId}",
                        Path = path
                    });
                }
            }

            return new MenuSearchResultDto { Results = results };
        }
        catch (Exception ex)
        {
            _logger.LogError("搜尋選單失敗", ex);
            throw;
        }
    }

    public async Task<List<MenuFavoriteDto>> GetFavoritesAsync(int? frameIndex, string? searchKeyword)
    {
        try
        {
            _logger.LogInfo($"查詢收藏選單: frameIndex={frameIndex}, searchKeyword={searchKeyword}");
            var userId = GetCurrentUserId();

            var sql = new StringBuilder(@"
                SELECT 
                    f.Id,
                    f.MenuId,
                    f.MenuType,
                    f.SortOrder,
                    f.FrameIndex,
                    CASE 
                        WHEN f.MenuType = 'PROGRAM' THEN p.ProgramName
                        WHEN f.MenuType = 'SUBSYSTEM' THEN s.SubSystemName
                        WHEN f.MenuType = 'SYSTEM' THEN sys.SystemName
                        ELSE ''
                    END AS MenuName,
                    CASE 
                        WHEN f.MenuType = 'PROGRAM' THEN '/system/programs/' + p.ProgramId
                        ELSE NULL
                    END AS Url
                FROM UserMenuFavorites f
                LEFT JOIN ConfigPrograms p ON f.MenuType = 'PROGRAM' AND f.MenuId = p.ProgramId
                LEFT JOIN ConfigSubSystems s ON f.MenuType = 'SUBSYSTEM' AND f.MenuId = s.SubSystemId
                LEFT JOIN ConfigSystems sys ON f.MenuType = 'SYSTEM' AND f.MenuId = sys.SystemId
                WHERE f.UserId = @UserId");

            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId);

            if (frameIndex.HasValue)
            {
                sql.Append(" AND f.FrameIndex = @FrameIndex");
                parameters.Add("FrameIndex", frameIndex.Value);
            }

            if (!string.IsNullOrEmpty(searchKeyword))
            {
                sql.Append(" AND (f.MenuId LIKE @SearchKeyword OR MenuName LIKE @SearchKeyword)");
                parameters.Add("SearchKeyword", $"%{searchKeyword}%");
            }

            sql.Append(" ORDER BY f.SortOrder");

            using var connection = _connectionFactory.CreateConnection();
            var favorites = await connection.QueryAsync<MenuFavoriteDto>(sql.ToString(), parameters);

            return favorites.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢收藏選單失敗", ex);
            throw;
        }
    }

    public async Task<MenuFavoriteDto> AddFavoriteAsync(AddMenuFavoriteDto dto)
    {
        try
        {
            _logger.LogInfo($"新增收藏選單: MenuId={dto.MenuId}, MenuType={dto.MenuType}");
            var userId = GetCurrentUserId();

            // 檢查是否已存在
            const string checkSql = @"
                SELECT COUNT(*) FROM UserMenuFavorites
                WHERE UserId = @UserId AND MenuId = @MenuId AND MenuType = @MenuType";

            using var connection = _connectionFactory.CreateConnection();
            var exists = await connection.QuerySingleAsync<int>(checkSql, new
            {
                UserId = userId,
                MenuId = dto.MenuId,
                MenuType = dto.MenuType
            });

            if (exists > 0)
            {
                throw new InvalidOperationException("該選單已存在於收藏中");
            }

            // 取得最大排序順序
            const string maxSortSql = @"
                SELECT ISNULL(MAX(SortOrder), 0) FROM UserMenuFavorites
                WHERE UserId = @UserId AND FrameIndex = @FrameIndex";

            var maxSort = await connection.QuerySingleAsync<int>(maxSortSql, new
            {
                UserId = userId,
                FrameIndex = dto.FrameIndex
            });

            // 新增收藏
            const string insertSql = @"
                INSERT INTO UserMenuFavorites (UserId, MenuId, MenuType, FrameIndex, SortOrder, CreatedBy, CreatedAt)
                VALUES (@UserId, @MenuId, @MenuType, @FrameIndex, @SortOrder, @CreatedBy, GETDATE());
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var favoriteId = await connection.QuerySingleAsync<long>(insertSql, new
            {
                UserId = userId,
                MenuId = dto.MenuId,
                MenuType = dto.MenuType,
                FrameIndex = dto.FrameIndex,
                SortOrder = maxSort + 1,
                CreatedBy = userId
            });

            // 查詢新增的收藏
            return await GetFavoriteByIdAsync(favoriteId);
        }
        catch (Exception ex)
        {
            _logger.LogError("新增收藏選單失敗", ex);
            throw;
        }
    }

    public async Task RemoveFavoriteAsync(long favoriteId)
    {
        try
        {
            _logger.LogInfo($"刪除收藏選單: favoriteId={favoriteId}");
            var userId = GetCurrentUserId();

            const string sql = @"
                DELETE FROM UserMenuFavorites
                WHERE Id = @FavoriteId AND UserId = @UserId";

            using var connection = _connectionFactory.CreateConnection();
            var affected = await connection.ExecuteAsync(sql, new { FavoriteId = favoriteId, UserId = userId });

            if (affected == 0)
            {
                throw new InvalidOperationException("收藏選單不存在或無權限刪除");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("刪除收藏選單失敗", ex);
            throw;
        }
    }

    public async Task UpdateFavoriteSortAsync(UpdateFavoriteSortDto dto)
    {
        try
        {
            _logger.LogInfo("更新收藏選單排序");
            var userId = GetCurrentUserId();

            using var connection = _connectionFactory.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var favorite in dto.Favorites)
                {
                    const string sql = @"
                        UPDATE UserMenuFavorites
                        SET SortOrder = @SortOrder
                        WHERE Id = @Id AND UserId = @UserId";

                    await connection.ExecuteAsync(sql, new
                    {
                        Id = favorite.Id,
                        SortOrder = favorite.SortOrder,
                        UserId = userId
                    }, transaction);
                }

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
            _logger.LogError("更新收藏選單排序失敗", ex);
            throw;
        }
    }

    public async Task SwitchFrameAsync(long favoriteId, int frameIndex)
    {
        try
        {
            _logger.LogInfo($"切換收藏選單框架: favoriteId={favoriteId}, frameIndex={frameIndex}");
            var userId = GetCurrentUserId();

            const string sql = @"
                UPDATE UserMenuFavorites
                SET FrameIndex = @FrameIndex
                WHERE Id = @FavoriteId AND UserId = @UserId";

            using var connection = _connectionFactory.CreateConnection();
            var affected = await connection.ExecuteAsync(sql, new
            {
                FavoriteId = favoriteId,
                FrameIndex = frameIndex,
                UserId = userId
            });

            if (affected == 0)
            {
                throw new InvalidOperationException("收藏選單不存在或無權限修改");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("切換收藏選單框架失敗", ex);
            throw;
        }
    }

    public async Task<SystemHeaderInfoDto> GetHeaderInfoAsync()
    {
        try
        {
            _logger.LogInfo("查詢系統標題資訊");
            var userId = GetCurrentUserId();
            var user = await _userRepository.GetByIdAsync(userId);

            var settings = _configuration.GetSection("SystemSettings");

            var result = new SystemHeaderInfoDto
            {
                ProjectTitle = settings["ProjectTitle"] ?? "IMS3 系統",
                ProjectTitleEn = settings["ProjectTitleEn"] ?? "IMS3 System",
                Version = GetVersion(),
                Welcome = settings["Welcome"] ?? "歡迎",
                UserName = user?.UserName ?? "",
                LogoPath = settings["LogoPath"] ?? "/images/logo_ims.jpg",
                ShowVersion = settings.GetValue<bool>("ShowHeadVersion", true),
                CurrentDate = DateTime.Now,
                ExpireDate = await GetExpireDateAsync(),
                ShowExpireDate = ShouldShowExpireDate(userId),
                IsTestEnvironment = IsTestEnvironment()
            };

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢系統標題資訊失敗", ex);
            throw;
        }
    }

    public CurrentTimeDto GetCurrentTime()
    {
        try
        {
            return new CurrentTimeDto
            {
                CurrentTime = DateTime.Now,
                Timezone = TimeZoneInfo.Local.Id
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢當前時間失敗", ex);
            throw;
        }
    }

    public async Task<List<ModuleDto>> GetUserModulesAsync()
    {
        try
        {
            _logger.LogInfo("查詢使用者可用的模組列表");
            var userId = GetCurrentUserId();

            // 簡化處理：查詢所有啟用的系統作為模組
            var systemQuery = new ConfigSystemQuery
            {
                PageIndex = 1,
                PageSize = 1000,
                Status = "A"
            };
            var systemsResult = await _configSystemRepository.QueryAsync(systemQuery);
            var systems = systemsResult.Items.ToList();
            var modules = systems
                .Where(s => s.Status == "A")
                .OrderBy(s => s.SeqNo)
                .Select(s => new ModuleDto
                {
                    ModuleId = s.SystemId,
                    ModuleName = s.SystemName,
                    ModuleNameEn = s.SystemNameEn,
                    ImgOnUrl = $"/images/modules/{s.SystemId}_on.png",
                    ImgOffUrl = $"/images/modules/{s.SystemId}_off.png",
                    SortOrder = s.SeqNo ?? 0
                })
                .ToList();

            return modules;
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢使用者可用的模組列表失敗", ex);
            throw;
        }
    }

    public async Task<List<SubsystemDto>> GetModuleSubsystemsAsync(string moduleId)
    {
        try
        {
            _logger.LogInfo($"查詢模組的子系統列表: moduleId={moduleId}");
            var subSystemQuery = new ConfigSubSystemQuery
            {
                PageIndex = 1,
                PageSize = 1000,
                SystemId = moduleId,
                Status = "A"
            };
            var subSystemsResult = await _configSubSystemRepository.QueryAsync(subSystemQuery);
            var subSystems = subSystemsResult.Items.ToList();

            var subsystems = subSystems
                .Where(s => s.Status == "A")
                .OrderBy(s => s.SeqNo)
                .Select(s => new SubsystemDto
                {
                    SystemId = s.SubSystemId ?? string.Empty,
                    SystemName = s.SubSystemName ?? string.Empty,
                    SystemUrl = $"/system/subsystems/{s.SubSystemId}",
                    SortOrder = s.SeqNo ?? 0
                })
                .ToList();

            return subsystems;
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢模組的子系統列表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<MessageDto>> GetMessagesAsync(MessageQueryDto query)
    {
        try
        {
            _logger.LogInfo("查詢訊息列表");
            var userId = GetCurrentUserId();

            var sql = new StringBuilder(@"
                SELECT 
                    m.MessageId,
                    m.Title,
                    m.Content,
                    m.MessageType,
                    m.Priority,
                    m.StartDate,
                    m.EndDate,
                    m.CreatedAt,
                    m.CreatedBy,
                    CASE WHEN r.Id IS NOT NULL THEN 1 ELSE 0 END AS IsRead
                FROM SystemMessages m
                LEFT JOIN UserMessageReads r ON m.MessageId = r.MessageId AND r.UserId = @UserId
                WHERE m.IsActive = 1
                    AND m.StartDate <= GETDATE()
                    AND (m.EndDate IS NULL OR m.EndDate >= GETDATE())");

            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId);

            if (!string.IsNullOrEmpty(query.MessageType))
            {
                sql.Append(" AND m.MessageType = @MessageType");
                parameters.Add("MessageType", query.MessageType);
            }

            if (query.StartDate.HasValue)
            {
                sql.Append(" AND m.StartDate >= @StartDate");
                parameters.Add("StartDate", query.StartDate.Value);
            }

            if (query.EndDate.HasValue)
            {
                sql.Append(" AND m.EndDate <= @EndDate");
                parameters.Add("EndDate", query.EndDate.Value);
            }

            if (query.IsRead.HasValue)
            {
                if (query.IsRead.Value)
                {
                    sql.Append(" AND r.Id IS NOT NULL");
                }
                else
                {
                    sql.Append(" AND r.Id IS NULL");
                }
            }

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                sql.Append(" AND (m.Title LIKE @Keyword OR m.Content LIKE @Keyword)");
                parameters.Add("Keyword", $"%{query.Keyword}%");
            }

            // 總筆數
            var countSql = $"SELECT COUNT(*) FROM ({sql}) AS t";
            using var countConnection = _connectionFactory.CreateConnection();
            var totalCount = await countConnection.QuerySingleAsync<int>(countSql, parameters);

            // 分頁
            sql.Append(" ORDER BY m.CreatedAt DESC");
            sql.Append(" OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY");
            parameters.Add("Offset", (query.Page - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            using var connection = _connectionFactory.CreateConnection();
            var messages = await connection.QueryAsync<MessageDto>(sql, parameters);

            return new PagedResult<MessageDto>
            {
                Items = messages.ToList(),
                TotalCount = totalCount,
                PageIndex = query.Page,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢訊息列表失敗", ex);
            throw;
        }
    }

    public async Task<MessageDto?> GetMessageAsync(string messageId)
    {
        try
        {
            _logger.LogInfo($"查詢單筆訊息: messageId={messageId}");
            var userId = GetCurrentUserId();

            const string sql = @"
                SELECT 
                    m.MessageId,
                    m.Title,
                    m.Content,
                    m.MessageType,
                    m.Priority,
                    m.StartDate,
                    m.EndDate,
                    m.CreatedAt,
                    m.CreatedBy,
                    CASE WHEN r.Id IS NOT NULL THEN 1 ELSE 0 END AS IsRead
                FROM SystemMessages m
                LEFT JOIN UserMessageReads r ON m.MessageId = r.MessageId AND r.UserId = @UserId
                WHERE m.MessageId = @MessageId AND m.IsActive = 1";

            using var connection = _connectionFactory.CreateConnection();
            var message = await connection.QueryFirstOrDefaultAsync<MessageDto>(sql, new
            {
                MessageId = messageId,
                UserId = userId
            });

            return message;
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢單筆訊息失敗", ex);
            throw;
        }
    }

    public async Task MarkAsReadAsync(string messageId)
    {
        try
        {
            _logger.LogInfo($"標記訊息為已讀: messageId={messageId}");
            var userId = GetCurrentUserId();

            const string checkSql = @"
                SELECT COUNT(*) FROM UserMessageReads
                WHERE UserId = @UserId AND MessageId = @MessageId";

            using var connection = _connectionFactory.CreateConnection();
            var exists = await connection.QuerySingleAsync<int>(checkSql, new
            {
                UserId = userId,
                MessageId = messageId
            });

            if (exists == 0)
            {
                const string insertSql = @"
                    INSERT INTO UserMessageReads (UserId, MessageId, ReadAt)
                    VALUES (@UserId, @MessageId, GETDATE())";

                await connection.ExecuteAsync(insertSql, new
                {
                    UserId = userId,
                    MessageId = messageId
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("標記訊息為已讀失敗", ex);
            throw;
        }
    }

    public async Task BatchMarkAsReadAsync(BatchReadDto dto)
    {
        try
        {
            _logger.LogInfo($"批次標記訊息為已讀: count={dto.MessageIds.Count}");
            var userId = GetCurrentUserId();

            using var connection = _connectionFactory.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var messageId in dto.MessageIds)
                {
                    const string checkSql = @"
                        SELECT COUNT(*) FROM UserMessageReads
                        WHERE UserId = @UserId AND MessageId = @MessageId";

                    var exists = await connection.QuerySingleAsync<int>(checkSql, new
                    {
                        UserId = userId,
                        MessageId = messageId
                    }, transaction);

                    if (exists == 0)
                    {
                        const string insertSql = @"
                            INSERT INTO UserMessageReads (UserId, MessageId, ReadAt)
                            VALUES (@UserId, @MessageId, GETDATE())";

                        await connection.ExecuteAsync(insertSql, new
                        {
                            UserId = userId,
                            MessageId = messageId
                        }, transaction);
                    }
                }

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
            _logger.LogError("批次標記訊息為已讀失敗", ex);
            throw;
        }
    }

    public async Task<List<MessageDto>> GetMessagesByDateAsync(DateTime? date, DateTime? startDate, DateTime? endDate)
    {
        try
        {
            _logger.LogInfo($"查詢日期範圍內的訊息: date={date}, startDate={startDate}, endDate={endDate}");
            var userId = GetCurrentUserId();

            var sql = new StringBuilder(@"
                SELECT 
                    m.MessageId,
                    m.Title,
                    m.Content,
                    m.MessageType,
                    m.Priority,
                    m.StartDate,
                    m.EndDate,
                    m.CreatedAt,
                    m.CreatedBy,
                    CASE WHEN r.Id IS NOT NULL THEN 1 ELSE 0 END AS IsRead
                FROM SystemMessages m
                LEFT JOIN UserMessageReads r ON m.MessageId = r.MessageId AND r.UserId = @UserId
                WHERE m.IsActive = 1");

            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId);

            if (date.HasValue)
            {
                sql.Append(" AND CAST(m.StartDate AS DATE) = CAST(@Date AS DATE)");
                parameters.Add("Date", date.Value);
            }
            else
            {
                if (startDate.HasValue)
                {
                    sql.Append(" AND m.StartDate >= @StartDate");
                    parameters.Add("StartDate", startDate.Value);
                }

                if (endDate.HasValue)
                {
                    sql.Append(" AND m.EndDate <= @EndDate");
                    parameters.Add("EndDate", endDate.Value);
                }
            }

            sql.Append(" ORDER BY m.CreatedAt DESC");

            using var connection = _connectionFactory.CreateConnection();
            var messages = await connection.QueryAsync<MessageDto>(sql, parameters);

            return messages.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢日期範圍內的訊息失敗", ex);
            throw;
        }
    }

    public async Task<UnreadCountDto> GetUnreadCountAsync()
    {
        try
        {
            _logger.LogInfo("查詢未讀訊息數量");
            var userId = GetCurrentUserId();

            const string sql = @"
                SELECT 
                    m.MessageType,
                    COUNT(*) AS UnreadCount
                FROM SystemMessages m
                LEFT JOIN UserMessageReads r ON m.MessageId = r.MessageId AND r.UserId = @UserId
                WHERE m.IsActive = 1
                    AND m.StartDate <= GETDATE()
                    AND (m.EndDate IS NULL OR m.EndDate >= GETDATE())
                    AND r.Id IS NULL
                GROUP BY m.MessageType";

            using var connection = _connectionFactory.CreateConnection();
            var results = await connection.QueryAsync<dynamic>(sql, new { UserId = userId });

            var unreadByType = new Dictionary<string, int>();
            foreach (var result in results)
            {
                var messageType = result.MessageType?.ToString() ?? "";
                var unreadCount = Convert.ToInt32(result.UnreadCount ?? 0);
                unreadByType[messageType] = unreadCount;
            }
            var totalUnread = unreadByType.Values.Sum();

            return new UnreadCountDto
            {
                UnreadCount = totalUnread,
                UnreadByType = unreadByType
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢未讀訊息數量失敗", ex);
            throw;
        }
    }

    #region 私有方法

    private async Task<bool> CheckSystemPermissionAsync(string userId, string systemId)
    {
        try
        {
            // 簡化處理：實際應根據權限表查詢
            // 這裡假設所有使用者都有權限，實際應查詢 UserPermissions 或 RolePermissions
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查系統權限失敗: userId={userId}, systemId={systemId}", ex);
            return false;
        }
    }

    private async Task<MenuFavoriteDto> GetFavoriteByIdAsync(long favoriteId)
    {
        const string sql = @"
            SELECT 
                f.Id,
                f.MenuId,
                f.MenuType,
                f.SortOrder,
                f.FrameIndex,
                CASE 
                    WHEN f.MenuType = 'PROGRAM' THEN p.ProgramName
                    WHEN f.MenuType = 'SUBSYSTEM' THEN s.SubSystemName
                    WHEN f.MenuType = 'SYSTEM' THEN sys.SystemName
                    ELSE ''
                END AS MenuName,
                CASE 
                    WHEN f.MenuType = 'PROGRAM' THEN '/system/programs/' + p.ProgramId
                    ELSE NULL
                END AS Url
            FROM UserMenuFavorites f
            LEFT JOIN ConfigPrograms p ON f.MenuType = 'PROGRAM' AND f.MenuId = p.ProgramId
            LEFT JOIN ConfigSubSystems s ON f.MenuType = 'SUBSYSTEM' AND f.MenuId = s.SubSystemId
            LEFT JOIN ConfigSystems sys ON f.MenuType = 'SYSTEM' AND f.MenuId = sys.SystemId
            WHERE f.Id = @FavoriteId";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<MenuFavoriteDto>(sql, new { FavoriteId = favoriteId });
    }

    private string GetVersion()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;
            return version?.ToString() ?? "v1.0.0";
        }
        catch
        {
            return "v1.0.0";
        }
    }

    private async Task<DateTime?> GetExpireDateAsync()
    {
        try
        {
            var companyId = _configuration["CompanyId"];
            if (string.IsNullOrEmpty(companyId)) return null;

            var filePath = Path.Combine(
                AppContext.BaseDirectory,
                "kernel",
                $"RegSys_{companyId}.dll"
            );

            if (File.Exists(filePath))
            {
                var content = await File.ReadAllTextAsync(filePath);
                var expireDateStr = DecodeBase64(content);
                if (DateTime.TryParse(expireDateStr, out var expireDate))
                {
                    return expireDate;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning("讀取註冊檔案失敗", ex);
        }

        return null;
    }

    private bool ShouldShowExpireDate(string userId)
    {
        var alwaysShow = _configuration.GetValue<bool>("SystemSettings:AlwaysShowExpDate", false);
        return alwaysShow && userId == "xcom";
    }

    private bool IsTestEnvironment()
    {
        var environment = _configuration["ASPNETCORE_ENVIRONMENT"];
        return environment == "Development" || environment == "Test";
    }

    private string DecodeBase64(string base64String)
    {
        try
        {
            var bytes = Convert.FromBase64String(base64String);
            return Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            return "";
        }
    }

    #endregion
}

