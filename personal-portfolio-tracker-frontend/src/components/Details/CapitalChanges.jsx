import React, { Fragment, useState } from 'react';
import './style/Details.css';
import CapitalChangeResponseDto from '../../dtos/Response/CapitalChangeResponseDto';
import DeleteButton from '../Buttons/DeleteButton';
import DeleteDialog from '../Dialogs/DeleteDialog';
import { CAPITAL_CHANGE_ENDPOINT, DELETE_METHOD } from '../../constants/constants';

function CapitalChanges({ data, positionStockCode, apiCall, isExpanded }) {

  const [capitalChangeIdWithDeleteDialogOpened, setCapitalChangeIdWithDeleteDialogOpened] = useState(-1);
  const [stockCodeWithDeleteDialogOpened, setStockCodeWithDeleteDialogOpened] = useState(null);

  let capitalChanges = undefined;
  if (data) {
    capitalChanges = data.map((capitalChange) =>
    new CapitalChangeResponseDto(
      capitalChange.id,
      capitalChange.entitlementDate,
      capitalChange.type,
      capitalChange.changeInNoOfShares,
      capitalChange.note
    ));
  }

  return (
    <>
      <DeleteDialog
        entityId = {capitalChangeIdWithDeleteDialogOpened}
        text = {`Delete capital change record for Position ${stockCodeWithDeleteDialogOpened}?`}
        closeDialog = {() => setCapitalChangeIdWithDeleteDialogOpened(-1)}
        apiCall = {apiCall}
        endpoint={`${CAPITAL_CHANGE_ENDPOINT}/${capitalChangeIdWithDeleteDialogOpened}`}
        method={DELETE_METHOD}
      />

      <div className={`details-card-content ${isExpanded ? 'expanded' : ''}`}>
        {capitalChanges?.map((capitalChange) => (
          <Fragment key={capitalChange.id}>
            <div className="details-item">
              <div className="details-info-row info-row">
                <span><h3 className="details-item-header">Entitlement Date: {capitalChange.entitlementDate}</h3></span>
                <span>
                  <DeleteButton
                    setEntityId={() => setCapitalChangeIdWithDeleteDialogOpened(capitalChange.id)}
                    setStockCode={() => setStockCodeWithDeleteDialogOpened(positionStockCode)}
                  />
                </span>
              </div>

              <div className="details-info-row info-row">
                <span>
                  <b>{capitalChange.type.toUpperCase()}</b> number of shares by: {capitalChange.changeInNoOfShares}
                </span>
              </div>

              <div className="details-info-row info-row">
                <span>Note: {capitalChange.note}</span>
              </div>
            </div>
          </Fragment>
        ))}
      </div>
    </>
  );
}

export default CapitalChanges;