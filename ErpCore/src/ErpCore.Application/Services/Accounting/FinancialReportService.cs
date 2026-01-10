using ErpCore.Application.DTOs.Accounting;
using ErpCore.Application.Services.Base;
using ErpCore.Infrastructure.Repositories.Accounting;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using Dapper;

namespace ErpCore.Application.Services.Accounting;

/// <summary>
/// 財務報表服務實作 (SYSN510-SYSN540)
/// </summary>
public class FinancialReportService : BaseService, IFinancialReportService
{
    private readonly IFinancialTransactionRepository _transactionRepository;
    private readonly IAccountSubjectRepository _accountSubjectRepository;
    private readonly ExportHelper _exportHelper;

    public FinancialReportService(
        IFinancialTransactionRepository transactionRepository,
        IAccountSubjectRepository accountSubjectRepository,
        ExportHelper exportHelper,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _transactionRepository = transactionRepository;
        _accountSubjectRepository = accountSubjectRepository;
        _exportHelper = exportHelper;
    }

    public async Task<PagedResult<FinancialReportDto>> GetFinancialReportsAsync(FinancialReportQueryDto query)
    {
        try
        {
            _logger.LogInfo($"查詢財務報表: {query.ReportType}");

            // 根據報表類型查詢財務交易資料
            var transactionQuery = new FinancialTransactionQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TxnDateFrom = query.DateFrom,
                TxnDateTo = query.DateTo,
                StypeId = query.StypeId
            };

            var transactions = await _transactionRepository.QueryAsync(transactionQuery);

            // 轉換為報表資料
            var reportData = new List<FinancialReportDto>();
            foreach (var transaction in transactions.Items)
            {
                var accountSubject = await _accountSubjectRepository.GetByIdAsync(transaction.StypeId ?? string.Empty);
                
                reportData.Add(new FinancialReportDto
                {
                    ReportType = query.ReportType,
                    ReportDate = transaction.TxnDate,
                    StypeId = transaction.StypeId ?? string.Empty,
                    StypeName = accountSubject?.StypeName,
                    DebitAmount = transaction.DebitAmount,
                    CreditAmount = transaction.CreditAmount,
                    Balance = transaction.DebitAmount - transaction.CreditAmount
                });
            }

            return new PagedResult<FinancialReportDto>
            {
                Items = reportData,
                TotalCount = transactions.TotalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalPages = transactions.TotalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢財務報表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportFinancialReportsAsync(ExportFinancialReportDto dto)
    {
        try
        {
            _logger.LogInfo($"匯出財務報表: {dto.Query.ReportType}, 格式: {dto.ExportFormat}");

            // 查詢所有資料（不分頁）
            var allDataQuery = new FinancialReportQueryDto
            {
                ReportType = dto.Query.ReportType,
                DateFrom = dto.Query.DateFrom,
                DateTo = dto.Query.DateTo,
                StypeId = dto.Query.StypeId,
                SiteId = dto.Query.SiteId,
                PageIndex = 1,
                PageSize = int.MaxValue
            };

            var result = await GetFinancialReportsAsync(allDataQuery);

            // 定義匯出欄位
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "ReportDate", DisplayName = "報表日期", DataType = ExportDataType.Date },
                new ExportColumn { PropertyName = "StypeId", DisplayName = "科目代號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "StypeName", DisplayName = "科目名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "DebitAmount", DisplayName = "借方金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "CreditAmount", DisplayName = "貸方金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "Balance", DisplayName = "餘額", DataType = ExportDataType.Decimal }
            };

            // 匯出
            if (dto.ExportFormat.ToUpper() == "PDF")
            {
                return _exportHelper.ExportToPdf(result.Items, columns, $"財務報表 - {dto.Query.ReportType}");
            }
            else
            {
                return _exportHelper.ExportToExcel(result.Items, columns, "財務報表", $"財務報表 - {dto.Query.ReportType}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出財務報表失敗", ex);
            throw;
        }
    }
}

