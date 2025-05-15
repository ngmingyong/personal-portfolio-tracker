export default class AddCapitalChangeRequestDto {
  constructor(positionId, entitlementDate, type, changeInNoOfShares, note) {
    this.positionId = positionId;
    this.entitlementDate = entitlementDate;
    this.type = type;
    this.changeInNoOfShares = changeInNoOfShares;
    this.note = note;
  }
};