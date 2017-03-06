using System.Collections.Generic;
using System.Threading.Tasks;
using TodoistDemo.Core.Validation.Reports.Web;

namespace TodoistDemo.Core.Communication
{
    public interface IRestClient
    {
        Task<BasicWebReport> PostAsync(string url, List<KeyValuePair<string, string>> formData);
    }
}