
using Microsoft.EntityFrameworkCore;
using NPO.SPARC._2.Entity;

namespace NPO.SPARC._2.Services
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Tests> Tests { get; set; }
        public DbSet<Parameters> Parameters { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=NPO.SPARC;Username=admin;Password=1");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");

            modelBuilder.Entity<Tests>(entity =>
            {
                entity.ToTable("Tests");

                entity.Property(e => e.TestId).HasColumnName("TestId");
                entity.Property(e => e.TestDate).HasColumnName("TestDate");
                entity.Property(e => e.BlockName).HasColumnName("BlockName");
                entity.Property(e => e.Note).HasColumnName("Note");
            });

            modelBuilder.Entity<Parameters>()
                .HasKey(p => p.ParameterId);

            modelBuilder.Entity<Tests>()
                .HasKey(p => p.TestId);

            modelBuilder.Entity<Parameters>(entity =>
            {
                entity.ToTable("Parameters");

                entity.Property(e => e.ParameterId).HasColumnName("ParameterId");
                entity.Property(e => e.TestId).HasColumnName("TestId");
                entity.Property(e => e.ParameterName).HasColumnName("ParameterName");
                entity.Property(e => e.RequiredValue).HasColumnName("RequiredValue");
                entity.Property(e => e.MeasuredValue).HasColumnName("MeasuredValue");

                entity.HasOne(p => p.Tests)
                    .WithMany(t => t.Parameters)
                    .HasForeignKey(p => p.TestId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Parameters_TestId_fkey");
            });
        }
    }
}
