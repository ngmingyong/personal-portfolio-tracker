using Moq;
using Xunit;
using PersonalPortfolioTracker.DTOs.Request;
using PersonalPortfolioTracker.DTOs.Response;
using PersonalPortfolioTracker.Integrations.Interfaces;
using PersonalPortfolioTracker.Models;
using PersonalPortfolioTracker.Repositories.Interfaces;
using PersonalPortfolioTracker.Services.Implementations;

namespace UnitTests;

public class CapitalChangeUnitTests
{
    private Mock<IStockRepository> _mockStockRepository;
    private Mock<IPositionRepository> _mockPositionRepository;
    private Mock<ITransactionRepository> _mockTransactionRepository;
    private Mock<IDividendRepository> _mockDividendRepository;
    private Mock<ICapitalChangeRepository> _mockCapitalChangeRepository;
    private Mock<IStockInfoHandler> _mockStockInfoHandler;
    private Service _service;
    private Stock _stock;

    public CapitalChangeUnitTests()
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
    public async Task AddCapitalChangeAsync_InvalidCapitalChangeType_DiscardCapitalChangeRecord()
    {
        // Arrange
        AddCapitalChangeRequestDto addCapitalChangeRequestDto = new AddCapitalChangeRequestDto()
        {
            PositionId = 1,
            EntitlementDate = DateTime.Now.ToString("dd-MMM-yyyy"),
            Type = "INVALID_CAPITAL_CHANGE_TYPE",
            ChangeInNoOfShares = 1,
            Note = "-"
        };

        // Act
        OperationResponseDto operationResponseDto = await _service.AddCapitalChangeAsync(addCapitalChangeRequestDto);

        // Assert
        _mockPositionRepository.Verify(p => p.GetPositionByIdAsync(It.IsAny<int>()), Times.Never());
        _mockCapitalChangeRepository.Verify(cc => cc.AddCapitalChangeAsync(It.IsAny<CapitalChange>()), Times.Never());
        _mockPositionRepository.Verify(p => p.UpdatePositionAsync(It.IsAny<Position>()), Times.Never());

        Assert.Equal(422, operationResponseDto.StatusCode);
        Assert.Equal("Capital change type INVALID_CAPITAL_CHANGE_TYPE is not valid.", operationResponseDto.Message);
    }

    [Fact]
    public async Task AddCapitalChangeAsync_InvalidChangeInNoOfShares_DiscardCapitalChangeRecord()
    {
        // Arrange
        AddCapitalChangeRequestDto addCapitalChangeRequestDto = new AddCapitalChangeRequestDto()
        {
            PositionId = 1,
            EntitlementDate = DateTime.Now.ToString("dd-MMM-yyyy"),
            Type = "INVALID_CAPITAL_CHANGE_TYPE",
            ChangeInNoOfShares = 0,
            Note = "-"
        };

        // Act
        OperationResponseDto operationResponseDto = await _service.AddCapitalChangeAsync(addCapitalChangeRequestDto);

        // Assert
        _mockPositionRepository.Verify(p => p.GetPositionByIdAsync(It.IsAny<int>()), Times.Never());
        _mockCapitalChangeRepository.Verify(cc => cc.AddCapitalChangeAsync(It.IsAny<CapitalChange>()), Times.Never());
        _mockPositionRepository.Verify(p => p.UpdatePositionAsync(It.IsAny<Position>()), Times.Never());

        Assert.Equal(422, operationResponseDto.StatusCode);
        Assert.Equal("Change in number of shares (0) is not valid.", operationResponseDto.Message);
    }

    [Theory]
    [InlineData(true, 422, "No position available.")]
    [InlineData(false, 200, "New capital change added.")]
    public async Task AddCapitalChangeAsync_AttemptToAddCapitalChangeToClosedPosition_DiscardCapitalChangeRecord(bool isPositionClosed, int expectedStatusCode, string expectedMessage)
    {
        // Arrange
        int existingNoOfShares = 1;
        Position position = new Position()
        {
            IsPositionClosed = isPositionClosed,
            NoOfSharesHeld = existingNoOfShares,
            StockId = _stock.Id,
            Stock = _stock
        };

        _mockPositionRepository.Setup(p => p.GetPositionByIdAsync(It.IsAny<int>())).ReturnsAsync(position);

        AddCapitalChangeRequestDto addCapitalChangeRequestDto = new AddCapitalChangeRequestDto()
        {
            PositionId = 1,
            EntitlementDate = DateTime.Now.ToString("dd-MMM-yyyy"),
            Type = "Increase",
            ChangeInNoOfShares = 1,
            Note = "-"
        };

        // Act
        OperationResponseDto operationResponseDto = await _service.AddCapitalChangeAsync(addCapitalChangeRequestDto);

        // Assert
        _mockPositionRepository.Verify(p => p.GetPositionByIdAsync(It.IsAny<int>()), Times.Once());
        _mockCapitalChangeRepository.Verify(cc => cc.AddCapitalChangeAsync(It.IsAny<CapitalChange>()), isPositionClosed ? Times.Never() : Times.Once());
        _mockPositionRepository.Verify(p => p.UpdatePositionAsync(It.IsAny<Position>()), isPositionClosed ? Times.Never() : Times.Once());

        Assert.Equal(isPositionClosed ? existingNoOfShares : existingNoOfShares + addCapitalChangeRequestDto.ChangeInNoOfShares, position.NoOfSharesHeld);
        Assert.Equal(expectedStatusCode, operationResponseDto.StatusCode);
        Assert.Equal(expectedMessage, operationResponseDto.Message);
    }

    [Theory]
    [InlineData(2, 422, "Number of shares held is not enough.")]
    [InlineData(1, 200, "New capital change added.")]
    public async Task AddCapitalChangeAsync_NotEnoughNumberOfSharesForDecrease_DiscardCapitalChangeRecord(int decreaseInNoOfShares, int expectedStatusCode, string expectedMessage)
    {
        // Arrange
        int existingNoOfShares = 1;
        Position position = new Position()
        {
            IsPositionClosed = false,
            NoOfSharesHeld = existingNoOfShares,
            StockId = _stock.Id,
            Stock = _stock
        };

        _mockPositionRepository.Setup(p => p.GetPositionByIdAsync(It.IsAny<int>())).ReturnsAsync(position);

        AddCapitalChangeRequestDto addCapitalChangeRequestDto = new AddCapitalChangeRequestDto()
        {
            PositionId = 1,
            EntitlementDate = DateTime.Now.ToString("dd-MMM-yyyy"),
            Type = "Decrease",
            ChangeInNoOfShares = decreaseInNoOfShares,
            Note = "-"
        };

        // Act
        OperationResponseDto operationResponseDto = await _service.AddCapitalChangeAsync(addCapitalChangeRequestDto);

        // Assert
        _mockPositionRepository.Verify(p => p.GetPositionByIdAsync(It.IsAny<int>()), Times.Once());
        _mockCapitalChangeRepository.Verify(cc => cc.AddCapitalChangeAsync(It.IsAny<CapitalChange>()), existingNoOfShares < decreaseInNoOfShares ? Times.Never() : Times.Once());
        _mockPositionRepository.Verify(p => p.UpdatePositionAsync(It.IsAny<Position>()), existingNoOfShares < decreaseInNoOfShares ? Times.Never() : Times.Once());

        Assert.Equal(existingNoOfShares < decreaseInNoOfShares ? existingNoOfShares : addCapitalChangeRequestDto.ChangeInNoOfShares - existingNoOfShares, position.NoOfSharesHeld);
        Assert.Equal(expectedStatusCode, operationResponseDto.StatusCode);
        Assert.Equal(expectedMessage, operationResponseDto.Message);
    }

    [Theory]
    [InlineData(2, 422, "The attempted deletion will cause the number of shares held to be invalid.")]
    [InlineData(1, 200, "Capital change record has been deleted.")]
    public async Task DeleteCapitalChangeAsync_DeletionCausingNoOfSharesHeldInvalid_DiscardDeleteAttempt(int changeInNoOfShares, int expectedStatusCode, string expectedMessage)
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

        CapitalChange capitalChange = new CapitalChange()
        {
            EntitlementDate = DateOnly.FromDateTime(DateTime.Now),
            ChangeInNoOfShares = changeInNoOfShares,
            CapitalChangeTypeId = 1,
            PositionId = 1,
            Position = position
        };

        _mockCapitalChangeRepository.Setup(t => t.GetCapitalChangeByIdAsync(It.IsAny<int>())).ReturnsAsync(capitalChange);
        _mockPositionRepository.Setup(p => p.GetPositionByIdAsync(It.IsAny<int>())).ReturnsAsync(position);

        // Act
        OperationResponseDto operationResponseDto = await _service.DeleteCapitalChangeAsync(It.IsAny<int>());

        // Assert
        _mockCapitalChangeRepository.Verify(t => t.GetCapitalChangeByIdAsync(It.IsAny<int>()), Times.Once());
        _mockPositionRepository.Verify(p => p.GetPositionByIdAsync(It.IsAny<int>()), Times.Once());

        _mockPositionRepository.Verify(p => p.UpdatePositionAsync(It.IsAny<Position>()), changeInNoOfShares < originalNoOfSharesHeld ? Times.Once() : Times.Never());
        _mockCapitalChangeRepository.Verify(t => t.DeleteCapitalChangeAsync(It.IsAny<CapitalChange>()), changeInNoOfShares < originalNoOfSharesHeld ? Times.Once() : Times.Never());

        Assert.Equal(expectedStatusCode, operationResponseDto.StatusCode);
        Assert.Equal(expectedMessage, operationResponseDto.Message);
    }
}