using Moq;
using Xunit;
using PersonalPortfolioTracker.DTOs.Request;
using PersonalPortfolioTracker.DTOs.Response;
using PersonalPortfolioTracker.Integrations.Interfaces;
using PersonalPortfolioTracker.Models;
using PersonalPortfolioTracker.Repositories.Interfaces;
using PersonalPortfolioTracker.Services.Implementations;

namespace UnitTests;

public class TransactionUnitTests
{
    private Mock<IStockRepository> _mockStockRepository;
    private Mock<IPositionRepository> _mockPositionRepository;
    private Mock<ITransactionRepository> _mockTransactionRepository;
    private Mock<IDividendRepository> _mockDividendRepository;
    private Mock<ICapitalChangeRepository> _mockCapitalChangeRepository;
    private Mock<IStockInfoHandler> _mockStockInfoHandler;
    private Service _service;
    private Stock _stock;

    public TransactionUnitTests()
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
    [InlineData("INVALID_STOCK_CODE", "Buy", 1, 10.00, 1.20, 422, "The stock code INVALID_STOCK_CODE is not valid.")]
    [InlineData("VALID_STOCK_CODE", "INVALID_TRANSACTION_TYPE", 1, 10.00, 1.20, 422, "Transaction type INVALID_TRANSACTION_TYPE is not valid.")]
    [InlineData("VALID_STOCK_CODE", "Buy", 0, 10.00, 1.20, 422, "Number of shares (0) to be transacted is not valid.")]
    [InlineData("VALID_STOCK_CODE", "Buy", 1, -10.00, 1.20, 422, "Monetary amount (-10) is not valid.")]
    [InlineData("VALID_STOCK_CODE", "Buy", 1, 10.00, -1.20, 422, "Monetary amount (-1.2) is not valid.")]
    public async Task AddTransactionAsync_InvalidStockCodeTransactionTypeNoOfSharesPriceExpenses_DiscardTransactionRecord(string stockCode, string type, int noOfSharesTransacted, decimal transactedPricePerShare, decimal totalTransactionRelatedExpenses, int expectedStatusCode, string expectedMessage)
    {
        // Arrange
        AddTransactionRequestDto addTransactionRequestDto = new AddTransactionRequestDto()
        {
            StockCode = stockCode,
            Date = DateTime.Now.ToString("dd-MMM-yyyy"),
            Type = type,
            NoOfSharesTransacted = noOfSharesTransacted,
            TransactedPricePerShare = transactedPricePerShare,
            TotalTransactionRelatedExpenses = totalTransactionRelatedExpenses
        };

        _mockStockRepository.Setup(s => s.GetStockByCodeAsync(It.IsAny<string>())).ReturnsAsync((Stock?)null);

        _mockStockInfoHandler.Setup(h => h.GetStockInfoAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((StockInfoDto[]?)null);

        // Act
        OperationResponseDto operationResponseDto = await _service.AddTransactionAsync(addTransactionRequestDto);

        // Assert
        _mockStockRepository.Verify(s => s.GetStockByCodeAsync(It.IsAny<string>()), type == "Buy" && noOfSharesTransacted > 0 && transactedPricePerShare >= 0 && totalTransactionRelatedExpenses >= 0 ? Times.Once() : Times. Never());
        _mockStockInfoHandler.Verify(h => h.GetStockInfoAsync(It.IsAny<string>(), It.IsAny<string>()), type == "Buy" && noOfSharesTransacted > 0 && transactedPricePerShare >= 0 && totalTransactionRelatedExpenses >= 0 ? Times.Once() : Times.Never());
        _mockStockRepository.Verify(s => s.AddStockAsync(It.IsAny<Stock>()), Times.Never());
        _mockPositionRepository.Verify(p => p.GetPositionsByStockIdAsync(It.IsAny<int>()), Times.Never());
        _mockPositionRepository.Verify(p => p.UpdatePositionAsync(It.IsAny<Position>()), Times.Never());
        _mockPositionRepository.Verify(p => p.AddPositionAsync(It.IsAny<Position>()), Times.Never());
        _mockTransactionRepository.Verify(t => t.AddTransactionAsync(It.IsAny<Transaction>()), Times.Never());

        Assert.Equal(expectedStatusCode, operationResponseDto.StatusCode);
        Assert.Equal(expectedMessage, operationResponseDto.Message);
    }

    [Theory]
    [InlineData(true, true, 200, "New transaction added for stock code NEW_VALID_STOCK_CODE.")]
    [InlineData(false, true, 200, "New transaction added for stock code 0000.")]
    [InlineData(false, false, 200, "New transaction added for stock code 0000.")]
    public async Task AddTransactionAsync_AttempToBuy_ProceedToAddTransactionRecord(bool newStock, bool newOpenPosition, int expectedStatusCode, string expectedMessage)
    {
        // Arrange
        AddTransactionRequestDto addTransactionRequestDto = new AddTransactionRequestDto()
        {
            StockCode = newStock ? "NEW_VALID_STOCK_CODE" : "0000",
            Date = DateTime.Now.ToString("dd-MMM-yyyy"),
            Type = "Buy",
            NoOfSharesTransacted = 1,
            TransactedPricePerShare = 10.00m,
            TotalTransactionRelatedExpenses = 1.20m
        };

        StockInfoDto[]? stockInfoDtos = newStock ?
        [
            new StockInfoDto()
            {
                PreviousClose = 9.99m,
                PreviousCloseDate = DateTime.Now.ToString("yyyy-MM-dd")
            }
        ] : null;

        _mockStockRepository.Setup(s => s.GetStockByCodeAsync(It.IsAny<string>())).ReturnsAsync(newStock ? null : _stock);

        _mockStockInfoHandler.Setup(h => h.GetStockInfoAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(stockInfoDtos);

        int existingNoOfShares = 1;
        IEnumerable<Position> positions = newOpenPosition ? [] : new List<Position>()
        {
            new Position()
            {
                IsPositionClosed = false,
                NoOfSharesHeld = existingNoOfShares,
                TotalPurchaseCost = 5.00m,
                StockId = _stock.Id,
                Stock = _stock
            }
        };

        _mockPositionRepository.Setup(p => p.GetPositionsByStockIdAsync(It.IsAny<int>())).ReturnsAsync(positions);

        // Act
        OperationResponseDto operationResponseDto = await _service.AddTransactionAsync(addTransactionRequestDto);

        // Assert
        _mockStockRepository.Verify(s => s.GetStockByCodeAsync(It.IsAny<string>()), Times.Once());
        _mockStockInfoHandler.Verify(h => h.GetStockInfoAsync(It.IsAny<string>(), It.IsAny<string>()), newStock ? Times.Once() : Times.Never());
        _mockStockRepository.Verify(s => s.AddStockAsync(It.IsAny<Stock>()), newStock ? Times.Once() : Times.Never());
        _mockPositionRepository.Verify(p => p.GetPositionsByStockIdAsync(It.IsAny<int>()), Times.Once());
        _mockPositionRepository.Verify(p => p.UpdatePositionAsync(It.IsAny<Position>()), newOpenPosition? Times.Never() : Times.Once());
        _mockPositionRepository.Verify(p => p.AddPositionAsync(It.IsAny<Position>()), newOpenPosition ? Times.Once() : Times.Never());
        _mockTransactionRepository.Verify(t => t.AddTransactionAsync(It.IsAny<Transaction>()), Times.Once());

        if (!newOpenPosition)
        {
            Assert.Equal(existingNoOfShares + addTransactionRequestDto.NoOfSharesTransacted, positions.First().NoOfSharesHeld);
            Assert.Equal(16.20m, positions.First().TotalPurchaseCost);
        }
        Assert.Equal(expectedStatusCode, operationResponseDto.StatusCode);
        Assert.Equal(expectedMessage, operationResponseDto.Message);
    }

    [Fact]
    public async Task AddTransactionAsync_AttemptToSellWhenThereIsNoOpenPositionForStock_DiscardTransactionRecord()
    {
        // Arrange
        AddTransactionRequestDto addTransactionRequestDto = new AddTransactionRequestDto()
        {
            StockCode = "0000",
            Date = DateTime.Now.ToString("dd-MMM-yyyy"),
            Type = "Sell",
            NoOfSharesTransacted = 1,
            TransactedPricePerShare = 10.00m,
            TotalTransactionRelatedExpenses = 1.20m
        };

        _mockStockRepository.Setup(s => s.GetStockByCodeAsync(It.IsAny<string>())).ReturnsAsync(_stock);
        
        IEnumerable<Position> positions = [];

        _mockPositionRepository.Setup(p => p.GetPositionsByStockIdAsync(It.IsAny<int>())).ReturnsAsync(positions);

        // Act
        OperationResponseDto operationResponseDto = await _service.AddTransactionAsync(addTransactionRequestDto);

        // Assert
        _mockStockRepository.Verify(s => s.GetStockByCodeAsync(It.IsAny<string>()), Times.Once());
        _mockStockInfoHandler.Verify(h => h.GetStockInfoAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        _mockStockRepository.Verify(s => s.AddStockAsync(It.IsAny<Stock>()), Times.Never());
        _mockPositionRepository.Verify(p => p.GetPositionsByStockIdAsync(It.IsAny<int>()), Times.Once());
        _mockPositionRepository.Verify(p => p.UpdatePositionAsync(It.IsAny<Position>()), Times.Never());
        _mockPositionRepository.Verify(p => p.AddPositionAsync(It.IsAny<Position>()), Times.Never());
        _mockTransactionRepository.Verify(t => t.AddTransactionAsync(It.IsAny<Transaction>()), Times.Never());

        Assert.Equal(422, operationResponseDto.StatusCode);
        Assert.Equal("No open position.", operationResponseDto.Message);
    }

    [Theory]
    [InlineData(0, 422, "Number of shares held is not enough.")]
    [InlineData(1, 200, "New transaction added for stock code 0000.")]
    [InlineData(2, 200, "New transaction added for stock code 0000.")]
    public async Task AddTransactionAsync_AttemptToSellMoreThanNumberOfSharesHeld_DiscardTransactionRecord(int noOfSharesHeld, int expectedStatusCode, string expectedMessage)
    {
        // Arrange
        AddTransactionRequestDto addTransactionRequestDto = new AddTransactionRequestDto()
        {
            StockCode = "0000",
            Date = DateTime.Now.ToString("dd-MMM-yyyy"),
            Type = "Sell",
            NoOfSharesTransacted = 1,
            TransactedPricePerShare = 10.00m,
            TotalTransactionRelatedExpenses = 1.20m
        };

        _mockStockRepository.Setup(s => s.GetStockByCodeAsync(It.IsAny<string>())).ReturnsAsync(_stock);

        IEnumerable<Position> positions = new List<Position>()
        {
            new Position()
            {
                IsPositionClosed = false,
                NoOfSharesHeld = noOfSharesHeld,
                TotalNetSalesProceeds = 5.00m,
                StockId = _stock.Id,
                Stock = _stock
            }
        };

        _mockPositionRepository.Setup(p => p.GetPositionsByStockIdAsync(It.IsAny<int>())).ReturnsAsync(positions);

        // Act
        OperationResponseDto operationResponseDto = await _service.AddTransactionAsync(addTransactionRequestDto);

        // Assert
        _mockStockRepository.Verify(s => s.GetStockByCodeAsync(It.IsAny<string>()), Times.Once());
        _mockStockInfoHandler.Verify(h => h.GetStockInfoAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        _mockStockRepository.Verify(s => s.AddStockAsync(It.IsAny<Stock>()), Times.Never());
        _mockPositionRepository.Verify(p => p.GetPositionsByStockIdAsync(It.IsAny<int>()), Times.Once());
        _mockPositionRepository.Verify(p => p.UpdatePositionAsync(It.IsAny<Position>()), addTransactionRequestDto.NoOfSharesTransacted > noOfSharesHeld ? Times.Never() : Times.Once());
        _mockPositionRepository.Verify(p => p.AddPositionAsync(It.IsAny<Position>()), Times.Never());
        _mockTransactionRepository.Verify(t => t.AddTransactionAsync(It.IsAny<Transaction>()), addTransactionRequestDto.NoOfSharesTransacted > noOfSharesHeld ? Times.Never() : Times.Once());

        if (addTransactionRequestDto.NoOfSharesTransacted <= noOfSharesHeld)
        {
            Assert.Equal(noOfSharesHeld - addTransactionRequestDto.NoOfSharesTransacted, positions.First().NoOfSharesHeld);
            Assert.Equal(13.80m, positions.First().TotalNetSalesProceeds);
        }

        Assert.Equal(expectedStatusCode, operationResponseDto.StatusCode);
        Assert.Equal(expectedMessage, operationResponseDto.Message);
    }

    [Theory]
    [InlineData(2, 422, "The attempted deletion will cause the number of shares held to be invalid.")]
    [InlineData(1, 200, "Transaction record has been deleted.")]
    public async Task DeleteTransactionAsync_DeletionCausingNoOfSharesHeldInvalid_DiscardDeleteAttempt(int noOfSharesTransacted, int expectedStatusCode, string expectedMessage)
    {
        // Arrange
        int originalNoOfSharesHeld = 2;

        Position position = new Position()
        {
            Id = 1,
            IsPositionClosed = false,
            NoOfSharesHeld = originalNoOfSharesHeld,
            StockId = _stock.Id,
            Stock = _stock
        };

        Transaction transaction = new Transaction()
        {
            Date = DateOnly.FromDateTime(DateTime.Now),
            NoOfSharesTransacted = noOfSharesTransacted,
            TransactionTypeId = 1,
            PositionId = 1,
            Position = position
        };

        _mockTransactionRepository.Setup(t => t.GetTransactionByIdAsync(It.IsAny<int>())).ReturnsAsync(transaction);
        _mockPositionRepository.Setup(p => p.GetPositionByIdAsync(It.IsAny<int>())).ReturnsAsync(position);

        // Act
        OperationResponseDto operationResponseDto = await _service.DeleteTransactionAsync(It.IsAny<int>());

        // Assert
        _mockTransactionRepository.Verify(t => t.GetTransactionByIdAsync(It.IsAny<int>()), Times.Once());
        _mockPositionRepository.Verify(p => p.GetPositionByIdAsync(It.IsAny<int>()), Times.Once());
        _mockTransactionRepository.Verify(t => t.GetTransactionsByPositionIdAsync(It.IsAny<int>()), Times.Once());

        _mockPositionRepository.Verify(p => p.UpdatePositionAsync(It.IsAny<Position>()), noOfSharesTransacted < originalNoOfSharesHeld ? Times.Once() : Times.Never());
        _mockTransactionRepository.Verify(t => t.DeleteTransactionAsync(It.IsAny<Transaction>()), noOfSharesTransacted < originalNoOfSharesHeld ? Times.Once() : Times.Never());

        Assert.Equal(expectedStatusCode, operationResponseDto.StatusCode);
        Assert.Equal(expectedMessage, operationResponseDto.Message);
    }

    [Fact]
    public async Task DeleteTransactionAsync_DeleteInitialBuy_DiscardDeleteAttempkt()
    {
        // Arrange

        Position position = new Position()
        {
            Id = 1,
            IsPositionClosed = false,
            NoOfSharesHeld = 2,
            StockId = _stock.Id,
            Stock = _stock
        };

        Transaction transaction = new Transaction()
        {
            Date = DateOnly.FromDateTime(DateTime.Now),
            NoOfSharesTransacted = 1,
            TransactionTypeId = 1,
            PositionId = 1,
            Position = position
        };

        IEnumerable<Transaction> transactions = [transaction];

        _mockTransactionRepository.Setup(t => t.GetTransactionByIdAsync(It.IsAny<int>())).ReturnsAsync(transaction);
        _mockPositionRepository.Setup(p => p.GetPositionByIdAsync(It.IsAny<int>())).ReturnsAsync(position);
        _mockTransactionRepository.Setup(t => t.GetTransactionsByPositionIdAsync(It.IsAny<int>())).ReturnsAsync(transactions);

        // Act
        OperationResponseDto operationResponseDto = await _service.DeleteTransactionAsync(It.IsAny<int>());

        // Assert
        _mockTransactionRepository.Verify(t => t.GetTransactionByIdAsync(It.IsAny<int>()), Times.Once());
        _mockPositionRepository.Verify(p => p.GetPositionByIdAsync(It.IsAny<int>()), Times.Once());
        _mockTransactionRepository.Verify(t => t.GetTransactionsByPositionIdAsync(It.IsAny<int>()), Times.Once());

        _mockPositionRepository.Verify(p => p.UpdatePositionAsync(It.IsAny<Position>()), Times.Never());
        _mockTransactionRepository.Verify(t => t.DeleteTransactionAsync(It.IsAny<Transaction>()), Times.Never());

        Assert.Equal(422, operationResponseDto.StatusCode);
        Assert.Equal("The transaction to be deleted is for the initial buy that opened the position. Consider deleting the position instead if it is no longer valid.", operationResponseDto.Message);
    }
}