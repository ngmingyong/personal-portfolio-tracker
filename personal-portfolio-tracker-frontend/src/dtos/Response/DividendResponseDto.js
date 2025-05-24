export default class DividendResponseDto {
  constructor(id, entitlementDate, noOfSharesEligible, dividendPerShare, amountBeforeWithholdingTax, isSubjectToWithholdingTax, withholdingTax, amountReceived) {
    this.id = id;
    this.entitlementDate = entitlementDate;
    this.noOfSharesEligible = noOfSharesEligible;
    this.dividendPerShare = dividendPerShare;
    this.amountBeforeWithholdingTax = amountBeforeWithholdingTax
    this.isSubjectToWithholdingTax = isSubjectToWithholdingTax;
    this.withholdingTax = withholdingTax;
    this.amountReceived = amountReceived;
  }
};