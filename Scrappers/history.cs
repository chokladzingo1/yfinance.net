using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using yfinance.data;
using yfinance.consts;
using yfinance.exceptions;
using yfinance.utils;

namespace yfinance.scrapers
{
    public class PriceHistory
    {
        private YfData _data;
        private string _ticker;
        private string _tz;
        private object _session;
        private Dictionary<string, DataTable> _historyCache;
        private JsonElement? _historyMetadata;
        private bool _historyMetadataFormatted;
        private string _reconstructStartInterval;

        public PriceHistory(YfData data, string ticker, string tz, object session = null, object proxy = null)
        {
            _data = data;
            _ticker = ticker.ToUpper();
            _tz = tz;
            if (proxy != null && !proxy.Equals(Consts.SENTINEL))
            {
                Debug.WriteLine("Set proxy via new config function: yf.set_config(proxy=proxy)");
                _data.SetProxy(proxy);
            }
            _session = session;
            _historyCache = new Dictionary<string, DataTable>();
            _historyMetadata = null;
            _historyMetadataFormatted = false;
            _reconstructStartInterval = null;
        }

        public DataTable History(
            string period = null,
            string interval = "1d",
            DateTime? start = null,
            DateTime? end = null,
            bool prepost = false,
            bool actions = true,
            bool autoAdjust = true,
            bool backAdjust = false,
            bool repair = false,
            bool keepna = false,
            object proxy = null,
            bool rounding = false,
            int timeout = 10,
            bool raiseErrors = false)
        {
            // Logging
            var logger = Utils.GetYfLogger();

            if (proxy != null && !proxy.Equals(Consts.SENTINEL))
            {
                Debug.WriteLine("Set proxy via new config function: yf.set_config(proxy=proxy)");
                _data.SetProxy(proxy);
            }

            // Prepare parameters
            var paramsDict = new Dictionary<string, string>();
            string intervalUser = interval;
            string periodUser = period;

            // ... (Handle period/start/end logic as in Python, omitted for brevity) ...

            // Compose Yahoo Finance API URL
            string url = $"{Consts.BASE_URL}/v8/finance/chart/{_ticker}";

            // Fetch data (synchronously for simplicity)
            string jsonStr;
            try
            {
                jsonStr = _data.GetRawJson(url, paramsDict);
            }
            catch (YFRateLimitError)
            {
                throw;
            }
            catch (Exception ex)
            {
                if (raiseErrors)
                    throw;
                logger.Error($"{_ticker}: {ex.Message}");
                return Utils.EmptyDf();
            }

            // Parse JSON
            JsonDocument doc = JsonDocument.Parse(jsonStr);
            var root = doc.RootElement;
            var chart = root.GetProperty("chart");
            var resultArr = chart.GetProperty("result");
            if (resultArr.GetArrayLength() == 0)
            {
                logger.Error($"{_ticker}: No chart result.");
                return Utils.EmptyDf();
            }
            var result = resultArr[0];

            // Store metadata
            _historyMetadata = result.GetProperty("meta");

            // Parse OHLCV
            DataTable quotes = Utils.ParseQuotes(result);

            // Parse actions
            var (dividends, splits, capitalGains) = Utils.ParseActions(result);

            // Merge actions into quotes
            if (dividends.Rows.Count > 0)
                quotes = Utils.SafeMergeDfs(quotes, dividends, interval);
            if (splits.Rows.Count > 0)
                quotes = Utils.SafeMergeDfs(quotes, splits, interval);
            if (capitalGains.Rows.Count > 0)
                quotes = Utils.SafeMergeDfs(quotes, capitalGains, interval);

            // Fill missing columns
            if (!quotes.Columns.Contains("Dividends"))
                quotes.Columns.Add("Dividends", typeof(double));
            if (!quotes.Columns.Contains("Stock Splits"))
                quotes.Columns.Add("Stock Splits", typeof(double));
            if (!quotes.Columns.Contains("Capital Gains"))
                quotes.Columns.Add("Capital Gains", typeof(double));

            // Auto/back adjust
            if (autoAdjust)
                quotes = Utils.AutoAdjust(quotes);
            else if (backAdjust)
                quotes = Utils.BackAdjust(quotes);

            // Rounding
            if (rounding)
            {
                foreach (DataColumn col in quotes.Columns)
                {
                    if (col.DataType == typeof(double))
                    {
                        foreach (DataRow row in quotes.Rows)
                        {
                            row[col] = Math.Round(Convert.ToDouble(row[col]), 2);
                        }
                    }
                }
            }

            // Set index name
            quotes.TableName = interval.EndsWith("m") || interval.EndsWith("h") ? "Datetime" : "Date";

            // Remove actions if not requested
            if (!actions)
            {
                if (quotes.Columns.Contains("Dividends")) quotes.Columns.Remove("Dividends");
                if (quotes.Columns.Contains("Stock Splits")) quotes.Columns.Remove("Stock Splits");
                if (quotes.Columns.Contains("Capital Gains")) quotes.Columns.Remove("Capital Gains");
            }

            // Remove rows with all NaN/zero if not keepna
            if (!keepna)
            {
                var toRemove = new List<DataRow>();
                foreach (DataRow row in quotes.Rows)
                {
                    bool allZeroOrNaN = true;
                    foreach (DataColumn col in quotes.Columns)
                    {
                        if (col.DataType == typeof(double) && !double.IsNaN(Convert.ToDouble(row[col])) && Convert.ToDouble(row[col]) != 0)
                        {
                            allZeroOrNaN = false;
                            break;
                        }
                    }
                    if (allZeroOrNaN)
                        toRemove.Add(row);
                }
                foreach (var row in toRemove)
                    quotes.Rows.Remove(row);
            }

            return quotes;
        }

        public DataTable GetHistoryCache(string period = "max", string interval = "1d")
        {
            string cacheKey = $"{interval}_{period}";
            if (_historyCache.ContainsKey(cacheKey))
                return _historyCache[cacheKey];

            var df = History(period: period, interval: interval, prepost: true);
            _historyCache[cacheKey] = df;
            return df;
        }

        public JsonElement? GetHistoryMetadata(object proxy = null)
        {
            if (proxy != null && !proxy.Equals(Consts.SENTINEL))
            {
                Debug.WriteLine("Set proxy via new config function: yf.set_config(proxy=proxy)");
                _data.SetProxy(proxy);
            }

            if (_historyMetadata == null)
            {
                // Request intraday data to get tradingPeriods
                GetHistoryCache(period: "5d", interval: "1h");
            }

            if (!_historyMetadataFormatted)
            {
                // Format metadata if needed (implement as needed)
                _historyMetadataFormatted = true;
            }

            return _historyMetadata;
        }

        // Add methods for get_dividends, get_capital_gains, get_splits, get_actions, etc.
        // using GetHistoryCache and DataTable filtering as needed.
    }
}