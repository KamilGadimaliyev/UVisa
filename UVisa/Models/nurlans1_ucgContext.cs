using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace UVisa.Models
{
    public partial class nurlans1_ucgContext : DbContext
    {
        public nurlans1_ucgContext()
        {
        }

        public nurlans1_ucgContext(DbContextOptions<nurlans1_ucgContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Status> Statuses { get; set; }
        public virtual DbSet<UserInfo> UserInfos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=wdb4.my-hosting-panel.com;Database=nurlans1_ucg;user id=nurlans1_ucg;password=2M003z$if; Trusted_Connection=False;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("nurlans1_ucg")
                .HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.ToTable("Contact");

                entity.Property(e => e.ContactEmail).HasMaxLength(50);

                entity.Property(e => e.ContactMessage).HasMaxLength(500);

                entity.Property(e => e.ContactName).HasMaxLength(50);

                entity.Property(e => e.ContactPhone).HasMaxLength(50);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.OrderDate).HasColumnType("datetime");

                entity.Property(e => e.OrderMoney).HasColumnType("money");

                entity.HasOne(d => d.OrderUserInfo)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.OrderUserInfoId)
                    .HasConstraintName("FK__Orders__OrderUse__34C8D9D1");
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.Property(e => e.StatusName).HasMaxLength(50);
            });

            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.ToTable("UserInfo");

                entity.Property(e => e.UserInfoAge).HasMaxLength(30);

                entity.Property(e => e.UserInfoEmail).HasMaxLength(30);

                entity.Property(e => e.UserInfoFile).HasMaxLength(30);

                entity.Property(e => e.UserInfoForCountry).HasMaxLength(30);

                entity.Property(e => e.UserInfoName).HasMaxLength(30);

                entity.Property(e => e.UserInfoPassportId)
                    .HasMaxLength(30)
                    .HasColumnName("UserInfoPassportID");

                entity.Property(e => e.UserInfoPhone).HasMaxLength(30);

                entity.Property(e => e.UserInfoSurname).HasMaxLength(30);

                entity.Property(e => e.UserInfoTypeVisa).HasMaxLength(30);

                entity.HasOne(d => d.UserInfoStatus)
                    .WithMany(p => p.UserInfos)
                    .HasForeignKey(d => d.UserInfoStatusId)
                    .HasConstraintName("FK__UserInfo__UserIn__398D8EEE");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
