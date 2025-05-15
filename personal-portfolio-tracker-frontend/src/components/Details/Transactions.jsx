import React, { Fragment, useState } from 'react';
import './style/Details.css';
import TransactionResponseDto from '../../dtos/Response/TransactionResponseDto';
import DeleteButton from '../Buttons/DeleteButton';
import DeleteDialog from '../Dialogs/DeleteDialog';
import { TRANSACTION_ENDPOINT, DELETE_METHOD } from '../../constants/constants';

function Transactions({ data, positionStockCode, apiCall, isExpanded }) {

  const [transactionIdWithDeleteDialogOpened, setTransactionIdWithDeleteDialogOpened] = useState(-1);
  const [stockCodeWithDeleteDialogOpened, setStockCodeWithDeleteDialogOpened] = useState(null);

  let transactions = undefined;
  if (data) {
    transactions = data.map((transaction) =>
    new TransactionResponseDto(
      transaction.id,
      transaction.date,
      transaction.type,
      transaction.noOfSharesTransacted,
      transaction.transactedPricePerShare,
      transaction.transactionValueBeforeExpenses,
      transaction.totalTransactionRelatedExpenses,
      transaction.purchaseCostOrNetSalesProceeds
    ));
  }

  return (
    <>
      <DeleteDialog
        entityId = {transactionIdWithDeleteDialogOpened}
        text = {`Delete transaction record for Position ${stockCodeWithDeleteDialogOpened}?`}
        closeDialog = {() => setTransactionIdWithDeleteDialogOpened(-1)}
        apiCall = {apiCall}
        endpoint={`${TRANSACTION_ENDPOINT}/${transactionIdWithDeleteDialogOpened}`}
        method={DELETE_METHOD}
      />

      <div className={`details-card-content ${isExpanded ? 'expanded' : ''}`}>
        {transactions?.map((transaction) => (
          <Fragment key={transaction.id}>
            <div className="details-item">
              <div className="details-info-row info-row">
                <span><h3 className="details-item-header">Transaction Date: {transaction.date}</h3></span>
                <span>
                  <DeleteButton
                    setEntityId={() => setTransactionIdWithDeleteDialogOpened(transaction.id)}
                    setStockCode={() => setStockCodeWithDeleteDialogOpened(positionStockCode)}
                  />
                </span>
              </div>

              <div className="details-info-row info-row">
                <span></span>
                <span>RM</span>
              </div>

              <div className="details-info-row info-row">
                <span>
                  <b>{transaction.type.toUpperCase()}</b> {transaction.noOfSharesTransacted} x RM{transaction.transactedPricePerShare.toFixed(3)}
                </span>
                <span>
                  {transaction.type === 'Buy' ? "(" + transaction.transactionValueBeforeExpenses.toFixed(2) + ")" : transaction.transactionValueBeforeExpenses.toFixed(2)}
                </span>
              </div>

              <div className="details-info-row info-row">
                <span>Related expenses</span>
                <span>({transaction.totalTransactionRelatedExpenses.toFixed(2)})</span>
              </div>

              <div className="details-info-row info-row">
                <span></span>
                <span className={transaction.type === 'Buy' ? "negative-value" : "positive-value"}>
                  {transaction.type === 'Buy' ? "(" + transaction.purchaseCostOrNetSalesProceeds.toFixed(2) + ")" : transaction.purchaseCostOrNetSalesProceeds.toFixed(2)}
                </span>
              </div>
            </div>
          </Fragment>
        ))}
      </div>
    </>
  );
}

export default Transactions;