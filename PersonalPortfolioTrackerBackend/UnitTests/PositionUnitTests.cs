using Moq;
using Xunit;
using PersonalPortfolioTracker.DTOs.Request;
using PersonalPortfolioTracker.DTOs.Response;
using PersonalPortfolioTracker.Integrations.Interfaces;
using PersonalPortfolioTracker.Models;
using PersonalPortfolioTracker.Repositories.Interfaces;
using PersonalPortfolioTracker.Services.Implementations;

namespace UnitTests;

public class PositionUnitTests
{
    private Mock<IStockRepository> _mockStockRepository;
    private Mock<IPositionRepository> _mockPositionRepository;
    private Mock<ITransactionRepository> _mockTransactionRepository;
    private Mock<IDividendRepository> _mockDividendRepository;
    private Mock<ICapitalChangeRepository> _mockCapitalChangeRepository;
    private Mock<IStockInfoHandler> _mockStockInfoHandler;
    private Service _service;
    private Stock _stock;

    public PositionUnitTests()
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

    [Fact]
    public async Task GetAllPositionsAsync_NoPosition_ReturnsEmptyDto()
    {
        // Arrange
        IEnumerable<Position> positions = new List<Position>();

        _mockPositionRepository.Setup(p => p.GetAllPositionsAsync()).ReturnsAsync(positions);

        // Act
        IEnumerable<PositionResponseDto> positionResponseDtos = await _service.GetAllPositionsAsync();

        // Assert
        _mockPositionRepository.Verify(p => p.GetAllPositionsAsync(), Times.Once());
        _mockStockInfoHandler.Verify(h => h.GetStockInfoAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        _mockStockRepository.Verify(s => s.UpdateStocksAsync(It.IsAny<IEnumerable<Stock>>()), Times.Never());
        
        Assert.NotNull(positionResponseDtos);
        Assert.Empty(positionResponseDtos);
    }

    [Fact]
    public async Task GetAllPositionsAsync_PositionClosed_DtoHasFinalizedTotalReturnValue()
    {
        // Arrange
        DateOnly positionOpenedDate = DateOnly.FromDateTime(DateTime.Now).AddDays(-2);
        DateOnly positionClosedDate = DateOnly.FromDateTime(DateTime.Now).AddDays(-1);

        IEnumerable<Position> positions = new List<Position>()
        {
            new Position()
            {
                PositionOpenedDate = positionOpenedDate,
                PositionClosedDate = positionClosedDate,
                IsPositionClosed = true,
                NoOfSharesHeld = 0,
                TotalPurchaseCost = 100.00m,
                TotalDividendsReceived = 10.00m,
                TotalNetSalesProceeds = 110.00m,
                StockId = _stock.Id,
                Stock = _stock
            }
        };

        _mockPositionRepository.Setup(p => p.GetAllPositionsAsync()).ReturnsAsync(positions);

        // Act
        IEnumerable<PositionResponseDto> positionResponseDtos = await _service.GetAllPositionsAsync();

        // Assert
        _mockPositionRepository.Verify(p => p.GetAllPositionsAsync(), Times.Once());
        _mockStockInfoHandler.Verify(h => h.GetStockInfoAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        _mockStockRepository.Verify(s => s.UpdateStocksAsync(It.IsAny<IEnumerable<Stock>>()), Times.Never());

        Assert.NotNull(positionResponseDtos);
        Assert.NotEmpty(positionResponseDtos);
        Assert.Equal(positionClosedDate.ToString("dd-MMM-yyyy"), positionResponseDtos.First().PositionClosedDate);
        Assert.Equal(20.00m, positionResponseDtos.First().FinalizedTotalReturn);
        Assert.Equal(0m, positionResponseDtos.First().StockPrice);
        Assert.Null(positionResponseDtos.First().StockPriceLastUpdated);
        Assert.Equal(0m, positionResponseDtos.First().UnrealizedValueOfSharesHeldBeforeFinalSalesExpenses);
        Assert.Equal(0m, positionResponseDtos.First().HypotheticalTotalReturnBeforeFinalSalesExpenses);
    }

    [Fact]
    public async Task GetAllPositionsAsync_PositionOpenedAndExternalApiReturnsValue_DtoHasUpdatedStockPriceAndHypotheticalTotalReturnValue()
    {
        // Arrange
        DateOnly positionOpenedDate = DateOnly.FromDateTime(DateTime.Now).AddDays(-3);
        DateOnly oldPreviousCloseDate = DateOnly.FromDateTime(DateTime.Now).AddDays(-2);
        DateOnly updatedPreviousCloseDate = DateOnly.FromDateTime(DateTime.Now).AddDays(-1);

        _stock.LastUpdated = oldPreviousCloseDate;

        IEnumerable<Position> positions = new List<Position>()
        {
            new Position()
            {
                PositionOpenedDate = positionOpenedDate,
                IsPositionClosed = false,
                NoOfSharesHeld = 10,
                TotalPurchaseCost = 100.00m,
                TotalDividendsReceived = 10.00m,
                TotalNetSalesProceeds = 110.00m,
                StockId = _stock.Id,
                Stock = _stock
            }
        };

        _mockPositionRepository.Setup(p => p.GetAllPositionsAsync()).ReturnsAsync(positions);

        StockInfoDto[] stockInfoDtos =
        [
            new StockInfoDto()
            {
                PreviousClose = 9.99m,
                PreviousCloseDate = updatedPreviousCloseDate.ToString("yyyy-MM-dd")
            }
        ];

        _mockStockInfoHandler.Setup(h => h.GetStockInfoAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(stockInfoDtos);

        // Act
        IEnumerable<PositionResponseDto> positionResponseDtos = await _service.GetAllPositionsAsync();

        // Assert
        _mockPositionRepository.Verify(p => p.GetAllPositionsAsync(), Times.Once());
        _mockStockInfoHandler.Verify(h => h.GetStockInfoAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        _mockStockRepository.Verify(s => s.UpdateStocksAsync(It.IsAny<IEnumerable<Stock>>()), Times.Once());
        
        Assert.NotNull(positionResponseDtos);
        Assert.NotEmpty(positionResponseDtos);
        Assert.Null(positionResponseDtos.First().PositionClosedDate);
        Assert.Equal(0.00m, positionResponseDtos.First().FinalizedTotalReturn);
        Assert.Equal(9.99m, positionResponseDtos.First().StockPrice);
        Assert.Equal(updatedPreviousCloseDate.ToString("dd-MMM-yyyy"), positionResponseDtos.First().StockPriceLastUpdated);
        Assert.Equal(99.90m, positionResponseDtos.First().UnrealizedValueOfSharesHeldBeforeFinalSalesExpenses);
        Assert.Equal(119.90m, positionResponseDtos.First().HypotheticalTotalReturnBeforeFinalSalesExpenses);
    }

    [Fact]
    public async Task GetAllPositionsAsync_PositionOpenedAndExternalApiReturnsNull_DtoHasOldStockPriceAndHypotheticalTotalReturnValue()
    {
        // Arrange
        DateOnly positionOpenedDate = DateOnly.FromDateTime(DateTime.Now).AddDays(-3);
        DateOnly oldPreviousCloseDate = DateOnly.FromDateTime(DateTime.Now).AddDays(-2);

        _stock.LastUpdated = oldPreviousCloseDate;

        IEnumerable<Position> positions = new List<Position>()
        {
            new Position()
            {
                PositionOpenedDate = positionOpenedDate,
                IsPositionClosed = false,
                NoOfSharesHeld = 10,
                TotalPurchaseCost = 100.00m,
                TotalDividendsReceived = 10.00m,
                TotalNetSalesProceeds = 110.00m,
                StockId = _stock.Id,
                Stock = _stock
            }
        };

        _mockPositionRepository.Setup(p => p.GetAllPositionsAsync()).ReturnsAsync(positions);

        _mockStockInfoHandler.Setup(h => h.GetStockInfoAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((StockInfoDto[]?)null);

        // Act
        IEnumerable<PositionResponseDto> positionResponseDtos = await _service.GetAllPositionsAsync();

        // Assert
        _mockPositionRepository.Verify(p => p.GetAllPositionsAsync(), Times.Once());
        _mockStockInfoHandler.Verify(h => h.GetStockInfoAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        _mockStockRepository.Verify(s => s.UpdateStocksAsync(It.IsAny<IEnumerable<Stock>>()), Times.Never());

        Assert.NotNull(positionResponseDtos);
        Assert.NotEmpty(positionResponseDtos);
        Assert.Null(positionResponseDtos.First().PositionClosedDate);
        Assert.Equal(0.00m, positionResponseDtos.First().FinalizedTotalReturn);
        Assert.Equal(1.00m, positionResponseDtos.First().StockPrice);
        Assert.Equal(oldPreviousCloseDate.ToString("dd-MMM-yyyy"), positionResponseDtos.First().StockPriceLastUpdated);
        Assert.Equal(10.00m, positionResponseDtos.First().UnrealizedValueOfSharesHeldBeforeFinalSalesExpenses);
        Assert.Equal(30.00m, positionResponseDtos.First().HypotheticalTotalReturnBeforeFinalSalesExpenses);
    }

    [Theory]
    [InlineData(0, true, 422, "Cannot open position. There are no shares being held.")]
    [InlineData(1, false, 200, "Position status has been updated.")]
    public async Task UpdatePositionStatusAsync_AttemptToOpenPositionWithNoSharesOnHand_DiscardPositionStatusChange(int noOfSharesHeld, bool expectedPositionStatusAfterOperation, int expectedStatusCode, string expectedMessage)
    {
        // Arrange
        Position position = new Position()
        {
            IsPositionClosed = true,
            NoOfSharesHeld = noOfSharesHeld,
            StockId = _stock.Id,
            Stock = _stock
        };

        _mockPositionRepository.Setup(p => p.GetPositionByIdAsync(It.IsAny<int>())).ReturnsAsync(position);

        UpdatePositionStatusRequestDto updatePositionStatusRequestDto = new UpdatePositionStatusRequestDto()
        {
            IsPositionClosed = false
        };

        // Act
        OperationResponseDto operationResponseDto = await _service.UpdatePositionStatusAsync(1, updatePositionStatusRequestDto);

        // Assert
        _mockPositionRepository.Verify(p => p.UpdatePositionAsync(It.IsAny<Position>()), noOfSharesHeld > 0 ? Times.Once() : Times.Never());

        Assert.Equal(expectedPositionStatusAfterOperation, position.IsPositionClosed);
        Assert.Equal(expectedStatusCode, operationResponseDto.StatusCode);
        Assert.Equal(expectedMessage, operationResponseDto.Message);
    }

    [Theory]
    [InlineData(1, false,  422, "Cannot close position. There are shares being held.")]
    [InlineData(0, true, 200, "Position status has been updated.")]
    public async Task UpdatePositionStatusAsync_AttemptToClosePositionWithSharesOnHand_DiscardPositionStatusChange(int noOfSharesHeld, bool expectedPositionStatusAfterOperation, int expectedStatusCode, string expectedMessage)
    {
        // Arrange
        Position position = new Position()
        {
            IsPositionClosed = false,
            NoOfSharesHeld = noOfSharesHeld,
            StockId = _stock.Id,
            Stock = _stock
        };

        _mockPositionRepository.Setup(p => p.GetPositionByIdAsync(It.IsAny<int>())).ReturnsAsync(position);

        UpdatePositionStatusRequestDto updatePositionStatusRequestDto = new UpdatePositionStatusRequestDto()
        {
            IsPositionClosed = true
        };

        // Act
        OperationResponseDto operationResponseDto = await _service.UpdatePositionStatusAsync(1, updatePositionStatusRequestDto);

        // Assert
        _mockPositionRepository.Verify(p => p.UpdatePositionAsync(It.IsAny<Position>()), noOfSharesHeld == 0 ? Times.Once() : Times.Never());

        Assert.Equal(expectedPositionStatusAfterOperation, position.IsPositionClosed);
        Assert.Equal(expectedStatusCode, operationResponseDto.StatusCode);
        Assert.Equal(expectedMessage, operationResponseDto.Message);
    }

    [Fact]
    public async Task UpdatePositionStatusAsync_PositionIdNotFound_DiscardPositionStatusChange()
    {
        // Arrange
        _mockPositionRepository.Setup(p => p.GetPositionByIdAsync(It.IsAny<int>())).ReturnsAsync((Position?)null);

        UpdatePositionStatusRequestDto updatePositionStatusRequestDto = new UpdatePositionStatusRequestDto()
        {
            IsPositionClosed = true
        };

        // Act
        OperationResponseDto operationResponseDto = await _service.UpdatePositionStatusAsync(-1, updatePositionStatusRequestDto);

        // Assert
        _mockPositionRepository.Verify(p => p.UpdatePositionAsync(It.IsAny<Position>()), Times.Never());

        Assert.Equal(404, operationResponseDto.StatusCode);
        Assert.Equal("The position record with ID -1 is not found.", operationResponseDto.Message);
    }
}