using Bifrost.Data;
using Bifrost.Data.Base;
using Bifrost.Models.Portals;

namespace Bifrost.Features.PortalDefinitions.Services;

public class PortalDefinitionRepository(ApplicationDbContext dbContext)
    : RepositoryBase<PortalDefinition>(dbContext), IPortalDefinitionRepository
{
}
