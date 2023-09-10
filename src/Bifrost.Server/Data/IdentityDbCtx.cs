using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bifrost.Server.Data;

public class IdentityDbCtx : IdentityDbContext
{
    public IdentityDbCtx(DbContextOptions<IdentityDbCtx> options)
        : base(options)
    {
    }
}