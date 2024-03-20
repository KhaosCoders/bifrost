using MediatR;

namespace Bifrost.Commands;

public interface ICommand : IRequest<CommandResponse>;

public interface ICommand<T> : IRequest<CommandResponse<T>>;
