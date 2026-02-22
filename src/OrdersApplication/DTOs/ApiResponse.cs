namespace OrdersApplication.DTOs;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public Guid RequestId { get; set; }
    public List<ErrorDetail> Errors { get; set; }

    public static ApiResponse<T> Ok(T data, string message = "Success", Guid? requestId = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message,
            RequestId = requestId ?? Guid.NewGuid(),
        };
    }

    public static ApiResponse<T> Error(string message, Guid requestId, string errorCode = "ERROR")
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            RequestId = requestId,
            Errors = new List<ErrorDetail>
            {
                new() { Code = errorCode, Message = message }
            }
        };
    }

    public static ApiResponse<T> ValidationError(List<ErrorDetail> errors, Guid requestId)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = "Validation failed",
            RequestId = requestId,
            Errors = errors
        };
    }
}

public class ErrorDetail
{
    public string Code { get; set; }
    public string Message { get; set; }
    public string Field { get; set; }
}