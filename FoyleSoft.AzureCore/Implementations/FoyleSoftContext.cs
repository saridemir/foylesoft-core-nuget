
using FoyleSoft.AzureCore.Extensions;
using FoyleSoft.Core.Implementations;
using FoyleSoft.Core.Implementations.Data;
using FoyleSoft.Core.Implementations.Data.Tables;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FoyleSoft.AzureCore.Implementations
{
    public abstract class FoyleSoftContext : DbContext
    {
        private Assembly _contextAssembly;
        private bool _hasDefaultTable;
        public FoyleSoftContext(DbContextOptions options, bool hasDefaultTable = true)
           : base(options)
        {
            _hasDefaultTable = hasDefaultTable;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet(null, DelegationModes.ApplyToColumns);
            if (_hasDefaultTable)
            {
                CreateModel<SystemUser>(modelBuilder, "SystemUser");
                CreateModel<Role>(modelBuilder, "Role");
                CreateModel<RoleMapping>(modelBuilder, "RoleMapping");
                CreateModel<UserRole>(modelBuilder, "UserRole");
                CreateModel<CustomUserRole>(modelBuilder, "CustomUserRole");

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
            var list = _contextAssembly.GetTypes().Where(f => f.IsClass
            && typeof(IBaseTable).IsAssignableFrom(f) && (f.BaseType.Name == "DefaultTable" || f.BaseType.Name == "BaseTable")
            && !f.FullName.Contains(".System.")).ToList();
            list.ForEach(f =>
            {
                MethodInfo method = typeof(FoyleSoftContext).GetMethod(nameof(FoyleSoftContext.CreateModel),
                       BindingFlags.Public | BindingFlags.Instance);
                method = method.MakeGenericMethod(f);
                method.Invoke(this, new object[] { modelBuilder, f.Name });
            });
        }

        public abstract void CreateModel<T>(ModelBuilder modelBuilder, string tableName) where T : class;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _contextAssembly = optionsBuilder.GetAssembly();
            base.OnConfiguring(optionsBuilder);
        }
    }
}
