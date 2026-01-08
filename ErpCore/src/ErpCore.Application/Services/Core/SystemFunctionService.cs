using ErpCore.Application.DTOs.Core;
using ErpCore.Application.Services.Base;
using ErpCore.Infrastructure.Data;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using Dapper;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography;

namespace ErpCore.Application.Services.Core;

/// <summary>
/// 系統功能服務實作
/// 提供系統識別、系統註冊、關於頁面等功能
/// </summary>
public class SystemFunctionService : BaseService, ISystemFunctionService
{
    private readonly IDbConnectionFactory _connectionFactory;

    public SystemFunctionService(
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _connectionFactory = connectionFactory;
    }

    #region Identify - 系統識別功能

    public async Task<SystemIdentityDto?> GetSystemIdentityAsync()
    {
        try
        {
            _logger.LogInfo("取得系統識別資訊");

            var sql = @"
                SELECT IdentityId, SystemId, ProjectTitle, CompanyTitle, EipUrl, EipEmbedded,
                       ShowTransEffect, InitShowTransEffect, NoResizeFrame, DebugUser, Status,
                       CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                FROM SystemIdentities
                WHERE Status = '1'
                ORDER BY CreatedAt DESC";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<SystemIdentityDto>(sql);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("取得系統識別資訊失敗", ex);
            throw;
        }
    }

    public async Task<MenuConfigDto> GetMenuConfigAsync()
    {
        try
        {
            _logger.LogInfo("取得選單設定");

            // 從系統參數或設定檔讀取選單設定
            // 這裡暫時返回預設值
            return new MenuConfigDto
            {
                SelectMessage = "請選擇功能",
                SelectToolbar = "工具列設定",
                ShowLeftButton = "Y"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("取得選單設定失敗", ex);
            throw;
        }
    }

    public async Task UpdateSystemIdentityAsync(UpdateSystemIdentityDto dto)
    {
        try
        {
            _logger.LogInfo("更新系統識別設定");

            var sql = @"
                UPDATE SystemIdentities
                SET ProjectTitle = ISNULL(@ProjectTitle, ProjectTitle),
                    CompanyTitle = ISNULL(@CompanyTitle, CompanyTitle),
                    EipUrl = ISNULL(@EipUrl, EipUrl),
                    EipEmbedded = ISNULL(@EipEmbedded, EipEmbedded),
                    ShowTransEffect = ISNULL(@ShowTransEffect, ShowTransEffect),
                    InitShowTransEffect = ISNULL(@InitShowTransEffect, InitShowTransEffect),
                    NoResizeFrame = ISNULL(@NoResizeFrame, NoResizeFrame),
                    DebugUser = ISNULL(@DebugUser, DebugUser),
                    Status = ISNULL(@Status, Status),
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = GETDATE()
                WHERE Status = '1'";

            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                dto.ProjectTitle,
                dto.CompanyTitle,
                dto.EipUrl,
                dto.EipEmbedded,
                dto.ShowTransEffect,
                dto.InitShowTransEffect,
                dto.NoResizeFrame,
                dto.DebugUser,
                dto.Status,
                UpdatedBy = GetCurrentUserId()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("更新系統識別設定失敗", ex);
            throw;
        }
    }

    public async Task<SystemInitDto> InitializeSystemAsync()
    {
        try
        {
            _logger.LogInfo("系統初始化");

            var identity = await GetSystemIdentityAsync();
            var menuConfig = await GetMenuConfigAsync();

            return new SystemInitDto
            {
                SystemId = identity?.SystemId ?? "IMS3",
                ProjectTitle = identity?.ProjectTitle ?? "IMS3 系統",
                CompanyTitle = identity?.CompanyTitle,
                EipUrl = identity?.EipUrl,
                MenuConfig = menuConfig,
                ToolbarConfig = new Dictionary<string, object>(),
                MessageConfig = new Dictionary<string, object>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("系統初始化失敗", ex);
            throw;
        }
    }

    #endregion

    #region MakeRegFile - 系統註冊功能

    public async Task<HardwareInfoDto> GetHardwareInfoAsync()
    {
        try
        {
            _logger.LogInfo("取得硬體資訊");

            var cpuNumber = GetCpuNumber();
            var computerName = Environment.MachineName;
            var macAddress = GetMacAddress();

            return new HardwareInfoDto
            {
                CpuNumber = cpuNumber,
                ComputerName = computerName,
                MacAddress = macAddress
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("取得硬體資訊失敗", ex);
            throw;
        }
    }

    public async Task<RegistrationResultDto> GenerateRegistrationAsync(GenerateRegistrationDto dto)
    {
        try
        {
            _logger.LogInfo($"生成註冊檔案: companyId={dto.CompanyId}, cpuNumber={dto.CpuNumber}");

            // 生成註冊金鑰
            var registrationKey = GenerateRegistrationKey(dto);

            // 儲存註冊記錄
            var sql = @"
                INSERT INTO SystemRegistrations (CompanyId, CpuNumber, ComputerName, MacAddress, RegistrationKey, 
                                                 ExpiryDate, LastDate, UseDownGo, Ticket, Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES (@CompanyId, @CpuNumber, @ComputerName, @MacAddress, @RegistrationKey, 
                        @ExpiryDate, @LastDate, @UseDownGo, @Ticket, 'ACTIVE', @CreatedBy, GETDATE(), @UpdatedBy, GETDATE());
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            using var connection = _connectionFactory.CreateConnection();
            var registrationId = await connection.ExecuteScalarAsync<long>(sql, new
            {
                dto.CompanyId,
                dto.CpuNumber,
                dto.ComputerName,
                dto.MacAddress,
                RegistrationKey = registrationKey,
                dto.ExpiryDate,
                LastDate = dto.ExpiryDate.ToString("yyyyMMdd"),
                dto.UseDownGo,
                dto.Ticket,
                CreatedBy = GetCurrentUserId(),
                UpdatedBy = GetCurrentUserId()
            });

            // 記錄操作日誌
            await SaveRegistrationLogAsync(registrationId, dto.CompanyId, "CREATE", dto.CpuNumber, dto.ComputerName, dto.MacAddress, "SUCCESS", null);

            return new RegistrationResultDto
            {
                RegistrationId = registrationId,
                RegistrationKey = registrationKey,
                DownloadUrl = $"/api/v1/core/system-function/registration/files/{registrationId}/download"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("生成註冊檔案失敗", ex);
            throw;
        }
    }

    public async Task<VerificationResultDto> VerifyRegistrationAsync(VerifyRegistrationDto dto)
    {
        try
        {
            _logger.LogInfo($"驗證註冊檔案: cpuNumber={dto.CpuNumber}");

            var sql = @"
                SELECT RegistrationId, CompanyId, CpuNumber, ComputerName, MacAddress, RegistrationKey, 
                       ExpiryDate, Status
                FROM SystemRegistrations
                WHERE CpuNumber = @CpuNumber AND RegistrationKey = @RegistrationKey AND Status = 'ACTIVE'";

            using var connection = _connectionFactory.CreateConnection();
            var registration = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new
            {
                dto.CpuNumber,
                dto.RegistrationKey
            });

            if (registration == null)
            {
                return new VerificationResultDto
                {
                    IsValid = false,
                    Status = "INVALID",
                    ErrorMessage = "註冊檔案不存在或已失效"
                };
            }

            // 驗證硬體資訊
            if (registration.ComputerName != dto.ComputerName || registration.MacAddress != dto.MacAddress)
            {
                return new VerificationResultDto
                {
                    IsValid = false,
                    Status = "HARDWARE_MISMATCH",
                    ErrorMessage = "硬體資訊不符"
                };
            }

            // 檢查到期日
            var expiryDate = (DateTime)registration.ExpiryDate;
            var daysRemaining = (expiryDate - DateTime.Now).Days;

            if (daysRemaining < 0)
            {
                return new VerificationResultDto
                {
                    IsValid = false,
                    Status = "EXPIRED",
                    ExpiryDate = expiryDate,
                    DaysRemaining = daysRemaining,
                    ErrorMessage = "註冊檔案已過期"
                };
            }

            return new VerificationResultDto
            {
                IsValid = true,
                Status = "ACTIVE",
                ExpiryDate = expiryDate,
                DaysRemaining = daysRemaining
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("驗證註冊檔案失敗", ex);
            throw;
        }
    }

    public async Task<VerificationResultDto> VerifyRegistrationFileAsync(byte[] fileContent)
    {
        try
        {
            _logger.LogInfo("驗證上傳的註冊檔案");

            // 解析註冊檔案內容
            // 這裡需要根據實際的註冊檔案格式實現
            // 暫時返回失敗
            return new VerificationResultDto
            {
                IsValid = false,
                Status = "INVALID",
                ErrorMessage = "不支援的註冊檔案格式"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("驗證註冊檔案失敗", ex);
            throw;
        }
    }

    public async Task<byte[]?> GetRegistrationFileAsync(long registrationId)
    {
        try
        {
            _logger.LogInfo($"取得註冊檔案: registrationId={registrationId}");

            var sql = "SELECT RegistrationFile FROM SystemRegistrations WHERE RegistrationId = @RegistrationId";

            using var connection = _connectionFactory.CreateConnection();
            var fileContent = await connection.QueryFirstOrDefaultAsync<byte[]>(sql, new { RegistrationId = registrationId });
            return fileContent;
        }
        catch (Exception ex)
        {
            _logger.LogError("取得註冊檔案失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<SystemRegistrationDto>> GetSystemRegistrationsAsync(SystemRegistrationQueryDto query)
    {
        try
        {
            _logger.LogInfo("查詢註冊記錄");

            var whereClause = new List<string> { "1=1" };
            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.CompanyId))
            {
                whereClause.Add("CompanyId = @CompanyId");
                parameters.Add("CompanyId", query.CompanyId);
            }
            if (!string.IsNullOrEmpty(query.CpuNumber))
            {
                whereClause.Add("CpuNumber LIKE @CpuNumber");
                parameters.Add("CpuNumber", $"%{query.CpuNumber}%");
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                whereClause.Add("Status = @Status");
                parameters.Add("Status", query.Status);
            }
            if (query.StartDate.HasValue)
            {
                whereClause.Add("CreatedAt >= @StartDate");
                parameters.Add("StartDate", query.StartDate.Value);
            }
            if (query.EndDate.HasValue)
            {
                whereClause.Add("CreatedAt <= @EndDate");
                parameters.Add("EndDate", query.EndDate.Value);
            }

            var whereSql = string.Join(" AND ", whereClause);
            var sql = $@"
                SELECT COUNT(*) FROM SystemRegistrations WHERE {whereSql};
                SELECT RegistrationId, CompanyId, CpuNumber, ComputerName, MacAddress, ExpiryDate, Status, CreatedAt
                FROM SystemRegistrations
                WHERE {whereSql}
                ORDER BY CreatedAt DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            using var connection = _connectionFactory.CreateConnection();
            using var multi = await connection.QueryMultipleAsync(sql, parameters);

            var totalCount = await multi.ReadSingleAsync<int>();
            var items = (await multi.ReadAsync<SystemRegistrationDto>()).ToList();

            return new PagedResult<SystemRegistrationDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢註冊記錄失敗", ex);
            throw;
        }
    }

    public async Task RevokeRegistrationAsync(long registrationId)
    {
        try
        {
            _logger.LogInfo($"撤銷註冊: registrationId={registrationId}");

            var sql = "UPDATE SystemRegistrations SET Status = 'REVOKED', UpdatedBy = @UpdatedBy, UpdatedAt = GETDATE() WHERE RegistrationId = @RegistrationId";

            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(sql, new { RegistrationId = registrationId, UpdatedBy = GetCurrentUserId() });
        }
        catch (Exception ex)
        {
            _logger.LogError("撤銷註冊失敗", ex);
            throw;
        }
    }

    public async Task<string> WebRegisterAsync(WebRegisterDto dto)
    {
        try
        {
            _logger.LogInfo($"網頁註冊: userId={dto.UserId}, cpuNumber={dto.CpuNumber}");

            // 解析到期日
            if (!DateTime.TryParse(dto.EDate, out var expiryDate))
            {
                throw new Exception("無效的到期日格式");
            }

            // 生成註冊
            var generateDto = new GenerateRegistrationDto
            {
                CompanyId = dto.TKey,
                CpuNumber = dto.CpuNumber,
                ComputerName = dto.ComputerName,
                MacAddress = dto.MacAddress,
                ExpiryDate = expiryDate
            };

            var result = await GenerateRegistrationAsync(generateDto);

            // 返回註冊資料字串 (格式: "OK:LAST_DATE^_^KEYS")
            var lastDate = expiryDate.ToString("yyyyMMdd");
            return $"OK:{lastDate}^_^{result.RegistrationKey}";
        }
        catch (Exception ex)
        {
            _logger.LogError("網頁註冊失敗", ex);
            throw;
        }
    }

    private string GetCpuNumber()
    {
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT ProcessorId FROM Win32_Processor");
            foreach (ManagementObject obj in searcher.Get())
            {
                return obj["ProcessorId"]?.ToString() ?? Environment.MachineName;
            }
        }
        catch
        {
            // 如果無法取得CPU編號，使用機器名稱
        }
        return Environment.MachineName;
    }

    private string GetMacAddress()
    {
        try
        {
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni => ni.OperationalStatus == OperationalStatus.Up && ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .FirstOrDefault();

            if (networkInterfaces != null)
            {
                return string.Join("-", networkInterfaces.GetPhysicalAddress().GetAddressBytes().Select(b => b.ToString("X2")));
            }
        }
        catch
        {
            // 如果無法取得MAC地址，返回預設值
        }
        return "00-00-00-00-00-00";
    }

    private string GenerateRegistrationKey(GenerateRegistrationDto dto)
    {
        // 組合註冊資訊
        var registrationData = $"{dto.CompanyId}|{dto.CpuNumber}|{dto.ComputerName}|{dto.MacAddress}|{dto.ExpiryDate:yyyyMMdd}";
        
        // 使用SHA256生成雜湊值
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(registrationData));
        var hashString = BitConverter.ToString(hashBytes).Replace("-", "");
        
        return hashString;
    }

    private async Task SaveRegistrationLogAsync(long? registrationId, string companyId, string operationType, string? cpuNumber, string? computerName, string? macAddress, string result, string? errorMessage)
    {
        try
        {
            var sql = @"
                INSERT INTO SystemRegistrationLogs (RegistrationId, CompanyId, OperationType, CpuNumber, ComputerName, 
                                                     MacAddress, Result, ErrorMessage, CreatedBy, CreatedAt)
                VALUES (@RegistrationId, @CompanyId, @OperationType, @CpuNumber, @ComputerName, 
                        @MacAddress, @Result, @ErrorMessage, @CreatedBy, GETDATE())";

            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                RegistrationId = registrationId,
                CompanyId = companyId,
                OperationType = operationType,
                CpuNumber = cpuNumber,
                ComputerName = computerName,
                MacAddress = macAddress,
                Result = result,
                ErrorMessage = errorMessage,
                CreatedBy = GetCurrentUserId()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("儲存註冊操作日誌失敗", ex);
            // 不拋出異常，避免影響主要功能
        }
    }

    #endregion

    #region about - 關於頁面功能

    public async Task<AboutInfoDto> GetAboutInfoAsync()
    {
        try
        {
            _logger.LogInfo("取得關於頁面資訊");

            // 從組件資訊讀取版本資訊
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version?.ToString() ?? "1.0.0";
            var buildDate = File.GetCreationTime(assembly.Location);

            return new AboutInfoDto
            {
                SystemName = "ErpCore 系統",
                Version = version,
                CompanyName = "RSL 公司",
                Copyright = $"© {DateTime.Now.Year} RSL. All rights reserved.",
                Description = "企業資源規劃系統",
                BuildDate = buildDate
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("取得關於頁面資訊失敗", ex);
            throw;
        }
    }

    #endregion
}

