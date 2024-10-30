using static Mango.Web.Utility.SD;

namespace Mango.Web.Models
{
    public class RequestDTO
    {
        public ApiType ApiType { get; set; }
        public string Url { get; set; } = string.Empty;
        public object Data { get; set; } = new object();
        public string AccessToken { get; set; } = string.Empty;
        public ContentType ContentType { get; set; } = ContentType.Json;
    }
}
