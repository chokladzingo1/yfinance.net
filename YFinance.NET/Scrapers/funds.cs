using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.Json;
using yfinance;

namespace yfinance.scrapers
{
    /// <summary>
    /// ETF and Mutual Funds Data
    /// Queried Modules: quoteType, summaryProfile, fundProfile, topHoldings
    /// </summary>
    public class FundsData
    {
        private YfData _data;
        private string _symbol;

        private string _quoteType;
        private string _description;
        private Dictionary<string, object> _fundOverview;
        private DataTable _fundOperations;

        private Dictionary<string, double?> _assetClasses;
        private DataTable _topHoldings;
        private DataTable _equityHoldings;
        private DataTable _bondHoldings;
        private Dictionary<string, double?> _bondRatings;
        private Dictionary<string, double?> _sectorWeightings;

        public FundsData(YfData data, string symbol, IWebProxy proxy = null)
        {
            _data = data;
            _symbol = symbol;
            if (proxy != null && !proxy.Equals(Consts.SENTINEL))
            {
                Debug.WriteLine("Set proxy via new config function: yf.set_config(proxy=proxy)");
                _data.SetProxy(proxy);
            }
        }

        public string QuoteType
        {
            get
            {
                if (_quoteType == null)
                    FetchAndParse();
                return _quoteType;
            }
        }

        public string Description
        {
            get
            {
                if (_description == null)
                    FetchAndParse();
                return _description;
            }
        }

        public Dictionary<string, object> FundOverview
        {
            get
            {
                if (_fundOverview == null)
                    FetchAndParse();
                return _fundOverview;
            }
        }

        public DataTable FundOperations
        {
            get
            {
                if (_fundOperations == null)
                    FetchAndParse();
                return _fundOperations;
            }
        }

        public Dictionary<string, double?> AssetClasses
        {
            get
            {
                if (_assetClasses == null)
                    FetchAndParse();
                return _assetClasses;
            }
        }

        public DataTable TopHoldings
        {
            get
            {
                if (_topHoldings == null)
                    FetchAndParse();
                return _topHoldings;
            }
        }

        public DataTable EquityHoldings
        {
            get
            {
                if (_equityHoldings == null)
                    FetchAndParse();
                return _equityHoldings;
            }
        }

        public DataTable BondHoldings
        {
            get
            {
                if (_bondHoldings == null)
                    FetchAndParse();
                return _bondHoldings;
            }
        }

        public Dictionary<string, double?> BondRatings
        {
            get
            {
                if (_bondRatings == null)
                    FetchAndParse();
                return _bondRatings;
            }
        }

        public Dictionary<string, double?> SectorWeightings
        {
            get
            {
                if (_sectorWeightings == null)
                    FetchAndParse();
                return _sectorWeightings;
            }
        }

        private JsonElement Fetch()
        {
            var modules = string.Join(",", new[] { "quoteType", "summaryProfile", "topHoldings", "fundProfile" });
            var paramsDict = new Dictionary<string, string>
            {
                { "modules", modules },
                { "corsDomain", "finance.yahoo.com" },
                { "symbol", _symbol },
                { "formatted", "false" }
            };
            var result = _data.GetRawJson($"{Consts.BASE_URL}/v10/finance/quoteSummary/{_symbol}", paramsDict);
            return result;
        }

        private void FetchAndParse()
        {
            var result = Fetch();
            try
            {
                var data = result.GetProperty("quoteSummary").GetProperty("result")[0];

                // quoteType
                _quoteType = data.GetProperty("quoteType").GetProperty("quoteType").GetString();

                // summaryProfile
                ParseDescription(data.GetProperty("summaryProfile"));

                // topHoldings
                ParseTopHoldings(data.GetProperty("topHoldings"));

                // fundProfile
                ParseFundProfile(data.GetProperty("fundProfile"));
            }
            catch (KeyNotFoundException)
            {
                throw new YFDataException("No Fund data found.");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Failed to get fund data for '{_symbol}' reason: {e}");
            }
        }

        private static double? ParseRawValue(JsonElement? elem)
        {
            if (elem == null || elem.Value.ValueKind == JsonValueKind.Undefined || elem.Value.ValueKind == JsonValueKind.Null)
                return null;
            if (elem.Value.ValueKind == JsonValueKind.Object && elem.Value.TryGetProperty("raw", out var rawElem))
                return rawElem.GetDouble();
            if (elem.Value.ValueKind == JsonValueKind.Number)
                return elem.Value.GetDouble();
            return null;
        }

        private void ParseDescription(JsonElement data)
        {
            _description = data.TryGetProperty("longBusinessSummary", out var descElem) ? descElem.GetString() : "";
        }

        private void ParseTopHoldings(JsonElement data)
        {
            // Asset classes
            _assetClasses = new Dictionary<string, double?>
            {
                { "cashPosition", ParseRawValue(data.TryGetProperty("cashPosition", out var v1) ? v1 : (JsonElement?)null) },
                { "stockPosition", ParseRawValue(data.TryGetProperty("stockPosition", out var v2) ? v2 : (JsonElement?)null) },
                { "bondPosition", ParseRawValue(data.TryGetProperty("bondPosition", out var v3) ? v3 : (JsonElement?)null) },
                { "preferredPosition", ParseRawValue(data.TryGetProperty("preferredPosition", out var v4) ? v4 : (JsonElement?)null) },
                { "convertiblePosition", ParseRawValue(data.TryGetProperty("convertiblePosition", out var v5) ? v5 : (JsonElement?)null) },
                { "otherPosition", ParseRawValue(data.TryGetProperty("otherPosition", out var v6) ? v6 : (JsonElement?)null) }
            };

            // Top holdings
            if (data.TryGetProperty("holdings", out var holdingsElem) && holdingsElem.ValueKind == JsonValueKind.Array)
            {
                var dt = new DataTable();
                dt.Columns.Add("Symbol", typeof(string));
                dt.Columns.Add("Name", typeof(string));
                dt.Columns.Add("Holding Percent", typeof(double));

                foreach (var item in holdingsElem.EnumerateArray())
                {
                    var row = dt.NewRow();
                    row["Symbol"] = item.GetProperty("symbol").GetString();
                    row["Name"] = item.GetProperty("holdingName").GetString();
                    row["Holding Percent"] = ParseRawValue(item.TryGetProperty("holdingPercent", out var hp) ? hp : (JsonElement?)null) ?? 0.0;
                    dt.Rows.Add(row);
                }
                dt.PrimaryKey = new[] { dt.Columns["Symbol"] };
                _topHoldings = dt;
            }

            // Equity holdings
            if (data.TryGetProperty("equityHoldings", out var eqElem) && eqElem.ValueKind == JsonValueKind.Object)
            {
                var dt = new DataTable();
                dt.Columns.Add("Average", typeof(string));
                dt.Columns.Add(_symbol, typeof(double));
                dt.Columns.Add("Category Average", typeof(double));

                string[] metrics = { "Price/Earnings", "Price/Book", "Price/Sales", "Price/Cashflow", "Median Market Cap", "3 Year Earnings Growth" };
                string[] keys = { "priceToEarnings", "priceToBook", "priceToSales", "priceToCashflow", "medianMarketCap", "threeYearEarningsGrowth" };
                string[] catKeys = { "priceToEarningsCat", "priceToBookCat", "priceToSalesCat", "priceToCashflowCat", "medianMarketCapCat", "threeYearEarningsGrowthCat" };

                for (int i = 0; i < metrics.Length; i++)
                {
                    var row = dt.NewRow();
                    row["Average"] = metrics[i];
                    row[_symbol] = ParseRawValue(eqElem.TryGetProperty(keys[i], out var v) ? v : (JsonElement?)null);
                    row["Category Average"] = ParseRawValue(eqElem.TryGetProperty(catKeys[i], out var vcat) ? vcat : (JsonElement?)null);
                    dt.Rows.Add(row);
                }
                dt.PrimaryKey = new[] { dt.Columns["Average"] };
                _equityHoldings = dt;
            }

            // Bond holdings
            if (data.TryGetProperty("bondHoldings", out var bondElem) && bondElem.ValueKind == JsonValueKind.Object)
            {
                var dt = new DataTable();
                dt.Columns.Add("Average", typeof(string));
                dt.Columns.Add(_symbol, typeof(double));
                dt.Columns.Add("Category Average", typeof(double));

                string[] metrics = { "Duration", "Maturity", "Credit Quality" };
                string[] keys = { "duration", "maturity", "creditQuality" };
                string[] catKeys = { "durationCat", "maturityCat", "creditQualityCat" };

                for (int i = 0; i < metrics.Length; i++)
                {
                    var row = dt.NewRow();
                    row["Average"] = metrics[i];
                    row[_symbol] = ParseRawValue(bondElem.TryGetProperty(keys[i], out var v) ? v : (JsonElement?)null);
                    row["Category Average"] = ParseRawValue(bondElem.TryGetProperty(catKeys[i], out var vcat) ? vcat : (JsonElement?)null);
                    dt.Rows.Add(row);
                }
                dt.PrimaryKey = new[] { dt.Columns["Average"] };
                _bondHoldings = dt;
            }

            // Bond ratings
            _bondRatings = new Dictionary<string, double?>();
            if (data.TryGetProperty("bondRatings", out var bondRatingsElem) && bondRatingsElem.ValueKind == JsonValueKind.Array)
            {
                foreach (var d in bondRatingsElem.EnumerateArray())
                {
                    foreach (var prop in d.EnumerateObject())
                        _bondRatings[prop.Name] = ParseRawValue(prop.Value);
                }
            }

            // Sector weightings
            _sectorWeightings = new Dictionary<string, double?>();
            if (data.TryGetProperty("sectorWeightings", out var sectorElem) && sectorElem.ValueKind == JsonValueKind.Array)
            {
                foreach (var d in sectorElem.EnumerateArray())
                {
                    foreach (var prop in d.EnumerateObject())
                        _sectorWeightings[prop.Name] = ParseRawValue(prop.Value);
                }
            }
        }

        private void ParseFundProfile(JsonElement data)
        {
            _fundOverview = new Dictionary<string, object>
            {
                { "categoryName", data.TryGetProperty("categoryName", out var v1) ? v1.GetString() : null },
                { "family", data.TryGetProperty("family", out var v2) ? v2.GetString() : null },
                { "legalType", data.TryGetProperty("legalType", out var v3) ? v3.GetString() : null }
            };

            var fundOps = data.TryGetProperty("feesExpensesInvestment", out var opsElem) ? opsElem : (JsonElement?)null;
            var fundOpsCat = data.TryGetProperty("feesExpensesInvestmentCat", out var opsCatElem) ? opsCatElem : (JsonElement?)null;

            var dt = new DataTable();
            dt.Columns.Add("Attributes", typeof(string));
            dt.Columns.Add(_symbol, typeof(double));
            dt.Columns.Add("Category Average", typeof(double));

            string[] metrics = { "Annual Report Expense Ratio", "Annual Holdings Turnover", "Total Net Assets" };
            string[] keys = { "annualReportExpenseRatio", "annualHoldingsTurnover", "totalNetAssets" };

            for (int i = 0; i < metrics.Length; i++)
            {
                var row = dt.NewRow();
                row["Attributes"] = metrics[i];
                row[_symbol] = ParseRawValue(fundOps?.TryGetProperty(keys[i], out var v) == true ? v : (JsonElement?)null);
                row["Category Average"] = ParseRawValue(fundOpsCat?.TryGetProperty(keys[i], out var vcat) == true ? vcat : (JsonElement?)null);
                dt.Rows.Add(row);
            }
            dt.PrimaryKey = new[] { dt.Columns["Attributes"] };
            _fundOperations = dt;
        }
    }
}