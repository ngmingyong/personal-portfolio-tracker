import { useState, useEffect } from 'react';
import './style/Dialogs.css';
import AddCapitalChangeRequestDto from '../../dtos/Request/AddCapitalChangeRequestDto';
import { format } from 'date-fns';

function AddCapitalChangeDialog({ positionIdWithAddCapitalChangeDialogOpened, stockCodeWithAddCapitalChangeDialogOpened, closeDialog, apiCall, endpoint, method }) {

  const [addCapitalChangeRequestDto, setAddCapitalChangeRequestDto] = useState(new AddCapitalChangeRequestDto(positionIdWithAddCapitalChangeDialogOpened, '', '', '', ''));
  const [selectedEntitlementDate, setSelectedEntitlementDate] = useState('');
  const [isIncreaseChecked, setIsIncreaseChecked] = useState(false);
  const [isDecreaseChecked, setIsDecreaseChecked] = useState(false);

  useEffect(() => {
    setAddCapitalChangeRequestDto(previousState => ({
      ...previousState,
      ['positionId']: positionIdWithAddCapitalChangeDialogOpened
    }))}, [positionIdWithAddCapitalChangeDialogOpened]);

    const handleChange = (e) => {

    const { name, value } = e.target;

    let formattedValue = value;
    if (name === 'entitlementDate') {
      formattedValue = format(value, 'dd-MMM-yyyy')
      setSelectedEntitlementDate(value);
    }

    if (name==='type')
    {
      if (value === 'increase'){
        setIsIncreaseChecked(true);
        setIsDecreaseChecked(false);
      }
      else if (value === 'decrease'){
        setIsIncreaseChecked(false);
        setIsDecreaseChecked(true);
      }
    }

    if (name === 'changeInNoOfShares' && !(/^([1-9][0-9]*)?$/.test(value))){
      alert('Change in number of shares must be an integer larger than zero.');
      return;
    }

    setAddCapitalChangeRequestDto(previousState => ({
      ...previousState,
      [name]: formattedValue
    }));
  };

  const handleClear = () => {
    setSelectedEntitlementDate('');
    setIsIncreaseChecked(false);
    setIsDecreaseChecked(false);
    setAddCapitalChangeRequestDto(new AddCapitalChangeRequestDto(positionIdWithAddCapitalChangeDialogOpened, '', '', '', ''));
  };

  const handleClose = () => {
    setSelectedEntitlementDate('');
    setIsIncreaseChecked(false);
    setIsDecreaseChecked(false);
    setAddCapitalChangeRequestDto(new AddCapitalChangeRequestDto(positionIdWithAddCapitalChangeDialogOpened, '', '', '', ''));
    closeDialog();
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    await apiCall(endpoint, method, addCapitalChangeRequestDto);
  };

  if (positionIdWithAddCapitalChangeDialogOpened < 0) return null;

  return (
    <div className="dialog">
      <h2>Add new capital change for Position {stockCodeWithAddCapitalChangeDialogOpened}</h2>
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
          <label>Type:</label>
          <div className="radio-group">
            <div className="radio-option">
              <input 
                id="increase"
                name="type"
                type="radio"
                value="increase"
                onChange={handleChange}
                required
                checked={isIncreaseChecked}
              />
              <label htmlFor="increase">Increase</label>
            </div>
            <div className="radio-option">
              <input
                id="decrease"
                name="type"
                type="radio"
                value="decrease"
                onChange={handleChange}
                required
                checked={isDecreaseChecked}
              />
              <label htmlFor="decrease">Decrease</label>
            </div>
          </div>
        </div>

        <div className="input-field-group">
          <label htmlFor="changeInNoOfShares">Change in number of shares:</label>
          <input
            id="changeInNoOfShares"
            name="changeInNoOfShares"
            type="number"
            value={addCapitalChangeRequestDto.changeInNoOfShares}
            onChange={handleChange}
            required
          />
        </div>

        <div className="input-field-group">  
          <label htmlFor="note">Note:</label>
          <input
            id="note"
            name="note"
            type="text"
            value={addCapitalChangeRequestDto.note}
            onChange={handleChange}
            required
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

export default AddCapitalChangeDialog;