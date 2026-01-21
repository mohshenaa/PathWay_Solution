using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PathWay_Solution.Data
{
    public class PathwayDBContext: IdentityDbContext  //IdentityDbContext class inherits from DbContext...Our application Context class needs to be inherited from the IdentityDbContext class instead of the DbContext class. This is because IdentityDbContext inherits from the Entity Framework Core DbContext class and provides all the DbSet properties needed to manage the Identity Tables.
    {
        public PathwayDBContext(DbContextOptions<PathwayDBContext> options):base(options) { }
    }
}
