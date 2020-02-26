using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletApp.Database
{
    public class WalletContext:DbContext
    {
        public WalletContext(DbContextOptions<WalletContext> options) : base(options)
        {
        }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletOperation> Operations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Wallet>().ToTable(nameof(Wallets)).HasMany<WalletOperation>();
            modelBuilder.Entity<WalletOperation>().ToTable(nameof(Operations)).HasOne<Wallet>();
        }
    }
}
