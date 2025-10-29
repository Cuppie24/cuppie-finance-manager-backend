namespace Application.Dto;

public class OperationResult(bool isSuccess, string message)
{
    public bool IsSuccess { get; set; } = isSuccess;
    public string? Message { get; set; } = message;

    public static OperationResult Success() => new OperationResult(true, string.Empty);
    public static OperationResult Failure(string message) => new OperationResult(false, message);
}

public class OperationResult<T>(T data, bool isSuccess, string message) : OperationResult(isSuccess, message)
{
    public T? Data { get; set; } = data;
    public static OperationResult<T> Success(T data) => new OperationResult<T>(data, true, string.Empty);
    public new static OperationResult<T?> Failure(string? message) => new OperationResult<T?>(default(T), false, message);
}