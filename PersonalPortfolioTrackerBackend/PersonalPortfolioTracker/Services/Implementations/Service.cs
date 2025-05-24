using System.Globalization;
using PersonalPortfolioTracker.DTOs.Request;
using PersonalPortfolioTracker.DTOs.Response;
using PersonalPortfolioTracker.Integrations.Interfaces;
using PersonalPortfolioTracker.Models;
using PersonalPortfolioTracker.Repositories.Interfaces;
using PersonalPortfolioTracker.Services.Interfaces;

namespace PersonalPortfolioTracker.Services.Implementations
{
    public class Service : IService
    {
        private readonly IStockRepository _stockRepository;
        private readonly IPositionRepository _positionRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IDividendRepository _dividendRepository;
        private readonly ICapitalChangeRepository _capitalChangeRepository;
        private readonly IStockInfoHandler _stockInfoHandler;

        private const string KLSE_EXCHANGE_NAME = "KLSE";

        public Service(
            IStockRepository stockRepository,
            IPositionRepository positionRepository,
            ITransactionRepository transactionRepository,
            IDividendRepository dividendRepository,
            ICapitalChangeRepository capitalChangeRepository,
            IStockInfoHandler stockInfoHandler
        )
        {
            _stockRepository = stockRepository;
            _positionRepository = positionRepository;
            _transactionRepository = transactionRepository;
            _dividendRepository = dividendRepository;
            _capitalChangeRepository = capitalChangeRepository;
            _stockInfoHandler = stockInfoHandler;
        }

        public async Task<IEnumerable<PositionResponseDto>> GetAllPositionsAsync()
        {
            IEnumerable<Stock>? stocks = await _stockRepository.GetAllStocksAsync();
            IEnumerable<Position>? positions = await _positionRepository.GetAllPositionsAsync();
            IEnumerable<Transaction>? transactions = await _transactionRepository.GetAllTransactionsAsync();
            IEnumerable<Dividend>? dividends = await _dividendRepository.GetAllDividendsAsync();
            IEnumerable<CapitalChange>? capitalChanges = await _capitalChangeRepository.GetAllCapitalChangesAsync();

            List<PositionResponseDto> positionResponseDtos = new List<PositionResponseDto>();
            List<Stock> stocksToUpdate = new List<Stock>();

            if (positions == null || !positions.Any())
            {
                return positionResponseDtos;
            }

            foreach (Position position in positions.OrderByDescending(p => p.PositionOpenedDate).ThenByDescending(p => p.Id))
            {
                PositionResponseDto positionResponseDto = new PositionResponseDto();

                positionResponseDto.Id = position.Id;
                positionResponseDto.StockCode = position.Stock.Code;
                positionResponseDto.StockName = position.Stock.Name;
                positionResponseDto.PositionOpenedDate = position.PositionOpenedDate.ToString("dd-MMM-yyyy");

                positionResponseDto.NoOfSharesHeld = position.NoOfSharesHeld;
                positionResponseDto.TotalPurchaseCost = position.TotalPurchaseCost;
                positionResponseDto.TotalDividendsReceived = position.TotalDividendsReceived;
                positionResponseDto.TotalNetSalesProceeds = position.TotalNetSalesProceeds;

                positionResponseDto.IsPositionClosed = position.IsPositionClosed;

                if (position.IsPositionClosed)
                {
                    positionResponseDto.PositionClosedDate = position.PositionClosedDate.ToString("dd-MMM-yyyy");

                    positionResponseDto.FinalizedTotalReturn = 
                        position.TotalDividendsReceived
                        + position.TotalNetSalesProceeds
                        - position.TotalPurchaseCost;
                }
                else
                {
                    // get recent closing price for open position
                    StockInfoDto[]? stockInfoDtos = await _stockInfoHandler.GetStockInfoAsync(position.Stock.Code ?? "-", KLSE_EXCHANGE_NAME);

                    if (stockInfoDtos != null && stockInfoDtos.Length > 0)
                    {
                        position.Stock.Price = stockInfoDtos[0].PreviousClose;
                        positionResponseDto.StockPrice = stockInfoDtos[0].PreviousClose;

                        DateOnly stockPriceLastUpdated = DateOnly.ParseExact(stockInfoDtos[0].PreviousCloseDate!, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        position.Stock.LastUpdated = stockPriceLastUpdated;
                        positionResponseDto.StockPriceLastUpdated = stockPriceLastUpdated.ToString("dd-MMM-yyyy");

                        stocksToUpdate.Add(position.Stock);
                    }
                    else
                    {
                        positionResponseDto.StockPrice = position.Stock.Price;
                        positionResponseDto.StockPriceLastUpdated = position.Stock.LastUpdated.ToString("dd-MMM-yyyy");
                    }

                    positionResponseDto.UnrealizedValueOfSharesHeldBeforeFinalSalesExpenses = 
                        position.Stock.Price
                        * position.NoOfSharesHeld;

                    positionResponseDto.HypotheticalTotalReturnBeforeFinalSalesExpenses = 
                        position.TotalDividendsReceived
                        + position.TotalNetSalesProceeds
                        + (position.Stock.Price * position.NoOfSharesHeld)
                        - position.TotalPurchaseCost;
                }

                positionResponseDto.Transactions = transactions
                    .Where(t => t.PositionId == position.Id)
                    .OrderByDescending(t => t.Date)
                    .ThenByDescending(t => t.Id)
                    .Select(t => new TransactionResponseDto()
                    {
                        Id = t.Id,
                        Date = t.Date.ToString("dd-MMM-yyyy"),
                        Type = (Enum.GetName(typeof(Enums.TransactionType), t.TransactionTypeId) ?? "").ToString(),
                        NoOfSharesTransacted = t.NoOfSharesTransacted,
                        TransactedPricePerShare = t.TransactedPricePerShare,
                        TransactionValueBeforeExpenses = t.NoOfSharesTransacted * t.TransactedPricePerShare,
                        TotalTransactionRelatedExpenses = t.TotalTransactionRelatedExpenses,
                        PurchaseCostOrNetSalesProceeds = (t.NoOfSharesTransacted * t.TransactedPricePerShare) + ((Enums.TransactionType)t.TransactionTypeId == Enums.TransactionType.Buy ? t.TotalTransactionRelatedExpenses : (t.TotalTransactionRelatedExpenses * -1))
                    });

                positionResponseDto.Dividends = dividends
                    .Where(d => d.PositionId == position.Id)
                    .OrderByDescending(d => d.EntitlementDate)
                    .ThenByDescending(d => d.Id)
                    .Select(d => new DividendResponseDto()
                    {
                        Id = d.Id,
                        EntitlementDate = d.EntitlementDate.ToString("dd-MMM-yyyy"),
                        NoOfSharesEligible = d.NoOfSharesEligible,
                        DividendPerShare = d.DividendPerShare,
                        AmountBeforeWithholdingTax = Math.Round((d.NoOfSharesEligible * d.DividendPerShare), 2, MidpointRounding.AwayFromZero),
                        IsSubjectToWithholdingTax = d.IsSubjectToWithholdingTax,
                        WithholdingTax = d.WithholdingTax,
                        AmountReceived = Math.Round((d.NoOfSharesEligible * d.DividendPerShare), 2, MidpointRounding.AwayFromZero) - d.WithholdingTax
                    });

                positionResponseDto.CapitalChanges = capitalChanges
                    .Where(cc => cc.PositionId == position.Id)
                    .OrderByDescending(cc => cc.EntitlementDate)
                    .ThenByDescending(cc => cc.Id)
                    .Select(cc => new CapitalChangeResponseDto()
                    {
                        Id = cc.Id,
                        EntitlementDate = cc.EntitlementDate.ToString("dd-MMM-yyyy"),
                        Type = (Enum.GetName(typeof(Enums.CapitalChangeType), cc.CapitalChangeTypeId) ?? "").ToString(),
                        ChangeInNoOfShares = cc.ChangeInNoOfShares,
                        Note = cc.Note
                    });

                positionResponseDtos.Add(positionResponseDto);
            }

            if (stocksToUpdate.Count > 0)
            {
                await _stockRepository.UpdateStocksAsync(stocksToUpdate);
            }

            return positionResponseDtos;
        }

        public async Task<OperationResponseDto> UpdatePositionStatusAsync(int id, UpdatePositionStatusRequestDto updatePositionStatusRequestDto)
        {
            Position? position = await _positionRepository.GetPositionByIdAsync(id);

            if (position == null)
            {
                return new OperationResponseDto()
                {
                    StatusCode = 404,
                    Message = $"The position record with ID {id} is not found."
                };
            }

            if (updatePositionStatusRequestDto.IsPositionClosed && position.NoOfSharesHeld > 0)
            {
                return new OperationResponseDto()
                {
                    StatusCode = 422,
                    Message = $"Cannot close position. There are shares being held."
                };
            }
            else if (!updatePositionStatusRequestDto.IsPositionClosed && position.NoOfSharesHeld == 0)
            {
                return new OperationResponseDto()
                {
                    StatusCode = 422,
                    Message = $"Cannot open position. There are no shares being held."
                };
            }

            position.IsPositionClosed = updatePositionStatusRequestDto.IsPositionClosed;
            
            if (updatePositionStatusRequestDto.IsPositionClosed)
            {
                position.PositionClosedDate = DateOnly.FromDateTime(DateTime.Now);
            }
            else
            {
                position.PositionClosedDate = new DateOnly(1, 1, 1);
            }

            await _positionRepository.UpdatePositionAsync(position);

            return new OperationResponseDto()
            {
                StatusCode = 200,
                Message = $"Position status has been updated.",
            };
        }

        public async Task<OperationResponseDto> DeletePositionAsync(int id)
        {
            Position? position = await _positionRepository.GetPositionByIdAsync(id);

            if (position == null)
            {
                return new OperationResponseDto()
                {
                    StatusCode = 404,
                    Message = $"The position with ID {id} is not found."
                };
            }

            await _positionRepository.DeletePositionAsync(position);

            return new OperationResponseDto()
            {
                StatusCode = 200,
                Message = $"Position record has been deleted.",
            };
        }

        public async Task<OperationResponseDto> AddTransactionAsync(AddTransactionRequestDto addTransactionRequestDto)
        {
            // check if number of shares to be transacted is valid
            if (addTransactionRequestDto.NoOfSharesTransacted <= 0)
            {
                return new OperationResponseDto()
                {
                    StatusCode = 422,
                    Message = $"Number of shares ({addTransactionRequestDto.NoOfSharesTransacted}) to be transacted is not valid."
                };
            }

            // check if transacted price per share and total transaction-related expenses are valid
            if (addTransactionRequestDto.TransactedPricePerShare < 0 || addTransactionRequestDto.TotalTransactionRelatedExpenses < 0)
            {
                decimal amount = addTransactionRequestDto.TransactedPricePerShare < 0 ? addTransactionRequestDto.TransactedPricePerShare : addTransactionRequestDto.TotalTransactionRelatedExpenses;
                return new OperationResponseDto()
                {
                    StatusCode = 422,
                    Message = $"Monetary amount ({amount}) is not valid."
                };
            }

            // check if transaction type is valid
            if (!Enum.TryParse(addTransactionRequestDto.Type, true, out Enums.TransactionType transactionType))
            {
                return new OperationResponseDto()
                {
                    StatusCode = 422,
                    Message = $"Transaction type {addTransactionRequestDto.Type} is not valid."
                };
            }

            // check if stock exist in db
            Stock? stock = await _stockRepository.GetStockByCodeAsync(addTransactionRequestDto.StockCode ?? "-");

            if (stock == null)
            {
                // get from api if it is not in db
                StockInfoDto[]? stockInfoDtos = await _stockInfoHandler.GetStockInfoAsync(addTransactionRequestDto.StockCode ?? "-", KLSE_EXCHANGE_NAME);

                if (stockInfoDtos == null || stockInfoDtos.Length == 0)
                {
                    return new OperationResponseDto()
                    {
                        StatusCode = 422,
                        Message = $"The stock code {addTransactionRequestDto.StockCode} is not valid."
                    };
                }
                else
                {
                    stock = new Stock()
                    {
                        Code = stockInfoDtos[0].Code,
                        Name = stockInfoDtos[0].Name,
                        Price = stockInfoDtos[0].PreviousClose,
                        LastUpdated = DateOnly.ParseExact(stockInfoDtos[0].PreviousCloseDate!, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                    };

                    await _stockRepository.AddStockAsync(stock);
                }
            }

            DateOnly.TryParseExact(addTransactionRequestDto.Date, "dd-MMM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly date);

            // check if there is open position for the stock
            IEnumerable<Position> positionsOfStock = await _positionRepository.GetPositionsByStockIdAsync(stock!.Id);
            Position? position;
            if (positionsOfStock.Any(p => !p.IsPositionClosed))
            {
                 position = positionsOfStock.Where(p => !p.IsPositionClosed).OrderByDescending(p => p.PositionOpenedDate).First();

                // check if number of shares to be sold is larger than shares held
                if (transactionType == Enums.TransactionType.Sell)
                {
                    if (addTransactionRequestDto.NoOfSharesTransacted > position.NoOfSharesHeld)
                    {
                        return new OperationResponseDto()
                        {
                            StatusCode = 422,
                            Message = $"Number of shares held is not enough."
                        };
                    }
                    else if (addTransactionRequestDto.NoOfSharesTransacted == position.NoOfSharesHeld)
                    {
                        position.NoOfSharesHeld -= addTransactionRequestDto.NoOfSharesTransacted;
                        position.TotalNetSalesProceeds += (addTransactionRequestDto.NoOfSharesTransacted * addTransactionRequestDto.TransactedPricePerShare) - addTransactionRequestDto.TotalTransactionRelatedExpenses;

                        position.IsPositionClosed = true;
                        position.PositionClosedDate = date;
                    }
                    else
                    {
                        position.NoOfSharesHeld -= addTransactionRequestDto.NoOfSharesTransacted;
                        position.TotalNetSalesProceeds += (addTransactionRequestDto.NoOfSharesTransacted * addTransactionRequestDto.TransactedPricePerShare) - addTransactionRequestDto.TotalTransactionRelatedExpenses;
                    }
                }
                else if (transactionType == Enums.TransactionType.Buy)
                {
                    position.NoOfSharesHeld += addTransactionRequestDto.NoOfSharesTransacted;
                    position.TotalPurchaseCost += (addTransactionRequestDto.NoOfSharesTransacted * addTransactionRequestDto.TransactedPricePerShare) + addTransactionRequestDto.TotalTransactionRelatedExpenses;
                }

                await _positionRepository.UpdatePositionAsync(position);
            }
            else
            {
                // open new position (buy only)

                if (transactionType == Enums.TransactionType.Sell)
                {
                    return new OperationResponseDto()
                    {
                        StatusCode = 422,
                        Message = $"No open position."
                    };
                }

                position = new Position()
                {
                    PositionOpenedDate = date,
                    IsPositionClosed = false,
                    NoOfSharesHeld = addTransactionRequestDto.NoOfSharesTransacted,
                    TotalPurchaseCost = (addTransactionRequestDto.NoOfSharesTransacted * addTransactionRequestDto.TransactedPricePerShare) + addTransactionRequestDto.TotalTransactionRelatedExpenses,
                    StockId = stock.Id,
                    Stock = stock
                };
                await _positionRepository.AddPositionAsync(position);
            }

            Transaction transaction = new Transaction()
            {
                Date = date,
                NoOfSharesTransacted = addTransactionRequestDto.NoOfSharesTransacted,
                TransactedPricePerShare = addTransactionRequestDto.TransactedPricePerShare,
                TotalTransactionRelatedExpenses = addTransactionRequestDto.TotalTransactionRelatedExpenses,
                TransactionTypeId = (int)transactionType,
                PositionId = position.Id,

                Position = position
            };
            await _transactionRepository.AddTransactionAsync(transaction);

            return new OperationResponseDto()
            {
                StatusCode = 200,
                Message = $"New transaction added for stock code {addTransactionRequestDto.StockCode}.",
            };
        }

        public async Task<OperationResponseDto> DeleteTransactionAsync(int id)
        {
            Transaction? transaction = await _transactionRepository.GetTransactionByIdAsync(id);

            if (transaction == null)
            {
                return new OperationResponseDto()
                {
                    StatusCode = 404,
                    Message = $"The transaction record with ID {id} is not found."
                };
            }

            Position? position = await _positionRepository.GetPositionByIdAsync(transaction.PositionId);
            
            if (position == null)
            {
                return new OperationResponseDto()
                {
                    StatusCode = 422,
                    Message = $"The position related to transaction ID {id} is not found."
                };
            }

            if ((Enums.TransactionType)transaction.TransactionTypeId == Enums.TransactionType.Sell)
            {
                position.TotalNetSalesProceeds -= (transaction.NoOfSharesTransacted * transaction.TransactedPricePerShare) - transaction.TotalTransactionRelatedExpenses;
                position.NoOfSharesHeld += transaction.NoOfSharesTransacted;
            }
            else if ((Enums.TransactionType)transaction.TransactionTypeId == Enums.TransactionType.Buy)
            {
                IEnumerable<Transaction>? allTransactionsOfSamePosition = await _transactionRepository.GetTransactionsByPositionIdAsync(transaction.PositionId);
                if (allTransactionsOfSamePosition
                    .Where(t => t.Date <= transaction.Date && t.TransactionTypeId == transaction.TransactionTypeId)
                    .Count() == 1)
                {
                    return new OperationResponseDto()
                    {
                        StatusCode = 422,
                        Message = $"The transaction to be deleted is for the initial buy that opened the position. Consider deleting the position instead if it is no longer valid."
                    };
                }

                position.TotalPurchaseCost -= (transaction.NoOfSharesTransacted * transaction.TransactedPricePerShare) + transaction.TotalTransactionRelatedExpenses;
                position.NoOfSharesHeld -= transaction.NoOfSharesTransacted;
            }

            if (position.NoOfSharesHeld <= 0)
            {
                return new OperationResponseDto()
                {
                    StatusCode = 422,
                    Message = $"The attempted deletion will cause the number of shares held to be invalid."
                };
            }

            await _positionRepository.UpdatePositionAsync(position);
            await _transactionRepository.DeleteTransactionAsync(transaction);

            return new OperationResponseDto()
            {
                StatusCode = 200,
                Message = $"Transaction record has been deleted.",
            };
        }

        public async Task<OperationResponseDto> AddDividendAsync(AddDividendRequestDto addDividendRequestDto)
        {
            // check if number of shares eligible for dividend is valid
            if (addDividendRequestDto.NoOfSharesEligible <= 0)
            {
                return new OperationResponseDto()
                {
                    StatusCode = 422,
                    Message = $"Number of shares ({addDividendRequestDto.NoOfSharesEligible}) eligible for dividend is not valid."
                };
            }

            // check if dividend per share and withholding tax are valid
            if (addDividendRequestDto.DividendPerShare < 0 || addDividendRequestDto.WithholdingTax < 0)
            {
                decimal amount;
                if (addDividendRequestDto.DividendPerShare < 0)
                {
                    amount = addDividendRequestDto.DividendPerShare;
                } 
                else
                {
                    amount = addDividendRequestDto.WithholdingTax;
                }

                return new OperationResponseDto()
                {
                    StatusCode = 422,
                    Message = $"Monetary amount ({amount}) is not valid."
                };
            }

            Position? position = await _positionRepository.GetPositionByIdAsync(addDividendRequestDto.PositionId);

            if (position == null || position.IsPositionClosed)
            {
                return new OperationResponseDto()
                {
                    StatusCode = 422,
                    Message = $"No position available.",
                };
            }

            DateOnly.TryParseExact(addDividendRequestDto.EntitlementDate, "dd-MMM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly entitlementDate);
            Dividend dividend = new Dividend()
            {                
                EntitlementDate = entitlementDate,
                NoOfSharesEligible = addDividendRequestDto.NoOfSharesEligible,
                DividendPerShare = addDividendRequestDto.DividendPerShare,
                IsSubjectToWithholdingTax = addDividendRequestDto.IsSubjectToWithholdingTax,
                WithholdingTax = addDividendRequestDto.WithholdingTax,
                PositionId = position.Id,
                Position = position
            };

            await _dividendRepository.AddDividendAsync(dividend);
            position.TotalDividendsReceived += Math.Round((dividend.NoOfSharesEligible * dividend.DividendPerShare), 2, MidpointRounding.AwayFromZero) - dividend.WithholdingTax;

            await _positionRepository.UpdatePositionAsync(position);

            return new OperationResponseDto()
            {
                StatusCode = 200,
                Message = $"New dividend added.",
            };
        }

        public async Task<OperationResponseDto> DeleteDividendAsync(int id)
        {
            Dividend? dividend = await _dividendRepository.GetDividendByIdAsync(id);

            if (dividend == null)
            {
                return new OperationResponseDto()
                {
                    StatusCode = 404,
                    Message = $"The dividend record with ID {id} is not found."
                };
            }

            Position? position = await _positionRepository.GetPositionByIdAsync(dividend.PositionId);

            if (position == null)
            {
                return new OperationResponseDto()
                {
                    StatusCode = 422,
                    Message = $"The position related to dividend ID {id} is not found."
                };
            }

            position.TotalDividendsReceived -= Math.Round((dividend.NoOfSharesEligible * dividend.DividendPerShare), 2, MidpointRounding.AwayFromZero) - dividend.WithholdingTax;

            await _positionRepository.UpdatePositionAsync(position);
            await _dividendRepository.DeleteDividendAsync(dividend);

            return new OperationResponseDto()
            {
                StatusCode = 200,
                Message = $"Dividend record has been deleted.",
            };
        }

        public async Task<OperationResponseDto> AddCapitalChangeAsync(AddCapitalChangeRequestDto addCapitalChangeRequestDto)
        {
            // check if change in number of shares is valid
            if (addCapitalChangeRequestDto.ChangeInNoOfShares <= 0)
            {
                return new OperationResponseDto()
                {
                    StatusCode = 422,
                    Message = $"Change in number of shares ({addCapitalChangeRequestDto.ChangeInNoOfShares}) is not valid."
                };
            }

            // check if capital change type is valid
            if (!Enum.TryParse(addCapitalChangeRequestDto.Type, true, out Enums.CapitalChangeType capitalChangeType))
            {
                return new OperationResponseDto()
                {
                    StatusCode = 422,
                    Message = $"Capital change type {addCapitalChangeRequestDto.Type} is not valid."
                };
            }

            Position? position = await _positionRepository.GetPositionByIdAsync(addCapitalChangeRequestDto.PositionId);

            if (position == null || position.IsPositionClosed)
            {
                return new OperationResponseDto()
                {
                    StatusCode = 422,
                    Message = $"No position available.",
                };
            }

            if (capitalChangeType == Enums.CapitalChangeType.Decrease && addCapitalChangeRequestDto.ChangeInNoOfShares > position.NoOfSharesHeld)
            {
                return new OperationResponseDto()
                {
                    StatusCode = 422,
                    Message = $"Number of shares held is not enough.",
                };
            }

            DateOnly.TryParseExact(addCapitalChangeRequestDto.EntitlementDate, "dd-MMM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly entitlementDate);
            CapitalChange capitalChange = new CapitalChange()
            {
                EntitlementDate = entitlementDate,
                ChangeInNoOfShares = addCapitalChangeRequestDto.ChangeInNoOfShares,
                Note = addCapitalChangeRequestDto.Note,
                CapitalChangeTypeId = (int)capitalChangeType,
                PositionId = position.Id,
                Position = position
            };

            await _capitalChangeRepository.AddCapitalChangeAsync(capitalChange);

            position.NoOfSharesHeld += (capitalChangeType == Enums.CapitalChangeType.Increase ? 1 : -1) * capitalChange.ChangeInNoOfShares;

            await _positionRepository.UpdatePositionAsync(position);

            return new OperationResponseDto()
            {
                StatusCode = 200,
                Message = $"New capital change added.",
            };
        }

        public async Task<OperationResponseDto> DeleteCapitalChangeAsync(int id)
        {
            CapitalChange? capitalChange = await _capitalChangeRepository.GetCapitalChangeByIdAsync(id);

            if (capitalChange == null)
            {
                return new OperationResponseDto()
                {
                    StatusCode = 404,
                    Message = $"The capital change record with ID {id} is not found."
                };
            }

            Position? position = await _positionRepository.GetPositionByIdAsync(capitalChange.PositionId);

            if (position == null)
            {
                return new OperationResponseDto()
                {
                    StatusCode = 422,
                    Message = $"The position related to capital change ID {id} is not found."
                };
            }

            position.NoOfSharesHeld += ((Enums.CapitalChangeType)capitalChange.CapitalChangeTypeId == Enums.CapitalChangeType.Decrease ? 1 : -1) * capitalChange.ChangeInNoOfShares;

            if (position.NoOfSharesHeld <= 0)
            {
                return new OperationResponseDto()
                {
                    StatusCode = 422,
                    Message = $"The attempted deletion will cause the number of shares held to be invalid."
                };
            }

            await _positionRepository.UpdatePositionAsync(position);
            await _capitalChangeRepository.DeleteCapitalChangeAsync(capitalChange);

            return new OperationResponseDto()
            {
                StatusCode = 200,
                Message = $"Capital change record has been deleted.",
            };
        }
    }
}