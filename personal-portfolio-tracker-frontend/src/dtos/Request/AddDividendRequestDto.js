export default class AddDividendRequestDto {
  constructor(positionId, entitlementDate, noOfSharesEligible, dividendPerShare, isSubjectToWithholdingTax, withholdingTax) {
    this.positionId = positionId;
    this.entitlementDate = entitlementDate;
    this.noOfSharesEligible = noOfSharesEligible;
    this.dividendPerShare = dividendPerShare;
    this.isSubjectToWithholdingTax = isSubjectToWithholdingTax;
    this.withholdingTax = withholdingTax;
  }
};