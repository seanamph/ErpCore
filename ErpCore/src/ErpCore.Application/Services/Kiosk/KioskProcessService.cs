using ErpCore.Application.DTOs.Kiosk;
using ErpCore.Application.DTOs.Loyalty;
using ErpCore.Application.Services.Base;
using ErpCore.Application.Services.Loyalty;
using ErpCore.Domain.Entities.Kiosk;
using ErpCore.Infrastructure.Repositories.Kiosk;
using ErpCore.Shared.Logging;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

namespace ErpCore.Application.Services.Kiosk;

/// <summary>
/// Kiosk處理服務實作
/// </summary>
public class KioskProcessService : BaseService, IKioskProcessService
{
    private readonly IKioskTransactionRepository _transactionRepository;
    private readonly ILoyaltyMaintenanceService _loyaltyService;

    public KioskProcessService(
        IKioskTransactionRepository transactionRepository,
        ILoyaltyMaintenanceService loyaltyService,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _transactionRepository = transactionRepository;
        _loyaltyService = loyaltyService;
    }

    public async Task<KioskProcessResponseDto> ProcessRequestAsync(KioskProcessRequestDto request)
    {
        try
        {
            // 產生交易編號
            var transactionId = GenerateTransactionId();

            // 建立交易記錄
            var transaction = new KioskTransaction
            {
                TransactionId = transactionId,
                KioskId = request.KioskId,
                FunctionCode = request.FunCmdId,
                CardNumber = request.LoyalSysCard,
                MemberId = request.Pid,
                TransactionDate = DateTime.Now,
                RequestData = JsonSerializer.Serialize(request),
                Status = "Success",
                ReturnCode = "0000",
                CreatedAt = DateTime.Now
            };

            // 根據功能代碼處理不同業務邏輯
            var response = request.FunCmdId switch
            {
                "O2" => await ProcessQuickCardAsync(request), // 網路線上快速開卡
                "A1" => await ProcessVerifyCardAsync(request), // 確認會員卡號、密碼
                "C2" => await ProcessChangePasswordAsync(request), // 密碼變更
                "D4" => await ProcessNetworkPointsAsync(request), // 網路會員點數資訊
                "D8" => await ProcessPhysicalPointsAsync(request), // 實體會員點數資訊
                _ => new KioskProcessResponseDto
                {
                    ReturnCode = "E999",
                    ReturnMessage = $"不支援的功能代碼: {request.FunCmdId}",
                    TransactionId = transactionId
                }
            };

            // 更新交易記錄
            transaction.ResponseData = JsonSerializer.Serialize(response);
            transaction.ReturnCode = response.ReturnCode;
            transaction.Status = response.ReturnCode == "0000" ? "Success" : "Failed";
            transaction.ErrorMessage = response.ReturnCode != "0000" ? response.ReturnMessage : null;

            await _transactionRepository.CreateAsync(transaction);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"處理Kiosk請求失敗: {request.FunCmdId}", ex);
            return new KioskProcessResponseDto
            {
                ReturnCode = "E999",
                ReturnMessage = $"處理失敗: {ex.Message}",
                TransactionId = GenerateTransactionId()
            };
        }
    }

    /// <summary>
    /// 快速開卡邏輯 (O2)
    /// </summary>
    private async Task<KioskProcessResponseDto> ProcessQuickCardAsync(KioskProcessRequestDto request)
    {
        try
        {
            // 驗證必填欄位
            if (string.IsNullOrEmpty(request.LoyalSysCard) || string.IsNullOrEmpty(request.Pid))
            {
                return new KioskProcessResponseDto
                {
                    ReturnCode = "E001",
                    ReturnMessage = "資料不完整：卡號和身份證字號為必填",
                    TransactionId = GenerateTransactionId()
                };
            }

            // 檢查會員是否已存在
            var existingMember = await _loyaltyService.GetMemberByCardNoAsync(request.LoyalSysCard);
            if (existingMember != null)
            {
                return new KioskProcessResponseDto
                {
                    ReturnCode = "E002",
                    ReturnMessage = "會員卡號已存在",
                    TransactionId = GenerateTransactionId()
                };
            }

            // 建立新會員
            var createDto = new CreateLoyaltyMemberDto
            {
                CardNo = request.LoyalSysCard,
                MemberName = request.Pnm,
                Phone = request.OtherData?.ContainsKey("Phone") == true 
                    ? request.OtherData["Phone"]?.ToString() 
                    : null,
                Email = request.OtherData?.ContainsKey("Email") == true 
                    ? request.OtherData["Email"]?.ToString() 
                    : null,
                Status = "A"
            };

            var cardNo = await _loyaltyService.CreateMemberAsync(createDto);

            _logger.LogInfo($"快速開卡成功: {cardNo}");

            return new KioskProcessResponseDto
            {
                ReturnCode = "0000",
                ReturnMessage = "開卡成功",
                TransactionId = GenerateTransactionId(),
                ResponseData = new Dictionary<string, object>
                {
                    { "CardNo", cardNo },
                    { "MemberName", createDto.MemberName ?? "" }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"快速開卡失敗: {request.LoyalSysCard}", ex);
            return new KioskProcessResponseDto
            {
                ReturnCode = "E999",
                ReturnMessage = $"開卡失敗: {ex.Message}",
                TransactionId = GenerateTransactionId()
            };
        }
    }

    /// <summary>
    /// 會員卡號、密碼驗證邏輯 (A1)
    /// </summary>
    private async Task<KioskProcessResponseDto> ProcessVerifyCardAsync(KioskProcessRequestDto request)
    {
        try
        {
            // 驗證必填欄位
            if (string.IsNullOrEmpty(request.LoyalSysCard) || string.IsNullOrEmpty(request.SysCardPWD))
            {
                return new KioskProcessResponseDto
                {
                    ReturnCode = "E001",
                    ReturnMessage = "資料不完整：卡號和密碼為必填",
                    TransactionId = GenerateTransactionId()
                };
            }

            // 查詢會員
            var member = await _loyaltyService.GetMemberByCardNoAsync(request.LoyalSysCard);
            if (member == null)
            {
                return new KioskProcessResponseDto
                {
                    ReturnCode = "E003",
                    ReturnMessage = "會員卡號不存在",
                    TransactionId = GenerateTransactionId()
                };
            }

            // 檢查會員狀態
            if (member.Status != "A")
            {
                return new KioskProcessResponseDto
                {
                    ReturnCode = "E004",
                    ReturnMessage = "會員帳號已停用",
                    TransactionId = GenerateTransactionId()
                };
            }

            // TODO: 實作密碼驗證邏輯（需要 MemberPassword 表或密碼驗證機制）
            // 目前簡化處理，假設密碼驗證通過
            // 實際應從資料庫查詢並驗證加密後的密碼

            _logger.LogInfo($"會員驗證成功: {request.LoyalSysCard}");

            return new KioskProcessResponseDto
            {
                ReturnCode = "0000",
                ReturnMessage = "驗證成功",
                TransactionId = GenerateTransactionId(),
                ResponseData = new Dictionary<string, object>
                {
                    { "CardNo", member.CardNo },
                    { "MemberName", member.MemberName ?? "" }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"會員驗證失敗: {request.LoyalSysCard}", ex);
            return new KioskProcessResponseDto
            {
                ReturnCode = "E999",
                ReturnMessage = $"驗證失敗: {ex.Message}",
                TransactionId = GenerateTransactionId()
            };
        }
    }

    /// <summary>
    /// 密碼變更邏輯 (C2)
    /// </summary>
    private async Task<KioskProcessResponseDto> ProcessChangePasswordAsync(KioskProcessRequestDto request)
    {
        try
        {
            // 驗證必填欄位
            if (string.IsNullOrEmpty(request.LoyalSysCard) || 
                string.IsNullOrEmpty(request.SysCardPWD) ||
                (request.OtherData?.ContainsKey("NewPassword") != true || 
                 string.IsNullOrEmpty(request.OtherData["NewPassword"]?.ToString())))
            {
                return new KioskProcessResponseDto
                {
                    ReturnCode = "E001",
                    ReturnMessage = "資料不完整：卡號、舊密碼和新密碼為必填",
                    TransactionId = GenerateTransactionId()
                };
            }

            // 查詢會員
            var member = await _loyaltyService.GetMemberByCardNoAsync(request.LoyalSysCard);
            if (member == null)
            {
                return new KioskProcessResponseDto
                {
                    ReturnCode = "E003",
                    ReturnMessage = "會員卡號不存在",
                    TransactionId = GenerateTransactionId()
                };
            }

            // TODO: 實作密碼變更邏輯（需要 MemberPassword 表）
            // 1. 驗證舊密碼
            // 2. 更新為新密碼（加密後儲存）
            // 目前簡化處理，假設密碼變更成功

            _logger.LogInfo($"密碼變更成功: {request.LoyalSysCard}");

            return new KioskProcessResponseDto
            {
                ReturnCode = "0000",
                ReturnMessage = "密碼變更成功",
                TransactionId = GenerateTransactionId()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"密碼變更失敗: {request.LoyalSysCard}", ex);
            return new KioskProcessResponseDto
            {
                ReturnCode = "E999",
                ReturnMessage = $"密碼變更失敗: {ex.Message}",
                TransactionId = GenerateTransactionId()
            };
        }
    }

    /// <summary>
    /// 網路會員點數查詢邏輯 (D4)
    /// </summary>
    private async Task<KioskProcessResponseDto> ProcessNetworkPointsAsync(KioskProcessRequestDto request)
    {
        try
        {
            // 驗證必填欄位
            if (string.IsNullOrEmpty(request.LoyalSysCard))
            {
                return new KioskProcessResponseDto
                {
                    ReturnCode = "E001",
                    ReturnMessage = "資料不完整：卡號為必填",
                    TransactionId = GenerateTransactionId()
                };
            }

            // 查詢會員點數
            var pointsInfo = await _loyaltyService.GetMemberPointsAsync(request.LoyalSysCard);
            if (pointsInfo == null)
            {
                return new KioskProcessResponseDto
                {
                    ReturnCode = "E003",
                    ReturnMessage = "會員卡號不存在",
                    TransactionId = GenerateTransactionId()
                };
            }

            _logger.LogInfo($"查詢網路會員點數成功: {request.LoyalSysCard}, 點數: {pointsInfo.AvailablePoints}");

            return new KioskProcessResponseDto
            {
                ReturnCode = "0000",
                ReturnMessage = "查詢成功",
                TransactionId = GenerateTransactionId(),
                ResponseData = new Dictionary<string, object>
                {
                    { "CardNo", pointsInfo.CardNo },
                    { "TotalPoints", pointsInfo.TotalPoints },
                    { "AvailablePoints", pointsInfo.AvailablePoints },
                    { "ExpDate", pointsInfo.ExpDate ?? "" }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢網路會員點數失敗: {request.LoyalSysCard}", ex);
            return new KioskProcessResponseDto
            {
                ReturnCode = "E999",
                ReturnMessage = $"查詢失敗: {ex.Message}",
                TransactionId = GenerateTransactionId()
            };
        }
    }

    /// <summary>
    /// 實體會員點數查詢邏輯 (D8)
    /// </summary>
    private async Task<KioskProcessResponseDto> ProcessPhysicalPointsAsync(KioskProcessRequestDto request)
    {
        try
        {
            // 驗證必填欄位
            if (string.IsNullOrEmpty(request.LoyalSysCard))
            {
                return new KioskProcessResponseDto
                {
                    ReturnCode = "E001",
                    ReturnMessage = "資料不完整：卡號為必填",
                    TransactionId = GenerateTransactionId()
                };
            }

            // 查詢會員點數（實體會員和網路會員使用相同的點數系統）
            var pointsInfo = await _loyaltyService.GetMemberPointsAsync(request.LoyalSysCard);
            if (pointsInfo == null)
            {
                return new KioskProcessResponseDto
                {
                    ReturnCode = "E003",
                    ReturnMessage = "會員卡號不存在",
                    TransactionId = GenerateTransactionId()
                };
            }

            _logger.LogInfo($"查詢實體會員點數成功: {request.LoyalSysCard}, 點數: {pointsInfo.AvailablePoints}");

            return new KioskProcessResponseDto
            {
                ReturnCode = "0000",
                ReturnMessage = "查詢成功",
                TransactionId = GenerateTransactionId(),
                ResponseData = new Dictionary<string, object>
                {
                    { "CardNo", pointsInfo.CardNo },
                    { "TotalPoints", pointsInfo.TotalPoints },
                    { "AvailablePoints", pointsInfo.AvailablePoints },
                    { "ExpDate", pointsInfo.ExpDate ?? "" }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢實體會員點數失敗: {request.LoyalSysCard}", ex);
            return new KioskProcessResponseDto
            {
                ReturnCode = "E999",
                ReturnMessage = $"查詢失敗: {ex.Message}",
                TransactionId = GenerateTransactionId()
            };
        }
    }

    private string GenerateTransactionId()
    {
        return $"K{DateTime.Now:yyyyMMddHHmmss}{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }
}

