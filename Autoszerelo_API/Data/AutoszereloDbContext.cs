using Autoszerelo.Shared;
using Autoszerelo_Shared;
using Microsoft.EntityFrameworkCore;

namespace Autoszerelo_API.Data
{
    public class AutoszereloDbContext : DbContext
    {
        public AutoszereloDbContext(DbContextOptions<AutoszereloDbContext> options) : base(options)
        {
        }

        public DbSet<Ugyfel> Ugyfelek { get; set; }
        public DbSet<Munka> Munkak { get; set; }
    }
}