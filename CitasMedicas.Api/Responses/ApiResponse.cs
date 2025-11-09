using CitasMedicas.Core.CustomEntities;

namespace CitasMedicas.Api.Responses
{
    public class ApiResponse<T>
    {
        public T Data { get; set; }
        public Pagination? Pagination { get; set; }
        public Message[]? Messages { get; set; }

        public ApiResponse(T data)
        {
            Data = data;
        }

        public ApiResponse(T data, Message[] messages)
        {
            Data = data;
            Messages = messages;
        }

        public ApiResponse(T data, Pagination pagination, Message[] messages)
        {
            Data = data;
            Pagination = pagination;
            Messages = messages;
        }
    }
}
