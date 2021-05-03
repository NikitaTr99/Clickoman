using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.SQLite;
using System.Data.SQLite.EF6;
using System.IO;

namespace Clickoman
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Player> Players { get; set; }
        
        public DbSet<Reward> Rewards { get; set; }
        public ApplicationContext():base("DefaultConnection")
        {
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>().Property(Player => Player.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            
            modelBuilder.Entity<Reward>().Property(Reward => Reward.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            
            base.OnModelCreating(modelBuilder);
        }
    }
}