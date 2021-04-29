using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SamuraiApp.Domain;
using System;

namespace SamuraiApp.Data
{
    public class SamuraiContext : DbContext
    {
        public DbSet<Samurai> Samurais { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Battle> Battles { get; set; }
        public DbSet<SamuraiBattleStat> SamuraiBattleStats { get; set; }

        //This OnConfiguring methos is used here only for demo purpose. We shall inject this info from config file later on.
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer("Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog=SamuraiAppData")
                //For logging
                //.LogTo(Console.WriteLine);
                .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information)
                //To Log sensitive data i.e. parameters, etc
                .EnableSensitiveDataLogging();
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

            //To rename table: From horse to horses
            modelBuilder.Entity<Horse>().ToTable("Horses");

            //To tell ef core that entity has no key, and that's intentional
            //ToView() ensures that ef core maps this SamuraiBattleStat entity to existing dbo.SamuraiBattleStats, and does not create new table with reference to dbset
            modelBuilder.Entity<SamuraiBattleStat>().HasNoKey().ToView("SamuraiBattleStats");
        }
    }
}
