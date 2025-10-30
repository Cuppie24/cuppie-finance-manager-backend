namespace Application.Dto;

public class OperationResult(bool isSuccess, string message)
{
    public bool IsSuccess { get; set; } = isSuccess;
    public string Message { get; set; } = message;
    public OperationStatusCode OperationStatusCode { get; set; }

    public static OperationResult Success() => new OperationResult(true, string.Empty);
    public static OperationResult Failure(string message) => new OperationResult(false, message);
}

public class OperationResult<T>(T data, bool isSuccess, string message, OperationStatusCode operationStatusCode) : OperationResult(isSuccess, message)
{
    public T Data { get; set; } = data;
    public static OperationResult<T> Success(T data) => new OperationResult<T>(data, true, string.Empty, OperationStatusCode.Ok);
    public new static OperationResult<T?> Failure(string message, OperationStatusCode operationStatusCode) 
        => new OperationResult<T?>(default(T), false, message, operationStatusCode);
}

public enum OperationStatusCode
{
    Ok = 0,
    NotFound = 1,
    ValidationError = 2,
    Unauthorized = 3,
    BadRequest = 4,
    Conflict = 5,
    InternalError = 6
}