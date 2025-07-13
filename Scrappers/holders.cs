using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using yfinance.data;
using yfinance.consts;
using yfinance.exceptions;

namespace yfinance.scrapers
{
    public class Holders
    {
        private YfData _data;
        private string _symbol;

        private DataTable _major;
        private DataTable _majorDirectHolders;
        private DataTable _institutional;
        private DataTable _mutualfund;

        private DataTable _insiderTransactions;
        private DataTable _insiderPurchases;
        private DataTable _insiderRoster;

        private static readonly string _SCRAPE_URL = "https://finance.yahoo.com/quote";

        public Holders(YfData data, string symbol, object proxy = null)
        {
            _data = data;
            _symbol = symbol;
            if (proxy != null && !proxy.Equals(Consts.SENTINEL))
            {
                Debug.WriteLine("Set proxy via new config function: yf.set_config(proxy=proxy)");
                _data.SetProxy(proxy);
            }
        }

        public DataTable Major
        {
            get
            {
                if (_major == null)
                    FetchAndParse();
                return _major;
            }
        }

        public DataTable Institutional
        {
            get
            {
                if (_institutional == null)
                    FetchAndParse();
                return _institutional;
            }
        }

        public DataTable MutualFund
        {
            get
            {
                if (_mutualfund == null)
                    FetchAndParse();
                return _mutualfund;
            }
        }

        public DataTable InsiderTransactions
        {
            get
            {
                if (_insiderTransactions == null)
                    FetchAndParse();
                return _insiderTransactions;
            }
        }

        public DataTable InsiderPurchases
        {
            get
            {
                if (_insiderPurchases == null)
                    FetchAndParse();
                return _insiderPurchases;
            }
        }

        public DataTable InsiderRoster
        {
            get
            {
                if (_insiderRoster == null)
                    FetchAndParse();
                return _insiderRoster;
            }
        }

        private JsonElement Fetch()
        {
            var modules = string.Join(",", new[]
            {
                "institutionOwnership", "fundOwnership", "majorDirectHolders", "majorHoldersBreakdown",
                "insiderTransactions", "insiderHolders", "netSharePurchaseActivity"
            });
            var paramsDict = new Dictionary<string, string>
            {
                { "modules", modules },
                { "corsDomain", "finance.yahoo.com" },
                { "formatted", "false" }
            };
            var result = _data.GetRawJson($"{Consts.BASE_URL}/v10/finance/quoteSummary/{_symbol}", paramsDict);
            return result;
        }

        private void FetchAndParse()
        {
            JsonElement result;
            try
            {
                result = Fetch();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                _major = new DataTable();
                _majorDirectHolders = new DataTable();
                _institutional = new DataTable();
                _mutualfund = new DataTable();
                _insiderTransactions = new DataTable();
                _insiderPurchases = new DataTable();
                _insiderRoster = new DataTable();
                return;
            }

            try
            {
                var data = result.GetProperty("quoteSummary").GetProperty("result")[0];

                ParseInstitutionOwnership(data.TryGetProperty("institutionOwnership", out var inst) ? inst : default);
                ParseFundOwnership(data.TryGetProperty("fundOwnership", out var fund) ? fund : default);
                //ParseMajorDirectHolders(data.TryGetProperty("majorDirectHolders", out var majdir) ? majdir : default); // Not implemented
                ParseMajorHoldersBreakdown(data.TryGetProperty("majorHoldersBreakdown", out var maj) ? maj : default);
                ParseInsiderTransactions(data.TryGetProperty("insiderTransactions", out var insTrans) ? insTrans : default);
                ParseInsiderHolders(data.TryGetProperty("insiderHolders", out var insHold) ? insHold : default);
                ParseNetSharePurchaseActivity(data.TryGetProperty("netSharePurchaseActivity", out var netShare) ? netShare : default);
            }
            catch (Exception)
            {
                throw new YFDataException("Failed to parse holders json data.");
            }
        }

        private static object ParseRawValues(JsonElement data)
        {
            if (data.ValueKind == JsonValueKind.Object && data.TryGetProperty("raw", out var rawElem))
                return rawElem.ValueKind == JsonValueKind.Number ? rawElem.GetDouble() : (object)rawElem.ToString();
            return data.ValueKind == JsonValueKind.Number ? data.GetDouble() :
                   data.ValueKind == JsonValueKind.String ? data.GetString() : (object)null;
        }

        private void ParseInstitutionOwnership(JsonElement data)
        {
            if (data.ValueKind != JsonValueKind.Object || !data.TryGetProperty("ownershipList", out var holdersElem) || holdersElem.ValueKind != JsonValueKind.Array)
            {
                _institutional = new DataTable();
                return;
            }
            var dt = new DataTable();
            dt.Columns.Add("Date Reported", typeof(DateTime));
            dt.Columns.Add("Holder", typeof(string));
            dt.Columns.Add("Shares", typeof(double));
            dt.Columns.Add("Value", typeof(double));
            foreach (var owner in holdersElem.EnumerateArray())
            {
                var row = dt.NewRow();
                row["Date Reported"] = owner.TryGetProperty("reportDate", out var dateElem) ? DateTimeOffset.FromUnixTimeSeconds(dateElem.GetInt64()).UtcDateTime : (object)DBNull.Value;
                row["Holder"] = owner.TryGetProperty("organization", out var orgElem) ? orgElem.GetString() : "";
                row["Shares"] = owner.TryGetProperty("position", out var posElem) ? ParseRawValues(posElem) : 0.0;
                row["Value"] = owner.TryGetProperty("value", out var valElem) ? ParseRawValues(valElem) : 0.0;
                dt.Rows.Add(row);
            }
            _institutional = dt;
        }

        private void ParseFundOwnership(JsonElement data)
        {
            if (data.ValueKind != JsonValueKind.Object || !data.TryGetProperty("ownershipList", out var holdersElem) || holdersElem.ValueKind != JsonValueKind.Array)
            {
                _mutualfund = new DataTable();
                return;
            }
            var dt = new DataTable();
            dt.Columns.Add("Date Reported", typeof(DateTime));
            dt.Columns.Add("Holder", typeof(string));
            dt.Columns.Add("Shares", typeof(double));
            dt.Columns.Add("Value", typeof(double));
            foreach (var owner in holdersElem.EnumerateArray())
            {
                var row = dt.NewRow();
                row["Date Reported"] = owner.TryGetProperty("reportDate", out var dateElem) ? DateTimeOffset.FromUnixTimeSeconds(dateElem.GetInt64()).UtcDateTime : (object)DBNull.Value;
                row["Holder"] = owner.TryGetProperty("organization", out var orgElem) ? orgElem.GetString() : "";
                row["Shares"] = owner.TryGetProperty("position", out var posElem) ? ParseRawValues(posElem) : 0.0;
                row["Value"] = owner.TryGetProperty("value", out var valElem) ? ParseRawValues(valElem) : 0.0;
                dt.Rows.Add(row);
            }
            _mutualfund = dt;
        }

        private void ParseMajorHoldersBreakdown(JsonElement data)
        {
            if (data.ValueKind != JsonValueKind.Object)
            {
                _major = new DataTable();
                return;
            }
            var dt = new DataTable();
            dt.Columns.Add("Breakdown", typeof(string));
            dt.Columns.Add("Value", typeof(double));
            foreach (var prop in data.EnumerateObject())
            {
                if (prop.Name == "maxAge") continue;
                var row = dt.NewRow();
                row["Breakdown"] = prop.Name;
                row["Value"] = ParseRawValues(prop.Value);
                dt.Rows.Add(row);
            }
            _major = dt;
        }

        private void ParseInsiderTransactions(JsonElement data)
        {
            if (data.ValueKind != JsonValueKind.Object || !data.TryGetProperty("transactions", out var holdersElem) || holdersElem.ValueKind != JsonValueKind.Array)
            {
                _insiderTransactions = new DataTable();
                return;
            }
            var dt = new DataTable();
            dt.Columns.Add("Start Date", typeof(DateTime));
            dt.Columns.Add("Insider", typeof(string));
            dt.Columns.Add("Position", typeof(string));
            dt.Columns.Add("URL", typeof(string));
            dt.Columns.Add("Transaction", typeof(string));
            dt.Columns.Add("Text", typeof(string));
            dt.Columns.Add("Shares", typeof(double));
            dt.Columns.Add("Value", typeof(double));
            dt.Columns.Add("Ownership", typeof(string));
            foreach (var owner in holdersElem.EnumerateArray())
            {
                var row = dt.NewRow();
                row["Start Date"] = owner.TryGetProperty("startDate", out var dateElem) ? DateTimeOffset.FromUnixTimeSeconds(dateElem.GetInt64()).UtcDateTime : (object)DBNull.Value;
                row["Insider"] = owner.TryGetProperty("filerName", out var nameElem) ? nameElem.GetString() : "";
                row["Position"] = owner.TryGetProperty("filerRelation", out var relElem) ? relElem.GetString() : "";
                row["URL"] = owner.TryGetProperty("filerUrl", out var urlElem) ? urlElem.GetString() : "";
                row["Transaction"] = owner.TryGetProperty("moneyText", out var moneyElem) ? moneyElem.GetString() : "";
                row["Text"] = owner.TryGetProperty("transactionText", out var textElem) ? textElem.GetString() : "";
                row["Shares"] = owner.TryGetProperty("shares", out var sharesElem) ? ParseRawValues(sharesElem) : 0.0;
                row["Value"] = owner.TryGetProperty("value", out var valElem) ? ParseRawValues(valElem) : 0.0;
                row["Ownership"] = owner.TryGetProperty("ownership", out var ownElem) ? ownElem.GetString() : "";
                dt.Rows.Add(row);
            }
            _insiderTransactions = dt;
        }

        private void ParseInsiderHolders(JsonElement data)
        {
            if (data.ValueKind != JsonValueKind.Object || !data.TryGetProperty("holders", out var holdersElem) || holdersElem.ValueKind != JsonValueKind.Array)
            {
                _insiderRoster = new DataTable();
                return;
            }
            var dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Position", typeof(string));
            dt.Columns.Add("URL", typeof(string));
            dt.Columns.Add("Most Recent Transaction", typeof(string));
            dt.Columns.Add("Latest Transaction Date", typeof(DateTime));
            dt.Columns.Add("Position Direct Date", typeof(DateTime));
            dt.Columns.Add("Shares Owned Directly", typeof(double));
            dt.Columns.Add("Position Indirect Date", typeof(DateTime));
            dt.Columns.Add("Shares Owned Indirectly", typeof(double));
            foreach (var owner in holdersElem.EnumerateArray())
            {
                var row = dt.NewRow();
                row["Name"] = owner.TryGetProperty("name", out var nameElem) ? nameElem.GetString() : "";
                row["Position"] = owner.TryGetProperty("relation", out var relElem) ? relElem.GetString() : "";
                row["URL"] = owner.TryGetProperty("url", out var urlElem) ? urlElem.GetString() : "";
                row["Most Recent Transaction"] = owner.TryGetProperty("transactionDescription", out var descElem) ? descElem.GetString() : "";
                row["Latest Transaction Date"] = owner.TryGetProperty("latestTransDate", out var latestElem) ? (object)DateTimeOffset.FromUnixTimeSeconds(latestElem.GetInt64()).UtcDateTime : DBNull.Value;
                row["Position Direct Date"] = owner.TryGetProperty("positionDirectDate", out var posDirElem) ? (object)DateTimeOffset.FromUnixTimeSeconds(posDirElem.GetInt64()).UtcDateTime : DBNull.Value;
                row["Shares Owned Directly"] = owner.TryGetProperty("positionDirect", out var posDir) ? ParseRawValues(posDir) : 0.0;
                row["Position Indirect Date"] = owner.TryGetProperty("positionIndirectDate", out var posIndElem) ? (object)DateTimeOffset.FromUnixTimeSeconds(posIndElem.GetInt64()).UtcDateTime : DBNull.Value;
                row["Shares Owned Indirectly"] = owner.TryGetProperty("positionIndirect", out var posInd) ? ParseRawValues(posInd) : 0.0;
                dt.Rows.Add(row);
            }
            _insiderRoster = dt;
        }

        private void ParseNetSharePurchaseActivity(JsonElement data)
        {
            if (data.ValueKind != JsonValueKind.Object)
            {
                _insiderPurchases = new DataTable();
                return;
            }
            var dt = new DataTable();
            dt.Columns.Add("Label", typeof(string));
            dt.Columns.Add("Shares", typeof(double));
            dt.Columns.Add("Trans", typeof(double));

            string period = data.TryGetProperty("period", out var periodElem) ? periodElem.GetString() : "";
            string[] labels = {
                $"Purchases (Last {period})",
                $"Sales (Last {period})",
                $"Net Shares Purchased (Sold) (Last {period})",
                "Total Insider Shares Held",
                "% Net Shares Purchased (Sold)",
                "% Buy Shares",
                "% Sell Shares"
            };
            string[] shareKeys = {
                "buyInfoShares", "sellInfoShares", "netInfoShares", "totalInsiderShares",
                "netPercentInsiderShares", "buyPercentInsiderShares", "sellPercentInsiderShares"
            };
            string[] transKeys = {
                "buyInfoCount", "sellInfoCount", "netInfoCount", null, null, null, null
            };

            for (int i = 0; i < labels.Length; i++)
            {
                var row = dt.NewRow();
                row["Label"] = labels[i];
                row["Shares"] = data.TryGetProperty(shareKeys[i], out var shareElem) ? ParseRawValues(shareElem) : 0.0;
                row["Trans"] = transKeys[i] != null && data.TryGetProperty(transKeys[i], out var transElem) ? ParseRawValues(transElem) : DBNull.Value;
                dt.Rows.Add(row);
            }
            _insiderPurchases = dt;
        }
    }
}