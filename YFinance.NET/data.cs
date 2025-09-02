using System;
using System.Net;
using System.Text.Json;
using System.Threading;

namespace yfinance
{
    // Singleton implementation for YfData
    public sealed class YfData
    {
        private static readonly Lazy<YfData> _instance = new Lazy<YfData>(() => new YfData());
        private static readonly object _lock = new object();

        private HttpClient _session;
        private IWebProxy _proxy;
        private string _crumb;
        private CookieContainer _cookieContainer;
        private string _cookieStrategy = "basic";
        private readonly object _cookieLock = new object();

        public YfData()
        {
            _cookieContainer = new CookieContainer(2);
            var handler = new HttpClientHandler
            {
                CookieContainer = _cookieContainer,
                UseCookies = true,
                Proxy = _proxy,
                UseProxy = _proxy != null
            };
            _session = new HttpClient(handler);
            _session.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");
        }

        public static YfData Instance => _instance.Value;

        public void SetProxy(IWebProxy proxy)
        {
            lock (_cookieLock)
            {
                _proxy = proxy;
                var handler = new HttpClientHandler
                {
                    CookieContainer = _cookieContainer,
                    UseCookies = true,
                    Proxy = _proxy,
                    UseProxy = _proxy != null
                };
                _session = new HttpClient(handler);
            }
        }

        public void SetSession(HttpClient session)
        {
            lock (_cookieLock)
            {
                _session = session;
            }
        }

        public HttpResponseMessage Get(string url, Dictionary<string, string> parameters = null, int timeout = 30)
        {
            return MakeRequest(url, HttpMethod.Get, null, parameters, timeout);
        }

        public HttpResponseMessage Post(string url, object body, Dictionary<string, string> parameters = null, int timeout = 30)
        {
            return MakeRequest(url, HttpMethod.Post, body, parameters, timeout);
        }

        private HttpResponseMessage MakeRequest(string url, HttpMethod method, object body = null, Dictionary<string, string> parameters = null, int timeout = 30)
        {
            // Logging
            //if (url.Length > 200)
            //    Utils.GetYfLogger().Debug($"url={url.Substring(0, 200)}...");
            //else
            //    Utils.GetYfLogger().Debug($"url={url}");
            //Utils.GetYfLogger().Debug($"params={parameters}");

            if (parameters == null)
                parameters = new Dictionary<string, string>();

            if (parameters.ContainsKey("crumb"))
                throw new Exception("Don't manually add 'crumb' to params dict, let data.py handle it");

            // Get crumb and strategy
            string crumb = GetCookieAndCrumb(out string strategy, timeout);
            if (!string.IsNullOrEmpty(crumb))
                parameters["crumb"] = crumb;

            // Build request
            var requestUri = BuildUriWithParams(url, parameters);
            var request = new HttpRequestMessage(method, requestUri);

            if (body != null && method == HttpMethod.Post)
            {
                string jsonBody = JsonSerializer.Serialize(body);
                request.Content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");
            }

            // Set timeout
            _session.Timeout = TimeSpan.FromSeconds(timeout);

            // Send request
            var response = _session.Send(request);

            //Utils.GetYfLogger().Debug($"response code={response.StatusCode}");

            if ((int)response.StatusCode >= 400)
            {
                // Retry with other cookie strategy
                if (strategy == "basic")
                    SetCookieStrategy("csrf");
                else
                    SetCookieStrategy("basic");

                crumb = GetCookieAndCrumb(out strategy, timeout);
                parameters["crumb"] = crumb;
                requestUri = BuildUriWithParams(url, parameters);
                request = new HttpRequestMessage(method, requestUri);

                if (body != null && method == HttpMethod.Post)
                {
                    string jsonBody = JsonSerializer.Serialize(body);
                    request.Content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");
                }

                response = _session.Send(request);
                //Utils.GetYfLogger().Debug($"response code={response.StatusCode}");

                if ((int)response.StatusCode == 429)
                    throw new YFRateLimitError();
            }

            return response;
        }

        // Helper to build URI with query parameters
        private Uri BuildUriWithParams(string url, Dictionary<string, string> parameters)
        {
            if (parameters == null || parameters.Count == 0)
                return new Uri(url);

            var uriBuilder = new UriBuilder(url);
            var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
            foreach (var kv in parameters)
                query[kv.Key] = kv.Value;
            uriBuilder.Query = query.ToString();
            return uriBuilder.Uri;
        }

        // Dummy implementations for crumb/cookie logic
        private string GetCookieAndCrumb(out string strategy, int timeout = 30)
        {
            // This should implement the crumb/cookie logic as in Python
            // For now, just return nulls for demonstration
            strategy = _cookieStrategy;
            return _crumb;
        }

        private void SetCookieStrategy(string strategy)
        {
            _cookieStrategy = strategy;
        }
        private async Task<string> GetCrumbAsync()
        {
            if (!string.IsNullOrEmpty(_crumb))
                return _crumb;

            var response = await _session.GetAsync("https://query1.finance.yahoo.com/v1/test/getcrumb");
            if (response.IsSuccessStatusCode)
            {
                _crumb = await response.Content.ReadAsStringAsync();
                return _crumb;
            }
            return null;
        }

        private async Task EnsureCookieAndCrumbAsync()
        {
            // For demonstration, just fetch crumb (cookie handled by HttpClientHandler)
            await GetCrumbAsync();
        }

        public async Task<HttpResponseMessage> GetAsync(string url, Dictionary<string, string> parameters = null, int timeoutSeconds = 30)
        {
            await EnsureCookieAndCrumbAsync();

            var uriBuilder = new UriBuilder(url);
            var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);

            if (parameters != null)
            {
                foreach (var kv in parameters)
                    query[kv.Key] = kv.Value;
            }
            if (!string.IsNullOrEmpty(_crumb))
                query["crumb"] = _crumb;

            uriBuilder.Query = query.ToString();
            var requestUri = uriBuilder.Uri;

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
            var response = await _session.GetAsync(requestUri, cts.Token);

            if ((int)response.StatusCode >= 400)
            {
                // Retry logic for demonstration
                SwitchCookieStrategy();
                await EnsureCookieAndCrumbAsync();
                uriBuilder.Query = query.ToString();
                response = await _session.GetAsync(uriBuilder.Uri, cts.Token);

                if (response.StatusCode == (HttpStatusCode)429)
                    throw new YFRateLimitError();
            }

            return response;
        }

        public async Task<string> GetRawJsonAsync(string url, Dictionary<string, string> parameters = null, int timeoutSeconds = 30)
        {
            var response = await GetAsync(url, parameters, timeoutSeconds);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public JsonElement GetRawJson(string url, Dictionary<string, string> parameters = null, int timeoutSeconds = 30)
        {
            var task = GetRawJsonAsync(url, parameters, timeoutSeconds);
            task.Wait();
            var json = task.Result;
            return JsonDocument.Parse(json).RootElement;
        }

        private void SwitchCookieStrategy()
        {
            lock (_cookieLock)
            {
                _cookieStrategy = _cookieStrategy == "basic" ? "csrf" : "basic";
                _crumb = null;
                _cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler
                {
                    CookieContainer = _cookieContainer,
                    UseCookies = true,
                    Proxy = _proxy,
                    UseProxy = _proxy != null
                };
                _session = new HttpClient(handler);
            }
        }
    }
}