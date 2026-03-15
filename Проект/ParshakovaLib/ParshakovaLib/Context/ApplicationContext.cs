using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ParshakovaLib.Models;

namespace ParshakovaLib.Context
{
    public partial class ApplicationContext : DbContext
    {
        public ApplicationContext()
        {
        }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Discount> Discount { get; set; }
        public virtual DbSet<Partner> Partner { get; set; }
        public virtual DbSet<Sale> Sale { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Database=parshakova_db;Username=app;Password=123456789");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Discount>(entity =>
            {
                entity.ToTable("discount", "app");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('discount_id_seq'::regclass)");

                entity.Property(e => e.Partnerid).HasColumnName("partnerid");

                entity.Property(e => e.Percentage).HasColumnName("percentage");

                entity.HasOne(d => d.Partner)
                    .WithMany(p => p.Discount)
                    .HasForeignKey(d => d.Partnerid)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("discount_partnerid_fkey");
            });

            modelBuilder.Entity<Partner>(entity =>
            {
                entity.ToTable("partner", "app");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('partner_id_seq'::regclass)");

                entity.Property(e => e.Director)
                    .IsRequired()
                    .HasColumnName("director");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email");

                entity.Property(e => e.Legaladdress)
                    .IsRequired()
                    .HasColumnName("legaladdress");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasColumnName("phone");

                entity.Property(e => e.Rating).HasColumnName("rating");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("type");
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.ToTable("sale", "app");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('sale_id_seq'::regclass)");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("date");

                entity.Property(e => e.Partnerid).HasColumnName("partnerid");

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasColumnName("product_name")
                    .HasColumnType("character varying");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.HasOne(d => d.Partner)
                    .WithMany(p => p.Sale)
                    .HasForeignKey(d => d.Partnerid)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("sale_partnerid_fkey");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
