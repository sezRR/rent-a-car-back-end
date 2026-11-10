namespace Rentar.Shared.Kernel.Results;

public interface IResult
{
    bool Success { get; }
    string Message { get; }
}
