namespace Rentar.Shared.Application.UseCases;

/// <summary>
/// Carries out one use case that changes state, described by <typeparamref name="TCommand"/>.
/// </summary>
public interface ICommandHandler<in TCommand>
{
    void Handle(TCommand command);
}

/// <summary>
/// Carries out one use case that changes state and hands back what the caller needs from it, e.g. the created record.
/// </summary>
public interface ICommandHandler<in TCommand, out TResult>
{
    TResult Handle(TCommand command);
}
