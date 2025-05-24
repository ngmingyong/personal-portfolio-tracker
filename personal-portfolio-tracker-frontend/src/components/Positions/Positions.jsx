import React, { Fragment, useState, useEffect } from 'react';
import './style/Positions.css';
import PositionResponseDto from '../../dtos/Response/PositionResponseDto';
import AddDividendDialog from '../Dialogs/AddDividendDialog';
import AddCapitalChangeDialog from '../Dialogs/AddCapitalChangeDialog';
import UpdatePositionStatusButton from '../Buttons/UpdatePositionStatusButton';
import DeleteButton from '../Buttons/DeleteButton';
import DeleteDialog from '../Dialogs/DeleteDialog';
import Transactions from '../Details/Transactions';
import Dividends from '../Details/Dividends';
import CapitalChanges from '../Details/CapitalChanges';
import { POST_METHOD, PATCH_METHOD, DELETE_METHOD, POSITION_ENDPOINT, DIVIDEND_ENDPOINT, CAPITAL_CHANGE_ENDPOINT } from '../../constants/constants';

function Positions({ data, apiCall}) {

  const [positionIdWithAddDividendDialogOpened, setPositionIdWithAddDividendDialogOpened] = useState(-1);
  const [stockCodeWithAddDividendDialogOpened, setStockCodeWithAddDividendDialogOpened] = useState(null);

  const [positionIdWithAddCapitalChangeDialogOpened, setPositionIdWithAddCapitalChangeDialogOpened] = useState(-1);
  const [stockCodeWithAddCapitalChangeDialogOpened, setStockCodeWithAddCapitalChangeDialogOpened] = useState(null);

  const [positionIdWithDeleteDialogOpened, setPositionIdWithDeleteDialogOpened] = useState(-1);
  const [stockCodeWithDeleteDialogOpened, setStockCodeWithDeleteDialogOpened] = useState(null);

  const [expandedSections, setExpandedSections] = useState({});

  let positions = undefined;
  if (data) {
    positions = data.map((position) =>
      new PositionResponseDto(
        position.id,
        position.stockCode,
        position.stockName,
        position.stockPrice,
        position.stockPriceLastUpdated,
        position.positionOpenedDate,
        position.positionClosedDate,
        position.isPositionClosed,
        position.noOfSharesHeld,
        position.totalPurchaseCost,
        position.totalDividendsReceived,
        position.totalNetSalesProceeds,
        position.unrealizedValueOfSharesHeldBeforeFinalSalesExpenses,
        position.hypotheticalTotalReturnBeforeFinalSalesExpenses,
        position.finalizedTotalReturn,
        position.transactions,
        position.dividends,
        position.capitalChanges,
      )
    );
  }

  useEffect(() => {
    if (data) {
      const initialExpandedSections = data.reduce((expandedSections, position) => ({
        ...expandedSections,
        [`${position.id}-positions`]: !position.isPositionClosed,
      }), {});
      setExpandedSections(initialExpandedSections);
    }
  }, [data]);

  const expandOrCollapse = (positionId, section) => {
    setExpandedSections(previousState => ({
      ...previousState,
      [`${positionId}-${section}`]: !previousState[`${positionId}-${section}`]
    }));
  };

  return (
    <>
      <AddDividendDialog
        positionIdWithAddDividendDialogOpened={positionIdWithAddDividendDialogOpened}
        stockCodeWithAddDividendDialogOpened={stockCodeWithAddDividendDialogOpened}
        closeDialog={() => setPositionIdWithAddDividendDialogOpened(-1)}
        apiCall={apiCall}
        endpoint={DIVIDEND_ENDPOINT}
        method={POST_METHOD}
      />

      <AddCapitalChangeDialog
        positionIdWithAddCapitalChangeDialogOpened={positionIdWithAddCapitalChangeDialogOpened}
        stockCodeWithAddCapitalChangeDialogOpened={stockCodeWithAddCapitalChangeDialogOpened}
        closeDialog={() => setPositionIdWithAddCapitalChangeDialogOpened(-1)}
        apiCall={apiCall}
        endpoint={CAPITAL_CHANGE_ENDPOINT}
        method={POST_METHOD}
      />

      <DeleteDialog
        entityId = {positionIdWithDeleteDialogOpened}
        text = {`Delete Position ${stockCodeWithDeleteDialogOpened}?`}
        closeDialog = {() => setPositionIdWithDeleteDialogOpened(-1)}
        apiCall = {apiCall}
        endpoint={`${POSITION_ENDPOINT}/${positionIdWithDeleteDialogOpened}`}
        method={DELETE_METHOD}
      />

      {positions?.length === 0 ? <h1 className="title-text">No position available.</h1> : null}
      {positions?.map((position) => (
        <Fragment key={position.id}>
          <div className="position-card">
            <div
              className="card-header"
              onClick={() => expandOrCollapse(position.id, 'positions')}
            >
              <span>
                <h2 className={`header-text ${position.isPositionClosed ? 'closed-position' : ''}`}>
                  {position.stockCode} {position.stockName.substring(0, 50)} {position.isPositionClosed ? "" : "(" + position.stockPrice.toFixed(3) + " on " + position.stockPriceLastUpdated + ")" }
                </h2>
              </span>
              <span>
                <div className="button-group">
                  <UpdatePositionStatusButton
                    noOfSharesHeld = {position.noOfSharesHeld}
                    isPositionClosed = {position.isPositionClosed}
                    stockCode = {position.stockCode}
                    expandOrCollapse={()=>expandOrCollapse(position.id, 'positions')}
                    apiCall = {apiCall}
                    endpoint= {`${POSITION_ENDPOINT}/${position.id}/status`}
                    method={PATCH_METHOD}
                  />
                  <DeleteButton
                    setEntityId={() => setPositionIdWithDeleteDialogOpened(position.id)}
                    setStockCode={() => setStockCodeWithDeleteDialogOpened(position.stockCode)}
                    expandOrCollapse={() => expandOrCollapse(position.id, 'positions')}
                  />
                </div>
              </span>
            </div>

            <div className={`position-card-content ${expandedSections[`${position.id}-positions`] ? 'expanded' : ''}`}>
              <div className="info-row position-info-row">
                <span>Position opened on {position.positionOpenedDate}</span>
              </div>

              {position.isPositionClosed ?
                <div className="info-row position-info-row">
                  <span>Position closed on {position.positionClosedDate}</span>
                </div> : null}

              <div className="info-row position-info-row">
                <span>Shares held: {position.noOfSharesHeld}</span>
              </div>

              <div className="info-row position-info-row">
                <span></span>
                <span>RM</span>
              </div>

              {position.isPositionClosed ? null :
                <div className="info-row position-info-row">
                  <span>Unrealized value of shares held ({position.noOfSharesHeld} x RM{position.stockPrice.toFixed(3)})</span>
                  <span>{position.unrealizedValueOfSharesHeldBeforeFinalSalesExpenses.toFixed(2)}</span>
                </div>}

              <div className="info-row position-info-row">
                <span>Total dividends received</span>
                <span>{position.totalDividendsReceived.toFixed(2)}</span>
              </div>

              <div className="info-row position-info-row">
                <span>Total net sales proceeds</span>
                <span>{position.totalNetSalesProceeds.toFixed(2)}</span>
              </div>

              <div className="info-row position-info-row">
                <span>Total purchase cost</span>
                <span>({position.totalPurchaseCost.toFixed(2)})</span>
              </div>

              {position.isPositionClosed ? null :
                <div className="info-row position-info-row">
                  <h3>Hypothetical total return (before final sales expenses)</h3>
                  <h3 className={position.hypotheticalTotalReturnBeforeFinalSalesExpenses === 0 ? "" : (position.hypotheticalTotalReturnBeforeFinalSalesExpenses > 0 ? "positive-value" : "negative-value")}>
                    {position.hypotheticalTotalReturnBeforeFinalSalesExpenses < 0 ? "(" + (position.hypotheticalTotalReturnBeforeFinalSalesExpenses.toFixed(2) + ")").substring(1) : position.hypotheticalTotalReturnBeforeFinalSalesExpenses.toFixed(2)}
                  </h3>
                </div>}

              {position.isPositionClosed ?
                <div className="info-row position-info-row">
                  <h3>Finalized total return</h3>
                  <h3 className={position.finalizedTotalReturn === 0 ? "" : (position.finalizedTotalReturn > 0 ? "positive-value" : "negative-value")}>
                    {position.finalizedTotalReturn < 0 ? "(" + (position.finalizedTotalReturn.toFixed(2) + ")").substring(1) : position.finalizedTotalReturn.toFixed(2)}
                  </h3>
                </div> : null}

                <div className="details-card">
                  <div
                    className="card-header"
                    onClick={() => expandOrCollapse(position.id, 'transactions')}
                  >
                    <h2 className="header-text">
                        {expandedSections[`${position.id}-transactions`] ? String.fromCodePoint(11167) : String.fromCodePoint(11166)} Transactions
                    </h2>
                  </div>
                  <Transactions
                    data={position.transactions}
                    positionStockCode={position.stockCode}
                    apiCall={apiCall}
                    isExpanded={expandedSections[`${position.id}-transactions`]}
                  />
                </div>

                <div className="details-card">
                  <div
                    className="card-header"
                    onClick={() => expandOrCollapse(position.id, 'dividends')}
                  >
                    <span>
                      <h2 className="header-text">
                        {expandedSections[`${position.id}-dividends`] ? String.fromCodePoint(11167) : String.fromCodePoint(11166)} Dividends
                      </h2>
                    </span>
                    <span>
                      {position.isPositionClosed ? null :
                      <button onClick={
                        () => {
                          setPositionIdWithAddDividendDialogOpened(position.id);
                          setStockCodeWithAddDividendDialogOpened(position.stockCode);
                          expandOrCollapse(position.id, 'dividends');
                        }}
                      >
                        <b>&#9547;</b>
                      </button>}
                    </span>
                  </div>
                  <Dividends
                    data={position.dividends}
                    positionStockCode={position.stockCode}
                    apiCall={apiCall}
                    isExpanded={expandedSections[`${position.id}-dividends`]}
                  />
                </div>

                <div className="details-card">
                  <div
                    className="card-header"
                    onClick={() => expandOrCollapse(position.id, 'capitalChanges')}
                  >
                    <span>
                      <h2 className="header-text">
                        {expandedSections[`${position.id}-capitalChanges`] ? String.fromCodePoint(11167) : String.fromCodePoint(11166)} Capital Changes
                      </h2>
                    </span>
                    <span>
                      {position.isPositionClosed ? null :
                      <button onClick={
                        () => {
                          setPositionIdWithAddCapitalChangeDialogOpened(position.id);
                          setStockCodeWithAddCapitalChangeDialogOpened(position.stockCode);
                          expandOrCollapse(position.id, 'capitalChanges');
                        }}
                      >
                        <b>&#9547;</b>
                      </button>}
                    </span>
                  </div>
                  <CapitalChanges
                    data={position.capitalChanges}
                    positionStockCode={position.stockCode}
                    apiCall={apiCall}
                    isExpanded={expandedSections[`${position.id}-capitalChanges`]}
                  />
                </div>
            </div>
          </div>
        </Fragment>
      ))}
    </>
  );
}

export default Positions;