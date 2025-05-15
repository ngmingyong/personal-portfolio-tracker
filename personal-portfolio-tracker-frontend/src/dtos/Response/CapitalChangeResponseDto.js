export default class CapitalChangeResponseDto {
  constructor(id, entitlementDate, type, changeInNoOfShares, note) {
    this.id = id;
    this.entitlementDate = entitlementDate;
    this.type = type;
    this.changeInNoOfShares = changeInNoOfShares;
    this.note = note;
  }
};