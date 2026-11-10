using System.Text.Json.Serialization;

namespace Rentar.Shared.Kernel.Results;

public class Result : IResult
{
    [JsonConstructor]
    public Result(bool success, string message)
    {
        Success = success;
        Message = message;
    }

    public Result(bool success)
    {
        Success = success;
    }

    public bool Success { get; }
    public string Message { get; }
}
