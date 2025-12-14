using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NotifySystem.Core.Domain.Entities;
using NotifySystem.Core.Domain.SharedKernel.Storage;

namespace NotifySystem.Infrastructure.Persistance.DataStorage
{
    public class NotifyDbContext : DbContext, IUnitOfWork
    {
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationChannel> NotificationChannels { get; set; }
        public DbSet<NotificationHistory> NotificationHistory { get; set; }
        public DbSet<NotificationTemplate> NotificationTemplates { get; set; }
        public DbSet<Recipient> Recipients { get; set; }

        public NotifyDbContext(DbContextOptions<NotifyDbContext> options) : base(options) { }
        
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await base.SaveChangesAsync(cancellationToken);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.Property(e => e.Type).HasConversion<string>();
                entity.Property(e => e.Status).HasConversion<string>();
                
                entity.HasMany(n => n.History)
                    .WithOne()
                    .HasForeignKey(h => h.NotificationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<NotificationHistory>(entity =>
            {
                entity.Property(h => h.Channel).IsRequired().HasMaxLength(50);
                entity.Property(h => h.ProviderResponse).HasMaxLength(1000);
                entity.Property(h => h.ErrorMessage).HasMaxLength(1000);
                entity.Property(h => h.AttemptedAt).IsRequired();
                entity.Property(h => h.IsSuccess).IsRequired();
            });

            modelBuilder.Entity<NotificationTemplate>(entity =>
            {
                entity.Property(e => e.Type).HasConversion<string>();
            });
        }
    }
}
