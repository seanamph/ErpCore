using ErpCore.Application.DTOs.Certificate;
using ErpCore.Application.Services.Base;
using ErpCore.Infrastructure.Repositories.Certificate;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Certificate;

/// <summary>
/// 憑證報表服務實作 (SYSK310-SYSK500)
/// </summary>
public class CertificateReportService : BaseService, ICertificateReportService
{
    private readonly IVoucherRepository _voucherRepository;
    private readonly IVoucherDetailRepository _voucherDetailRepository;

    public CertificateReportService(
        IVoucherRepository voucherRepository,
        IVoucherDetailRepository voucherDetailRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _voucherRepository = voucherRepository;
        _voucherDetailRepository = voucherDetailRepository;
    }

    /// <summary>
    /// 憑證明細報表查詢
    /// </summary>
    public async Task<PagedResult<VoucherDto>> GetVoucherDetailReportAsync(VoucherReportQueryDto query)
    {
        try
        {
            var entities = await _voucherRepository.GetPagedAsync(
                query.PageIndex,
                query.PageSize,
                null,
                query.VoucherType,
                query.ShopId,
                query.Status,
                query.VoucherDateFrom,
                query.VoucherDateTo);

            var totalCount = await _voucherRepository.GetCountAsync(
                null,
                query.VoucherType,
                query.ShopId,
                query.Status,
                query.VoucherDateFrom,
                query.VoucherDateTo);

            var items = new List<VoucherDto>();
            foreach (var entity in entities)
            {
                var dto = MapToDto(entity);
                
                // 載入明細
                var details = await _voucherDetailRepository.GetByVoucherIdAsync(entity.VoucherId);
                dto.Details = details.Select(MapDetailToDto).ToList();
                
                items.Add(dto);
            }

            return new PagedResult<VoucherDto>
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
            _logger.LogError("查詢憑證明細報表失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 憑證統計報表查詢
    /// </summary>
    public async Task<VoucherStatisticsReportDto> GetVoucherStatisticsReportAsync(VoucherReportQueryDto query)
    {
        try
        {
            // 取得所有符合條件的憑證
            var allVouchers = await _voucherRepository.GetPagedAsync(
                1,
                10000, // 取得大量資料用於統計
                null,
                query.VoucherType,
                query.ShopId,
                query.Status,
                query.VoucherDateFrom,
                query.VoucherDateTo);

            // 計算統計摘要
            var summary = new VoucherStatisticsSummaryDto
            {
                TotalCount = allVouchers.Count(),
                TotalAmount = allVouchers.Sum(v => v.TotalAmount),
                TotalDebitAmount = allVouchers.Sum(v => v.TotalDebitAmount),
                TotalCreditAmount = allVouchers.Sum(v => v.TotalCreditAmount)
            };

            // 依憑證類型分組統計
            var groups = allVouchers
                .GroupBy(v => v.VoucherType)
                .Select(g => new VoucherStatisticsGroupDto
                {
                    GroupKey = g.Key,
                    GroupName = GetVoucherTypeName(g.Key),
                    Count = g.Count(),
                    TotalAmount = g.Sum(v => v.TotalAmount)
                })
                .ToList();

            return new VoucherStatisticsReportDto
            {
                Summary = summary,
                Groups = groups
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢憑證統計報表失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 憑證分析報表查詢
    /// </summary>
    public async Task<PagedResult<VoucherDto>> GetVoucherAnalysisReportAsync(VoucherReportQueryDto query)
    {
        try
        {
            // 分析報表與明細報表類似，但可以加入更多分析邏輯
            // 例如：依日期區間、狀態、類型等進行分析
            return await GetVoucherDetailReportAsync(query);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢憑證分析報表失敗", ex);
            throw;
        }
    }

    private VoucherDto MapToDto(Domain.Entities.Certificate.Voucher entity)
    {
        return new VoucherDto
        {
            TKey = entity.TKey,
            VoucherId = entity.VoucherId,
            VoucherDate = entity.VoucherDate,
            VoucherType = entity.VoucherType,
            VoucherTypeName = GetVoucherTypeName(entity.VoucherType),
            ShopId = entity.ShopId,
            Status = entity.Status,
            StatusName = GetStatusName(entity.Status),
            ApplyUserId = entity.ApplyUserId,
            ApplyDate = entity.ApplyDate,
            ApproveUserId = entity.ApproveUserId,
            ApproveDate = entity.ApproveDate,
            TotalAmount = entity.TotalAmount,
            TotalDebitAmount = entity.TotalDebitAmount,
            TotalCreditAmount = entity.TotalCreditAmount,
            Memo = entity.Memo,
            SiteId = entity.SiteId,
            OrgId = entity.OrgId,
            CurrencyId = entity.CurrencyId,
            ExchangeRate = entity.ExchangeRate,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private VoucherDetailDto MapDetailToDto(Domain.Entities.Certificate.VoucherDetail entity)
    {
        return new VoucherDetailDto
        {
            TKey = entity.TKey,
            VoucherId = entity.VoucherId,
            LineNum = entity.LineNum,
            AccountId = entity.AccountId,
            DebitAmount = entity.DebitAmount,
            CreditAmount = entity.CreditAmount,
            Description = entity.Description,
            Memo = entity.Memo,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private string GetVoucherTypeName(string voucherType)
    {
        return voucherType switch
        {
            "V" => "傳票",
            "R" => "收據",
            "I" => "發票",
            _ => voucherType
        };
    }

    private string GetStatusName(string status)
    {
        return status switch
        {
            "D" => "草稿",
            "S" => "已送出",
            "A" => "已審核",
            "X" => "已取消",
            "C" => "已結案",
            _ => status
        };
    }
}

