namespace SharedKernel;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }
    protected List<string> _errors = new List<string>();
    public IReadOnlyCollection<string> Errors => _errors.AsReadOnly();

    protected Result(bool isSuccess, string error)
    {
        if (isSuccess && !string.IsNullOrEmpty(error))
            throw new InvalidOperationException("A successful result cannot contain an error message.");

        if (!isSuccess && string.IsNullOrEmpty(error))
            throw new InvalidOperationException("A failure result must contain an error message.");

        IsSuccess = isSuccess;
        Error = error;

        if (!string.IsNullOrEmpty(error))
            _errors.Add(error);
    }

    protected Result(bool isSuccess, List<string> errors)
    {
        if (isSuccess && errors != null && errors.Count > 0)
            throw new InvalidOperationException("A successful result cannot contain error messages.");

        if (!isSuccess && (errors == null || errors.Count == 0))
            throw new InvalidOperationException("A failure result must contain at least one error message.");

        IsSuccess = isSuccess;
        Error = errors?.Count > 0 ? errors[0] : string.Empty;
        _errors = errors ?? new List<string>();
    }

    public static Result Success() => new Result(true, string.Empty);
    public static Result Failure(string error) => new Result(false, error);
    public static Result Failure(List<string> errors) => new Result(false, errors);

    public static Result<T> Success<T>(T value) => new Result<T>(value, true, string.Empty);
    public static Result<T> Failure<T>(string error) => new Result<T>(default, false, error);
    public static Result<T> Failure<T>(List<string> errors) => new Result<T>(default, false, errors);
}

public class Result<T> : Result
{
    public T Value { get; }

    protected internal Result(T value, bool isSuccess, string error)
        : base(isSuccess, error)
    {
        Value = value;
    }

    protected internal Result(T value, bool isSuccess, List<string> errors)
        : base(isSuccess, errors)
    {
        Value = value;
    }
}