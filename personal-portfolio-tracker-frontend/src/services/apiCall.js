async function apiCall(endpoint, method, body, signal) {

  try { 
    const options = {
      mode: 'cors',
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      },
      method: method,
      body: body ? JSON.stringify(body) : undefined,
      signal: signal
    };

    const response = await fetch(`${import.meta.env.VITE_API_BASE_URL}/${endpoint}`, options);
    const responseData = await response.json();

    if (response.ok) {
      return responseData;
    } else {
      throw new Error(`Error ${response.status}: ${responseData}`);
    }

  } catch (error) {
    throw error;
  }
}

export default apiCall;