using Bifrost.Models.Portals;
using MediatR;

namespace Bifrost.Features.Portals.Commands;

public record TriggerPortalDeleteCommand(PortalDefinition Portal) : IRequest;
