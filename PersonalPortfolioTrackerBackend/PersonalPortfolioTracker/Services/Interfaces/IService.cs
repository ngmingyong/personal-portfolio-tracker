using PersonalPortfolioTracker.DTOs.Request;
using PersonalPortfolioTracker.DTOs.Response;

namespace PersonalPortfolioTracker.Services.Interfaces
{
    public interface IService
    {
        Task<IEnumerable<PositionResponseDto>> GetAllPositionsAsync();
        Task<OperationResponseDto> UpdatePositionStatusAsync(int id, UpdatePositionStatusRequestDto updatePositionStatusRequestDto);
        Task<OperationResponseDto> DeletePositionAsync(int id);
        Task<OperationResponseDto> AddTransactionAsync(AddTransactionRequestDto addTransactionRequestDto);
        Task<OperationResponseDto> DeleteTransactionAsync(int id);
        Task<OperationResponseDto> AddDividendAsync(AddDividendRequestDto addDividendRequestDto);
        Task<OperationResponseDto> DeleteDividendAsync(int id);
        Task<OperationResponseDto> AddCapitalChangeAsync(AddCapitalChangeRequestDto addCapitalChangeRequestDto);
        Task<OperationResponseDto> DeleteCapitalChangeAsync(int id);
    }
}