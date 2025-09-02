using System.Data;
using System.Text.Json;
// using YFinance.Cache; // Uncomment if Cache is used


namespace yfinance.scrapers
{

    // Define other classes and methods as needed
    public class FastInfo
    {
        private TickerBase _tkr;
        private object _prices1y;
        private object _prices1wk1hPrepost;
        private object _prices1wk1hReg;
        private Dictionary<string, object> _md;

        private string _currency;
        private string _quoteType;
        private string _exchange;
        private string _timezone;

        private long? _shares;
        private double? _mcap;

        private double? _open;
        private double? _dayHigh;
        private double? _dayLow;
        private double? _lastPrice;
        private long? _lastVolume;

        private double? _prevClose;
        private double? _regPrevClose;

        private double? _fiftyDayAverage;
        private double? _twoHundredDayAverage;
        private double? _yearHigh;
        private double? _yearLow;
        private double? _yearChange;

        private long? _tenDayAvgVol;
        private long? _threeMonthAvgVol;

        public FastInfo(TickerBase tickerBaseObject)
        {
            _tkr = tickerBaseObject;
            // Initialize other fields as needed
        }

        public string Currency
        {
            get
            {
                if (_currency != null)
                    return _currency;
                //var md = _tkr.GetHistoryMetadata();
                //_currency = md.ContainsKey("currency") ? md["currency"].ToString() : null;
                return _currency;
            }
        }

        // Implement other properties as needed, following the same pattern
    }

    public class Quote
    {
        private YfData _data;
        private string _symbol;
        private Dictionary<string, object> _info;
        public Dictionary<string, object> _retiredInfo;
        private DataTable _sustainability;
        private DataTable _recommendations;
        private DataTable _upgradesDowngrades;
        private Dictionary<string, object> _calendar;
        private object _secFilings;

        private bool _alreadyScraped = false;
        private bool _alreadyFetched = false;
        private bool _alreadyFetchedComplementary = false;

        public Quote(YfData data, string symbol)
        {
            _data = data;
            _symbol = symbol;
        }

        public Dictionary<string, object> Info
        {
            get
            {
                if (_info == null)
                {
                    FetchInfo();
                    FetchComplementary();
                }
                return _info;
            }
        }

        public DataTable Sustainability
        {
            get
            {
                if (_sustainability == null)
                {
                    _sustainability = new DataTable();
                    JsonElement result = Fetch(new List<string> { "esgScores" }) ?? new JsonElement();
                    try
                    {
                        var data = result.GetProperty("quoteSummary").GetProperty("result")[0];
                        // Convert data to DataTable as needed
                    }
                    catch
                    {
                        throw new YFDataException("Failed to parse json response from Yahoo Finance");
                    }
                }
                return _sustainability;
            }
        }

        public DataTable Recommendations
        {
            get
            {
                if (_recommendations == null)
                {
                    _recommendations = new DataTable();
                    JsonElement result = Fetch(new List<string> { "recommendationTrend" }) ?? new JsonElement();
                    try
                    {
                        var data = result.GetProperty("quoteSummary").GetProperty("result")[0]
                            .GetProperty("recommendationTrend").GetProperty("trend");
                        // Convert data to DataTable as needed
                    }
                    catch
                    {
                        throw new YFDataException("Failed to parse json response from Yahoo Finance");
                    }
                }
                return _recommendations;
            }
        }

        // Add other properties (UpgradesDowngrades, Calendar, SecFilings) as needed

        private JsonElement? Fetch(List<string> modules)
        {
            if (modules == null || modules.Count == 0)
                throw new YFException("No valid modules provided");

            var modulesStr = string.Join(",", modules);
            var paramsDict = new Dictionary<string, string>
        {
            { "modules", modulesStr },
            { "corsDomain", "finance.yahoo.com" },
            { "formatted", "false" },
            { "symbol", _symbol }
        };

            try
            {
                string json = _data.GetRawJson($"{Consts.BASE_URL}/v10/finance/quoteSummary/{_symbol}", paramsDict).ToString();
                var doc = JsonDocument.Parse(json);
                return doc.RootElement;
            }
            catch (HttpRequestException)
            {
                // Log error
                return null;
            }
        }

        private void FetchInfo()
        {
            if (_alreadyFetched)
                return;
            _alreadyFetched = true;
            var modules = new List<string> { "financialData", "quoteType", "defaultKeyStatistics", "assetProfile", "summaryDetail" };
            var result = Fetch(modules);
            var additionalInfo = FetchAdditionalInfo();
            // Merge and flatten as needed
            var infoDict = new Dictionary<string, object>();
            if (result.HasValue)
            {
                foreach (var prop in result.Value.EnumerateObject())
                {
                    infoDict[prop.Name] = prop.Value;
                }
            }
            if (additionalInfo.HasValue)
            {
                foreach (var prop in additionalInfo.Value.EnumerateObject())
                {
                    infoDict[prop.Name] = prop.Value;
                }
            }
            _info = infoDict;
        }

        private JsonElement? FetchAdditionalInfo()
        {
            var paramsDict = new Dictionary<string, string>
        {
            { "symbols", _symbol },
            { "formatted", "false" }
        };
            try
            {
                string json = _data.GetRawJson($"{Consts.QUERY1_URL}/v7/finance/quote?", paramsDict).ToString();
                var doc = JsonDocument.Parse(json);
                return doc.RootElement;
            }
            catch (HttpRequestException)
            {
                // Log error
                return null;
            }
        }

        private void FetchComplementary()
        {
            if (_alreadyFetchedComplementary)
                return;
            _alreadyFetchedComplementary = true;
            // Implement as needed
        }
    }
}