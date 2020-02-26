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
            modelBuilder.Entity<Wallet>()
                .HasMany((w => w.Operations))
                .WithOne(o => o.Wallet)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<Wallet>()
                .ToTable(nameof(Wallets))
                .HasIndex(x=>x.UserId);

            modelBuilder.Entity<WalletOperation>()
                .ToTable(nameof(Operations))
                .HasIndex(x=>x.WalletId);
        }
    }
}
