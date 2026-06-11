using Platform.Common.Results;

namespace Platform.Contracts.Queries;

public interface IQueryHandler<in TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    Task<Result<TResult>> Handle(
        TQuery query,
        CancellationToken cancellationToken);
}