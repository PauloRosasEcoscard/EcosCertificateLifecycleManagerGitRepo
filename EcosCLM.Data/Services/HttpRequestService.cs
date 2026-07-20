using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace EcosCLM.Data.Services
{
    public static class HttpRequestService
    {
        public static async Task<HttpResponseMessage> PostAsync<T>(string postUrl, T body, Dictionary<string, string>? headers = null, ILogger? logger = null)
        {
            string requestBodyJson;
            StringContent requestContent;
            HttpResponseMessage? response = null;

            try
            {
                requestBodyJson = JsonConvert.SerializeObject(body);
                requestContent = new StringContent(requestBodyJson, Encoding.UTF8, "application/json");

                using HttpClient httpClient = new HttpClient();

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                if (logger != null && logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("--------------Request--------------");
                    logger.LogInformation("HttpRequestService: Request URL: {PostUrl}", postUrl);
                    logger.LogInformation("HttpRequestService: Request Headers: {Headers}", JsonConvert.SerializeObject(httpClient.DefaultRequestHeaders));
                    logger.LogInformation("HttpRequestService: Request Body: {RequestBody}", requestBodyJson);
                }

                response = await httpClient.PostAsync(postUrl, requestContent).ConfigureAwait(false);

                string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (logger != null && logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("--------------Response--------------");
                    logger.LogInformation("HttpRequestService: Response Status: {StatusCode}", response.StatusCode);
                    logger.LogInformation("HttpRequestService: Response Headers: {Headers}", JsonConvert.SerializeObject(response.Headers));
                    logger.LogInformation("HttpRequestService: Response Body: {ResponseBody}", responseBody);
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Exception trying to POST at {PostUrl}", postUrl);

                if (ex.InnerException != null)
                {
                    logger?.LogError("Inner Exception: {Message}", ex.InnerException.Message);
                    logger?.LogError("Inner Exception StackTrace: {StackTrace}", ex.InnerException.StackTrace);
                }

                throw;
            }

            return response;
        }

        public static async Task<HttpResponseMessage> PostAsync<T>(string postUrl, T body, ILogger? logger = null)
        {
            string requestBodyJson;
            StringContent requestContent;
            HttpResponseMessage response;

            requestBodyJson = JsonConvert.SerializeObject(body);
            requestContent = new StringContent(requestBodyJson, Encoding.UTF8, "application/json");

            using HttpClient httpClient = new HttpClient();

            if (logger != null && logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("--------------Request--------------");
                logger.LogInformation("HttpRequestService: Request URL: {PostUrl}", postUrl);
                logger.LogInformation("HttpRequestService: Request Headers: {Headers}", JsonConvert.SerializeObject(httpClient.DefaultRequestHeaders));
                logger.LogInformation("HttpRequestService: Request Body: {RequestBody}", requestBodyJson);
            }

            response = await httpClient.PostAsync(postUrl, requestContent).ConfigureAwait(false);

            if (logger != null && logger.IsEnabled(LogLevel.Information))
            {
                string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                logger.LogInformation("--------------Response--------------");
                logger.LogInformation("HttpRequestService: Response Status: {StatusCode}", response.StatusCode);
                logger.LogInformation("HttpRequestService: Response Headers: {Headers}", JsonConvert.SerializeObject(response.Headers));
                logger.LogInformation("HttpRequestService: Response Body: {ResponseBody}", responseBody);
            }

            return response;
        }

        public static async Task<HttpResponseMessage> GetAsync(string getUrl, Dictionary<string, string>? headers = null, ILogger? logger = null)
        {
            HttpResponseMessage response;

            using HttpClient httpClient = new HttpClient();

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            if (logger != null && logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("--------------Request--------------");
                logger.LogInformation("HttpRequestService: Request URL: {GetUrl}", getUrl);
                logger.LogInformation("HttpRequestService: Request Headers: {Headers}", JsonConvert.SerializeObject(httpClient.DefaultRequestHeaders));
            }

            response = await httpClient.GetAsync(getUrl).ConfigureAwait(false);

            if (logger != null && logger.IsEnabled(LogLevel.Information))
            {
                string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                logger.LogInformation("--------------Response--------------");
                logger.LogInformation("HttpRequestService: Response Status: {StatusCode}", response.StatusCode);
                logger.LogInformation("HttpRequestService: Response Headers: {Headers}", JsonConvert.SerializeObject(response.Headers));
                logger.LogInformation("HttpRequestService: Response Body: {ResponseBody}", responseBody);
            }

            return response;
        }

        public static async Task<HttpResponseMessage> GetAsync(string getUrl, ILogger? logger = null)
        {
            HttpResponseMessage response;

            using HttpClient httpClient = new HttpClient();

            if (logger != null && logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("--------------Request--------------");
                logger.LogInformation("HttpRequestService: Request URL: {GetUrl}", getUrl);
                logger.LogInformation("HttpRequestService: Request Headers: {Headers}", JsonConvert.SerializeObject(httpClient.DefaultRequestHeaders));
            }

            response = await httpClient.GetAsync(getUrl).ConfigureAwait(false);

            if (logger != null && logger.IsEnabled(LogLevel.Information))
            {
                string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                logger.LogInformation("--------------Response--------------");
                logger.LogInformation("HttpRequestService: Response Status: {StatusCode}", response.StatusCode);
                logger.LogInformation("HttpRequestService: Response Headers: {Headers}", JsonConvert.SerializeObject(response.Headers));
                logger.LogInformation("HttpRequestService: Response Body: {ResponseBody}", responseBody);
            }

            return response;
        }

        public static async Task<HttpResponseMessage> PutAsync<T>(string putUrl, T body, Dictionary<string, string>? headers = null, ILogger? logger = null)
        {
            string requestBodyJson;
            StringContent requestContent;
            HttpResponseMessage response;

            requestBodyJson = JsonConvert.SerializeObject(body);
            requestContent = new StringContent(requestBodyJson, Encoding.UTF8, "application/json");

            using HttpClient httpClient = new HttpClient();

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            if (logger != null && logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("--------------Request--------------");
                logger.LogInformation("HttpRequestService: Request URL: {PutUrl}", putUrl);
                logger.LogInformation("HttpRequestService: Request Headers: {Headers}", JsonConvert.SerializeObject(httpClient.DefaultRequestHeaders));
                logger.LogInformation("HttpRequestService: Request Body: {RequestBody}", requestBodyJson);
            }

            response = await httpClient.PutAsync(putUrl, requestContent).ConfigureAwait(false);

            if (logger != null && logger.IsEnabled(LogLevel.Information))
            {
                string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                logger.LogInformation("--------------Response--------------");
                logger.LogInformation("HttpRequestService: Response Status: {StatusCode}", response.StatusCode);
                logger.LogInformation("HttpRequestService: Response Headers: {Headers}", JsonConvert.SerializeObject(response.Headers));
                logger.LogInformation("HttpRequestService: Response Body: {ResponseBody}", responseBody);
            }

            return response;
        }

        public static async Task<HttpResponseMessage> PutAsync<T>(string putUrl, T body, ILogger? logger = null)
        {
            string requestBodyJson;
            StringContent requestContent;
            HttpResponseMessage response;

            requestBodyJson = JsonConvert.SerializeObject(body);
            requestContent = new StringContent(requestBodyJson, Encoding.UTF8, "application/json");

            using HttpClient httpClient = new HttpClient();

            if (logger != null && logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("--------------Request--------------");
                logger.LogInformation("HttpRequestService: Request URL: {PutUrl}", putUrl);
                logger.LogInformation("HttpRequestService: Request Headers: {Headers}", JsonConvert.SerializeObject(httpClient.DefaultRequestHeaders));
                logger.LogInformation("HttpRequestService: Request Body: {RequestBody}", requestBodyJson);
            }

            response = await httpClient.PutAsync(putUrl, requestContent).ConfigureAwait(false);

            if (logger != null && logger.IsEnabled(LogLevel.Information))
            {
                string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                logger.LogInformation("--------------Response--------------");
                logger.LogInformation("HttpRequestService: Response Status: {StatusCode}", response.StatusCode);
                logger.LogInformation("HttpRequestService: Response Headers: {Headers}", JsonConvert.SerializeObject(response.Headers));
                logger.LogInformation("HttpRequestService: Response Body: {ResponseBody}", responseBody);
            }

            return response;
        }

        public static async Task<HttpResponseMessage> DeleteAsync(string deleteUrl, Dictionary<string, string>? headers = null, ILogger? logger = null)
        {
            HttpResponseMessage response;

            using HttpClient httpClient = new HttpClient();

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            if (logger != null && logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("--------------Request--------------");
                logger.LogInformation("HttpRequestService: Request URL: {DeleteUrl}", deleteUrl);
                logger.LogInformation("HttpRequestService: Request Headers: {Headers}", JsonConvert.SerializeObject(httpClient.DefaultRequestHeaders));
            }

            response = await httpClient.DeleteAsync(deleteUrl).ConfigureAwait(false);

            if (logger != null && logger.IsEnabled(LogLevel.Information))
            {
                string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                logger.LogInformation("--------------Response--------------");
                logger.LogInformation("HttpRequestService: Response Status: {StatusCode}", response.StatusCode);
                logger.LogInformation("HttpRequestService: Response Headers: {Headers}", JsonConvert.SerializeObject(response.Headers));
                logger.LogInformation("HttpRequestService: Response Body: {ResponseBody}", responseBody);
            }

            return response;
        }

        public static async Task<HttpResponseMessage> DeleteAsync(string deleteUrl, ILogger? logger = null)
        {
            HttpResponseMessage response;

            using HttpClient httpClient = new HttpClient();

            if (logger != null && logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("--------------Request--------------");
                logger.LogInformation("HttpRequestService: Request URL: {DeleteUrl}", deleteUrl);
                logger.LogInformation("HttpRequestService: Request Headers: {Headers}", JsonConvert.SerializeObject(httpClient.DefaultRequestHeaders));
            }

            response = await httpClient.DeleteAsync(deleteUrl).ConfigureAwait(false);

            if (logger != null && logger.IsEnabled(LogLevel.Information))
            {
                string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                logger.LogInformation("--------------Response--------------");
                logger.LogInformation("HttpRequestService: Response Status: {StatusCode}", response.StatusCode);
                logger.LogInformation("HttpRequestService: Response Headers: {Headers}", JsonConvert.SerializeObject(response.Headers));
                logger.LogInformation("HttpRequestService: Response Body: {ResponseBody}", responseBody);
            }

            return response;
        }
    }
}