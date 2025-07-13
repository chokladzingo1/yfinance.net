using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace yfinance
{
    /// <summary>
    /// Singleton class for managing Yahoo Finance data requests.
    /// </summary>
    /// <remarks>
    /// This class handles HTTP requests to Yahoo Finance, manages cookies, and supports proxy settings.
    /// It is designed to be thread-safe and can be used across multiple threads.
    /// </remarks>
    /// <example>
    /// Usage:
    /// var yfData = YfData.Instance;
    /// var response = await yfData.GetAsync("https://query1.finance.yahoo.com/v7/finance/quote?symbols=AAPL");
    /// </example>
    ///

    public class YfData
    {
        private static readonly Lazy<YfData> _instance = new Lazy<YfData>(() => new YfData());
        private static readonly object _lock = new object();

        private HttpClient _client;
        private IWebProxy _proxy;
        private string _crumb;
        private CookieContainer _cookieContainer;
        private string _cookieStrategy = "basic";
        private readonly object _cookieLock = new object();

        private YfData()
        {
            _cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = _cookieContainer,
                UseCookies = true,
                Proxy = _proxy,
                UseProxy = _proxy != null
            };
            _client = new HttpClient(handler);
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
                _client = new HttpClient(handler);
            }
        }

        public async Task<HttpResponseMessage> GetAsync(string url, int timeoutSeconds = 30)
        {
            return await MakeRequestAsync(url, HttpMethod.Get, null, timeoutSeconds);
        }

        public async Task<HttpResponseMessage> PostAsync(string url, HttpContent content, int timeoutSeconds = 30)
        {
            return await MakeRequestAsync(url, HttpMethod.Post, content, timeoutSeconds);
        }

        private async Task<HttpResponseMessage> MakeRequestAsync(string url, HttpMethod method, HttpContent content, int timeoutSeconds)
        {
            // Simplified crumb/cookie logic for demonstration
            string crumb = await GetCrumbAsync();

            var request = new HttpRequestMessage(method, url);
            if (content != null)
                request.Content = content;

            // Add crumb as query parameter if needed
            if (!string.IsNullOrEmpty(crumb))
            {
                var uriBuilder = new UriBuilder(url);
                var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
                query["crumb"] = crumb;
                uriBuilder.Query = query.ToString();
                request.RequestUri = uriBuilder.Uri;
            }

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
            var response = await _client.SendAsync(request, cts.Token);

            if ((int)response.StatusCode >= 400)
            {
                // Retry logic for demonstration
                SwitchCookieStrategy();
                crumb = await GetCrumbAsync();
                var retryRequest = new HttpRequestMessage(method, url);
                if (content != null)
                    retryRequest.Content = content;
                if (!string.IsNullOrEmpty(crumb))
                {
                    var uriBuilder = new UriBuilder(url);
                    var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
                    query["crumb"] = crumb;
                    uriBuilder.Query = query.ToString();
                    retryRequest.RequestUri = uriBuilder.Uri;
                }
                response = await _client.SendAsync(retryRequest, cts.Token);

                if (response.StatusCode == (HttpStatusCode)429)
                    throw new Exception("Rate limited by Yahoo Finance");
            }

            return response;
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
                _client = new HttpClient(handler);
            }
        }

        private async Task<string> GetCrumbAsync()
        {
            // This is a placeholder for the crumb/cookie logic.
            // In a real implementation, you would fetch the crumb from Yahoo as in the Python code.
            if (!string.IsNullOrEmpty(_crumb))
                return _crumb;

            // Example: fetch crumb from Yahoo endpoint
            var response = await _client.GetAsync("https://query1.finance.yahoo.com/v1/test/getcrumb");
            if (response.IsSuccessStatusCode)
            {
                _crumb = await response.Content.ReadAsStringAsync();
                return _crumb;
            }
            return null;
        }

        // Example method for getting raw JSON
        public async Task<string> GetRawJsonAsync(string url, int timeoutSeconds = 30)
        {
            var response = await GetAsync(url, timeoutSeconds);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}