using ErpCore.Application.DTOs.Recruitment;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Recruitment;
using ErpCore.Infrastructure.Repositories.Recruitment;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Recruitment;

/// <summary>
/// 潛客服務實作 (SYSC180)
/// </summary>
public class ProspectService : BaseService, IProspectService
{
    private readonly IProspectRepository _repository;

    public ProspectService(
        IProspectRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<ProspectDto>> GetProspectsAsync(ProspectQueryDto query)
    {
        try
        {
            var repositoryQuery = new ProspectQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                ProspectId = query.ProspectId,
                ProspectName = query.ProspectName,
                Status = query.Status,
                SiteId = query.SiteId,
                RecruitId = query.RecruitId,
                ContactDateFrom = query.ContactDateFrom,
                ContactDateTo = query.ContactDateTo
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<ProspectDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢潛客列表失敗", ex);
            throw;
        }
    }

    public async Task<ProspectDto> GetProspectByIdAsync(string prospectId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(prospectId);
            if (entity == null)
            {
                throw new InvalidOperationException($"潛客不存在: {prospectId}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢潛客失敗: {prospectId}", ex);
            throw;
        }
    }

    public async Task<string> CreateProspectAsync(CreateProspectDto dto)
    {
        try
        {
            // 驗證必填欄位
            if (string.IsNullOrWhiteSpace(dto.ProspectId))
            {
                throw new ArgumentException("潛客代碼不能為空");
            }

            if (string.IsNullOrWhiteSpace(dto.ProspectName))
            {
                throw new ArgumentException("潛客名稱不能為空");
            }

            // 檢查潛客代碼是否已存在
            var exists = await _repository.ExistsAsync(dto.ProspectId);
            if (exists)
            {
                throw new InvalidOperationException($"潛客代碼已存在: {dto.ProspectId}");
            }

            // 驗證狀態值
            if (!string.IsNullOrEmpty(dto.Status) && 
                dto.Status != "PENDING" && dto.Status != "INTERVIEWING" && 
                dto.Status != "SIGNED" && dto.Status != "CANCELLED")
            {
                throw new ArgumentException("狀態值必須為 PENDING、INTERVIEWING、SIGNED 或 CANCELLED");
            }

            var entity = new Prospect
            {
                ProspectId = dto.ProspectId,
                ProspectName = dto.ProspectName,
                ContactPerson = dto.ContactPerson,
                ContactTel = dto.ContactTel,
                ContactFax = dto.ContactFax,
                ContactEmail = dto.ContactEmail,
                ContactAddress = dto.ContactAddress,
                StoreName = dto.StoreName,
                StoreTel = dto.StoreTel,
                SiteId = dto.SiteId,
                RecruitId = dto.RecruitId,
                StoreId = dto.StoreId,
                VendorId = dto.VendorId,
                OrgId = dto.OrgId,
                BtypeId = dto.BtypeId,
                SalesType = dto.SalesType,
                Status = dto.Status ?? "PENDING",
                OverallStatus = dto.OverallStatus,
                PaperType = dto.PaperType,
                LocationType = dto.LocationType,
                DecoType = dto.DecoType,
                CommType = dto.CommType,
                PdType = dto.PdType,
                BaseRent = dto.BaseRent,
                Deposit = dto.Deposit,
                CreditCard = dto.CreditCard,
                TargetAmountM = dto.TargetAmountM,
                TargetAmountV = dto.TargetAmountV,
                ExerciseFees = dto.ExerciseFees,
                CheckDay = dto.CheckDay,
                AgmDateB = dto.AgmDateB,
                AgmDateE = dto.AgmDateE,
                ContractProidB = dto.ContractProidB,
                ContractProidE = dto.ContractProidE,
                FeedbackDate = dto.FeedbackDate,
                DueDate = dto.DueDate,
                ContactDate = dto.ContactDate,
                VersionNo = dto.VersionNo,
                GuiId = dto.GuiId,
                BankId = dto.BankId,
                AccName = dto.AccName,
                AccNo = dto.AccNo,
                InvEmail = dto.InvEmail,
                EdcYn = dto.EdcYn ?? "N",
                ReceYn = dto.ReceYn ?? "N",
                PosYn = dto.PosYn ?? "N",
                CashYn = dto.CashYn ?? "N",
                CommYn = dto.CommYn ?? "N",
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now,
                CreatedPriority = null,
                CreatedGroup = null
            };

            var result = await _repository.CreateAsync(entity);
            return result.ProspectId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增潛客失敗: {dto.ProspectId}", ex);
            throw;
        }
    }

    public async Task UpdateProspectAsync(string prospectId, UpdateProspectDto dto)
    {
        try
        {
            // 驗證必填欄位
            if (string.IsNullOrWhiteSpace(dto.ProspectName))
            {
                throw new ArgumentException("潛客名稱不能為空");
            }

            // 檢查潛客是否存在
            var existing = await _repository.GetByIdAsync(prospectId);
            if (existing == null)
            {
                throw new InvalidOperationException($"潛客不存在: {prospectId}");
            }

            // 驗證狀態值
            if (!string.IsNullOrEmpty(dto.Status) && 
                dto.Status != "PENDING" && dto.Status != "INTERVIEWING" && 
                dto.Status != "SIGNED" && dto.Status != "CANCELLED")
            {
                throw new ArgumentException("狀態值必須為 PENDING、INTERVIEWING、SIGNED 或 CANCELLED");
            }

            existing.ProspectName = dto.ProspectName;
            existing.ContactPerson = dto.ContactPerson;
            existing.ContactTel = dto.ContactTel;
            existing.ContactFax = dto.ContactFax;
            existing.ContactEmail = dto.ContactEmail;
            existing.ContactAddress = dto.ContactAddress;
            existing.StoreName = dto.StoreName;
            existing.StoreTel = dto.StoreTel;
            existing.SiteId = dto.SiteId;
            existing.RecruitId = dto.RecruitId;
            existing.StoreId = dto.StoreId;
            existing.VendorId = dto.VendorId;
            existing.OrgId = dto.OrgId;
            existing.BtypeId = dto.BtypeId;
            existing.SalesType = dto.SalesType;
            existing.Status = dto.Status ?? existing.Status;
            existing.OverallStatus = dto.OverallStatus;
            existing.PaperType = dto.PaperType;
            existing.LocationType = dto.LocationType;
            existing.DecoType = dto.DecoType;
            existing.CommType = dto.CommType;
            existing.PdType = dto.PdType;
            existing.BaseRent = dto.BaseRent;
            existing.Deposit = dto.Deposit;
            existing.CreditCard = dto.CreditCard;
            existing.TargetAmountM = dto.TargetAmountM;
            existing.TargetAmountV = dto.TargetAmountV;
            existing.ExerciseFees = dto.ExerciseFees;
            existing.CheckDay = dto.CheckDay;
            existing.AgmDateB = dto.AgmDateB;
            existing.AgmDateE = dto.AgmDateE;
            existing.ContractProidB = dto.ContractProidB;
            existing.ContractProidE = dto.ContractProidE;
            existing.FeedbackDate = dto.FeedbackDate;
            existing.DueDate = dto.DueDate;
            existing.ContactDate = dto.ContactDate;
            existing.VersionNo = dto.VersionNo;
            existing.GuiId = dto.GuiId;
            existing.BankId = dto.BankId;
            existing.AccName = dto.AccName;
            existing.AccNo = dto.AccNo;
            existing.InvEmail = dto.InvEmail;
            existing.EdcYn = dto.EdcYn ?? existing.EdcYn;
            existing.ReceYn = dto.ReceYn ?? existing.ReceYn;
            existing.PosYn = dto.PosYn ?? existing.PosYn;
            existing.CashYn = dto.CashYn ?? existing.CashYn;
            existing.CommYn = dto.CommYn ?? existing.CommYn;
            existing.Notes = dto.Notes;
            existing.UpdatedBy = GetCurrentUserId();
            existing.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(existing);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改潛客失敗: {prospectId}", ex);
            throw;
        }
    }

    public async Task DeleteProspectAsync(string prospectId)
    {
        try
        {
            // 檢查潛客是否存在
            var existing = await _repository.GetByIdAsync(prospectId);
            if (existing == null)
            {
                throw new InvalidOperationException($"潛客不存在: {prospectId}");
            }

            // 檢查是否有關聯的訪談記錄
            var hasRelated = await _repository.HasRelatedInterviewsAsync(prospectId);
            if (hasRelated)
            {
                throw new InvalidOperationException($"無法刪除潛客，因為有關聯的訪談記錄: {prospectId}");
            }

            await _repository.DeleteAsync(prospectId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除潛客失敗: {prospectId}", ex);
            throw;
        }
    }

    public async Task BatchDeleteProspectsAsync(BatchDeleteProspectDto dto)
    {
        try
        {
            if (dto.ProspectIds == null || dto.ProspectIds.Count == 0)
            {
                throw new ArgumentException("潛客代碼列表不能為空");
            }

            foreach (var prospectId in dto.ProspectIds)
            {
                await DeleteProspectAsync(prospectId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除潛客失敗", ex);
            throw;
        }
    }

    public async Task UpdateProspectStatusAsync(string prospectId, UpdateProspectStatusDto dto)
    {
        try
        {
            // 驗證狀態值
            if (string.IsNullOrEmpty(dto.Status) || 
                (dto.Status != "PENDING" && dto.Status != "INTERVIEWING" && 
                 dto.Status != "SIGNED" && dto.Status != "CANCELLED"))
            {
                throw new ArgumentException("狀態值必須為 PENDING、INTERVIEWING、SIGNED 或 CANCELLED");
            }

            // 檢查潛客是否存在
            var existing = await _repository.GetByIdAsync(prospectId);
            if (existing == null)
            {
                throw new InvalidOperationException($"潛客不存在: {prospectId}");
            }

            existing.Status = dto.Status;
            existing.UpdatedBy = GetCurrentUserId();
            existing.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(existing);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新潛客狀態失敗: {prospectId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 將 Entity 轉換為 DTO
    /// </summary>
    private static ProspectDto MapToDto(Prospect entity)
    {
        return new ProspectDto
        {
            ProspectId = entity.ProspectId,
            ProspectName = entity.ProspectName,
            ContactPerson = entity.ContactPerson,
            ContactTel = entity.ContactTel,
            ContactFax = entity.ContactFax,
            ContactEmail = entity.ContactEmail,
            ContactAddress = entity.ContactAddress,
            StoreName = entity.StoreName,
            StoreTel = entity.StoreTel,
            SiteId = entity.SiteId,
            RecruitId = entity.RecruitId,
            StoreId = entity.StoreId,
            VendorId = entity.VendorId,
            OrgId = entity.OrgId,
            BtypeId = entity.BtypeId,
            SalesType = entity.SalesType,
            Status = entity.Status,
            OverallStatus = entity.OverallStatus,
            PaperType = entity.PaperType,
            LocationType = entity.LocationType,
            DecoType = entity.DecoType,
            CommType = entity.CommType,
            PdType = entity.PdType,
            BaseRent = entity.BaseRent,
            Deposit = entity.Deposit,
            CreditCard = entity.CreditCard,
            TargetAmountM = entity.TargetAmountM,
            TargetAmountV = entity.TargetAmountV,
            ExerciseFees = entity.ExerciseFees,
            CheckDay = entity.CheckDay,
            AgmDateB = entity.AgmDateB,
            AgmDateE = entity.AgmDateE,
            ContractProidB = entity.ContractProidB,
            ContractProidE = entity.ContractProidE,
            FeedbackDate = entity.FeedbackDate,
            DueDate = entity.DueDate,
            ContactDate = entity.ContactDate,
            VersionNo = entity.VersionNo,
            GuiId = entity.GuiId,
            BankId = entity.BankId,
            AccName = entity.AccName,
            AccNo = entity.AccNo,
            InvEmail = entity.InvEmail,
            EdcYn = entity.EdcYn,
            ReceYn = entity.ReceYn,
            PosYn = entity.PosYn,
            CashYn = entity.CashYn,
            CommYn = entity.CommYn,
            Notes = entity.Notes,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

