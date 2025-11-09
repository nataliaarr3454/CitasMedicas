using System.Net;
using System.Text.Json.Serialization;

namespace CitasMedicas.Core.CustomEntities
{
    public class ResponseData
    {
        public PagedList<object>? Pagination { get; set; }
        public Message[]? Messages { get; set; }

        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }
    }
}
