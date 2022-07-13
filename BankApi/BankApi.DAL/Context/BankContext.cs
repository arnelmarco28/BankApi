using Bank.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bank.DAL.Context
{
    public class BankContext : DbContext
    {
        public BankContext(DbContextOptions<BankContext> options) : base(options)
        {
        }

        public BankContext(string connectionString)
            : base(new DbContextOptionsBuilder().UseSqlServer(connectionString).Options)
        {
        }

        public DbSet<Account> Account { get; set; }
        public DbSet<Transfer> Transfer { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Account>()
                .HasKey(x => x.AccountID);
            builder.Entity<Account>()
                .HasIndex(u => u.Username)
                .IsUnique();

            builder.Entity<Transfer>()
                .HasKey(x => x.TransferID);
        }

    }
}
