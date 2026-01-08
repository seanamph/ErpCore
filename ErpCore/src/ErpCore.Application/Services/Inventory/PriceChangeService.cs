using ErpCore.Application.DTOs.Inventory;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Inventory;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.Inventory;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Inventory;

/// <summary>
/// 變價單服務實作
/// </summary>
public class PriceChangeService : BaseService, IPriceChangeService
{
    private readonly IPriceChangeRepository _repository;
    private readonly IDbConnectionFactory _connectionFactory;

    public PriceChangeService(
        IPriceChangeRepository repository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<PriceChangeDto>> GetPriceChangesAsync(PriceChangeQueryDto query)
    {
        try
        {
            var repositoryQuery = new PriceChangeQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                PriceChangeId = query.PriceChangeId,
                PriceChangeType = query.PriceChangeType,
                SupplierId = query.SupplierId,
                LogoId = query.LogoId,
                Status = query.Status,
                ApplyDateFrom = query.ApplyDateFrom,
                ApplyDateTo = query.ApplyDateTo,
                StartDateFrom = query.StartDateFrom,
                StartDateTo = query.StartDateTo
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => new PriceChangeDto
            {
                PriceChangeId = x.PriceChangeId,
                PriceChangeType = x.PriceChangeType,
                PriceChangeTypeName = x.PriceChangeType == "1" ? "進價" : "售價",
                SupplierId = x.SupplierId,
                LogoId = x.LogoId,
                ApplyEmpId = x.ApplyEmpId,
                ApplyOrgId = x.ApplyOrgId,
                ApplyDate = x.ApplyDate,
                StartDate = x.StartDate,
                ApproveEmpId = x.ApproveEmpId,
                ApproveDate = x.ApproveDate,
                ConfirmEmpId = x.ConfirmEmpId,
                ConfirmDate = x.ConfirmDate,
                Status = x.Status,
                StatusName = GetStatusName(x.Status),
                TotalAmount = x.TotalAmount,
                Notes = x.Notes,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return new PagedResult<PriceChangeDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢變價單列表失敗", ex);
            throw;
        }
    }

    public async Task<PriceChangeDetailDto> GetPriceChangeByIdAsync(string priceChangeId, string priceChangeType)
    {
        try
        {
            var master = await _repository.GetByIdAsync(priceChangeId, priceChangeType);
            if (master == null)
            {
                throw new InvalidOperationException($"變價單不存在: {priceChangeId}/{priceChangeType}");
            }

            var details = await _repository.GetDetailsAsync(priceChangeId, priceChangeType);

            var dto = new PriceChangeDetailDto
            {
                PriceChangeId = master.PriceChangeId,
                PriceChangeType = master.PriceChangeType,
                PriceChangeTypeName = master.PriceChangeType == "1" ? "進價" : "售價",
                SupplierId = master.SupplierId,
                LogoId = master.LogoId,
                ApplyEmpId = master.ApplyEmpId,
                ApplyOrgId = master.ApplyOrgId,
                ApplyDate = master.ApplyDate,
                StartDate = master.StartDate,
                ApproveEmpId = master.ApproveEmpId,
                ApproveDate = master.ApproveDate,
                ConfirmEmpId = master.ConfirmEmpId,
                ConfirmDate = master.ConfirmDate,
                Status = master.Status,
                StatusName = GetStatusName(master.Status),
                TotalAmount = master.TotalAmount,
                Notes = master.Notes,
                CreatedBy = master.CreatedBy,
                CreatedAt = master.CreatedAt,
                UpdatedBy = master.UpdatedBy,
                UpdatedAt = master.UpdatedAt,
                Details = details.Select(d => new PriceChangeDetailItemDto
                {
                    Id = d.Id,
                    LineNum = d.LineNum,
                    GoodsId = d.GoodsId,
                    BeforePrice = d.BeforePrice,
                    AfterPrice = d.AfterPrice,
                    ChangeQty = d.ChangeQty,
                    Notes = d.Notes
                }).ToList()
            };

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢變價單失敗: {priceChangeId}/{priceChangeType}", ex);
            throw;
        }
    }

    public async Task<string> CreatePriceChangeAsync(CreatePriceChangeDto dto)
    {
        try
        {
            // 驗證資料
            ValidateCreateDto(dto);

            // 產生變價單號
            var priceChangeId = GeneratePriceChangeId(dto.PriceChangeType);

            // 計算總金額
            var totalAmount = dto.Details.Sum(d => d.AfterPrice * (d.ChangeQty > 0 ? d.ChangeQty : 1));

            var master = new PriceChangeMaster
            {
                PriceChangeId = priceChangeId,
                PriceChangeType = dto.PriceChangeType,
                SupplierId = dto.SupplierId,
                LogoId = dto.LogoId,
                ApplyEmpId = dto.ApplyEmpId ?? GetCurrentUserId(),
                ApplyOrgId = dto.ApplyOrgId ?? GetCurrentOrgId(),
                ApplyDate = dto.ApplyDate ?? DateTime.Now,
                StartDate = dto.StartDate,
                Status = "1", // 已申請
                TotalAmount = totalAmount,
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                UpdatedBy = GetCurrentUserId()
            };

            var details = dto.Details.Select((d, index) => new PriceChangeDetail
            {
                PriceChangeId = priceChangeId,
                PriceChangeType = dto.PriceChangeType,
                LineNum = d.LineNum > 0 ? d.LineNum : index + 1,
                GoodsId = d.GoodsId,
                BeforePrice = d.BeforePrice,
                AfterPrice = d.AfterPrice,
                ChangeQty = d.ChangeQty,
                Notes = d.Notes,
                CreatedBy = GetCurrentUserId()
            }).ToList();

            await _repository.CreateAsync(master, details);
            return priceChangeId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增變價單失敗", ex);
            throw;
        }
    }

    public async Task UpdatePriceChangeAsync(string priceChangeId, string priceChangeType, UpdatePriceChangeDto dto)
    {
        try
        {
            // 檢查是否存在
            var master = await _repository.GetByIdAsync(priceChangeId, priceChangeType);
            if (master == null)
            {
                throw new InvalidOperationException($"變價單不存在: {priceChangeId}/{priceChangeType}");
            }

            // 只有狀態為「已申請」的變價單可以修改
            if (master.Status != "1")
            {
                throw new InvalidOperationException($"只有狀態為「已申請」的變價單可以修改");
            }

            // 計算總金額
            var totalAmount = dto.Details.Sum(d => d.AfterPrice * (d.ChangeQty > 0 ? d.ChangeQty : 1));

            master.SupplierId = dto.SupplierId;
            master.LogoId = dto.LogoId;
            master.ApplyOrgId = dto.ApplyOrgId;
            master.ApplyDate = dto.ApplyDate ?? master.ApplyDate;
            master.StartDate = dto.StartDate;
            master.TotalAmount = totalAmount;
            master.Notes = dto.Notes;
            master.UpdatedBy = GetCurrentUserId();

            var details = dto.Details.Select((d, index) => new PriceChangeDetail
            {
                PriceChangeId = priceChangeId,
                PriceChangeType = priceChangeType,
                LineNum = d.LineNum > 0 ? d.LineNum : index + 1,
                GoodsId = d.GoodsId,
                BeforePrice = d.BeforePrice,
                AfterPrice = d.AfterPrice,
                ChangeQty = d.ChangeQty,
                Notes = d.Notes,
                CreatedBy = GetCurrentUserId()
            }).ToList();

            await _repository.UpdateAsync(master, details);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改變價單失敗: {priceChangeId}/{priceChangeType}", ex);
            throw;
        }
    }

    public async Task DeletePriceChangeAsync(string priceChangeId, string priceChangeType)
    {
        try
        {
            // 檢查是否存在
            var master = await _repository.GetByIdAsync(priceChangeId, priceChangeType);
            if (master == null)
            {
                throw new InvalidOperationException($"變價單不存在: {priceChangeId}/{priceChangeType}");
            }

            // 只有狀態為「已申請」的變價單可以刪除
            if (master.Status != "1")
            {
                throw new InvalidOperationException($"只有狀態為「已申請」的變價單可以刪除");
            }

            await _repository.DeleteAsync(priceChangeId, priceChangeType);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除變價單失敗: {priceChangeId}/{priceChangeType}", ex);
            throw;
        }
    }

    public async Task ApprovePriceChangeAsync(string priceChangeId, string priceChangeType, ApprovePriceChangeDto dto)
    {
        try
        {
            var master = await _repository.GetByIdAsync(priceChangeId, priceChangeType);
            if (master == null)
            {
                throw new InvalidOperationException($"變價單不存在: {priceChangeId}/{priceChangeType}");
            }

            // 只有狀態為「已申請」的變價單可以審核
            if (master.Status != "1")
            {
                throw new InvalidOperationException($"只有狀態為「已申請」的變價單可以審核");
            }

            await _repository.UpdateStatusAsync(priceChangeId, priceChangeType, "2", GetCurrentUserId(), dto.ApproveDate);
        }
        catch (Exception ex)
        {
            _logger.LogError($"審核變價單失敗: {priceChangeId}/{priceChangeType}", ex);
            throw;
        }
    }

    public async Task ConfirmPriceChangeAsync(string priceChangeId, string priceChangeType, ConfirmPriceChangeDto dto)
    {
        try
        {
            var master = await _repository.GetByIdAsync(priceChangeId, priceChangeType);
            if (master == null)
            {
                throw new InvalidOperationException($"變價單不存在: {priceChangeId}/{priceChangeType}");
            }

            // 只有狀態為「已審核」的變價單可以確認
            if (master.Status != "2")
            {
                throw new InvalidOperationException($"只有狀態為「已審核」的變價單可以確認");
            }

            // 更新變價單狀態
            await _repository.UpdateStatusAsync(priceChangeId, priceChangeType, "10", GetCurrentUserId(), dto.ConfirmDate);

            // 更新商品主檔的價格
            // 根據 PriceChangeType 更新商品的進價或售價
            var details = await _repository.GetDetailsAsync(priceChangeId, priceChangeType);
            
            // 更新商品價格
            foreach (var detail in details)
            {
                if (priceChangeType == "1")
                {
                    // 更新進價
                    await _repository.UpdateProductPurchasePriceAsync(detail.GoodsId, detail.AfterPrice, GetCurrentUserId());
                }
                else if (priceChangeType == "2")
                {
                    // 更新售價
                    await _repository.UpdateProductSalePriceAsync(detail.GoodsId, detail.AfterPrice, GetCurrentUserId());
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"確認變價單失敗: {priceChangeId}/{priceChangeType}", ex);
            throw;
        }
    }

    public async Task CancelPriceChangeAsync(string priceChangeId, string priceChangeType)
    {
        try
        {
            var master = await _repository.GetByIdAsync(priceChangeId, priceChangeType);
            if (master == null)
            {
                throw new InvalidOperationException($"變價單不存在: {priceChangeId}/{priceChangeType}");
            }

            // 已作廢的變價單不能再作廢
            if (master.Status == "9")
            {
                throw new InvalidOperationException($"變價單已作廢");
            }

            await _repository.UpdateStatusAsync(priceChangeId, priceChangeType, "9", null, null);
        }
        catch (Exception ex)
        {
            _logger.LogError($"作廢變價單失敗: {priceChangeId}/{priceChangeType}", ex);
            throw;
        }
    }

    private string GetStatusName(string status)
    {
        return status switch
        {
            "1" => "已申請",
            "2" => "已審核",
            "9" => "已作廢",
            "10" => "已確認",
            _ => "未知"
        };
    }

    private string GeneratePriceChangeId(string priceChangeType)
    {
        // 產生變價單號：PC + 類型(1或2) + YYYYMMDD + 序號(3碼)
        var prefix = priceChangeType == "1" ? "PC1" : "PC2";
        var dateStr = DateTime.Now.ToString("yyyyMMdd");
        var sequence = DateTime.Now.ToString("HHmmss"); // 簡化版，實際應該從資料庫取得序號
        return $"{prefix}{dateStr}{sequence}";
    }

    private void ValidateCreateDto(CreatePriceChangeDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.PriceChangeType))
        {
            throw new ArgumentException("變價類型不能為空");
        }

        if (dto.PriceChangeType != "1" && dto.PriceChangeType != "2")
        {
            throw new ArgumentException("變價類型必須為 1(進價) 或 2(售價)");
        }

        if (dto.Details == null || dto.Details.Count == 0)
        {
            throw new ArgumentException("變價明細不能為空");
        }

        foreach (var detail in dto.Details)
        {
            if (string.IsNullOrWhiteSpace(detail.GoodsId))
            {
                throw new ArgumentException("商品編號不能為空");
            }

            if (detail.AfterPrice <= 0)
            {
                throw new ArgumentException("調整後單價必須大於0");
            }
        }
    }
}

