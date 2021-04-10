using Microsoft.EntityFrameworkCore;
using SamuraiApp.Domain;

namespace SamuraiApp.Data
{
    public class SamuraiContext : DbContext
    {
        public DbSet<Samurai> Samurais { get; set; }
        public DbSet<Quote> Qoutes { get; set; }
        public DbSet<Battle> Battles { get; set; }

        //This OnConfiguring methos is used here only for demo purpose. We shall inject this info from config file later on.
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog=SamuraiAppData");
        }

        //EF core has already generated class BattleSamurai on it's own for many to many relationship.
        //But, now, we have to save some extra info (payload), to this table, apart from just the 2 id columns.
        //So, we need below code which tells the EF core to consider payloads as well (DateJoined in this case).
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Samurai>()
                .HasMany(s => s.Battles)
                .WithMany(b => b.Samurais)
                .UsingEntity<BattleSamurai>
                (bs => bs.HasOne<Battle>().WithMany(),
                 bs => bs.HasOne<Samurai>().WithMany())
                .Property(bs => bs.DateJoined)
                .HasDefaultValueSql("getdate()");
        }
    }
}
