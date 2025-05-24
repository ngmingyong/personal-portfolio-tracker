import { useState } from 'react';
import './style/Dialogs.css';
import AddTransactionRequestDto from '../../dtos/Request/AddTransactionRequestDto';
import { format } from 'date-fns';

function AddTransactionDialog({ isDialogOpen, closeDialog, apiCall, endpoint, method }) {

  const [addTransactionRequestDto, setAddransactionRequestDto] = useState(new AddTransactionRequestDto('', '', '', '', '', ''));
  const [selectedDate, setSelectedDate] = useState('');
  const [isBuyChecked, setIsBuyChecked] = useState(false);
  const [isSellChecked, setIsSellChecked] = useState(false);

  const handleChange = (e) => {
    const { name, value } = e.target;

    let formattedValue = value;
    if (name === 'date') {
      formattedValue = format(value, 'dd-MMM-yyyy')
      setSelectedDate(value);
    }

    if (name==='type')
    {
      if (value === 'buy'){
        setIsBuyChecked(true);
        setIsSellChecked(false);
      }
      else if (value === 'sell'){
        setIsBuyChecked(false);
        setIsSellChecked(true);
      }
    }

    if (name === 'noOfSharesTransacted' && !(/^([1-9][0-9]*)?$/.test(value))){
      alert('Number of shares to be transacted must be an integer larger than zero.');
      return;
    }

    if (name === 'transactedPricePerShare' &&  !(/^(0|[1-9]\d*)(\.\d{1,3})?$|^$/.test(value))){
      alert('Transacted price per share cannot be negative, and must be either an integer or a decimal with up to three decimal places.');
      return;
    }

    if (name === 'totalTransactionRelatedExpenses' &&  !(/^(0|[1-9]\d*)(\.\d{1,2})?$|^$/.test(value))){
      alert('Total transaction-related expenses cannot be negative, and must be either an integer or a decimal with up to two decimal places.');
      return;
    }

    setAddransactionRequestDto(previousState => ({
      ...previousState,
      [name]: formattedValue
    }));
  };

  const handleClear = () => {
    setSelectedDate('');
    setIsBuyChecked(false);
    setIsSellChecked(false);
    setAddransactionRequestDto(new AddTransactionRequestDto('', '', '', '', '', ''));
  };

  const handleClose = () => {
    setSelectedDate('');
    setIsBuyChecked(false);
    setIsSellChecked(false);
    setAddransactionRequestDto(new AddTransactionRequestDto('', '', '', '', '', ''));
    closeDialog();
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    await apiCall(endpoint, method, addTransactionRequestDto);
  };

  if (!isDialogOpen) return null;

  return (
    <div className="dialog">
      <h2>Add new transaction</h2>
      <form onSubmit={handleSubmit}>

        <div className="input-field-group">
          <label htmlFor="stockCode">Stock code:</label>
          <input
            id="stockCode"
            name="stockCode"
            type="text"
            value={addTransactionRequestDto.stockCode}
            onChange={handleChange}
            required
          />
        </div>

        <div className="input-field-group">
          <label htmlFor="date">Date:</label>
          <input
            id="date"
            name="date"
            type="date"
            value={selectedDate}
            onChange={handleChange}
            required
          />
        </div>

        <div className="input-field-group">
          <label>Type:</label>
          <div className="radio-group">
            <div className="radio-option">
              <input 
                id="buy"
                name="type"
                type="radio"
                value="buy"
                onChange={handleChange}
                required
                checked={isBuyChecked}
              />
              <label htmlFor="buy">Buy</label>
            </div>
            <div className="radio-option">
              <input
                id="sell"
                name="type"
                type="radio"
                value="sell"
                onChange={handleChange}
                required
                checked={isSellChecked}
              />
              <label htmlFor="sell">Sell</label>
            </div>
          </div>
        </div>

        <div className="input-field-group">
          <label htmlFor="noOfSharesTransacted">Number of shares transacted:</label>
          <input
            id="noOfSharesTransacted"
            name="noOfSharesTransacted"
            type="number"
            value={addTransactionRequestDto.noOfSharesTransacted}
            onChange={handleChange}
            required
          />
        </div>

        <div className="input-field-group">
          <label htmlFor="transactedPricePerShare">Transacted price per share (RM):</label>
          <input
            id="transactedPricePerShare"
            name="transactedPricePerShare"
            type="number"
            value={addTransactionRequestDto.transactedPricePerShare}
            onChange={handleChange}
            required
          />
        </div>

        <div className="input-field-group">
          <label htmlFor="totalTransactionRelatedExpenses">Total transaction-related expenses (RM):</label>
          <input
            id="totalTransactionRelatedExpenses"
            name="totalTransactionRelatedExpenses"
            type="number"
            value={addTransactionRequestDto.totalTransactionRelatedExpenses}
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

export default AddTransactionDialog;