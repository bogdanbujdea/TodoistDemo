using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Web;
using Windows.Web.Http;
using Newtonsoft.Json;
using TodoistDemo.Core.Validation.Reports.Web;
using HttpClient = Windows.Web.Http.HttpClient;
using HttpResponseMessage = Windows.Web.Http.HttpResponseMessage;

namespace TodoistDemo.Core.Communication
{
    public class RestClient : IRestClient
    {
        private readonly HttpClient _httpClient;

        public RestClient()
        {
            _httpClient = new HttpClient();
        }

        public async Task<BasicWebReport> PostAsync(string url, List<KeyValuePair<string, string>> formData)
        {
            BasicWebReport httpReport = new BasicWebReport();
            try
            {
                var content = new HttpFormUrlEncodedContent(formData);

                var responseMessage = await _httpClient.PostAsync(new Uri(url), content);
                httpReport = await CreateHttpReport(responseMessage);
            }
            catch (Exception exception)
            {
                httpReport.ErrorMessage = GetErrorMessageFromWebException(exception);
            }
            return httpReport;
        }

        private string GetErrorMessageFromWebException(Exception exception)
        {
            var webErrorStatus = WebError.GetStatus(exception.HResult);
            if (webErrorStatus == WebErrorStatus.CannotConnect)
                return "Please check your internet connection.";
            return exception.Message;
        }

        private static async Task<BasicWebReport> CreateHttpReport(HttpResponseMessage responseMessage)
        {
            var httpReport = new BasicWebReport
            {
                HttpCode = responseMessage.StatusCode,
                StringResponse = await responseMessage.Content.ReadAsStringAsync(),
                IsSuccessful = responseMessage.IsSuccessStatusCode
            };
            if (httpReport.IsSuccessful)
                return httpReport;
            try
            {
                httpReport.FailedRequestInfo = JsonConvert.DeserializeObject<FailedRequest>(httpReport.StringResponse);
            }
            catch (Exception)
            {
            }

            return httpReport;
        }
    }
}