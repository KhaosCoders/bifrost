using MediatR;

namespace Bifrost.Commands;

public interface ICommand : IRequest<CommandResponse>;
