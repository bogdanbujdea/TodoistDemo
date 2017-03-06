using System;

namespace TodoistDemo.Core.Communication
{
    public class ApiException : Exception
    {
        public ApiException(string message)
        {
            ErrorMessage = message;
        }

        public string ErrorMessage { get; set; }
    }
}