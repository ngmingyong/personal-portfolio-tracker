import { useState, useEffect } from 'react';
import apiCall from '../services/apiCall';

function useApiCall(endpoint, method, body = null) {

  const [data, setData] = useState(null);

  const controller = new AbortController();

  async function makeApiCall(endpoint, method, body, refresh = false) {

    try {
      const responseData = await apiCall(endpoint, method, body, controller.signal);
      setData(responseData);

      if (refresh){
        alert(responseData);
        window.location.reload();
      }
    } catch (err) {
      if (err?.name !== 'AbortError') {
        alert(err.message);
        window.location.reload();
      }
    }
  }

  useEffect(() => {

    makeApiCall(endpoint, method, body);

    return () => {
      controller.abort();
    };

  }, []);

  return { data, makeApiCall };
}

export default useApiCall;