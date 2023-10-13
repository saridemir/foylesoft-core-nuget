using FoyleSoft.Core.Implementations;
using FoyleSoft.Core.Implementations.Data.Tables;
using FoyleSoft.Core.Implementations.Data.Tables.Logs;
using Microsoft.EntityFrameworkCore;

namespace FoyleSoft.AzureCore.Implementations
{
    public class FoyleSoftLogContext : DbContext
    {
        private bool _hasDefaultTable;

        public FoyleSoftLogContext(DbContextOptions options, bool hasDefaultTable = true)
           : base(options)
        {
            _hasDefaultTable = hasDefaultTable;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (_hasDefaultTable)
            {
                modelBuilder.Entity<SystemUser>().ToTable("SystemUser");
                //modelBuilder.Entity<SystemCompany>().ToTable("SystemCompany");
                modelBuilder.Entity<Role>().ToTable("Role");
                modelBuilder.Entity<RoleMapping>().ToTable("RoleMapping");
                modelBuilder.Entity<UserRole>().ToTable("UserRole");
                modelBuilder.Entity<CustomUserRole>().ToTable("CustomUserRole");
                modelBuilder.Entity<FcTable>().ToTable("FcTable");
                modelBuilder.Entity<FcTableLog>().ToTable("FcTableLog");

                modelBuilder.Entity<CustomUserRole>()
                   .HasOne<Role>(f => f.Role)
                   .WithMany(f => f.CustomUserRoles)
                   .HasForeignKey(f => f.RoleId);
                modelBuilder.Entity<RoleMapping>()
                  .HasOne<Role>(f => f.Role)
                  .WithMany(f => f.RoleMappings)
                  .HasForeignKey(f => f.RoleId);

                modelBuilder.Entity<UserRole>()
                  .HasOne<Role>(f => f.Role)
                  .WithMany(f => f.UserRoles)
                  .HasForeignKey(f => f.RoleId);
            }
            base.OnModelCreating(modelBuilder);
        }
    }
}