using System.Text.Json;

namespace yfinance.screener
{
    public static class Screener
    {
        public static readonly Dictionary<string, Dictionary<string, object>> PREDEFINED_SCREENER_QUERIES = new Dictionary<string, Dictionary<string, object>>
        {
            // Only a few shown for brevity; add all as needed
            ["aggressive_small_caps"] = new Dictionary<string, object>
            {
                {"sortField", "eodvolume"},
                {"sortType", "desc"},
                {"query", new EquityQuery("and", new List<object>
                    {
                        new EquityQuery("is-in", new List<object> { "exchange", "NMS", "NYQ" }),
                        new EquityQuery("lt", new List<object> { "epsgrowth.lasttwelvemonths", 15 })
                    })
                }
            },
            ["day_gainers"] = new Dictionary<string, object>
            {
                {"sortField", "percentchange"},
                {"sortType", "DESC"},
                {"query", new EquityQuery("and", new List<object>
                    {
                        new EquityQuery("gt", new List<object> { "percentchange", 3 }),
                        new EquityQuery("eq", new List<object> { "region", "us" }),
                        new EquityQuery("gte", new List<object> { "intradaymarketcap", 2000000000 }),
                        new EquityQuery("gte", new List<object> { "intradayprice", 5 }),
                        new EquityQuery("gt", new List<object> { "dayvolume", 15000 })
                    })
                }
            }
            // ... Add the rest as needed ...
        };

        public static object Screen(
            object query,
            int? offset = null,
            int? size = null,
            int? count = null,
            string sortField = null,
            bool? sortAsc = null,
            string userId = null,
            string userIdType = null,
            HttpClient session = null,
            object proxy = null)
        {
            // Proxy/session handling
            YfData _data;
            if (proxy != null)
            {
                _data = YfData.Instance;
                _data.SetProxy(proxy as System.Net.IWebProxy);
            }
            else
            {
                _data = YfData.Instance;
            }

            // Only use defaults when user NOT give a predefined, because Yahoo's predefined endpoint auto-applies defaults.
            var defaults = new Dictionary<string, object>
            {
                {"offset", 0},
                {"count", 25},
                {"sortField", "ticker"},
                {"sortAsc", false},
                {"userId", ""},
                {"userIdType", "guid"}
            };

            if (count.HasValue && count > 250)
                throw new ArgumentException("Yahoo limits query count to 250, reduce count.");

            if (size.HasValue && size > 250)
                throw new ArgumentException("Yahoo limits query size to 250, reduce size.");

            Dictionary<string, object> fields = new Dictionary<string, object>
            {
                {"offset", offset},
                {"count", count},
                {"size", size},
                {"sortField", sortField},
                {"sortAsc", sortAsc},
                {"userId", userId},
                {"userIdType", userIdType}
            };

            var paramsDict = new Dictionary<string, string>
            {
                {"corsDomain", "finance.yahoo.com"},
                {"formatted", "false"},
                {"lang", "en-US"},
                {"region", "US"}
            };

            Dictionary<string, object> postQuery = null;

            if (query is string queryStr)
            {
                // Predefined query
                if (size.HasValue)
                {
                    count = size;
                    size = null;
                    fields["count"] = fields["size"];
                    fields.Remove("size");
                }

                paramsDict["scrIds"] = queryStr;
                foreach (var kv in fields)
                {
                    if (kv.Value != null)
                        paramsDict[kv.Key] = kv.Value.ToString();
                }

                var resp = _data.Get(url: Consts.BASE_URL, parameters: paramsDict);
                if (!resp.IsSuccessStatusCode)
                {
                    if (!PREDEFINED_SCREENER_QUERIES.ContainsKey(queryStr))
                        Console.WriteLine($"yfinance.screen: '{queryStr}' is probably not a predefined query.");
                    throw new HttpRequestException($"HTTP error: {resp.StatusCode}");
                }
                var json = resp.Content.ReadAsStringAsync().Result;
                var doc = JsonDocument.Parse(json);
                return doc.RootElement.GetProperty("finance").GetProperty("result")[0];
            }
            else if (query is QueryBase qb)
            {
                // Prepare other fields
                foreach (var k in defaults.Keys)
                {
                    if (!fields.ContainsKey(k) || fields[k] == null)
                        fields[k] = defaults[k];
                }
                fields["sortType"] = (fields["sortAsc"] is bool b && b) ? "ASC" : "DESC";
                fields.Remove("sortAsc");

                postQuery = new Dictionary<string, object>(fields);
                postQuery["query"] = qb;
            }
            else
            {
                throw new ArgumentException($"Query must be type string or QueryBase, not \"{query?.GetType()}\"");
            }

            if (query == null)
                throw new ArgumentException("No query provided");

            if (postQuery["query"] is EquityQuery)
                postQuery["quoteType"] = "EQUITY";
            else if (postQuery["query"] is FundQuery)
                postQuery["quoteType"] = "MUTUALFUND";

            // Convert QueryBase to dictionary for serialization
            if (postQuery["query"] is QueryBase qbase)
                postQuery["query"] = qbase.ToDict();

            // Fetch
            var response = _data.Post(Consts.BASE_URL, body: postQuery, parameters: paramsDict);
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"HTTP error: {response.StatusCode}");

            var respJson = response.Content.ReadAsStringAsync().Result;
            var respDoc = JsonDocument.Parse(respJson);
            return respDoc.RootElement.GetProperty("finance").GetProperty("result")[0];
        }
    }
}