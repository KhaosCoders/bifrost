using Bifrost.Queries;

namespace Bifrost;

public class DummyQueryHandler : MediatR.IRequestHandler<DummyQuery, QueryResponse<string?>>
{
    public Task<QueryResponse<string?>> Handle(DummyQuery request, CancellationToken cancellationToken) =>
        Task.FromResult(QueryResponse<string?>.Problem("---", "Not Implemented"));
}
