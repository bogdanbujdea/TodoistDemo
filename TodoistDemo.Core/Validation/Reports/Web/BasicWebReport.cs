using Windows.Web.Http;
using TodoistDemo.Core.Validation.Reports.Operation;

namespace TodoistDemo.Core.Validation.Reports.Web
{
    public class BasicWebReport: BasicReport
    {
        private FailedRequest _failedRequestInfo;

        public HttpStatusCode HttpCode { get; set; }

        public string StringResponse { get; set; }

        public FailedRequest FailedRequestInfo
        {
            get { return _failedRequestInfo; }
            set
            {
                _failedRequestInfo = value;
                if (_failedRequestInfo.ErrorCode == 0)
                    IsSuccessful = true;
                else ErrorMessage = _failedRequestInfo.ErrorMessage;
            }
        }
    }
}