using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using yfinance.exceptions;
using yfinance.utils;
using yfinance.consts;

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
            _session = new HttpClient(handler);
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