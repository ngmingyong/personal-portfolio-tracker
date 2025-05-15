import { useState, useEffect } from 'react';
import UpdatePositionStatusRequestDto from '../../dtos/Request/UpdatePositionStatusRequestDto';

function UpdatePositionStatusButton({ noOfSharesHeld, isPositionClosed, stockCode, expandOrCollapse, apiCall, endpoint, method }) {  

  const [ isButtonShown, setIsButtonShown ] = useState(false);
  const [ buttonText, setButtonText ] = useState(null);

  useEffect(() => {
    if (isPositionClosed && noOfSharesHeld > 0){
      setButtonText(`Set Position ${stockCode} as OPEN`);
      setIsButtonShown(true);
    }
    else if (!isPositionClosed && noOfSharesHeld === 0){
      setButtonText(`Set Position ${stockCode} as CLOSED`);
      setIsButtonShown(true);
    }
    else{
      setButtonText(null);
      setIsButtonShown(false);
    }
  }, []);

  const handleUpdateStatus = async (e) => {
    e.preventDefault();
    expandOrCollapse();
    await apiCall(endpoint, method, new UpdatePositionStatusRequestDto(!isPositionClosed));
  };

  if (!isButtonShown) return null;

  return (
    <button onClick={handleUpdateStatus}>
      {buttonText}
    </button>
  );
}

export default UpdatePositionStatusButton;