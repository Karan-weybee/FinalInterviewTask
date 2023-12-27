using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PartyProductCore.Models
{
    public partial class PartyProductCoreContext : DbContext
    {
        public PartyProductCoreContext()
        {
        }

        public PartyProductCoreContext(DbContextOptions<PartyProductCoreContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AssignParties> AssignParties { get; set; }
        public virtual DbSet<Invoices> Invoices { get; set; }
        public virtual DbSet<Parties> Parties { get; set; }
        public virtual DbSet<ProductRates> ProductRates { get; set; }
        public virtual DbSet<Products> Products { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=DESKTOP-9IJS7NM;Database=PartyProductCore;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AssignParties>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.PartyId).HasColumnName("Party_id");

                entity.Property(e => e.ProductId).HasColumnName("Product_id");

                entity.HasOne(d => d.Party)
                    .WithMany(p => p.AssignParties)
                    .HasForeignKey(d => d.PartyId)
                    .HasConstraintName("FK_dbo.AssignParties_dbo.Parties_Party_id");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.AssignParties)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_dbo.AssignParties_dbo.Products_Product_id");
            });

            modelBuilder.Entity<Invoices>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DateOfInvoice).HasColumnType("datetime");

                entity.Property(e => e.PartyId).HasColumnName("Party_id");

                entity.Property(e => e.ProductId).HasColumnName("Product_id");

                entity.Property(e => e.RateOfProduct)
                    .HasColumnName("Rate_Of_Product")
                    .HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Party)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.PartyId)
                    .HasConstraintName("FK_dbo.Invoices_dbo.Parties_Party_id");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_dbo.Invoices_dbo.Products_Product_id");
            });

            modelBuilder.Entity<Parties>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.PartyName).IsRequired();
            });

            modelBuilder.Entity<ProductRates>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DateOfRate)
                    .HasColumnName("Date_Of_Rate")
                    .HasColumnType("datetime");

                entity.Property(e => e.ProductId).HasColumnName("Product_id");

                entity.Property(e => e.Rate).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductRates)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_dbo.ProductRates_dbo.Products_Product_id");
            });

            modelBuilder.Entity<Products>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ProductName).IsRequired();
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
