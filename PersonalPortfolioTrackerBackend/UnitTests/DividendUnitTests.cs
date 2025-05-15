using Moq;
using Xunit;
using PersonalPortfolioTracker.DTOs.Request;
using PersonalPortfolioTracker.DTOs.Response;
using PersonalPortfolioTracker.Integrations.Interfaces;
using PersonalPortfolioTracker.Models;
using PersonalPortfolioTracker.Repositories.Interfaces;
using PersonalPortfolioTracker.Services.Implementations;

namespace UnitTests;

public class DividendUnitTests
{
    private Mock<IStockRepository> _mockStockRepository;
    private Mock<IPositionRepository> _mockPositionRepository;
    private Mock<ITransactionRepository> _mockTransactionRepository;
    private Mock<IDividendRepository> _mockDividendRepository;
    private Mock<ICapitalChangeRepository> _mockCapitalChangeRepository;
    private Mock<IStockInfoHandler> _mockStockInfoHandler;
    private Service _service;
    private Stock _stock;

    public DividendUnitTests()
    {
        _mockStockRepository = new Mock<IStockRepository>();
        _mockPositionRepository = new Mock<IPositionRepository>();
        _mockTransactionRepository = new Mock<ITransactionRepository>();
        _mockDividendRepository = new Mock<IDividendRepository>();
        _mockCapitalChangeRepository = new Mock<ICapitalChangeRepository>();
        _mockStockInfoHandler = new Mock<IStockInfoHandler>();

        _service = new Service(_mockStockRepository.Object, _mockPositionRepository.Object, _mockTransactionRepository.Object, _mockDividendRepository.Object, _mockCapitalChangeRepository.Object, _mockStockInfoHandler.Object);

        _stock = new Stock()
        {
            Id = 1,
            Code = "0000",
            Name = "CompanyName",
            Price = 1.00m,
            LastUpdated = DateOnly.FromDateTime(DateTime.Now)
        };
    }

    [Theory]
    [InlineData(0, 1.00, 0.00, 422, "Number of shares (0) eligible for dividend is not valid.")]
    [InlineData(10, -1.00, 0.00, 422, "Monetary amount (-1) is not valid.")]
    [InlineData(10, 1.00, -0.05, 422, "Monetary amount (-0.05) is not valid.")]
    public async Task AddDividendAsync_InvalidNoOfSharesEligibleDividendPerShareWithholdingTaxAmountReceived_DiscardDividendRecord(int noOfSharesEligible, decimal dividendPerShare, decimal withholdingTax, int expectedStatusCode, string expectedMessage)
    {
        // Arrange
        Position position = new Position()
        {
            IsPositionClosed = false,
            TotalDividendsReceived = 5.00m,
            StockId = _stock.Id,
            Stock = _stock
        };

        _mockPositionRepository.Setup(p => p.GetPositionByIdAsync(It.IsAny<int>())).ReturnsAsync(position);

        AddDividendRequestDto addDividendRequestDto = new AddDividendRequestDto()
        {
            PositionId = 1,
            EntitlementDate = DateTime.Now.ToString("dd-MMM-yyyy"),
            NoOfSharesEligible = noOfSharesEligible,
            DividendPerShare = dividendPerShare,
            IsSubjectToWithholdingTax = false,
            WithholdingTax = withholdingTax
        };

        // Act
        OperationResponseDto operationResponseDto = await _service.AddDividendAsync(addDividendRequestDto);

        // Assert
        _mockDividendRepository.Verify(d => d.AddDividendAsync(It.IsAny<Dividend>()), Times.Never());
        _mockPositionRepository.Verify(p => p.UpdatePositionAsync(It.IsAny<Position>()), Times.Never());

        Assert.Equal(5.00m, position.TotalDividendsReceived);
        Assert.Equal(expectedStatusCode, operationResponseDto.StatusCode);
        Assert.Equal(expectedMessage, operationResponseDto.Message);
    }

    [Theory]
    [InlineData(true, 422, "No position available.")]
    [InlineData(false, 200, "New dividend added.")]
    public async Task AddDividendAsync_AttemptToAddDividendToClosedPosition_DiscardDividendRecord(bool isPositionClosed, int expectedStatusCode, string expectedMessage)
    {
        // Arrange
        Position position = new Position()
        {
            IsPositionClosed = isPositionClosed,
            TotalDividendsReceived = 5.00m,
            StockId = _stock.Id,
            Stock = _stock
        };

        _mockPositionRepository.Setup(p => p.GetPositionByIdAsync(It.IsAny<int>())).ReturnsAsync(position);

        AddDividendRequestDto addDividendRequestDto = new AddDividendRequestDto()
        {
            PositionId = 1,
            EntitlementDate = DateTime.Now.ToString("dd-MMM-yyyy"),
            NoOfSharesEligible = 10,
            DividendPerShare = 1.00m,
            IsSubjectToWithholdingTax = false,
            WithholdingTax = 0.00m
        };

        // Act
        OperationResponseDto operationResponseDto = await _service.AddDividendAsync(addDividendRequestDto);

        // Assert
        _mockDividendRepository.Verify(d => d.AddDividendAsync(It.IsAny<Dividend>()), isPositionClosed ? Times.Never() : Times.Once());
        _mockPositionRepository.Verify(p => p.UpdatePositionAsync(It.IsAny<Position>()), isPositionClosed ? Times.Never() : Times.Once());

        Assert.Equal(isPositionClosed ? 5.00m : 15.00m, position.TotalDividendsReceived);
        Assert.Equal(expectedStatusCode, operationResponseDto.StatusCode);
        Assert.Equal(expectedMessage, operationResponseDto.Message);
    }
}