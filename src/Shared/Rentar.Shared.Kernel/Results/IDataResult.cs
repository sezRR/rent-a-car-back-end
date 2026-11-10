namespace Rentar.Shared.Kernel.Results;

public interface IDataResult<T> : IResult
{
    T Data { get; }
}
