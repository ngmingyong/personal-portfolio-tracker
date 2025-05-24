import React, { Fragment, useState } from 'react';
import './style/Details.css';
import DividendResponseDto from '../../dtos/Response/DividendResponseDto';
import DeleteButton from '../Buttons/DeleteButton';
import DeleteDialog from '../Dialogs/DeleteDialog';
import { DIVIDEND_ENDPOINT, DELETE_METHOD } from '../../constants/constants';
function Dividends({ data, positionStockCode, apiCall, isExpanded }) {

  const [dividendIdWithDeleteDialogOpened, setDividendIdWithDeleteDialogOpened] = useState(-1);
  const [stockCodeWithDeleteDialogOpened, setStockCodeWithDeleteDialogOpened] = useState(null);

  let dividends = undefined;
  if (data) {
    dividends = data.map((dividend) =>
    new DividendResponseDto(
      dividend.id,
      dividend.entitlementDate,
      dividend.noOfSharesEligible,
      dividend.dividendPerShare,
      dividend.amountBeforeWithholdingTax,
      dividend.isSubjectToWithholdingTax,
      dividend.withholdingTax,
      dividend.amountReceived
    ));
  }

  return (
    <>
      <DeleteDialog
        entityId = {dividendIdWithDeleteDialogOpened}
        text = {`Delete dividend record for Position ${stockCodeWithDeleteDialogOpened}?`}
        closeDialog = {() => setDividendIdWithDeleteDialogOpened(-1)}
        apiCall = {apiCall}
        endpoint={`${DIVIDEND_ENDPOINT}/${dividendIdWithDeleteDialogOpened}`}
        method={DELETE_METHOD}
      />

      <div className={`details-card-content ${isExpanded ? 'expanded' : ''}`}>
        {dividends?.map((dividend) => (
          <Fragment key={dividend.id}>
            <div className="details-item">
              <div className="details-info-row info-row">
                <span><h3 className="details-item-header">Entitlement Date: {dividend.entitlementDate}</h3></span>
                <span>
                  <DeleteButton
                    setEntityId={() => setDividendIdWithDeleteDialogOpened(dividend.id)}
                    setStockCode={() => setStockCodeWithDeleteDialogOpened(positionStockCode)}
                  />
                </span>
              </div>

              <div className="details-info-row info-row">
                <span></span>
                <span>RM</span>
              </div>

              <div className="details-info-row info-row">
                <span>{dividend.noOfSharesEligible} x RM{dividend.dividendPerShare.toFixed(Math.max(4, (dividend.dividendPerShare.toString().split('.')[1] || []).length))}</span>
                <span>{dividend.amountBeforeWithholdingTax.toFixed(2)}</span>
              </div>

              <div className="details-info-row info-row">
                <span>Withholding tax</span>
                <span>{dividend.isSubjectToWithholdingTax ? "(" + dividend.withholdingTax.toFixed(2) + ")" : "-" }</span>
              </div>

              <div className="details-info-row info-row">
                <span></span>
                <span className="positive-value">
                  {dividend.amountReceived.toFixed(2)}
                </span>
              </div>
            </div>
          </Fragment>
        ))}
      </div>
    </>
  );
}

export default Dividends;