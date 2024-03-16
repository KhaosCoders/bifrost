using MediatR;

namespace Bifrost.Queries;

public interface IQuery<T> : IRequest<QueryResponse<T>>;
