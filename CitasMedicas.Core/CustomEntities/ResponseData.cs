using System.Net;
using System.Text.Json.Serialization;

namespace CitasMedicas.Core.CustomEntities
{
    public class ResponseData
    {
        public PagedList<object> Pagination { get; set; } = null!;
        public List<Message> Messages { get; set; } = new List<Message>();

        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }
    }
}
