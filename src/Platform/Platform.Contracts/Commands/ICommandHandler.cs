using Platform.Common.Results;

namespace Platform.Contracts.Commands;

public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task<Result> Handle(
        TCommand command,
        CancellationToken cancellationToken);
}