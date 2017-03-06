using System;

namespace TodoistDemo.Core.Validation.Reports.Operation
{
    public class BasicReport
    {
        public bool IsSuccessful { get; set; }

        public string ErrorMessage { get; set; }

        public string DevErrorMessage { get; set; }
        
        public void SetFromException(Exception exception)
        {
            ErrorMessage = exception.Message;
            DevErrorMessage = exception.StackTrace;
        }
    }
}