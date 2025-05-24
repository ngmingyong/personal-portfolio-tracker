import { useState } from 'react';
import './style/App.css';
import useApiCall from '../hooks/useApiCall';
import AddTransactionDialog from '../components/Dialogs/AddTransactionDialog';
import Positions from '../components/Positions/Positions';
import { POST_METHOD, GET_METHOD, TRANSACTION_ENDPOINT, POSITIONS_ENDPOINT } from '../constants/constants';

function App() {

  const { data, makeApiCall } = useApiCall(POSITIONS_ENDPOINT, GET_METHOD);

  const apiCall = async (endpoint, method, body, refresh = true) => {
    await makeApiCall(endpoint, method, body, refresh);
  };

  const [isAddTransactionDialogOpen, setIsAddTransactionDialogOpen] = useState(false);

  if (!data) {
    return <div>Loading...</div>;
  } else{
    return (
      <>
        <AddTransactionDialog
          isDialogOpen={isAddTransactionDialogOpen}
          closeDialog={() => setIsAddTransactionDialogOpen(false)}
          apiCall={apiCall}
          endpoint={TRANSACTION_ENDPOINT}
          method={POST_METHOD}
        />

        <div className="frame">
          <div className="title-bar">
            <span>
              <h1 className="title-text">
                Personal Portfolio Tracker
              </h1>
            </span>
            <span className="test">
              <button onClick={() => setIsAddTransactionDialogOpen(true)}>
                + Transaction
              </button>
            </span>
          </div>
          <Positions data={data} apiCall={apiCall} />
        </div>
      </>
    );
  }
}

export default App;