export default class TransactionResponseDto {
  constructor(id, date, type, noOfSharesTransacted, transactedPricePerShare, transactionValueBeforeExpenses, totalTransactionRelatedExpenses, purchaseCostOrNetSalesProceeds) {
    this.id = id;
    this.date = date;
    this.type = type;
    this.noOfSharesTransacted = noOfSharesTransacted;
    this.transactedPricePerShare = transactedPricePerShare;
    this.transactionValueBeforeExpenses = transactionValueBeforeExpenses;
    this.totalTransactionRelatedExpenses = totalTransactionRelatedExpenses;
    this.purchaseCostOrNetSalesProceeds = purchaseCostOrNetSalesProceeds;
  }
};