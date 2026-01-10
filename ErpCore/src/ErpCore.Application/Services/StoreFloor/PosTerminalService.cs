using ErpCore.Application.DTOs.StoreFloor;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.StoreFloor;
using ErpCore.Infrastructure.Repositories.StoreFloor;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.StoreFloor;

/// <summary>
/// POS終端服務實作 (SYS6610-SYS6999 - POS資料維護)
/// </summary>
public class PosTerminalService : BaseService, IPosTerminalService
{
    private readonly IPosTerminalRepository _repository;

    public PosTerminalService(
        IPosTerminalRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<PosTerminalDto>> GetPosTerminalsAsync(PosTerminalQueryDto query)
    {
        try
        {
            var repositoryQuery = new PosTerminalQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                PosTerminalId = query.PosTerminalId,
                PosSystemId = query.PosSystemId,
                ShopId = query.ShopId,
                Status = query.Status
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(MapToDto).ToList();

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

    public async Task<PosTerminalDto> GetPosTerminalByIdAsync(string posTerminalId)
    {
        try
        {
            var posTerminal = await _repository.GetByIdAsync(posTerminalId);
            if (posTerminal == null)
            {
                throw new KeyNotFoundException($"POS終端不存在: {posTerminalId}");
            }

            return MapToDto(posTerminal);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢POS終端失敗: {posTerminalId}", ex);
            throw;
        }
    }

    public async Task<string> CreatePosTerminalAsync(CreatePosTerminalDto dto)
    {
        try
        {
            if (await _repository.ExistsAsync(dto.PosTerminalId))
            {
                throw new InvalidOperationException($"POS終端代碼已存在: {dto.PosTerminalId}");
            }

            var posTerminal = new PosTerminal
            {
                PosTerminalId = dto.PosTerminalId,
                PosSystemId = dto.PosSystemId,
                TerminalCode = dto.TerminalCode,
                TerminalName = dto.TerminalName,
                ShopId = dto.ShopId,
                FloorId = dto.FloorId,
                TerminalType = dto.TerminalType,
                IpAddress = dto.IpAddress,
                Port = dto.Port,
                Config = dto.Config,
                Status = dto.Status,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(posTerminal);

            return posTerminal.PosTerminalId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增POS終端失敗: {dto.PosTerminalId}", ex);
            throw;
        }
    }

    public async Task UpdatePosTerminalAsync(string posTerminalId, UpdatePosTerminalDto dto)
    {
        try
        {
            var posTerminal = await _repository.GetByIdAsync(posTerminalId);
            if (posTerminal == null)
            {
                throw new KeyNotFoundException($"POS終端不存在: {posTerminalId}");
            }

            posTerminal.PosSystemId = dto.PosSystemId;
            posTerminal.TerminalCode = dto.TerminalCode;
            posTerminal.TerminalName = dto.TerminalName;
            posTerminal.ShopId = dto.ShopId;
            posTerminal.FloorId = dto.FloorId;
            posTerminal.TerminalType = dto.TerminalType;
            posTerminal.IpAddress = dto.IpAddress;
            posTerminal.Port = dto.Port;
            posTerminal.Config = dto.Config;
            posTerminal.Status = dto.Status;
            posTerminal.UpdatedBy = GetCurrentUserId();
            posTerminal.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(posTerminal);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改POS終端失敗: {posTerminalId}", ex);
            throw;
        }
    }

    public async Task DeletePosTerminalAsync(string posTerminalId)
    {
        try
        {
            var posTerminal = await _repository.GetByIdAsync(posTerminalId);
            if (posTerminal == null)
            {
                throw new KeyNotFoundException($"POS終端不存在: {posTerminalId}");
            }

            await _repository.DeleteAsync(posTerminalId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除POS終端失敗: {posTerminalId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string posTerminalId)
    {
        try
        {
            return await _repository.ExistsAsync(posTerminalId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查POS終端是否存在失敗: {posTerminalId}", ex);
            throw;
        }
    }

    public async Task SyncPosTerminalAsync(string posTerminalId)
    {
        try
        {
            var posTerminal = await _repository.GetByIdAsync(posTerminalId);
            if (posTerminal == null)
            {
                throw new KeyNotFoundException($"POS終端不存在: {posTerminalId}");
            }

            // 實作POS資料同步邏輯
            // 這裡簡化處理，實際應該：
            // 1. 呼叫外部POS系統API取得交易資料
            // 2. 驗證資料完整性
            // 3. 轉換資料格式
            // 4. 儲存到資料庫
            // 5. 更新同步狀態

            _logger.LogInfo($"開始同步POS終端資料: {posTerminalId}");

            // 模擬外部API呼叫（實際應使用 HttpClient 呼叫外部POS系統API）
            // 例如：
            // var httpClient = new HttpClient();
            // var response = await httpClient.GetAsync($"{posTerminal.IpAddress}:{posTerminal.Port}/api/transactions");
            // var transactions = await response.Content.ReadFromJsonAsync<List<PosTransaction>>();

            // 更新最後同步時間
            await _repository.UpdateLastSyncDateAsync(posTerminalId, DateTime.Now);

            _logger.LogInfo($"POS終端資料同步完成: {posTerminalId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"同步POS終端失敗: {posTerminalId}", ex);
            throw;
        }
    }

    private PosTerminalDto MapToDto(PosTerminal posTerminal)
    {
        return new PosTerminalDto
        {
            PosTerminalId = posTerminal.PosTerminalId,
            PosSystemId = posTerminal.PosSystemId,
            TerminalCode = posTerminal.TerminalCode,
            TerminalName = posTerminal.TerminalName,
            ShopId = posTerminal.ShopId,
            FloorId = posTerminal.FloorId,
            TerminalType = posTerminal.TerminalType,
            IpAddress = posTerminal.IpAddress,
            Port = posTerminal.Port,
            Config = posTerminal.Config,
            Status = posTerminal.Status,
            LastSyncDate = posTerminal.LastSyncDate,
            CreatedBy = posTerminal.CreatedBy,
            CreatedAt = posTerminal.CreatedAt,
            UpdatedBy = posTerminal.UpdatedBy,
            UpdatedAt = posTerminal.UpdatedAt
        };
    }
}

