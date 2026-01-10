using ErpCore.Application.DTOs.StoreFloor;
using ErpCore.Application.Services.Base;
using ErpCore.Infrastructure.Repositories.StoreFloor;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.StoreFloor;

/// <summary>
/// POS查詢服務實作 (SYS6A04-SYS6A19 - POS查詢作業)
/// </summary>
public class PosTerminalQueryService : BaseService, IPosTerminalQueryService
{
    private readonly IPosTerminalRepository _repository;

    public PosTerminalQueryService(
        IPosTerminalRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<PosTerminalDto>> QueryPosTerminalsAsync(PosTerminalQueryRequestDto request)
    {
        try
        {
            _logger.LogInfo("查詢POS終端列表（進階查詢）");

            var filters = request.Filters ?? new PosTerminalQueryFilters();

            var query = new PosTerminalQuery
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                SortField = request.SortField,
                SortOrder = request.SortOrder,
                PosTerminalId = filters.PosTerminalId,
                PosSystemId = filters.PosSystemId,
                ShopId = filters.ShopId,
                Status = filters.Status
            };

            var result = await _repository.QueryAsync(query);

            var dtos = new List<PosTerminalDto>();
            foreach (var x in result.Items)
            {
                dtos.Add(new PosTerminalDto
                {
                    PosTerminalId = x.PosTerminalId,
                    PosSystemId = x.PosSystemId,
                    TerminalCode = x.TerminalCode,
                    TerminalName = x.TerminalName,
                    ShopId = x.ShopId,
                    FloorId = x.FloorId,
                    TerminalType = x.TerminalType,
                    IpAddress = x.IpAddress,
                    Port = x.Port,
                    Config = x.Config,
                    Status = x.Status,
                    LastSyncDate = x.LastSyncDate,
                    CreatedBy = x.CreatedBy,
                    CreatedAt = x.CreatedAt,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedAt = x.UpdatedAt
                });
            }

            return new PagedResult<PosTerminalDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢POS終端列表失敗", ex);
            throw;
        }
    }

    public async Task<PosTerminalStatisticsDto> GetPosTerminalStatisticsAsync(PosTerminalStatisticsRequestDto request)
    {
        try
        {
            _logger.LogInfo("查詢POS終端統計資訊");

            var query = new PosTerminalQuery
            {
                PageIndex = 1,
                PageSize = int.MaxValue,
                PosSystemId = request.PosSystemId
            };

            var result = await _repository.QueryAsync(query);

            var statistics = new PosTerminalStatisticsDto
            {
                TotalTerminals = result.TotalCount,
                ActiveTerminals = result.Items.Count(t => t.Status == "A"),
                TerminalStatistics = new List<PosTerminalStatisticsItemDto>()
            };

            // 計算各終端的交易次數
            foreach (var x in result.Items)
            {
                var transactionCount = await _repository.GetTransactionCountAsync(x.PosTerminalId);
                statistics.TerminalStatistics.Add(new PosTerminalStatisticsItemDto
                {
                    PosTerminalId = x.PosTerminalId,
                    TerminalName = x.TerminalName,
                    ShopId = x.ShopId,
                    TransactionCount = transactionCount,
                    LastSyncDate = x.LastSyncDate
                });
            }

            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢POS終端統計資訊失敗", ex);
            throw;
        }
    }
}

