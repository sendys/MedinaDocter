
using MedinaDokter.Api.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedinaDokter.Api.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();

        public DbSet<Pegawai> Pegawai => Set<Pegawai>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
                base.OnModelCreating(modelBuilder);

                // Mapping tabel users
                modelBuilder.Entity<User>(entity =>
                {
                        entity.ToTable("users");

                        entity.HasKey(e => e.Id);

                        entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint unsigned")
                    .ValueGeneratedOnAdd();

                        entity.Property(e => e.Username)
                    .HasColumnName("username")
                    .HasMaxLength(191)
                    .IsRequired();

                        entity.Property(e => e.Password)
                    .HasColumnName("password")
                    .HasMaxLength(191)
                    .IsRequired();

                        entity.Property(e => e.PegawaiId)
                    .HasColumnName("pegawai_id")
                    .HasColumnType("bigint unsigned");

                        entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at");

                        entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at");

                        entity.HasOne(e => e.Pegawai)
                    .WithMany(e => e.Users)
                    .HasForeignKey(e => e.PegawaiId)
                    .HasPrincipalKey(e => e.Id);
                });

                // Mapping tabel pegawai
                modelBuilder.Entity<Pegawai>(entity =>
                {
                        entity.ToTable("pegawai");

                        entity.HasKey(e => e.Id);

                        entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint unsigned")
                    .ValueGeneratedOnAdd();

                        entity.Property(e => e.Nama)
                    .HasColumnName("nama")
                    .HasMaxLength(191)
                    .IsRequired();

                        entity.Property(e => e.Flag)
                    .HasColumnName("flag");

                });
        }
}