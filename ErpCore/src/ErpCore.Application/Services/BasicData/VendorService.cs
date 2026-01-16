using ErpCore.Application.DTOs.BasicData;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.BasicData;
using ErpCore.Infrastructure.Repositories.BasicData;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.BasicData;

/// <summary>
/// 廠商服務實作
/// </summary>
public class VendorService : BaseService, IVendorService
{
    private readonly IVendorRepository _repository;

    public VendorService(
        IVendorRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<VendorDto>> GetVendorsAsync(VendorQueryDto query)
    {
        try
        {
            var repositoryQuery = new VendorQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                VendorId = query.VendorId,
                GuiId = query.GuiId,
                VendorName = query.VendorName,
                Status = query.Status,
                SysId = query.SysId,
                OrgId = query.OrgId
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => new VendorDto
            {
                VendorId = x.VendorId,
                GuiId = x.GuiId,
                GuiType = x.GuiType,
                VendorName = x.VendorName,
                VendorNameE = x.VendorNameE,
                VendorNameS = x.VendorNameS,
                Mcode = x.Mcode,
                VendorRegAddr = x.VendorRegAddr,
                TaxAddr = x.TaxAddr,
                VendorRegTel = x.VendorRegTel,
                VendorFax = x.VendorFax,
                VendorEmail = x.VendorEmail,
                InvEmail = x.InvEmail,
                ChargeStaff = x.ChargeStaff,
                ChargeTitle = x.ChargeTitle,
                ChargePid = x.ChargePid,
                ChargeTel = x.ChargeTel,
                ChargeAddr = x.ChargeAddr,
                ChargeEmail = x.ChargeEmail,
                Status = x.Status,
                SysId = x.SysId,
                PayType = x.PayType,
                SuplBankId = x.SuplBankId,
                SuplBankAcct = x.SuplBankAcct,
                SuplAcctName = x.SuplAcctName,
                TicketBe = x.TicketBe,
                CheckTitle = x.CheckTitle,
                OrgId = x.OrgId,
                Notes = x.Notes,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return new PagedResult<VendorDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢廠商列表失敗", ex);
            throw;
        }
    }

    public async Task<VendorDto> GetVendorByIdAsync(string vendorId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(vendorId);
            if (entity == null)
            {
                throw new InvalidOperationException($"廠商不存在: {vendorId}");
            }

            return new VendorDto
            {
                VendorId = entity.VendorId,
                GuiId = entity.GuiId,
                GuiType = entity.GuiType,
                VendorName = entity.VendorName,
                VendorNameE = entity.VendorNameE,
                VendorNameS = entity.VendorNameS,
                Mcode = entity.Mcode,
                VendorRegAddr = entity.VendorRegAddr,
                TaxAddr = entity.TaxAddr,
                VendorRegTel = entity.VendorRegTel,
                VendorFax = entity.VendorFax,
                VendorEmail = entity.VendorEmail,
                InvEmail = entity.InvEmail,
                ChargeStaff = entity.ChargeStaff,
                ChargeTitle = entity.ChargeTitle,
                ChargePid = entity.ChargePid,
                ChargeTel = entity.ChargeTel,
                ChargeAddr = entity.ChargeAddr,
                ChargeEmail = entity.ChargeEmail,
                Status = entity.Status,
                SysId = entity.SysId,
                PayType = entity.PayType,
                SuplBankId = entity.SuplBankId,
                SuplBankAcct = entity.SuplBankAcct,
                SuplAcctName = entity.SuplAcctName,
                TicketBe = entity.TicketBe,
                CheckTitle = entity.CheckTitle,
                OrgId = entity.OrgId,
                Notes = entity.Notes,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢廠商失敗: {vendorId}", ex);
            throw;
        }
    }

    public async Task<CheckGuiIdResultDto> CheckGuiIdAsync(string guiId)
    {
        try
        {
            var exists = await _repository.ExistsByGuiIdAsync(guiId);
            if (exists)
            {
                var vendor = await _repository.GetByGuiIdAsync(guiId);
                return new CheckGuiIdResultDto
                {
                    Exists = true,
                    VendorId = vendor?.VendorId
                };
            }

            return new CheckGuiIdResultDto
            {
                Exists = false,
                VendorId = null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查統一編號失敗: {guiId}", ex);
            throw;
        }
    }

    public async Task<string> CreateVendorAsync(CreateVendorDto dto)
    {
        try
        {
            // 驗證資料
            ValidateCreateDto(dto);

            // 檢查統一編號是否已存在
            var exists = await _repository.ExistsByGuiIdAsync(dto.GuiId);
            if (exists)
            {
                throw new InvalidOperationException($"統一編號已存在: {dto.GuiId}");
            }

            // 產生廠商編號
            var vendorId = await GenerateVendorIdAsync(dto.GuiId);

            var entity = new Vendor
            {
                VendorId = vendorId,
                GuiId = dto.GuiId,
                GuiType = dto.GuiType,
                VendorName = dto.VendorName,
                VendorNameE = dto.VendorNameE,
                VendorNameS = dto.VendorNameS,
                Mcode = dto.Mcode,
                VendorRegAddr = dto.VendorRegAddr,
                TaxAddr = dto.TaxAddr,
                VendorRegTel = dto.VendorRegTel,
                VendorFax = dto.VendorFax,
                VendorEmail = dto.VendorEmail,
                InvEmail = dto.InvEmail,
                ChargeStaff = dto.ChargeStaff,
                ChargeTitle = dto.ChargeTitle,
                ChargePid = dto.ChargePid,
                ChargeTel = dto.ChargeTel,
                ChargeAddr = dto.ChargeAddr,
                ChargeEmail = dto.ChargeEmail,
                Status = dto.Status ?? "A",
                SysId = dto.SysId ?? "1",
                PayType = dto.PayType,
                SuplBankId = dto.SuplBankId,
                SuplBankAcct = dto.SuplBankAcct,
                SuplAcctName = dto.SuplAcctName,
                TicketBe = dto.TicketBe,
                CheckTitle = dto.CheckTitle,
                OrgId = dto.OrgId,
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now,
                CreatedPriority = null,
                CreatedGroup = GetCurrentOrgId()
            };

            await _repository.CreateAsync(entity);

            return entity.VendorId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增廠商失敗: {dto.GuiId}", ex);
            throw;
        }
    }

    public async Task UpdateVendorAsync(string vendorId, UpdateVendorDto dto)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(vendorId);
            if (entity == null)
            {
                throw new InvalidOperationException($"廠商不存在: {vendorId}");
            }

            entity.VendorName = dto.VendorName;
            entity.VendorNameE = dto.VendorNameE;
            entity.VendorNameS = dto.VendorNameS;
            entity.Mcode = dto.Mcode;
            entity.VendorRegAddr = dto.VendorRegAddr;
            entity.TaxAddr = dto.TaxAddr;
            entity.VendorRegTel = dto.VendorRegTel;
            entity.VendorFax = dto.VendorFax;
            entity.VendorEmail = dto.VendorEmail;
            entity.InvEmail = dto.InvEmail;
            entity.ChargeStaff = dto.ChargeStaff;
            entity.ChargeTitle = dto.ChargeTitle;
            entity.ChargePid = dto.ChargePid;
            entity.ChargeTel = dto.ChargeTel;
            entity.ChargeAddr = dto.ChargeAddr;
            entity.ChargeEmail = dto.ChargeEmail;
            entity.Status = dto.Status ?? "A";
            entity.SysId = dto.SysId ?? "1";
            entity.PayType = dto.PayType;
            entity.SuplBankId = dto.SuplBankId;
            entity.SuplBankAcct = dto.SuplBankAcct;
            entity.SuplAcctName = dto.SuplAcctName;
            entity.TicketBe = dto.TicketBe;
            entity.CheckTitle = dto.CheckTitle;
            entity.OrgId = dto.OrgId;
            entity.Notes = dto.Notes;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改廠商失敗: {vendorId}", ex);
            throw;
        }
    }

    public async Task DeleteVendorAsync(string vendorId)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(vendorId);
            if (entity == null)
            {
                throw new InvalidOperationException($"廠商不存在: {vendorId}");
            }

            await _repository.DeleteAsync(vendorId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除廠商失敗: {vendorId}", ex);
            throw;
        }
    }

    public async Task DeleteVendorsBatchAsync(BatchDeleteVendorDto dto)
    {
        try
        {
            foreach (var vendorId in dto.VendorIds)
            {
                await DeleteVendorAsync(vendorId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除廠商失敗", ex);
            throw;
        }
    }

    public async Task<string> GenerateVendorIdAsync(string guiId)
    {
        try
        {
            var sequence = await _repository.GetNextSequenceAsync(guiId);
            return $"{guiId}-{sequence}";
        }
        catch (Exception ex)
        {
            _logger.LogError($"產生廠商編號失敗: {guiId}", ex);
            throw;
        }
    }

    private void ValidateCreateDto(CreateVendorDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.GuiId))
        {
            throw new ArgumentException("統一編號/身份證字號/自編編號不能為空");
        }

        if (string.IsNullOrWhiteSpace(dto.GuiType))
        {
            throw new ArgumentException("識別類型不能為空");
        }

        if (dto.GuiType != "1" && dto.GuiType != "2" && dto.GuiType != "3")
        {
            throw new ArgumentException("識別類型必須為 1 (統一編號)、2 (身份證字號) 或 3 (自編編號)");
        }

        if (string.IsNullOrWhiteSpace(dto.VendorName))
        {
            throw new ArgumentException("廠商名稱不能為空");
        }

        if (!string.IsNullOrEmpty(dto.Status) && dto.Status != "A" && dto.Status != "I")
        {
            throw new ArgumentException("狀態值必須為 A (啟用) 或 I (停用)");
        }
    }
}
