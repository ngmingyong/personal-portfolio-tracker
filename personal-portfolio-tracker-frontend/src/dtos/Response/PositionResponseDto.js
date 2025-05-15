export default class PositionResponseDto {
  constructor(id, stockCode, stockName, stockPrice, stockPriceLastUpdated, positionOpenedDate, positionClosedDate, isPositionClosed, noOfSharesHeld, totalPurchaseCost, totalDividendsReceived, totalNetSalesProceeds, unrealizedValueOfSharesHeldBeforeFinalSalesExpenses, hypotheticalTotalReturnBeforeFinalSalesExpenses, finalizedTotalReturn, transactions, dividends, capitalChanges) {
    this.id = id;
    this.stockCode = stockCode;
    this.stockName = stockName;
    this.stockPrice = stockPrice;
    this.stockPriceLastUpdated = stockPriceLastUpdated;
    this.positionOpenedDate = positionOpenedDate;
    this.positionClosedDate = positionClosedDate;
    this.isPositionClosed = isPositionClosed;
    this.noOfSharesHeld = noOfSharesHeld;
    this.totalPurchaseCost = totalPurchaseCost;
    this.totalDividendsReceived = totalDividendsReceived;
    this.totalNetSalesProceeds = totalNetSalesProceeds;
    this.unrealizedValueOfSharesHeldBeforeFinalSalesExpenses = unrealizedValueOfSharesHeldBeforeFinalSalesExpenses;
    this.hypotheticalTotalReturnBeforeFinalSalesExpenses = hypotheticalTotalReturnBeforeFinalSalesExpenses;
    this.finalizedTotalReturn = finalizedTotalReturn;
    this.transactions = transactions;
    this.dividends = dividends;
    this.capitalChanges = capitalChanges;
  }
};