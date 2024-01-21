using Bifrost.Data;
using Bifrost.Data.Base;
using Bifrost.Features.Portals.Model;

namespace Bifrost.Features.Portals.Services;

public class PortalDefinitionRepository(ApplicationDbContext dbContext)
    : RepositoryBase<PortalDefinition>(dbContext), IPortalDefinitionRepository
{
}
