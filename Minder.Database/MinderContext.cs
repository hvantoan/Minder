using Microsoft.EntityFrameworkCore;
using Minder.Database.Models;
using System.Reflection;

namespace Minder.Database {

    public partial class MinderContext : DbContext {
        public virtual DbSet<Permission> Permissions => Set<Permission>();
        public virtual DbSet<Role> Roles => Set<Role>();
        public virtual DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        public virtual DbSet<User> Users => Set<User>();

        public MinderContext() {
        }

        public MinderContext(DbContextOptions<MinderContext> options) : base(options) {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if (!optionsBuilder.IsConfigured) {
                optionsBuilder.UseNpgsql("");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}