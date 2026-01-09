using ErpCore.Application.DTOs.CustomerCustomJgjn;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.CustomerCustomJgjn;
using ErpCore.Infrastructure.Repositories.CustomerCustomJgjn;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.CustomerCustomJgjn;

/// <summary>
/// JGJN發票服務實作
/// </summary>
public class JgjNInvoiceService : BaseService, IJgjNInvoiceService
{
    private readonly IJgjNInvoiceRepository _repository;

    public JgjNInvoiceService(
        IJgjNInvoiceRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<JgjNInvoiceDto>> GetJgjNInvoiceListAsync(JgjNInvoiceQueryDto query)
    {
        try
        {
            var repositoryQuery = new JgjNInvoiceQuery
            {
                CustomerId = query.CustomerId,
                StartDate = query.StartDate,
                EndDate = query.EndDate,
                Status = query.Status,
                PrintStatus = query.PrintStatus,
                Keyword = query.Keyword,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(MapToDto).ToList();

            return new PagedResult<JgjNInvoiceDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢JGJN發票列表失敗", ex);
            throw;
        }
    }

    public async Task<JgjNInvoiceDto?> GetJgjNInvoiceByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢JGJN發票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<JgjNInvoiceDto?> GetJgjNInvoiceByInvoiceIdAsync(string invoiceId)
    {
        try
        {
            var entity = await _repository.GetByInvoiceIdAsync(invoiceId);
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢JGJN發票失敗: {invoiceId}", ex);
            throw;
        }
    }

    public async Task<long> CreateJgjNInvoiceAsync(CreateJgjNInvoiceDto dto)
    {
        try
        {
            // 檢查發票代碼是否已存在
            var existing = await _repository.GetByInvoiceIdAsync(dto.InvoiceId);
            if (existing != null)
            {
                throw new InvalidOperationException($"發票代碼已存在: {dto.InvoiceId}");
            }

            var entity = new JgjNInvoice
            {
                InvoiceId = dto.InvoiceId,
                InvoiceNo = dto.InvoiceNo,
                InvoiceDate = dto.InvoiceDate,
                CustomerId = dto.CustomerId,
                Amount = dto.Amount,
                Currency = dto.Currency,
                Status = dto.Status,
                Memo = dto.Memo,
                CreatedBy = _userContext.UserId,
                CreatedAt = DateTime.Now,
                UpdatedBy = _userContext.UserId,
                UpdatedAt = DateTime.Now
            };

            var tKey = await _repository.CreateAsync(entity);
            _logger.LogInfo($"新增JGJN發票成功: {dto.InvoiceId}");
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增JGJN發票失敗", ex);
            throw;
        }
    }

    public async Task UpdateJgjNInvoiceAsync(long tKey, UpdateJgjNInvoiceDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"資料不存在: {tKey}");
            }

            entity.InvoiceNo = dto.InvoiceNo;
            entity.InvoiceDate = dto.InvoiceDate;
            entity.CustomerId = dto.CustomerId;
            entity.Amount = dto.Amount;
            entity.Currency = dto.Currency;
            entity.Status = dto.Status;
            entity.Memo = dto.Memo;
            entity.UpdatedBy = _userContext.UserId;
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"修改JGJN發票成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改JGJN發票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteJgjNInvoiceAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"資料不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
            _logger.LogInfo($"刪除JGJN發票成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除JGJN發票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task PrintJgjNInvoiceAsync(long tKey, PrintJgjNInvoiceDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"資料不存在: {tKey}");
            }

            // 更新列印狀態
            await _repository.UpdatePrintStatusAsync(tKey, "SUCCESS", DateTime.Now, dto.FilePath);
            _logger.LogInfo($"列印JGJN發票成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"列印JGJN發票失敗: {tKey}", ex);
            throw;
        }
    }

    private JgjNInvoiceDto MapToDto(JgjNInvoice entity)
    {
        return new JgjNInvoiceDto
        {
            TKey = entity.TKey,
            InvoiceId = entity.InvoiceId,
            InvoiceNo = entity.InvoiceNo,
            InvoiceDate = entity.InvoiceDate,
            CustomerId = entity.CustomerId,
            Amount = entity.Amount,
            Currency = entity.Currency,
            Status = entity.Status,
            PrintStatus = entity.PrintStatus,
            PrintDate = entity.PrintDate,
            FilePath = entity.FilePath,
            Memo = entity.Memo,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

