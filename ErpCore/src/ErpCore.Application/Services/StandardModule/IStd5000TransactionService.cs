using ErpCore.Application.DTOs.StandardModule;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.StandardModule;

/// <summary>
/// STD5000 交易服務介面 (SYS5310-SYS53C6 - 交易管理)
/// </summary>
public interface IStd5000TransactionService
{
    Task<PagedResult<Std5000TransactionDto>> GetStd5000TransactionListAsync(Std5000TransactionQueryDto query);
    Task<Std5000TransactionDto?> GetStd5000TransactionByIdAsync(long tKey);
    Task<Std5000TransactionDto?> GetStd5000TransactionByTransIdAsync(string transId);
    Task<long> CreateStd5000TransactionAsync(CreateStd5000TransactionDto dto);
    Task UpdateStd5000TransactionAsync(long tKey, UpdateStd5000TransactionDto dto);
    Task DeleteStd5000TransactionAsync(long tKey);
}

