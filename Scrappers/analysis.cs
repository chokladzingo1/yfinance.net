using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using yfinance;

namespace yfinance.scrapers
{
    public class Analysis
    {
        private YfData _data;
        private string _symbol;

        private JsonElement? _earningsTrend;
        private Dictionary<string, object> _analystPriceTargets;
        private DataTable _earningsEstimate;
        private DataTable _revenueEstimate;
        private DataTable _earningsHistory;
        private DataTable _epsTrend;
        private DataTable _epsRevisions;
        private DataTable _growthEstimates;

        public Analysis(YfData data, string symbol, object proxy = null)
        {
            if (proxy != null && !proxy.Equals(Consts.SENTINEL))
            {
                Debug.WriteLine("Set proxy via new config function: yf.set_config(proxy=proxy)");
                data.SetProxy(proxy);
            }
            _data = data;
            _symbol = symbol;
            _earningsTrend = null;
            _analystPriceTargets = null;
            _earningsEstimate = null;
            _revenueEstimate = null;
            _earningsHistory = null;
            _epsTrend = null;
            _epsRevisions = null;
            _growthEstimates = null;
        }

        private DataTable GetPeriodicDataFrame(string key)
        {
            if (_earningsTrend == null)
                FetchEarningsTrend();

            var data = new List<Dictionary<string, object>>();
            var trendArr = _earningsTrend.Value.EnumerateArray().Take(4);
            foreach (var item in trendArr)
            {
                var row = new Dictionary<string, object>
                {
                    ["period"] = item.GetProperty("period").GetString()
                };
                if (item.TryGetProperty(key, out var keyObj) && keyObj.ValueKind == JsonValueKind.Object)
                {
                    foreach (var kv in keyObj.EnumerateObject())
                    {
                        if (kv.Value.ValueKind == JsonValueKind.Object && kv.Value.TryGetProperty("raw", out var rawElem))
                        {
                            row[kv.Name] = rawElem.GetDouble();
                        }
                    }
                }
                data.Add(row);
            }
            if (data.Count == 0)
                return new DataTable();

            var dt = new DataTable();
            dt.Columns.Add("period", typeof(string));
            foreach (var row in data)
            {
                foreach (var k in row.Keys)
                {
                    if (!dt.Columns.Contains(k))
                        dt.Columns.Add(k, typeof(double));
                }
            }
            foreach (var row in data)
            {
                var dr = dt.NewRow();
                foreach (var k in row.Keys)
                    dr[k] = row[k];
                dt.Rows.Add(dr);
            }
            dt.PrimaryKey = new[] { dt.Columns["period"] };
            return dt;
        }

        public DataTable EarningsEstimate
        {
            get
            {
                if (_earningsEstimate != null)
                    return _earningsEstimate;
                _earningsEstimate = GetPeriodicDataFrame("earningsEstimate");
                return _earningsEstimate;
            }
        }

        public DataTable RevenueEstimate
        {
            get
            {
                if (_revenueEstimate != null)
                    return _revenueEstimate;
                _revenueEstimate = GetPeriodicDataFrame("revenueEstimate");
                return _revenueEstimate;
            }
        }

        public DataTable EpsTrend
        {
            get
            {
                if (_epsTrend != null)
                    return _epsTrend;
                _epsTrend = GetPeriodicDataFrame("epsTrend");
                return _epsTrend;
            }
        }

        public DataTable EpsRevisions
        {
            get
            {
                if (_epsRevisions != null)
                    return _epsRevisions;
                _epsRevisions = GetPeriodicDataFrame("epsRevisions");
                return _epsRevisions;
            }
        }

        public Dictionary<string, object> AnalystPriceTargets
        {
            get
            {
                if (_analystPriceTargets != null)
                    return _analystPriceTargets;

                try
                {
                    var data = Fetch(new List<string> { "financialData" });
                    var fd = data.GetProperty("quoteSummary").GetProperty("result")[0].GetProperty("financialData");
                    var result = new Dictionary<string, object>();
                    foreach (var prop in fd.EnumerateObject())
                    {
                        if (prop.Name.StartsWith("target"))
                        {
                            var newKey = prop.Name.Replace("target", "").ToLower().Replace("price", "").Trim();
                            result[newKey] = prop.Value;
                        }
                        else if (prop.Name == "currentPrice")
                        {
                            result["current"] = prop.Value;
                        }
                    }
                    _analystPriceTargets = result;
                }
                catch
                {
                    _analystPriceTargets = new Dictionary<string, object>();
                }
                return _analystPriceTargets;
            }
        }

        public DataTable EarningsHistory
        {
            get
            {
                if (_earningsHistory != null)
                    return _earningsHistory;

                try
                {
                    var data = Fetch(new List<string> { "earningsHistory" });
                    var history = data.GetProperty("quoteSummary").GetProperty("result")[0]
                        .GetProperty("earningsHistory").GetProperty("history");
                    var rows = new List<Dictionary<string, object>>();
                    foreach (var item in history.EnumerateArray())
                    {
                        var row = new Dictionary<string, object>
                        {
                            ["quarter"] = item.TryGetProperty("quarter", out var qtr) && qtr.TryGetProperty("fmt", out var fmt) ? fmt.GetString() : null
                        };
                        foreach (var kv in item.EnumerateObject())
                        {
                            if (kv.Name == "quarter") continue;
                            if (kv.Value.ValueKind == JsonValueKind.Object && kv.Value.TryGetProperty("raw", out var rawElem))
                                row[kv.Name] = rawElem.GetDouble();
                        }
                        rows.Add(row);
                    }
                    if (rows.Count == 0)
                        return new DataTable();

                    var dt = new DataTable();
                    dt.Columns.Add("quarter", typeof(string));
                    foreach (var row in rows)
                    {
                        foreach (var k in row.Keys)
                        {
                            if (!dt.Columns.Contains(k))
                                dt.Columns.Add(k, typeof(double));
                        }
                    }
                    foreach (var row in rows)
                    {
                        var dr = dt.NewRow();
                        foreach (var k in row.Keys)
                            dr[k] = row[k];
                        dt.Rows.Add(dr);
                    }
                    dt.PrimaryKey = new[] { dt.Columns["quarter"] };
                    _earningsHistory = dt;
                }
                catch
                {
                    _earningsHistory = new DataTable();
                }
                return _earningsHistory;
            }
        }

        public DataTable GrowthEstimates
        {
            get
            {
                if (_growthEstimates != null)
                    return _growthEstimates;

                if (_earningsTrend == null)
                    FetchEarningsTrend();

                try
                {
                    var trends = Fetch(new List<string> { "industryTrend", "sectorTrend", "indexTrend" });
                    var trendsObj = trends.GetProperty("quoteSummary").GetProperty("result")[0];

                    var data = new List<Dictionary<string, object>>();
                    foreach (var item in _earningsTrend.Value.EnumerateArray())
                    {
                        var row = new Dictionary<string, object>
                        {
                            ["period"] = item.GetProperty("period").GetString(),
                            ["stockTrend"] = item.TryGetProperty("growth", out var growth) && growth.TryGetProperty("raw", out var rawElem) ? (object)rawElem.GetDouble() : null
                        };
                        data.Add(row);
                    }

                    foreach (var trendName in new[] { "industryTrend", "sectorTrend", "indexTrend" })
                    {
                        if (trendsObj.TryGetProperty(trendName, out var trendInfo) && trendInfo.TryGetProperty("estimates", out var estimates) && estimates.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var estimate in estimates.EnumerateArray())
                            {
                                var period = estimate.GetProperty("period").GetString();
                                var existingRow = data.FirstOrDefault(r => r["period"].ToString() == period);
                                if (existingRow != null)
                                    existingRow[trendName] = estimate.TryGetProperty("growth", out var g) ? (object)g.GetDouble() : null;
                                else
                                    data.Add(new Dictionary<string, object> { ["period"] = period, [trendName] = estimate.TryGetProperty("growth", out var g) ? (object)g.GetDouble() : null });
                            }
                        }
                    }

                    if (data.Count == 0)
                        return new DataTable();

                    var dt = new DataTable();
                    dt.Columns.Add("period", typeof(string));
                    foreach (var row in data)
                    {
                        foreach (var k in row.Keys)
                        {
                            if (!dt.Columns.Contains(k))
                                dt.Columns.Add(k, typeof(double));
                        }
                    }
                    foreach (var row in data)
                    {
                        var dr = dt.NewRow();
                        foreach (var k in row.Keys)
                            dr[k] = row[k];
                        dt.Rows.Add(dr);
                    }
                    dt.PrimaryKey = new[] { dt.Columns["period"] };
                    _growthEstimates = dt;
                }
                catch
                {
                    _growthEstimates = new DataTable();
                }
                return _growthEstimates;
            }
        }

        private JsonElement Fetch(List<string> modules)
        {
            if (modules == null || modules.Count == 0)
                throw new YFException("Should provide a list of modules, see available modules using `valid_modules`");

            var validModules = Consts.QuoteSummaryValidModules;
            var filteredModules = modules.Where(m => validModules.Contains(m)).ToList();
            if (filteredModules.Count == 0)
                throw new YFException("No valid modules provided, see available modules using `valid_modules`");

            var modulesStr = string.Join(",", filteredModules);
            var paramsDict = new Dictionary<string, string>
            {
                { "modules", modulesStr },
                { "corsDomain", "finance.yahoo.com" },
                { "formatted", "false" },
                { "symbol", _symbol }
            };

            try
            {
                var result = _data.GetRawJson($"{Consts.BASE_URL}/v10/finance/quoteSummary/{_symbol}", paramsDict);
                return result;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return default;
            }
        }

        private void FetchEarningsTrend()
        {
            try
            {
                var data = Fetch(new List<string> { "earningsTrend" });
                _earningsTrend = data.GetProperty("quoteSummary").GetProperty("result")[0]
                    .GetProperty("earningsTrend").GetProperty("trend");
            }
            catch
            {
                _earningsTrend = default;
            }
        }
    }
}