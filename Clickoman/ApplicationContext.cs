using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace Clickoman
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext() : base("DefaultConnection")
        {

        }
        public DbSet<Player> Players { get; set; }

        public DbSet<Reward> Rewards { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>().Property(player => player.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<Reward>().Property(reward => reward.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            base.OnModelCreating(modelBuilder);
        }
    }
}