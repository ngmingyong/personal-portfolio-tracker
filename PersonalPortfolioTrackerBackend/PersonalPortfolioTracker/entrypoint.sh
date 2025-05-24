#!/bin/sh

set -e

export PERSONAL_PORTFOLIO_TRACKER_DB_CONNECTION_STRING=$(cat /run/secrets/personal_portfolio_tracker_db_connection_string)
export PERSONAL_PORTFOLIO_TRACKER_FRONTEND=$(cat /run/secrets/personal_portfolio_tracker_frontend)
export STOCK_SEARCH_API_PATH=$(cat /run/secrets/stock_search_api_path)
export STOCK_SEARCH_API_TOKEN=$(cat /run/secrets/stock_search_api_token)

exec "$@"