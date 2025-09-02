using System.Net;
using yfinance.scrapers;

namespace yfinance.test
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var data = YfData.Instance;
        }

        [TestMethod]
        public void TestMethod2() {

            var data = YfData.Instance;

            var quote = new Quote(data, "PFE");
            var fastInfo = quote.Info;
            Assert.AreEqual(fastInfo["currency"], "USD");
        }

        [TestMethod]
        public void TestMethod3()
        {
            string url1 = "https://finance.yahoo.com/quote/PFE/";
            string url2 = "https://query2.finance.yahoo.com/v10/finance/quoteSummary/PFE";
            MakeRequest(url2);
        }

        public bool MakeRequest(string url)
        {
            TimeSpan _timeout = TimeSpan.FromMinutes(2);
            using (var client = new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip }) { Timeout = _timeout })
            {
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Get
                };

                HttpResponseMessage response = client.SendAsync(request).Result;
                var statusCode = (int)response.StatusCode;

                // We want to handle redirects ourselves so that we can determine the final redirect Location (via header)
                if (statusCode >= 300 && statusCode <= 399)
                {
                    var redirectUri = response.Headers.Location;
                    if (!redirectUri.IsAbsoluteUri)
                    {
                        redirectUri = new Uri(request.RequestUri.GetLeftPart(UriPartial.Authority) + redirectUri);
                    }
                    //_status.AddStatus(string.Format("Redirecting to {0}", redirectUri));
                    return MakeRequest(redirectUri.OriginalString);
                }
                else if (!response.IsSuccessStatusCode)
                {
                    throw new Exception();
                }

                return true;
            }
        }


    }
}
