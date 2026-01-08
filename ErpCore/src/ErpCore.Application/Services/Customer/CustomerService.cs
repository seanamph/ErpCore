using Dapper;
using ErpCore.Application.DTOs.Customer;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Customer;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.Customer;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using System.Data;

namespace ErpCore.Application.Services.Customer;

/// <summary>
/// 客戶服務實作
/// </summary>
public class CustomerService : BaseService, ICustomerService
{
    private readonly ICustomerRepository _repository;
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ExportHelper _exportHelper;

    public CustomerService(
        ICustomerRepository repository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext,
        ExportHelper exportHelper) : base(logger, userContext)
    {
        _repository = repository;
        _connectionFactory = connectionFactory;
        _exportHelper = exportHelper;
    }

    public async Task<PagedResult<CustomerDto>> GetCustomersAsync(CustomerQueryDto query)
    {
        try
        {
            var repositoryQuery = new CustomerQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                CustomerId = query.CustomerId,
                CustomerName = query.CustomerName,
                GuiId = query.GuiId,
                Status = query.Status,
                SalesId = query.SalesId
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<CustomerDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢客戶列表失敗", ex);
            throw;
        }
    }

    public async Task<CustomerDto> GetCustomerByIdAsync(string customerId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(customerId);
            if (entity == null)
            {
                throw new InvalidOperationException($"客戶不存在: {customerId}");
            }

            var dto = MapToDto(entity);

            // 查詢聯絡人
            var contacts = await _repository.GetContactsByCustomerIdAsync(customerId);
            dto.Contacts = contacts.Select(x => new CustomerContactDto
            {
                ContactId = x.ContactId,
                ContactName = x.ContactName,
                ContactTitle = x.ContactTitle,
                ContactTel = x.ContactTel,
                ContactCell = x.ContactCell,
                ContactEmail = x.ContactEmail,
                IsPrimary = x.IsPrimary
            }).ToList();

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢客戶失敗: {customerId}", ex);
            throw;
        }
    }

    public async Task<string> CreateCustomerAsync(CreateCustomerDto dto)
    {
        try
        {
            // 驗證資料
            ValidateCreateDto(dto);

            // 檢查客戶編號是否已存在
            var exists = await _repository.ExistsAsync(dto.CustomerId);
            if (exists)
            {
                throw new InvalidOperationException($"客戶編號已存在: {dto.CustomerId}");
            }

            // 檢查統一編號是否已存在
            if (!string.IsNullOrEmpty(dto.GuiId))
            {
                var guiIdExists = await _repository.ExistsByGuiIdAsync(dto.GuiId);
                if (guiIdExists)
                {
                    throw new InvalidOperationException($"統一編號已存在: {dto.GuiId}");
                }
            }

            using var connection = _connectionFactory.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                var entity = new Customer
                {
                    CustomerId = dto.CustomerId,
                    GuiId = dto.GuiId,
                    GuiType = dto.GuiType,
                    CustomerName = dto.CustomerName,
                    CustomerNameE = dto.CustomerNameE,
                    ShortName = dto.ShortName,
                    ContactStaff = dto.ContactStaff,
                    HomeTel = dto.HomeTel,
                    CompTel = dto.CompTel,
                    Fax = dto.Fax,
                    Cell = dto.Cell,
                    Email = dto.Email,
                    Sex = dto.Sex,
                    Title = dto.Title,
                    City = dto.City,
                    Canton = dto.Canton,
                    Addr = dto.Addr,
                    TaxAddr = dto.TaxAddr,
                    DelyAddr = dto.DelyAddr,
                    PostId = dto.PostId,
                    DiscountYn = dto.DiscountYn,
                    DiscountNo = dto.DiscountNo,
                    SalesId = dto.SalesId,
                    MonthlyYn = dto.MonthlyYn,
                    Status = dto.Status,
                    Notes = dto.Notes,
                    AccAmt = 0,
                    CreatedBy = GetCurrentUserId(),
                    CreatedAt = DateTime.Now,
                    UpdatedBy = GetCurrentUserId(),
                    UpdatedAt = DateTime.Now
                };

                await _repository.CreateAsync(entity);

                // 新增聯絡人
                if (dto.Contacts != null && dto.Contacts.Any())
                {
                    foreach (var contactDto in dto.Contacts)
                    {
                        var contact = new CustomerContact
                        {
                            ContactId = Guid.NewGuid(),
                            CustomerId = entity.CustomerId,
                            ContactName = contactDto.ContactName,
                            ContactTitle = contactDto.ContactTitle,
                            ContactTel = contactDto.ContactTel,
                            ContactCell = contactDto.ContactCell,
                            ContactEmail = contactDto.ContactEmail,
                            IsPrimary = contactDto.IsPrimary,
                            CreatedBy = GetCurrentUserId(),
                            CreatedAt = DateTime.Now,
                            UpdatedBy = GetCurrentUserId(),
                            UpdatedAt = DateTime.Now
                        };

                        await _repository.CreateContactAsync(contact);
                    }
                }

                transaction.Commit();
                return entity.CustomerId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增客戶失敗: {dto.CustomerId}", ex);
            throw;
        }
    }

    public async Task UpdateCustomerAsync(string customerId, UpdateCustomerDto dto)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(customerId);
            if (entity == null)
            {
                throw new InvalidOperationException($"客戶不存在: {customerId}");
            }

            using var connection = _connectionFactory.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                // 更新客戶資料
                entity.GuiId = dto.GuiId;
                entity.GuiType = dto.GuiType;
                entity.CustomerName = dto.CustomerName;
                entity.CustomerNameE = dto.CustomerNameE;
                entity.ShortName = dto.ShortName;
                entity.ContactStaff = dto.ContactStaff;
                entity.HomeTel = dto.HomeTel;
                entity.CompTel = dto.CompTel;
                entity.Fax = dto.Fax;
                entity.Cell = dto.Cell;
                entity.Email = dto.Email;
                entity.Sex = dto.Sex;
                entity.Title = dto.Title;
                entity.City = dto.City;
                entity.Canton = dto.Canton;
                entity.Addr = dto.Addr;
                entity.TaxAddr = dto.TaxAddr;
                entity.DelyAddr = dto.DelyAddr;
                entity.PostId = dto.PostId;
                entity.DiscountYn = dto.DiscountYn;
                entity.DiscountNo = dto.DiscountNo;
                entity.SalesId = dto.SalesId;
                entity.MonthlyYn = dto.MonthlyYn;
                entity.Status = dto.Status;
                entity.Notes = dto.Notes;
                entity.UpdatedBy = GetCurrentUserId();
                entity.UpdatedAt = DateTime.Now;

                await _repository.UpdateAsync(entity);

                // 刪除舊聯絡人並新增新聯絡人
                await _repository.DeleteContactsByCustomerIdAsync(customerId);
                if (dto.Contacts != null && dto.Contacts.Any())
                {
                    foreach (var contactDto in dto.Contacts)
                    {
                        var contact = new CustomerContact
                        {
                            ContactId = Guid.NewGuid(),
                            CustomerId = customerId,
                            ContactName = contactDto.ContactName,
                            ContactTitle = contactDto.ContactTitle,
                            ContactTel = contactDto.ContactTel,
                            ContactCell = contactDto.ContactCell,
                            ContactEmail = contactDto.ContactEmail,
                            IsPrimary = contactDto.IsPrimary,
                            CreatedBy = GetCurrentUserId(),
                            CreatedAt = DateTime.Now,
                            UpdatedBy = GetCurrentUserId(),
                            UpdatedAt = DateTime.Now
                        };

                        await _repository.CreateContactAsync(contact);
                    }
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改客戶失敗: {customerId}", ex);
            throw;
        }
    }

    public async Task DeleteCustomerAsync(string customerId)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(customerId);
            if (entity == null)
            {
                throw new InvalidOperationException($"客戶不存在: {customerId}");
            }

            await _repository.DeleteAsync(customerId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除客戶失敗: {customerId}", ex);
            throw;
        }
    }

    public async Task BatchDeleteCustomersAsync(BatchDeleteCustomerDto dto)
    {
        try
        {
            foreach (var customerId in dto.CustomerIds)
            {
                await DeleteCustomerAsync(customerId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除客戶失敗", ex);
            throw;
        }
    }

    public async Task<GuiIdValidationResultDto> ValidateGuiIdAsync(ValidateGuiIdDto dto)
    {
        try
        {
            // 基本格式驗證
            if (string.IsNullOrWhiteSpace(dto.GuiId))
            {
                return new GuiIdValidationResultDto
                {
                    IsValid = false,
                    Message = "統一編號不能為空"
                };
            }

            // 統一編號驗證 (8碼數字)
            if (dto.GuiType == "1")
            {
                if (dto.GuiId.Length != 8 || !dto.GuiId.All(char.IsDigit))
                {
                    return new GuiIdValidationResultDto
                    {
                        IsValid = false,
                        Message = "統一編號必須為8碼數字"
                    };
                }

                // 檢查是否已存在
                var exists = await _repository.ExistsByGuiIdAsync(dto.GuiId);
                if (exists)
                {
                    return new GuiIdValidationResultDto
                    {
                        IsValid = false,
                        Message = "統一編號已存在"
                    };
                }
            }
            // 身份證字號驗證 (10碼，第一碼為英文字母)
            else if (dto.GuiType == "2")
            {
                if (dto.GuiId.Length != 10 || !char.IsLetter(dto.GuiId[0]) || !dto.GuiId.Substring(1).All(char.IsDigit))
                {
                    return new GuiIdValidationResultDto
                    {
                        IsValid = false,
                        Message = "身份證字號格式錯誤"
                    };
                }
            }

            return new GuiIdValidationResultDto
            {
                IsValid = true,
                Message = "驗證通過"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"驗證統一編號失敗: {dto.GuiId}", ex);
            throw;
        }
    }

    private CustomerDto MapToDto(Customer entity)
    {
        return new CustomerDto
        {
            CustomerId = entity.CustomerId,
            GuiId = entity.GuiId,
            GuiType = entity.GuiType,
            CustomerName = entity.CustomerName,
            CustomerNameE = entity.CustomerNameE,
            ShortName = entity.ShortName,
            ContactStaff = entity.ContactStaff,
            HomeTel = entity.HomeTel,
            CompTel = entity.CompTel,
            Fax = entity.Fax,
            Cell = entity.Cell,
            Email = entity.Email,
            Sex = entity.Sex,
            Title = entity.Title,
            City = entity.City,
            Canton = entity.Canton,
            Addr = entity.Addr,
            TaxAddr = entity.TaxAddr,
            DelyAddr = entity.DelyAddr,
            PostId = entity.PostId,
            DiscountYn = entity.DiscountYn,
            DiscountNo = entity.DiscountNo,
            SalesId = entity.SalesId,
            TransDate = entity.TransDate,
            TransNo = entity.TransNo,
            AccAmt = entity.AccAmt,
            MonthlyYn = entity.MonthlyYn,
            Status = entity.Status,
            Notes = entity.Notes,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private void ValidateCreateDto(CreateCustomerDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.CustomerId))
        {
            throw new ArgumentException("客戶編號不能為空");
        }

        if (string.IsNullOrWhiteSpace(dto.CustomerName))
        {
            throw new ArgumentException("客戶名稱不能為空");
        }

        if (!string.IsNullOrEmpty(dto.GuiType) && dto.GuiType != "1" && dto.GuiType != "2" && dto.GuiType != "3")
        {
            throw new ArgumentException("識別類型必須為 1(統一編號)、2(身份證字號) 或 3(自編編號)");
        }

        if (dto.Status != "A" && dto.Status != "I")
        {
            throw new ArgumentException("狀態值必須為 A(啟用) 或 I(停用)");
        }
    }

    public async Task<PagedResult<CustomerDto>> AdvancedQueryAsync(CustomerAdvancedQueryDto query)
    {
        try
        {
            var repositoryQuery = new CustomerAdvancedQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                CustomerId = query.Filters?.CustomerId,
                CustomerName = query.Filters?.CustomerName,
                GuiId = query.Filters?.GuiId,
                GuiType = query.Filters?.GuiType,
                ContactStaff = query.Filters?.ContactStaff,
                CompTel = query.Filters?.CompTel,
                Cell = query.Filters?.Cell,
                Email = query.Filters?.Email,
                City = query.Filters?.City,
                Canton = query.Filters?.Canton,
                SalesId = query.Filters?.SalesId,
                Status = query.Filters?.Status,
                DiscountYn = query.Filters?.DiscountYn,
                MonthlyYn = query.Filters?.MonthlyYn,
                TransDateFrom = query.Filters?.TransDateFrom ?? query.DateRange?.From,
                TransDateTo = query.Filters?.TransDateTo ?? query.DateRange?.To,
                AccAmtFrom = query.Filters?.AccAmtFrom ?? query.AmountRange?.From,
                AccAmtTo = query.Filters?.AccAmtTo ?? query.AmountRange?.To
            };

            var result = await _repository.AdvancedQueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<CustomerDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("進階查詢客戶列表失敗", ex);
            throw;
        }
    }

    public async Task<List<CustomerSearchResultDto>> SearchAsync(CustomerSearchDto query)
    {
        try
        {
            var customers = await _repository.SearchAsync(query.Keyword, query.Limit);

            return customers.Select(x => new CustomerSearchResultDto
            {
                CustomerId = x.CustomerId,
                CustomerName = x.CustomerName,
                GuiId = x.GuiId,
                ShortName = x.ShortName,
                CompTel = x.CompTel,
                Cell = x.Cell
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"快速搜尋客戶失敗: {query.Keyword}", ex);
            throw;
        }
    }

    public async Task<PagedResult<CustomerTransactionDto>> GetTransactionsAsync(CustomerTransactionQueryDto query)
    {
        try
        {
            var repositoryQuery = new CustomerTransactionQuery
            {
                CustomerId = query.CustomerId,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                DateFrom = query.DateFrom,
                DateTo = query.DateTo
            };

            var result = await _repository.GetTransactionsAsync(repositoryQuery);

            var dtos = result.Items.Select(x => new CustomerTransactionDto
            {
                TransactionId = x.TransactionId,
                CustomerId = x.CustomerId,
                TransactionDate = x.TransactionDate,
                TransactionNo = x.TransactionNo,
                TransactionType = x.TransactionType,
                Amount = x.Amount,
                Status = x.Status,
                Notes = x.Notes,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt
            }).ToList();

            return new PagedResult<CustomerTransactionDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢客戶交易記錄失敗: {query.CustomerId}", ex);
            throw;
        }
    }

    public async Task<QueryHistoryDto> SaveQueryHistoryAsync(SaveQueryHistoryDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException("無法取得使用者資訊");
            }

            var history = new QueryHistory
            {
                HistoryId = Guid.NewGuid(),
                UserId = userId,
                ModuleCode = "CUS5120",
                QueryName = dto.QueryName,
                QueryConditions = System.Text.Json.JsonSerializer.Serialize(dto.QueryConditions),
                IsFavorite = dto.IsFavorite,
                CreatedBy = userId,
                CreatedAt = DateTime.Now
            };

            var result = await _repository.SaveQueryHistoryAsync(history);

            return new QueryHistoryDto
            {
                HistoryId = result.HistoryId,
                UserId = result.UserId,
                ModuleCode = result.ModuleCode,
                QueryName = result.QueryName,
                QueryConditions = result.QueryConditions,
                IsFavorite = result.IsFavorite,
                CreatedBy = result.CreatedBy,
                CreatedAt = result.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("儲存查詢歷史記錄失敗", ex);
            throw;
        }
    }

    public async Task<List<QueryHistoryDto>> GetQueryHistoryAsync(string moduleCode)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException("無法取得使用者資訊");
            }

            var histories = await _repository.GetQueryHistoryAsync(userId, moduleCode);

            return histories.Select(x => new QueryHistoryDto
            {
                HistoryId = x.HistoryId,
                UserId = x.UserId,
                ModuleCode = x.ModuleCode,
                QueryName = x.QueryName,
                QueryConditions = x.QueryConditions,
                IsFavorite = x.IsFavorite,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"取得查詢歷史記錄失敗: {moduleCode}", ex);
            throw;
        }
    }

    public async Task DeleteQueryHistoryAsync(Guid historyId)
    {
        try
        {
            await _repository.DeleteQueryHistoryAsync(historyId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除查詢歷史記錄失敗: {historyId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<CustomerReportDto>> GetReportAsync(CustomerReportQueryDto query)
    {
        try
        {
            // 使用視圖查詢報表資料
            var sql = @"
                SELECT * FROM V_CUS5130_Report
                WHERE 1=1";

            var parameters = new Dapper.DynamicParameters();

            if (!string.IsNullOrEmpty(query.Filters?.CustomerId))
            {
                sql += " AND CustomerId LIKE @CustomerId";
                parameters.Add("CustomerId", $"%{query.Filters.CustomerId}%");
            }

            if (!string.IsNullOrEmpty(query.Filters?.CustomerName))
            {
                sql += " AND CustomerName LIKE @CustomerName";
                parameters.Add("CustomerName", $"%{query.Filters.CustomerName}%");
            }

            if (!string.IsNullOrEmpty(query.Filters?.GuiId))
            {
                sql += " AND GuiId LIKE @GuiId";
                parameters.Add("GuiId", $"%{query.Filters.GuiId}%");
            }

            if (!string.IsNullOrEmpty(query.Filters?.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Filters.Status);
            }

            if (!string.IsNullOrEmpty(query.Filters?.City))
            {
                sql += " AND City = @City";
                parameters.Add("City", query.Filters.City);
            }

            if (!string.IsNullOrEmpty(query.Filters?.Canton))
            {
                sql += " AND Canton = @Canton";
                parameters.Add("Canton", query.Filters.Canton);
            }

            if (!string.IsNullOrEmpty(query.Filters?.MonthlyYn))
            {
                sql += " AND MonthlyYn = @MonthlyYn";
                parameters.Add("MonthlyYn", query.Filters.MonthlyYn);
            }

            if (query.Filters?.TransDateFrom.HasValue == true)
            {
                sql += " AND TransDate >= @TransDateFrom";
                parameters.Add("TransDateFrom", query.Filters.TransDateFrom.Value);
            }

            if (query.Filters?.TransDateTo.HasValue == true)
            {
                sql += " AND TransDate <= @TransDateTo";
                parameters.Add("TransDateTo", query.Filters.TransDateTo.Value);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "CustomerId" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            using var connection = _connectionFactory.CreateConnection();
            var items = await connection.QueryAsync<CustomerReportDto>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM V_CUS5130_Report
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.Filters?.CustomerId))
            {
                countSql += " AND CustomerId LIKE @CustomerId";
                countParameters.Add("CustomerId", $"%{query.Filters.CustomerId}%");
            }
            if (!string.IsNullOrEmpty(query.Filters?.CustomerName))
            {
                countSql += " AND CustomerName LIKE @CustomerName";
                countParameters.Add("CustomerName", $"%{query.Filters.CustomerName}%");
            }
            if (!string.IsNullOrEmpty(query.Filters?.GuiId))
            {
                countSql += " AND GuiId LIKE @GuiId";
                countParameters.Add("GuiId", $"%{query.Filters.GuiId}%");
            }
            if (!string.IsNullOrEmpty(query.Filters?.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Filters.Status);
            }
            if (!string.IsNullOrEmpty(query.Filters?.City))
            {
                countSql += " AND City = @City";
                countParameters.Add("City", query.Filters.City);
            }
            if (!string.IsNullOrEmpty(query.Filters?.Canton))
            {
                countSql += " AND Canton = @Canton";
                countParameters.Add("Canton", query.Filters.Canton);
            }
            if (!string.IsNullOrEmpty(query.Filters?.MonthlyYn))
            {
                countSql += " AND MonthlyYn = @MonthlyYn";
                countParameters.Add("MonthlyYn", query.Filters.MonthlyYn);
            }
            if (query.Filters?.TransDateFrom.HasValue == true)
            {
                countSql += " AND TransDate >= @TransDateFrom";
                countParameters.Add("TransDateFrom", query.Filters.TransDateFrom.Value);
            }
            if (query.Filters?.TransDateTo.HasValue == true)
            {
                countSql += " AND TransDate <= @TransDateTo";
                countParameters.Add("TransDateTo", query.Filters.TransDateTo.Value);
            }

            var totalCount = await connection.QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<CustomerReportDto>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢客戶報表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportReportToExcelAsync(CustomerReportQueryDto query)
    {
        try
        {
            // 查詢所有資料（不分頁）
            var allDataQuery = new CustomerReportQueryDto
            {
                PageIndex = 1,
                PageSize = int.MaxValue,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                Filters = query.Filters
            };

            var result = await GetReportAsync(allDataQuery);

            // 定義匯出欄位
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "CustomerId", DisplayName = "客戶編號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "CustomerName", DisplayName = "客戶名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GuiId", DisplayName = "統一編號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GuiType", DisplayName = "統一編號類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ContactStaff", DisplayName = "聯絡人", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "CompTel", DisplayName = "公司電話", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Cell", DisplayName = "手機", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Email", DisplayName = "電子郵件", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "City", DisplayName = "城市", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Canton", DisplayName = "區域", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Addr", DisplayName = "地址", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Status", DisplayName = "狀態", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "TransDate", DisplayName = "交易日期", DataType = ExportDataType.Date },
                new ExportColumn { PropertyName = "AccAmt", DisplayName = "應收金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "MonthlyYn", DisplayName = "月結", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "TransactionCount", DisplayName = "交易筆數", DataType = ExportDataType.Number },
                new ExportColumn { PropertyName = "TotalAmount", DisplayName = "總金額", DataType = ExportDataType.Decimal }
            };

            return _exportHelper.ExportToExcel(result.Items, columns, "客戶報表", "客戶報表 (CUS5130)");
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出客戶報表到 Excel 失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportReportToPdfAsync(CustomerReportQueryDto query)
    {
        try
        {
            // 查詢所有資料（不分頁）
            var allDataQuery = new CustomerReportQueryDto
            {
                PageIndex = 1,
                PageSize = int.MaxValue,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                Filters = query.Filters
            };

            var result = await GetReportAsync(allDataQuery);

            // 定義匯出欄位
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "CustomerId", DisplayName = "客戶編號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "CustomerName", DisplayName = "客戶名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GuiId", DisplayName = "統一編號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GuiType", DisplayName = "統一編號類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ContactStaff", DisplayName = "聯絡人", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "CompTel", DisplayName = "公司電話", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Cell", DisplayName = "手機", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Email", DisplayName = "電子郵件", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "City", DisplayName = "城市", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Canton", DisplayName = "區域", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Addr", DisplayName = "地址", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Status", DisplayName = "狀態", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "TransDate", DisplayName = "交易日期", DataType = ExportDataType.Date },
                new ExportColumn { PropertyName = "AccAmt", DisplayName = "應收金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "MonthlyYn", DisplayName = "月結", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "TransactionCount", DisplayName = "交易筆數", DataType = ExportDataType.Number },
                new ExportColumn { PropertyName = "TotalAmount", DisplayName = "總金額", DataType = ExportDataType.Decimal }
            };

            return _exportHelper.ExportToPdf(result.Items, columns, "客戶報表 (CUS5130)");
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出客戶報表到 PDF 失敗", ex);
            throw;
        }
    }
}

