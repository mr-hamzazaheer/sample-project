using System.Net;
using System.Text.Json.Serialization;

namespace Shared
{
    public class Response
    {
        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
        public string Message { get; set; } = Shared.Message.Success;
        public dynamic Data { get; set; }
        public bool Status { get { return StatusCode == HttpStatusCode.OK; } }
    }
}
