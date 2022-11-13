using Microsoft.EntityFrameworkCore;
using Minder.Database.Models;
using System.Reflection;

namespace Minder.Database {
    public partial class MinderContext : DbContext {
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<User> Users { get; set; }

        public MinderContext() {
        }

        public MinderContext(DbContextOptions<MinderContext> options) : base(options) {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if (!optionsBuilder.IsConfigured) {
                optionsBuilder.UseNpgsql("Server=ec2-54-160-109-68.compute-1.amazonaws.com;Port=5432;Database=d3ih7n6m8eu2e6;User Id=ucwxvrzcbbyzkj;Password=8f102d41495102fbf474ddf0e57b399548db14d4a1636a92f158835f653f50e8;sslmode=Require;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
