using ErpCore.Application.DTOs.CustomerInvoice;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.CustomerInvoice;
using ErpCore.Infrastructure.Repositories.CustomerInvoice;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.CustomerInvoice;

/// <summary>
/// 客戶資料服務實作 (SYS2000 - 客戶資料維護)
/// </summary>
public class CustomerDataService : BaseService, ICustomerDataService
{
    private readonly ICustomerDataRepository _repository;

    public CustomerDataService(
        ICustomerDataRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<CustomerDataDto>> GetCustomersAsync(CustomerDataQueryDto query)
    {
        try
        {
            var repositoryQuery = new CustomerDataQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                CustomerId = query.CustomerId,
                CustomerName = query.CustomerName,
                CustomerType = query.CustomerType,
                TaxId = query.TaxId,
                Status = query.Status
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = new List<CustomerDataDto>();
            foreach (var item in result.Items)
            {
                var contacts = await _repository.GetContactsByCustomerIdAsync(item.CustomerId);
                var addresses = await _repository.GetAddressesByCustomerIdAsync(item.CustomerId);
                var bankAccounts = await _repository.GetBankAccountsByCustomerIdAsync(item.CustomerId);

                dtos.Add(MapToDto(item, contacts, addresses, bankAccounts));
            }

            return new PagedResult<CustomerDataDto>
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

    public async Task<CustomerDataDto> GetCustomerByIdAsync(string customerId)
    {
        try
        {
            var customer = await _repository.GetByIdAsync(customerId);
            if (customer == null)
            {
                throw new KeyNotFoundException($"客戶不存在: {customerId}");
            }

            var contacts = await _repository.GetContactsByCustomerIdAsync(customerId);
            var addresses = await _repository.GetAddressesByCustomerIdAsync(customerId);
            var bankAccounts = await _repository.GetBankAccountsByCustomerIdAsync(customerId);

            return MapToDto(customer, contacts, addresses, bankAccounts);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢客戶失敗: {customerId}", ex);
            throw;
        }
    }

    public async Task<string> CreateCustomerAsync(CreateCustomerDataDto dto)
    {
        try
        {
            // 檢查客戶編號是否已存在
            if (await _repository.ExistsAsync(dto.CustomerId))
            {
                throw new InvalidOperationException($"客戶編號已存在: {dto.CustomerId}");
            }

            var customer = new CustomerData
            {
                CustomerId = dto.CustomerId,
                CustomerName = dto.CustomerName,
                CustomerType = dto.CustomerType,
                TaxId = dto.TaxId,
                ContactPerson = dto.ContactPerson,
                ContactPhone = dto.ContactPhone,
                ContactEmail = dto.ContactEmail,
                ContactFax = dto.ContactFax,
                Address = dto.Address,
                CityId = dto.CityId,
                ZoneId = dto.ZoneId,
                ZipCode = dto.ZipCode,
                PaymentTerm = dto.PaymentTerm,
                CreditLimit = dto.CreditLimit,
                CurrencyId = dto.CurrencyId,
                Status = dto.Status,
                Memo = dto.Memo,
                CreatedBy = GetCurrentUserId(),
                UpdatedBy = GetCurrentUserId()
            };

            var contacts = dto.Contacts?.Select(x => new CustomerContact
            {
                ContactName = x.ContactName,
                ContactTitle = x.ContactTitle,
                ContactPhone = x.ContactPhone,
                ContactMobile = x.ContactMobile,
                ContactEmail = x.ContactEmail,
                ContactFax = x.ContactFax,
                IsPrimary = x.IsPrimary,
                Memo = x.Memo,
                CreatedBy = GetCurrentUserId(),
                UpdatedBy = GetCurrentUserId()
            }).ToList();

            var addresses = dto.Addresses?.Select(x => new CustomerAddress
            {
                AddressType = x.AddressType,
                Address = x.Address,
                CityId = x.CityId,
                ZoneId = x.ZoneId,
                ZipCode = x.ZipCode,
                IsDefault = x.IsDefault,
                Memo = x.Memo,
                CreatedBy = GetCurrentUserId(),
                UpdatedBy = GetCurrentUserId()
            }).ToList();

            var bankAccounts = dto.BankAccounts?.Select(x => new CustomerBankAccount
            {
                BankId = x.BankId,
                AccountNo = x.AccountNo,
                AccountName = x.AccountName,
                IsDefault = x.IsDefault,
                Memo = x.Memo,
                CreatedBy = GetCurrentUserId(),
                UpdatedBy = GetCurrentUserId()
            }).ToList();

            await _repository.CreateAsync(customer, contacts, addresses, bankAccounts);

            _logger.LogInfo($"新增客戶成功: {dto.CustomerId}");
            return customer.CustomerId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增客戶失敗: {dto.CustomerId}", ex);
            throw;
        }
    }

    public async Task UpdateCustomerAsync(string customerId, UpdateCustomerDataDto dto)
    {
        try
        {
            var customer = await _repository.GetByIdAsync(customerId);
            if (customer == null)
            {
                throw new KeyNotFoundException($"客戶不存在: {customerId}");
            }

            customer.CustomerName = dto.CustomerName;
            customer.CustomerType = dto.CustomerType;
            customer.TaxId = dto.TaxId;
            customer.ContactPerson = dto.ContactPerson;
            customer.ContactPhone = dto.ContactPhone;
            customer.ContactEmail = dto.ContactEmail;
            customer.ContactFax = dto.ContactFax;
            customer.Address = dto.Address;
            customer.CityId = dto.CityId;
            customer.ZoneId = dto.ZoneId;
            customer.ZipCode = dto.ZipCode;
            customer.PaymentTerm = dto.PaymentTerm;
            customer.CreditLimit = dto.CreditLimit;
            customer.CurrencyId = dto.CurrencyId;
            customer.Status = dto.Status;
            customer.Memo = dto.Memo;
            customer.UpdatedBy = GetCurrentUserId();

            var contacts = dto.Contacts?.Select(x => new CustomerContact
            {
                ContactName = x.ContactName,
                ContactTitle = x.ContactTitle,
                ContactPhone = x.ContactPhone,
                ContactMobile = x.ContactMobile,
                ContactEmail = x.ContactEmail,
                ContactFax = x.ContactFax,
                IsPrimary = x.IsPrimary,
                Memo = x.Memo,
                CreatedBy = GetCurrentUserId(),
                UpdatedBy = GetCurrentUserId()
            }).ToList();

            var addresses = dto.Addresses?.Select(x => new CustomerAddress
            {
                AddressType = x.AddressType,
                Address = x.Address,
                CityId = x.CityId,
                ZoneId = x.ZoneId,
                ZipCode = x.ZipCode,
                IsDefault = x.IsDefault,
                Memo = x.Memo,
                CreatedBy = GetCurrentUserId(),
                UpdatedBy = GetCurrentUserId()
            }).ToList();

            var bankAccounts = dto.BankAccounts?.Select(x => new CustomerBankAccount
            {
                BankId = x.BankId,
                AccountNo = x.AccountNo,
                AccountName = x.AccountName,
                IsDefault = x.IsDefault,
                Memo = x.Memo,
                CreatedBy = GetCurrentUserId(),
                UpdatedBy = GetCurrentUserId()
            }).ToList();

            await _repository.UpdateAsync(customer, contacts, addresses, bankAccounts);

            _logger.LogInfo($"修改客戶成功: {customerId}");
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
            var customer = await _repository.GetByIdAsync(customerId);
            if (customer == null)
            {
                throw new KeyNotFoundException($"客戶不存在: {customerId}");
            }

            await _repository.DeleteAsync(customerId);

            _logger.LogInfo($"刪除客戶成功: {customerId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除客戶失敗: {customerId}", ex);
            throw;
        }
    }

    public async Task BatchDeleteCustomersAsync(BatchDeleteCustomerDataDto dto)
    {
        try
        {
            foreach (var customerId in dto.CustomerIds)
            {
                await DeleteCustomerAsync(customerId);
            }

            _logger.LogInfo($"批次刪除客戶成功: {dto.CustomerIds.Count} 筆");
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除客戶失敗", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string customerId)
    {
        try
        {
            return await _repository.ExistsAsync(customerId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查客戶編號是否存在失敗: {customerId}", ex);
            throw;
        }
    }

    private CustomerDataDto MapToDto(CustomerData customer, IEnumerable<CustomerContact> contacts, IEnumerable<CustomerAddress> addresses, IEnumerable<CustomerBankAccount> bankAccounts)
    {
        return new CustomerDataDto
        {
            TKey = customer.TKey,
            CustomerId = customer.CustomerId,
            CustomerName = customer.CustomerName,
            CustomerType = customer.CustomerType,
            TaxId = customer.TaxId,
            ContactPerson = customer.ContactPerson,
            ContactPhone = customer.ContactPhone,
            ContactEmail = customer.ContactEmail,
            ContactFax = customer.ContactFax,
            Address = customer.Address,
            CityId = customer.CityId,
            ZoneId = customer.ZoneId,
            ZipCode = customer.ZipCode,
            PaymentTerm = customer.PaymentTerm,
            CreditLimit = customer.CreditLimit,
            CurrencyId = customer.CurrencyId,
            Status = customer.Status,
            Memo = customer.Memo,
            CreatedBy = customer.CreatedBy,
            CreatedAt = customer.CreatedAt,
            UpdatedBy = customer.UpdatedBy,
            UpdatedAt = customer.UpdatedAt,
            Contacts = contacts.Select(x => new CustomerContactDto
            {
                TKey = x.TKey,
                CustomerId = x.CustomerId,
                ContactName = x.ContactName,
                ContactTitle = x.ContactTitle,
                ContactPhone = x.ContactPhone,
                ContactMobile = x.ContactMobile,
                ContactEmail = x.ContactEmail,
                ContactFax = x.ContactFax,
                IsPrimary = x.IsPrimary,
                Memo = x.Memo
            }).ToList(),
            Addresses = addresses.Select(x => new CustomerAddressDto
            {
                TKey = x.TKey,
                CustomerId = x.CustomerId,
                AddressType = x.AddressType,
                Address = x.Address,
                CityId = x.CityId,
                ZoneId = x.ZoneId,
                ZipCode = x.ZipCode,
                IsDefault = x.IsDefault,
                Memo = x.Memo
            }).ToList(),
            BankAccounts = bankAccounts.Select(x => new CustomerBankAccountDto
            {
                TKey = x.TKey,
                CustomerId = x.CustomerId,
                BankId = x.BankId,
                AccountNo = x.AccountNo,
                AccountName = x.AccountName,
                IsDefault = x.IsDefault,
                Memo = x.Memo
            }).ToList()
        };
    }
}

