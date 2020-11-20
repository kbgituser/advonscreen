using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Dal.Models;
using Microsoft.AspNetCore.Identity;

namespace Dal.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Advertisement> Advertisements { get; set; }        
        public DbSet<City> Cities { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<AdvertisementStatus> AdvertisementStatuses { get; set; }
        public DbSet<AdvertisementStatusHistory> AdvertisementStatusHistories { get; set; }
        public DbSet<PointPrice> PointPrices { get; set; }
        public DbSet<Point> Points { get; set; }
        public DbSet<SecondsForAdv> SecondsForAdvs { get; set; }
        public DbSet<DaysForAdv> DaysForAdvs { get; set; }
        public DbSet<Payment> Payments { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Advertisement>()
                .HasOne(r => r.ApplicationUser)
                .WithMany(u => u.Advertisements)
                .HasForeignKey(o => o.UserId)
                .IsRequired()
                ;
            builder.Entity<Advertisement>()
                .HasOne(r => r.AdvertisementStatus)
                .WithMany(u => u.Advertisements)
                .HasForeignKey(o => o.AdvertisementStatusId)                
                .OnDelete(DeleteBehavior.NoAction)
                ;

        }
    }
}