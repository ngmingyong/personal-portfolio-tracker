import './style/Dialogs.css';

function DeleteDialog({ entityId, text, closeDialog, apiCall, endpoint, method }) {

  const handleClose = () => {
    closeDialog();
  };

  const handleDelete = async (e) => {
    e.preventDefault();
    await apiCall(endpoint, method);
  };

  if (entityId < 0) return null;

  return (
    <div className="dialog">
      <h2>{text}</h2>
      <div className="button-group">
        <button className="dialog-button dialog-delete-button" onClick={handleDelete}>Delete</button>
        <button className="dialog-button" type="button" onClick={handleClose}>Cancel</button>
      </div>
    </div>
  );
}

export default DeleteDialog;