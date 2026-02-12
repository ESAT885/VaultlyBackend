namespace VaultlyBackend.Api.Models.BaseModels
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public string? TraceId { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }

        public static ApiResponse<T> Ok(T data, string traceId, string? message = null)
            => new() { Success = true, Data = data, Message = message, TraceId = traceId };

        public static ApiResponse<T> Fail(
            string message,
            string traceId,
            Dictionary<string, string[]>? errors = null)
            => new() { Success = false, Message = message, Errors = errors, };
    }
}
