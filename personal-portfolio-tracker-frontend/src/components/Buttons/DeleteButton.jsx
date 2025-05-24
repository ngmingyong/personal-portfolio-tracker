import './style/Buttons.css';

function DeleteButton({ setEntityId, setStockCode, expandOrCollapse = null }) {  

  const handleClick = async (e) => {
    e.preventDefault();
    setEntityId();
    setStockCode();
    if (expandOrCollapse != null){
        expandOrCollapse();
    }
  };

  return (
    <button className="delete-button" onClick={handleClick}>
      &#128465;
    </button>
  );
}

export default DeleteButton;