using System.Text.Json.Serialization;

namespace Core.Utilities.Results;

public class DataResult<T> : Result, IDataResult<T>
{
    [JsonConstructor]
    public DataResult(T data, bool success, string message)
        : base(success, message)
    {
        Data = data;
    }

    public DataResult(T data, bool success)
        : base(success)
    {
        Data = data;
    }

    public T Data { get; }
}