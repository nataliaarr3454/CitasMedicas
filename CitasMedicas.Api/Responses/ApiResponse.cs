using CitasMedicas.Core.CustomEntities;
using CitasMedicas.Core.Enums;

namespace CitasMedicas.Api.Responses
{
    public class ApiResponse<T>
    {
        public ApiResponse(T data, List<Message>? messages = null)
        {
            Data = data;
            Messages = messages;
        }
        public ApiResponse(T data, Pagination pagination, List<Message>? messages = null)
        {
            Data = data;
            Pagination = pagination;
            Messages = messages;
        }
        public ApiResponse(List<Message> messages)
        {
            Messages = messages;
        }

        public T? Data { get; set; }
        public Pagination? Pagination { get; set; }
        public List<Message>? Messages { get; set; }
    }
}
