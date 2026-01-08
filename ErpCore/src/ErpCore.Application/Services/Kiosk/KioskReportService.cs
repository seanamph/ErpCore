using Dapper;
using ErpCore.Application.DTOs.Kiosk;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Kiosk;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.Kiosk;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using System.Text.Json;

namespace ErpCore.Application.Services.Kiosk;

/// <summary>
/// Kiosk報表服務實作
/// </summary>
public class KioskReportService : BaseService, IKioskReportService
{
    private readonly IKioskTransactionRepository _transactionRepository;
    private readonly IDbConnectionFactory _connectionFactory;

    public KioskReportService(
        IKioskTransactionRepository transactionRepository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _transactionRepository = transactionRepository;
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<KioskTransactionDto>> GetTransactionsAsync(KioskTransactionQueryDto query)
    {
        try
        {
            var repositoryQuery = new KioskTransactionQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField ?? "TransactionDate",
                SortOrder = query.SortOrder,
                KioskId = query.Filters?.KioskId,
                FunctionCode = query.Filters?.FunctionCode,
                CardNumber = query.Filters?.CardNumber,
                MemberId = query.Filters?.MemberId,
                Status = query.Filters?.Status,
                StartDate = !string.IsNullOrEmpty(query.Filters?.StartDate) ? DateTime.Parse(query.Filters.StartDate) : null,
                EndDate = !string.IsNullOrEmpty(query.Filters?.EndDate) ? DateTime.Parse(query.Filters.EndDate) : null
            };

            var result = await _transactionRepository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => new KioskTransactionDto
            {
                Id = x.Id,
                TransactionId = x.TransactionId,
                KioskId = x.KioskId,
                FunctionCode = x.FunctionCode,
                FunctionCodeName = GetFunctionCodeName(x.FunctionCode),
                CardNumber = x.CardNumber,
                MemberId = x.MemberId,
                TransactionDate = x.TransactionDate,
                RequestData = x.RequestData,
                ResponseData = x.ResponseData,
                Status = x.Status,
                ReturnCode = x.ReturnCode,
                ErrorMessage = x.ErrorMessage,
                CreatedAt = x.CreatedAt
            }).ToList();

            return new PagedResult<KioskTransactionDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢Kiosk交易記錄失敗", ex);
            throw;
        }
    }

    public async Task<List<KioskStatisticsDto>> GetStatisticsAsync(KioskStatisticsQueryDto query)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();

            var groupByClause = query.GroupBy switch
            {
                "Kiosk" => "kt.KioskId",
                "FunctionCode" => "kt.FunctionCode",
                _ => "CAST(kt.TransactionDate AS DATE)"
            };

            var sql = $@"
                SELECT 
                    {groupByClause} AS GroupKey,
                    CASE 
                        WHEN @GroupBy = 'Date' THEN FORMAT(CAST(kt.TransactionDate AS DATE), 'yyyy-MM-dd')
                        WHEN @GroupBy = 'Kiosk' THEN kt.KioskId
                        WHEN @GroupBy = 'FunctionCode' THEN kt.FunctionCode
                        ELSE CAST(kt.TransactionDate AS DATE).ToString()
                    END AS GroupName,
                    COUNT(*) AS TotalCount,
                    SUM(CASE WHEN kt.Status = 'Success' THEN 1 ELSE 0 END) AS SuccessCount,
                    SUM(CASE WHEN kt.Status = 'Failed' THEN 1 ELSE 0 END) AS FailedCount,
                    CASE 
                        WHEN COUNT(*) > 0 THEN CAST(SUM(CASE WHEN kt.Status = 'Success' THEN 1 ELSE 0 END) * 100.0 / COUNT(*) AS DECIMAL(5,2))
                        ELSE 0
                    END AS SuccessRate
                FROM KioskTransactions kt
                WHERE 1 = 1
                    AND (@StartDate IS NULL OR CAST(kt.TransactionDate AS DATE) >= @StartDate)
                    AND (@EndDate IS NULL OR CAST(kt.TransactionDate AS DATE) <= @EndDate)
                    AND (@KioskId IS NULL OR kt.KioskId = @KioskId)
                    AND (@FunctionCode IS NULL OR kt.FunctionCode = @FunctionCode)
                GROUP BY {groupByClause}
                ORDER BY {groupByClause}";

            var parameters = new
            {
                GroupBy = query.GroupBy,
                StartDate = !string.IsNullOrEmpty(query.StartDate) ? DateTime.Parse(query.StartDate) : (DateTime?)null,
                EndDate = !string.IsNullOrEmpty(query.EndDate) ? DateTime.Parse(query.EndDate) : (DateTime?)null,
                KioskId = string.IsNullOrEmpty(query.KioskId) ? (string?)null : query.KioskId,
                FunctionCode = string.IsNullOrEmpty(query.FunctionCode) ? (string?)null : query.FunctionCode
            };

            var results = await connection.QueryAsync<KioskStatisticsDto>(sql, parameters);
            return results.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢Kiosk交易統計失敗", ex);
            throw;
        }
    }

    public async Task<List<KioskFunctionStatisticsDto>> GetFunctionStatisticsAsync(KioskStatisticsQueryDto query)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = @"
                SELECT 
                    kt.FunctionCode,
                    CASE kt.FunctionCode
                        WHEN 'O2' THEN '網路線上快速開卡'
                        WHEN 'A1' THEN '確認會員卡號、密碼'
                        WHEN 'C2' THEN '密碼變更'
                        WHEN 'D4' THEN '網路會員點數資訊'
                        WHEN 'D8' THEN '實體會員點數資訊'
                        ELSE kt.FunctionCode
                    END AS FunctionCodeName,
                    COUNT(*) AS TotalCount,
                    SUM(CASE WHEN kt.Status = 'Success' THEN 1 ELSE 0 END) AS SuccessCount,
                    SUM(CASE WHEN kt.Status = 'Failed' THEN 1 ELSE 0 END) AS FailedCount,
                    CASE 
                        WHEN COUNT(*) > 0 THEN CAST(SUM(CASE WHEN kt.Status = 'Success' THEN 1 ELSE 0 END) * 100.0 / COUNT(*) AS DECIMAL(5,2))
                        ELSE 0
                    END AS SuccessRate
                FROM KioskTransactions kt
                WHERE 1 = 1
                    AND (@StartDate IS NULL OR CAST(kt.TransactionDate AS DATE) >= @StartDate)
                    AND (@EndDate IS NULL OR CAST(kt.TransactionDate AS DATE) <= @EndDate)
                    AND (@KioskId IS NULL OR kt.KioskId = @KioskId)
                GROUP BY kt.FunctionCode
                ORDER BY TotalCount DESC";

            var parameters = new
            {
                StartDate = !string.IsNullOrEmpty(query.StartDate) ? DateTime.Parse(query.StartDate) : (DateTime?)null,
                EndDate = !string.IsNullOrEmpty(query.EndDate) ? DateTime.Parse(query.EndDate) : (DateTime?)null,
                KioskId = string.IsNullOrEmpty(query.KioskId) ? (string?)null : query.KioskId
            };

            var results = await connection.QueryAsync<KioskFunctionStatisticsDto>(sql, parameters);
            return results.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢Kiosk功能代碼統計失敗", ex);
            throw;
        }
    }

    public async Task<List<KioskErrorAnalysisDto>> GetErrorAnalysisAsync(KioskStatisticsQueryDto query)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = @"
                SELECT 
                    kt.ReturnCode,
                    kt.ErrorMessage,
                    COUNT(*) AS Count,
                    CAST(COUNT(*) * 100.0 / (
                        SELECT COUNT(*) FROM KioskTransactions 
                        WHERE Status = 'Failed'
                            AND (@StartDate IS NULL OR CAST(TransactionDate AS DATE) >= @StartDate)
                            AND (@EndDate IS NULL OR CAST(TransactionDate AS DATE) <= @EndDate)
                            AND (@KioskId IS NULL OR KioskId = @KioskId)
                            AND (@FunctionCode IS NULL OR FunctionCode = @FunctionCode)
                    ) AS DECIMAL(5,2)) AS Percentage
                FROM KioskTransactions kt
                WHERE kt.Status = 'Failed'
                    AND (@StartDate IS NULL OR CAST(kt.TransactionDate AS DATE) >= @StartDate)
                    AND (@EndDate IS NULL OR CAST(kt.TransactionDate AS DATE) <= @EndDate)
                    AND (@KioskId IS NULL OR kt.KioskId = @KioskId)
                    AND (@FunctionCode IS NULL OR kt.FunctionCode = @FunctionCode)
                GROUP BY kt.ReturnCode, kt.ErrorMessage
                ORDER BY Count DESC";

            var parameters = new
            {
                StartDate = !string.IsNullOrEmpty(query.StartDate) ? DateTime.Parse(query.StartDate) : (DateTime?)null,
                EndDate = !string.IsNullOrEmpty(query.EndDate) ? DateTime.Parse(query.EndDate) : (DateTime?)null,
                KioskId = string.IsNullOrEmpty(query.KioskId) ? (string?)null : query.KioskId,
                FunctionCode = string.IsNullOrEmpty(query.FunctionCode) ? (string?)null : query.FunctionCode
            };

            var results = await connection.QueryAsync<KioskErrorAnalysisDto>(sql, parameters);
            return results.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢Kiosk錯誤分析失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportReportAsync(KioskReportExportDto dto)
    {
        try
        {
            var query = new KioskTransactionQueryDto
            {
                PageIndex = 1,
                PageSize = int.MaxValue,
                Filters = dto.Filters
            };

            var transactions = await GetTransactionsAsync(query);

            if (dto.ExportType == "Excel")
            {
                return ExportToExcel(transactions.Items.ToList());
            }
            else
            {
                throw new NotImplementedException("PDF匯出功能待實作");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出Kiosk交易報表失敗", ex);
            throw;
        }
    }

    private byte[] ExportToExcel(List<KioskTransactionDto> items)
    {
        // 使用 EPPlus 或其他 Excel 庫實作
        // 這裡僅為範例，需根據實際需求實作
        throw new NotImplementedException("Excel匯出功能待實作（需安裝EPPlus套件）");
    }

    private string GetFunctionCodeName(string functionCode)
    {
        return functionCode switch
        {
            "O2" => "網路線上快速開卡",
            "A1" => "確認會員卡號、密碼",
            "C2" => "密碼變更",
            "D4" => "網路會員點數資訊",
            "D8" => "實體會員點數資訊",
            _ => functionCode
        };
    }
}

