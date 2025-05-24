import { useState, useEffect } from 'react';
import './style/Dialogs.css';
import AddDividendRequestDto from '../../dtos/Request/AddDividendRequestDto';
import { format } from 'date-fns';

function AddDividendDialog({ positionIdWithAddDividendDialogOpened, stockCodeWithAddDividendDialogOpened, closeDialog, apiCall, endpoint, method }) {

  const [addDividendRequestDto, setAddDividendRequestDto] = useState(new AddDividendRequestDto(positionIdWithAddDividendDialogOpened, '', '', '', false, 0));
  const [selectedEntitlementDate, setSelectedEntitlementDate] = useState('');
  const [isWithholdingTaxDisabled, setIsWithholdingTaxDisabled] = useState(true);

  useEffect(() => {
    setAddDividendRequestDto(previousState => ({
      ...previousState,
      ['positionId']: positionIdWithAddDividendDialogOpened
    }))}, [positionIdWithAddDividendDialogOpened]);

  const handleChange = (e) => {
    const { name, value } = e.target;

    let formattedValue = value;
    if (name === 'entitlementDate') {
      formattedValue = format(value, 'dd-MMM-yyyy')
      setSelectedEntitlementDate(value);
    }

    if (name === 'noOfSharesEligible' && !(/^([1-9][0-9]*)?$/.test(value))){
      alert('Number of shares eligible for dividend must be an integer larger than zero.');
      return;
    }

    if ((name === 'dividendPerShare') &&  !(/^(0|[1-9]\d*)(\.\d+)?$|^$/.test(value))){
      alert('Dividend per share cannot be negative, and must be either an integer or a decimal.');
      return;
    }

    if (name === 'isSubjectToWithholdingTax'){
      formattedValue = e.target.checked;

      if (e.target.checked){
        setIsWithholdingTaxDisabled(false);
        setAddDividendRequestDto(previousState => ({
          ...previousState,
          withholdingTax: ''
        }));
      }
      else {
        setIsWithholdingTaxDisabled(true);
        setAddDividendRequestDto(previousState => ({
          ...previousState,
          withholdingTax: 0
        }));
      }
    }

    if ((name === 'withholdingTax') &&  !(/^(0|[1-9]\d*)(\.\d{1,2})?$|^$/.test(value))){
      alert('Withholding Tax cannot be negative, and must be either an integer or a decimal with up to two decimal places.');
      return;
    }

    setAddDividendRequestDto(previousState => ({
      ...previousState,
      [name]: formattedValue
    }));
  };

  const handleClear = () => {
    setSelectedEntitlementDate('');
    setIsWithholdingTaxDisabled(true);
    setAddDividendRequestDto(new AddDividendRequestDto(positionIdWithAddDividendDialogOpened, '', '', '', false, 0));
  };

  const handleClose = () => {
    setSelectedEntitlementDate('');
    setIsWithholdingTaxDisabled(true);
    setAddDividendRequestDto(new AddDividendRequestDto(positionIdWithAddDividendDialogOpened, '', '', '', false, 0));
    closeDialog();
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    await apiCall(endpoint, method, addDividendRequestDto);
  };

  if (positionIdWithAddDividendDialogOpened < 0) return null;

  return (
    <div className="dialog">
      <h2>Add new dividend for Position {stockCodeWithAddDividendDialogOpened}</h2>
        <form onSubmit={handleSubmit}>

          <div className="input-field-group">
            <label htmlFor="entitlementDate">Entitlement date:</label>
            <input
              id="entitlementDate"
              name="entitlementDate"
              type="date"
              value={selectedEntitlementDate}
              onChange={handleChange}
              required
            />
          </div>

          <div className="input-field-group">
            <label htmlFor="noOfSharesEligible">Number of shares eligible:</label>
            <input
              id="noOfSharesEligible"
              name="noOfSharesEligible"
              type="number"
              value={addDividendRequestDto.noOfSharesEligible}
              onChange={handleChange}
              required
            />
          </div>

          <div className="input-field-group">
            <label htmlFor="dividendPerShare">Dividend per share (RM):</label>
            <input
              id="dividendPerShare"
              name="dividendPerShare"
              type="number"
              value={addDividendRequestDto.dividendPerShare}
              onChange={handleChange}
              required
            />
          </div>

          <div className="input-field-group">
            <label htmlFor='isSubjectToWithholdingTax'>Subject to withholding tax?</label>
            <input
              id="isSubjectToWithholdingTax"
              name="isSubjectToWithholdingTax"
              type="checkbox"
              onChange={handleChange}
              checked={addDividendRequestDto.isSubjectToWithholdingTax}
            />
          </div>

          <div className="input-field-group">
            <label htmlFor="withholdingTax">Withholding tax (RM):</label>
            <input
              id="withholdingTax"
              name="withholdingTax"
              type="number"
              value={addDividendRequestDto.withholdingTax}
              onChange={handleChange}
              required
              disabled={isWithholdingTaxDisabled}
            />
          </div>

          <div className="button-group">
            <button className="dialog-button dialog-submit-button" type="submit">Submit</button>
            <button className="dialog-button" type="button" onClick={handleClear}>Clear</button>
            <button className="dialog-button" type="button" onClick={handleClose}>Cancel</button>
          </div>

        </form>
    </div>
  );
}

export default AddDividendDialog;