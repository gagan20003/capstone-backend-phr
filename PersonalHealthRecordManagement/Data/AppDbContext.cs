using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PersonalHealthRecordManagement.Models;

namespace PersonalHealthRecordManagement.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        // Main entities
        public DbSet<Appointments> Appointments { get; set; }
        public DbSet<MedicalRecords> MedicalRecords { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Allergies> Allergies { get; set; }
        public DbSet<Medications> Medications { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------- RELATIONSHIPS ----------

            // ApplicationUser 1–1 UserProfile
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.UserProfile)
                .WithOne(p => p.User)
                .HasForeignKey<UserProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ensure UserId is unique in UserProfile (one profile per user)
            modelBuilder.Entity<UserProfile>()
                .HasIndex(p => p.UserId)
                .IsUnique();

            // ApplicationUser 1–many Appointments
            modelBuilder.Entity<Appointments>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ApplicationUser 1–many MedicalRecords
            modelBuilder.Entity<MedicalRecords>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserProfile 1–many Allergies
            modelBuilder.Entity<UserProfile>()
                .HasMany(p => p.Allergies)
                .WithOne(a => a.UserProfile)
                .HasForeignKey(a => a.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserProfile 1–many Medications
            modelBuilder.Entity<UserProfile>()
                .HasMany(p => p.Medications)
                .WithOne(m => m.UserProfile)
                .HasForeignKey(m => m.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            

            // Convert ALL bool properties to NUMBER(1)
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    if (property.ClrType == typeof(bool) || property.ClrType == typeof(bool?))
                    {
                        property.SetColumnType("NUMBER(1)");
                    }
                }
            }

            // Uppercase table and column names
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // TABLE name => UPPERCASE
                entity.SetTableName(entity.GetTableName().ToUpper());

                // COLUMN names => UPPERCASE
                foreach (var prop in entity.GetProperties())
                {
                    var tableName = entity.GetTableName();
                    var columnName = prop.GetColumnName(
                        StoreObjectIdentifier.Table(tableName, entity.GetSchema())
                    );
                    prop.SetColumnName(columnName.ToUpper());
                }
            }
        }
    }
}