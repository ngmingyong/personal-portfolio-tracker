export default class AddTransactionRequestDto {
  constructor(stockCode, date, type, noOfSharesTransacted, transactedPricePerShare, totalTransactionRelatedExpenses) {
    this.stockCode = stockCode;
    this.date = date;
    this.type = type;
    this.noOfSharesTransacted = noOfSharesTransacted;
    this.transactedPricePerShare = transactedPricePerShare;
    this.totalTransactionRelatedExpenses = totalTransactionRelatedExpenses;
  }
};