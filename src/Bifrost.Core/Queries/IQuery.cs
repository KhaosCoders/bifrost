using Bifrost.Commands;
using MediatR;

namespace Bifrost.Queries;

public interface IQuery<T> : IRequest<CommandResponse<T>>;
