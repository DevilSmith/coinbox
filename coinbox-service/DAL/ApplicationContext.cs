using coinbox_service.Web.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace WeatherArchiveApp.Models
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext() { }

        public DbSet<ChangesToCoins> ChangesToCoins { get; set; } = null!;

        public DbSet<NumberOfCoins> NumberOfCoins { get; set; } = null!;

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlite("Data Source=coinbox-service.db");
    }
}
