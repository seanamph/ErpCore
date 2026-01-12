using Dapper;
using ErpCore.Application.DTOs.AnalysisReport;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.AnalysisReport;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.AnalysisReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using System.Data;

namespace ErpCore.Application.Services.AnalysisReport;

/// <summary>
/// 單位領用申請單服務實作 (SYSA210)
/// </summary>
public class MaterialApplyService : BaseService, IMaterialApplyService
{
    private readonly IMaterialApplyRepository _repository;
    private readonly IDbConnectionFactory _connectionFactory;

    public MaterialApplyService(
        IMaterialApplyRepository repository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<MaterialApplyDto>> GetListAsync(MaterialApplyQueryDto query)
    {
        try
        {
            var repositoryQuery = new MaterialApplyQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                ApplyId = query.Filters?.ApplyId,
                EmpId = query.Filters?.EmpId,
                OrgId = query.Filters?.OrgId,
                SiteId = query.Filters?.SiteId,
                ApplyDateFrom = query.Filters?.ApplyDateFrom,
                ApplyDateTo = query.Filters?.ApplyDateTo,
                AprvDateFrom = query.Filters?.AprvDateFrom,
                AprvDateTo = query.Filters?.AprvDateTo,
                CheckDate = query.Filters?.CheckDate,
                ApplyStatus = query.Filters?.ApplyStatus,
                GoodsId = query.Filters?.GoodsId,
                WhId = query.Filters?.WhId,
                StoreId = query.Filters?.StoreId,
                SortField = query.SortField,
                SortOrder = query.SortOrder
            };

            var result = await _repository.GetListAsync(repositoryQuery);

            // 取得所有不重複的 EmpId, OrgId, SiteId, AprvEmpId
            var empIds = result.Items.Select(x => x.EmpId).Distinct().ToList();
            var orgIds = result.Items.Select(x => x.OrgId).Distinct().ToList();
            var siteIds = result.Items.Where(x => !string.IsNullOrEmpty(x.SiteId)).Select(x => x.SiteId!).Distinct().ToList();
            var aprvEmpIds = result.Items.Where(x => !string.IsNullOrEmpty(x.AprvEmpId)).Select(x => x.AprvEmpId!).Distinct().ToList();

            var empNameMap = new Dictionary<string, string>();
            var orgNameMap = new Dictionary<string, string>();
            var siteNameMap = new Dictionary<string, string>();

            // 批量查詢員工名稱
            if (empIds.Any() || aprvEmpIds.Any())
            {
                using var connection = _connectionFactory.CreateConnection();
                var allEmpIds = empIds.Union(aprvEmpIds).Distinct().ToList();
                var sql = @"
                    SELECT USER_ID, USER_NAME 
                    FROM V_EMP_USER 
                    WHERE USER_ID IN @EmpIds";
                var emps = await connection.QueryAsync<(string UserId, string UserName)>(sql, new { EmpIds = allEmpIds });
                empNameMap = emps.ToDictionary(x => x.UserId, x => x.UserName);
            }

            // 批量查詢組織名稱
            if (orgIds.Any())
            {
                using var connection = _connectionFactory.CreateConnection();
                var sql = @"
                    SELECT ORG_ID, ORG_NAME 
                    FROM ORG_GROUP 
                    WHERE ORG_ID IN @OrgIds";
                var orgs = await connection.QueryAsync<(string OrgId, string OrgName)>(sql, new { OrgIds = orgIds });
                orgNameMap = orgs.ToDictionary(x => x.OrgId, x => x.OrgName);
            }

            // 批量查詢分店名稱
            if (siteIds.Any())
            {
                using var connection = _connectionFactory.CreateConnection();
                var sql = @"
                    SELECT ShopId, ShopName 
                    FROM Shops 
                    WHERE ShopId IN @SiteIds";
                var sites = await connection.QueryAsync<(string ShopId, string ShopName)>(sql, new { SiteIds = siteIds });
                siteNameMap = sites.ToDictionary(x => x.ShopId, x => x.ShopName);
            }

            var dtos = result.Items.Select(x => new MaterialApplyDto
            {
                TKey = x.TKey,
                ApplyId = x.ApplyId,
                EmpId = x.EmpId,
                EmpName = empNameMap.GetValueOrDefault(x.EmpId),
                OrgId = x.OrgId,
                OrgName = orgNameMap.GetValueOrDefault(x.OrgId),
                SiteId = x.SiteId,
                SiteName = siteNameMap.GetValueOrDefault(x.SiteId ?? ""),
                ApplyDate = x.ApplyDate,
                ApplyStatus = x.ApplyStatus,
                ApplyStatusName = GetStatusName(x.ApplyStatus),
                Amount = x.Amount,
                AprvEmpId = x.AprvEmpId,
                AprvEmpName = x.AprvEmpId != null ? empNameMap.GetValueOrDefault(x.AprvEmpId) : null,
                AprvDate = x.AprvDate,
                CheckDate = x.CheckDate,
                WhId = x.WhId,
                StoreId = x.StoreId,
                Notes = x.Notes,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return new PagedResult<MaterialApplyDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢單位領用申請單列表失敗", ex);
            throw;
        }
    }

    public async Task<MaterialApplyDetailDto> GetByApplyIdAsync(string applyId)
    {
        try
        {
            var master = await _repository.GetByApplyIdAsync(applyId);
            if (master == null)
            {
                throw new KeyNotFoundException($"單位領用申請單不存在: {applyId}");
            }

            // 查詢員工名稱、組織名稱、分店名稱
            string? empName = null;
            string? orgName = null;
            string? siteName = null;
            string? aprvEmpName = null;

            using var connection = _connectionFactory.CreateConnection();

            if (!string.IsNullOrEmpty(master.EmpId))
            {
                var sql = "SELECT USER_NAME FROM V_EMP_USER WHERE USER_ID = @EmpId";
                empName = await connection.QueryFirstOrDefaultAsync<string>(sql, new { EmpId = master.EmpId });
            }

            if (!string.IsNullOrEmpty(master.OrgId))
            {
                var sql = "SELECT ORG_NAME FROM ORG_GROUP WHERE ORG_ID = @OrgId";
                orgName = await connection.QueryFirstOrDefaultAsync<string>(sql, new { OrgId = master.OrgId });
            }

            if (!string.IsNullOrEmpty(master.SiteId))
            {
                var sql = "SELECT ShopName FROM Shops WHERE ShopId = @SiteId";
                siteName = await connection.QueryFirstOrDefaultAsync<string>(sql, new { SiteId = master.SiteId });
            }

            if (!string.IsNullOrEmpty(master.AprvEmpId))
            {
                var sql = "SELECT USER_NAME FROM V_EMP_USER WHERE USER_ID = @AprvEmpId";
                aprvEmpName = await connection.QueryFirstOrDefaultAsync<string>(sql, new { AprvEmpId = master.AprvEmpId });
            }

            // 查詢明細的品項名稱
            var goodsIds = master.Details.Select(d => d.GoodsId).Distinct().ToList();
            var goodsNameMap = new Dictionary<string, string>();
            if (goodsIds.Any())
            {
                var sql = "SELECT GOODS_ID, GOODS_NAME FROM AM_GOODS WHERE GOODS_ID IN @GoodsIds";
                var goods = await connection.QueryAsync<(string GoodsId, string GoodsName)>(sql, new { GoodsIds = goodsIds });
                goodsNameMap = goods.ToDictionary(x => x.GoodsId, x => x.GoodsName);
            }

            var dto = new MaterialApplyDetailDto
            {
                TKey = master.TKey,
                ApplyId = master.ApplyId,
                EmpId = master.EmpId,
                EmpName = empName,
                OrgId = master.OrgId,
                OrgName = orgName,
                SiteId = master.SiteId,
                SiteName = siteName,
                ApplyDate = master.ApplyDate,
                ApplyStatus = master.ApplyStatus,
                ApplyStatusName = GetStatusName(master.ApplyStatus),
                Amount = master.Amount,
                AprvEmpId = master.AprvEmpId,
                AprvEmpName = aprvEmpName,
                AprvDate = master.AprvDate,
                CheckDate = master.CheckDate,
                WhId = master.WhId,
                StoreId = master.StoreId,
                Notes = master.Notes,
                CreatedBy = master.CreatedBy,
                CreatedAt = master.CreatedAt,
                UpdatedBy = master.UpdatedBy,
                UpdatedAt = master.UpdatedAt,
                Details = master.Details.Select(d => new MaterialApplyDetailItemDto
                {
                    TKey = d.TKey,
                    GoodsTKey = d.GoodsTKey,
                    GoodsId = d.GoodsId,
                    GoodsName = goodsNameMap.GetValueOrDefault(d.GoodsId),
                    ApplyQty = d.ApplyQty,
                    IssueQty = d.IssueQty,
                    Unit = d.Unit,
                    UnitPrice = d.UnitPrice,
                    Amount = d.Amount,
                    Notes = d.Notes,
                    SeqNo = d.SeqNo
                }).ToList()
            };

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢單位領用申請單詳細資料失敗: {applyId}", ex);
            throw;
        }
    }

    public async Task<MaterialApplyDetailDto> CreateAsync(CreateMaterialApplyDto dto, string userId)
    {
        try
        {
            // 產生領用單號
            var applyId = string.IsNullOrEmpty(dto.ApplyId) 
                ? await _repository.GenerateApplyIdAsync() 
                : dto.ApplyId;

            // 檢查領用單號是否已存在
            if (await _repository.ExistsAsync(applyId))
            {
                throw new InvalidOperationException($"領用單號已存在: {applyId}");
            }

            // 驗證申請日期不能是未來日期
            if (dto.ApplyDate > DateTime.Now.Date)
            {
                throw new ArgumentException("申請日期不能是未來日期");
            }

            // 查詢品項資訊並計算金額
            decimal totalAmount = 0;
            var details = new List<MaterialApplyDetail>();

            using var connection = _connectionFactory.CreateConnection();
            for (int i = 0; i < dto.Details.Count; i++)
            {
                var detailDto = dto.Details[i];

                // 驗證品項是否存在並取得品項資訊
                var sql = @"
                    SELECT T_KEY, GOODS_ID, GOODS_NAME, UNIT, UNIT_PRICE 
                    FROM AM_GOODS 
                    WHERE GOODS_ID = @GoodsId";
                var goods = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { GoodsId = detailDto.GoodsId });

                if (goods == null)
                {
                    throw new ArgumentException($"品項不存在: {detailDto.GoodsId}");
                }

                if (detailDto.ApplyQty <= 0)
                {
                    throw new ArgumentException($"申請數量必須大於0: {detailDto.GoodsId}");
                }

                var unitPrice = detailDto.UnitPrice > 0 ? detailDto.UnitPrice : (decimal?)goods.UNIT_PRICE ?? 0;
                var amount = detailDto.ApplyQty * unitPrice;
                totalAmount += amount;

                var detail = new MaterialApplyDetail
                {
                    ApplyMasterKey = 0, // 稍後會更新
                    ApplyId = applyId,
                    GoodsTKey = detailDto.GoodsTKey > 0 ? detailDto.GoodsTKey : (long)goods.T_KEY,
                    GoodsId = detailDto.GoodsId,
                    ApplyQty = detailDto.ApplyQty,
                    IssueQty = null,
                    Unit = goods.UNIT?.ToString() ?? "",
                    UnitPrice = unitPrice,
                    Amount = amount,
                    Notes = detailDto.Notes,
                    SeqNo = detailDto.SeqNo > 0 ? detailDto.SeqNo : i + 1,
                    CreatedBy = userId,
                    CreatedAt = DateTime.Now,
                    UpdatedBy = userId,
                    UpdatedAt = DateTime.Now
                };

                details.Add(detail);
            }

            if (details.Count == 0)
            {
                throw new ArgumentException("明細資料不可為空");
            }

            var master = new MaterialApplyMaster
            {
                ApplyId = applyId,
                EmpId = dto.EmpId,
                OrgId = dto.OrgId,
                SiteId = dto.SiteId,
                ApplyDate = dto.ApplyDate,
                ApplyStatus = "0",
                Amount = totalAmount,
                AprvEmpId = null,
                AprvDate = null,
                CheckDate = null,
                WhId = dto.WhId,
                StoreId = dto.StoreId,
                Notes = dto.Notes,
                CreatedBy = userId,
                CreatedAt = DateTime.Now,
                UpdatedBy = userId,
                UpdatedAt = DateTime.Now,
                Details = details
            };

            var created = await _repository.CreateAsync(master);
            return await GetByApplyIdAsync(created.ApplyId);
        }
        catch (Exception ex)
        {
            _logger.LogError("新增單位領用申請單失敗", ex);
            throw;
        }
    }

    public async Task<MaterialApplyDetailDto> UpdateAsync(string applyId, UpdateMaterialApplyDto dto, string userId)
    {
        try
        {
            var master = await _repository.GetByApplyIdAsync(applyId);
            if (master == null)
            {
                throw new KeyNotFoundException($"單位領用申請單不存在: {applyId}");
            }

            if (master.ApplyStatus != "0")
            {
                throw new InvalidOperationException($"只有待審核狀態的領用單才能修改: {applyId}");
            }

            // 驗證申請日期不能是未來日期
            if (dto.ApplyDate > DateTime.Now.Date)
            {
                throw new ArgumentException("申請日期不能是未來日期");
            }

            // 查詢品項資訊並計算金額
            decimal totalAmount = 0;
            var details = new List<MaterialApplyDetail>();

            using var connection = _connectionFactory.CreateConnection();
            for (int i = 0; i < dto.Details.Count; i++)
            {
                var detailDto = dto.Details[i];

                // 驗證品項是否存在並取得品項資訊
                var sql = @"
                    SELECT T_KEY, GOODS_ID, GOODS_NAME, UNIT, UNIT_PRICE 
                    FROM AM_GOODS 
                    WHERE GOODS_ID = @GoodsId";
                var goods = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { GoodsId = detailDto.GoodsId });

                if (goods == null)
                {
                    throw new ArgumentException($"品項不存在: {detailDto.GoodsId}");
                }

                if (detailDto.ApplyQty <= 0)
                {
                    throw new ArgumentException($"申請數量必須大於0: {detailDto.GoodsId}");
                }

                var unitPrice = detailDto.UnitPrice > 0 ? detailDto.UnitPrice : (decimal?)goods.UNIT_PRICE ?? 0;
                var amount = detailDto.ApplyQty * unitPrice;
                totalAmount += amount;

                var detail = new MaterialApplyDetail
                {
                    ApplyMasterKey = master.TKey,
                    ApplyId = applyId,
                    GoodsTKey = detailDto.GoodsTKey > 0 ? detailDto.GoodsTKey : (long)goods.T_KEY,
                    GoodsId = detailDto.GoodsId,
                    ApplyQty = detailDto.ApplyQty,
                    IssueQty = null,
                    Unit = goods.UNIT?.ToString() ?? "",
                    UnitPrice = unitPrice,
                    Amount = amount,
                    Notes = detailDto.Notes,
                    SeqNo = detailDto.SeqNo > 0 ? detailDto.SeqNo : i + 1,
                    CreatedBy = userId,
                    CreatedAt = DateTime.Now,
                    UpdatedBy = userId,
                    UpdatedAt = DateTime.Now
                };

                details.Add(detail);
            }

            if (details.Count == 0)
            {
                throw new ArgumentException("明細資料不可為空");
            }

            master.EmpId = dto.EmpId;
            master.OrgId = dto.OrgId;
            master.SiteId = dto.SiteId;
            master.ApplyDate = dto.ApplyDate;
            master.Amount = totalAmount;
            master.WhId = dto.WhId;
            master.StoreId = dto.StoreId;
            master.Notes = dto.Notes;
            master.UpdatedBy = userId;
            master.UpdatedAt = DateTime.Now;
            master.Details = details;

            await _repository.UpdateAsync(master);
            return await GetByApplyIdAsync(applyId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新單位領用申請單失敗: {applyId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string applyId, string userId)
    {
        try
        {
            var master = await _repository.GetByApplyIdAsync(applyId);
            if (master == null)
            {
                throw new KeyNotFoundException($"單位領用申請單不存在: {applyId}");
            }

            if (master.ApplyStatus != "0")
            {
                throw new InvalidOperationException($"只有待審核狀態的領用單才能刪除: {applyId}");
            }

            await _repository.DeleteAsync(applyId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除單位領用申請單失敗: {applyId}", ex);
            throw;
        }
    }

    public async Task<MaterialApplyDetailDto> ApproveAsync(string applyId, ApproveMaterialApplyDto dto, string userId)
    {
        try
        {
            var master = await _repository.GetByApplyIdAsync(applyId);
            if (master == null)
            {
                throw new KeyNotFoundException($"單位領用申請單不存在: {applyId}");
            }

            if (master.ApplyStatus != "0")
            {
                throw new InvalidOperationException($"只有待審核狀態的領用單才能審核: {applyId}");
            }

            master.ApplyStatus = "1";
            master.AprvEmpId = dto.AprvEmpId;
            master.AprvDate = dto.AprvDate;
            master.UpdatedBy = userId;
            master.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(master);
            return await GetByApplyIdAsync(applyId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"審核單位領用申請單失敗: {applyId}", ex);
            throw;
        }
    }

    public async Task<MaterialApplyDetailDto> IssueAsync(string applyId, IssueMaterialApplyDto dto, string userId)
    {
        try
        {
            var master = await _repository.GetByApplyIdAsync(applyId);
            if (master == null)
            {
                throw new KeyNotFoundException($"單位領用申請單不存在: {applyId}");
            }

            if (master.ApplyStatus != "1")
            {
                throw new InvalidOperationException($"只有已審核狀態的領用單才能發料: {applyId}");
            }

            // 更新明細的發料數量
            foreach (var detailDto in dto.Details)
            {
                var detail = master.Details.FirstOrDefault(d => d.TKey == detailDto.TKey);
                if (detail != null)
                {
                    if (detailDto.IssueQty < 0 || detailDto.IssueQty > detail.ApplyQty)
                    {
                        throw new ArgumentException($"發料數量必須在0到申請數量之間: {detail.GoodsId}");
                    }
                    detail.IssueQty = detailDto.IssueQty;
                    detail.UpdatedBy = userId;
                    detail.UpdatedAt = DateTime.Now;
                }
            }

            master.ApplyStatus = "2";
            master.CheckDate = dto.CheckDate;
            master.UpdatedBy = userId;
            master.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(master);
            return await GetByApplyIdAsync(applyId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"發料作業失敗: {applyId}", ex);
            throw;
        }
    }

    public async Task<List<MaterialApplyDetailDto>> BatchCreateAsync(BatchCreateMaterialApplyDto dto, string userId)
    {
        try
        {
            var results = new List<MaterialApplyDetailDto>();

            foreach (var item in dto.Items)
            {
                var createDto = new CreateMaterialApplyDto
                {
                    EmpId = dto.EmpId,
                    OrgId = dto.OrgId,
                    SiteId = dto.SiteId,
                    ApplyDate = dto.ApplyDate,
                    Details = new List<CreateMaterialApplyDetailDto>
                    {
                        new CreateMaterialApplyDetailDto
                        {
                            GoodsId = item.GoodsId,
                            ApplyQty = item.ApplyQty,
                            SeqNo = 1
                        }
                    }
                };

                var result = await CreateAsync(createDto, userId);
                results.Add(result);
            }

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError("批次新增單位領用申請單失敗", ex);
            throw;
        }
    }

    public async Task<string> GenerateApplyIdAsync()
    {
        try
        {
            return await _repository.GenerateApplyIdAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("產生領用單號失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 取得狀態名稱
    /// </summary>
    private string GetStatusName(string status)
    {
        return status switch
        {
            "0" => "待審核",
            "1" => "已審核",
            "2" => "已發料",
            "3" => "已取消",
            _ => status
        };
    }
}
