namespace Rentar.Shared.Application.UseCases;

/// <summary>
/// Answers one use case that only reads, described by <typeparamref name="TQuery"/>. It changes nothing.
/// </summary>
public interface IQueryHandler<in TQuery, out TResult>
{
    TResult Handle(TQuery query);
}
