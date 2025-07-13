using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using System.Diagnostics;

using utils;
using consts;
using data;
using exceptions;

namespace yfinance.scrapers
{
    public class Fundamentals
    {
        private YfData _data;
        private string _symbol;

        private object _earnings;
        private object _shares;

        private object _financialsData;
        private object _finDataQuote;
        private bool _basicsAlreadyScraped;
        private Financials _financials;

        public Fundamentals(YfData data, string symbol, object proxy = null)
        {
            if (proxy != null && !proxy.Equals(Consts.SENTINEL))
            {
                Debug.WriteLine("Set proxy via new config function: yf.set_config(proxy=proxy)");
                _data.SetProxy(proxy);
            }

            _data = data;
            _symbol = symbol;

            _earnings = null;
            _shares = null;
            _financialsData = null;
            _finDataQuote = null;
            _basicsAlreadyScraped = false;
            _financials = new Financials(data, symbol);
        }

        public Financials Financials => _financials;

        public object Earnings
        {
            get
            {
                Debug.WriteLine("'Ticker.earnings' is deprecated as not available via API. Look for \"Net Income\" in Ticker.income_stmt.");
                return null;
            }
        }

        public DataTable Shares
        {
            get
            {
                if (_shares == null)
                    throw new YFNotImplementedError("shares");
                return (DataTable)_shares;
            }
        }
    }

    public class Financials
    {
        private YfData _data;
        private string _symbol;
        private Dictionary<string, DataTable> _incomeTimeSeries;
        private Dictionary<string, DataTable> _balanceSheetTimeSeries;
        private Dictionary<string, DataTable> _cashFlowTimeSeries;

        public Financials(YfData data, string symbol)
        {
            _data = data;
            _symbol = symbol;
            _incomeTimeSeries = new Dictionary<string, DataTable>();
            _balanceSheetTimeSeries = new Dictionary<string, DataTable>();
            _cashFlowTimeSeries = new Dictionary<string, DataTable>();
        }

        public DataTable GetIncomeTimeSeries(string freq = "yearly", object proxy = null)
        {
            if (proxy != null && !proxy.Equals(Consts.SENTINEL))
            {
                Debug.WriteLine("Set proxy via new config function: yf.set_config(proxy=proxy)");
                _data.SetProxy(proxy);
            }

            if (!_incomeTimeSeries.ContainsKey(freq))
                _incomeTimeSeries[freq] = FetchTimeSeries("income", freq);
            return _incomeTimeSeries[freq];
        }

        public DataTable GetBalanceSheetTimeSeries(string freq = "yearly", object proxy = null)
        {
            if (proxy != null && !proxy.Equals(Consts.SENTINEL))
            {
                Debug.WriteLine("Set proxy via new config function: yf.set_config(proxy=proxy)");
                _data.SetProxy(proxy);
            }

            if (!_balanceSheetTimeSeries.ContainsKey(freq))
                _balanceSheetTimeSeries[freq] = FetchTimeSeries("balance-sheet", freq);
            return _balanceSheetTimeSeries[freq];
        }

        public DataTable GetCashFlowTimeSeries(string freq = "yearly", object proxy = null)
        {
            if (proxy != null && !proxy.Equals(Consts.SENTINEL))
            {
                Debug.WriteLine("Set proxy via new config function: yf.set_config(proxy=proxy)");
                _data.SetProxy(proxy);
            }

            if (!_cashFlowTimeSeries.ContainsKey(freq))
                _cashFlowTimeSeries[freq] = FetchTimeSeries("cash-flow", freq);
            return _cashFlowTimeSeries[freq];
        }

        private DataTable FetchTimeSeries(string name, string timescale)
        {
            var allowedNames = new HashSet<string> { "income", "balance-sheet", "cash-flow" };
            var allowedTimescales = new HashSet<string> { "yearly", "quarterly", "trailing" };

            if (!allowedNames.Contains(name))
                throw new ArgumentException($"Illegal argument: name must be one of: {string.Join(", ", allowedNames)}");
            if (!allowedTimescales.Contains(timescale))
                throw new ArgumentException($"Illegal argument: timescale must be one of: {string.Join(", ", allowedTimescales)}");
            if (timescale == "trailing" && name != "income" && name != "cash-flow")
                throw new ArgumentException("Illegal argument: frequency 'trailing' only available for cash-flow or income data.");

            try
            {
                var statement = CreateFinancialsTable(name, timescale);
                if (statement != null)
                    return statement;
            }
            catch (YFException e)
            {
                Debug.WriteLine($"{_symbol}: Failed to create {name} financials table for reason: {e.Message}");
            }
            return new DataTable();
        }

        private DataTable CreateFinancialsTable(string name, string timescale)
        {
            string internalName = name == "income" ? "financials" : name;
            var keys = Consts.FundamentalsKeys[internalName];

            try
            {
                return GetFinancialsTimeSeries(timescale, keys);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private DataTable GetFinancialsTimeSeries(string timescale, List<string> keys)
        {
            var timescaleTranslation = new Dictionary<string, string>
            {
                { "yearly", "annual" },
                { "quarterly", "quarterly" },
                { "trailing", "trailing" }
            };
            string ts = timescaleTranslation[timescale];

            // Construct URL
            string tsUrlBase = $"https://query2.finance.yahoo.com/ws/fundamentals-timeseries/v1/finance/timeseries/{_symbol}?symbol={_symbol}";
            string url = tsUrlBase + "&type=" + string.Join(",", keys.Select(k => ts + k));
            DateTime startDt = new DateTime(2016, 12, 31, 0, 0, 0, DateTimeKind.Utc);
            DateTime end = DateTime.UtcNow.Date;
            url += $"&period1={new DateTimeOffset(startDt).ToUnixTimeSeconds()}&period2={new DateTimeOffset(end).ToUnixTimeSeconds()}";

            // Fetch and reshape data
            string jsonStr = _data.CacheGet(url: url).Result;
            var jsonDoc = JsonDocument.Parse(jsonStr);
            var root = jsonDoc.RootElement;
            var dataRaw = root.GetProperty("timeseries").GetProperty("result").EnumerateArray().ToList();

            // Remove "meta" from each dictionary
            foreach (var d in dataRaw)
            {
                if (d.TryGetProperty("meta", out _))
                {
                    // meta is not needed, skip or remove
                }
            }

            // Reshape data into a table
            var timestamps = new HashSet<long>();
            var dataUnpacked = new Dictionary<string, JsonElement>();

            foreach (var x in dataRaw)
            {
                foreach (var prop in x.EnumerateObject())
                {
                    if (prop.Name == "timestamp")
                    {
                        foreach (var tsVal in prop.Value.EnumerateArray())
                            timestamps.Add(tsVal.GetInt64());
                    }
                    else
                    {
                        dataUnpacked[prop.Name] = prop.Value;
                    }
                }
            }

            var sortedTimestamps = timestamps.OrderByDescending(t => t).ToList();
            var dates = sortedTimestamps.Select(ts => DateTimeOffset.FromUnixTimeSeconds(ts).UtcDateTime).ToList();

            // Create DataTable
            DataTable df = new DataTable();
            df.Columns.Add("Metric");
            foreach (var date in dates)
                df.Columns.Add(date.ToString("yyyy-MM-dd"));

            foreach (var kvp in dataUnpacked)
            {
                var row = df.NewRow();
                row["Metric"] = kvp.Key.Replace("^" + ts, "");
                var valueArray = kvp.Value.EnumerateArray().ToList();
                foreach (var v in valueArray)
                {
                    if (v.TryGetProperty("asOfDate", out var asOfDateElem) &&
                        v.TryGetProperty("reportedValue", out var reportedValueElem) &&
                        reportedValueElem.TryGetProperty("raw", out var rawElem))
                    {
                        var asOfDate = DateTime.Parse(asOfDateElem.GetString());
                        var colName = asOfDate.ToString("yyyy-MM-dd");
                        if (df.Columns.Contains(colName))
                            row[colName] = rawElem.GetDouble();
                    }
                }
                df.Rows.Add(row);
            }

            // Reorder table to match order on Yahoo website
            var orderedRows = new List<DataRow>();
            foreach (var k in keys)
            {
                foreach (DataRow r in df.Rows)
                {
                    if (r["Metric"].ToString() == k)
                        orderedRows.Add(r);
                }
            }
            var newDf = df.Clone();
            foreach (var r in orderedRows)
                newDf.ImportRow(r);

            // Trailing 12 months return only the first column.
            if (ts == "trailing" && newDf.Columns.Count > 1)
            {
                for (int i = newDf.Columns.Count - 1; i > 1; i--)
                    newDf.Columns.RemoveAt(i);
            }

            return newDf;
        }
    }
}