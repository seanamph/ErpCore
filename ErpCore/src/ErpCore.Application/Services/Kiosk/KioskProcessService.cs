using ErpCore.Application.DTOs.Kiosk;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Kiosk;
using ErpCore.Infrastructure.Repositories.Kiosk;
using ErpCore.Shared.Logging;
using System.Text.Json;

namespace ErpCore.Application.Services.Kiosk;

/// <summary>
/// Kiosk處理服務實作
/// </summary>
public class KioskProcessService : BaseService, IKioskProcessService
{
    private readonly IKioskTransactionRepository _transactionRepository;

    public KioskProcessService(
        IKioskTransactionRepository transactionRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _transactionRepository = transactionRepository;
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

    private async Task<KioskProcessResponseDto> ProcessQuickCardAsync(KioskProcessRequestDto request)
    {
        // TODO: 實作快速開卡邏輯
        await Task.CompletedTask;
        return new KioskProcessResponseDto
        {
            ReturnCode = "0000",
            ReturnMessage = "開卡成功",
            TransactionId = GenerateTransactionId()
        };
    }

    private async Task<KioskProcessResponseDto> ProcessVerifyCardAsync(KioskProcessRequestDto request)
    {
        // TODO: 實作會員卡號、密碼驗證邏輯
        await Task.CompletedTask;
        return new KioskProcessResponseDto
        {
            ReturnCode = "0000",
            ReturnMessage = "驗證成功",
            TransactionId = GenerateTransactionId()
        };
    }

    private async Task<KioskProcessResponseDto> ProcessChangePasswordAsync(KioskProcessRequestDto request)
    {
        // TODO: 實作密碼變更邏輯
        await Task.CompletedTask;
        return new KioskProcessResponseDto
        {
            ReturnCode = "0000",
            ReturnMessage = "密碼變更成功",
            TransactionId = GenerateTransactionId()
        };
    }

    private async Task<KioskProcessResponseDto> ProcessNetworkPointsAsync(KioskProcessRequestDto request)
    {
        // TODO: 實作網路會員點數查詢邏輯
        await Task.CompletedTask;
        return new KioskProcessResponseDto
        {
            ReturnCode = "0000",
            ReturnMessage = "查詢成功",
            TransactionId = GenerateTransactionId(),
            ResponseData = new Dictionary<string, object> { { "Points", 0 } }
        };
    }

    private async Task<KioskProcessResponseDto> ProcessPhysicalPointsAsync(KioskProcessRequestDto request)
    {
        // TODO: 實作實體會員點數查詢邏輯
        await Task.CompletedTask;
        return new KioskProcessResponseDto
        {
            ReturnCode = "0000",
            ReturnMessage = "查詢成功",
            TransactionId = GenerateTransactionId(),
            ResponseData = new Dictionary<string, object> { { "Points", 0 } }
        };
    }

    private string GenerateTransactionId()
    {
        return $"K{DateTime.Now:yyyyMMddHHmmss}{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }
}

