using Microsoft.EntityFrameworkCore;
using Minder.Database.Models;
using System.Reflection;

namespace Minder.Database {

    public partial class MinderContext : DbContext {
        // Permission

        public virtual DbSet<Role> Roles => Set<Role>();
        public virtual DbSet<Permission> Permissions => Set<Permission>();
        public virtual DbSet<RolePermission> RolePermissions => Set<RolePermission>();

        // Game setting

        public virtual DbSet<GameTime> GameTime => Set<GameTime>();


        //User

        public virtual DbSet<User> Users => Set<User>();
        public virtual DbSet<RegistrationInformation> RegistrationInformations => Set<RegistrationInformation>();
        public virtual DbSet<GameSetting> GameSettings => Set<GameSetting>();

        //Team

        public virtual DbSet<Team> Teams => Set<Team>();
        public virtual DbSet<Member> Members => Set<Member>();
        public virtual DbSet<Invitation> Invites => Set<Invitation>();

        // Stadium

        public virtual DbSet<Stadium> Stadiums => Set<Stadium>();

        //Chat

        public virtual DbSet<Conversation> Conversations => Set<Conversation>();
        public virtual DbSet<Participant> Participants => Set<Participant>();
        public virtual DbSet<Message> Messages => Set<Message>();
        public virtual DbSet<Models.File> Files => Set<Models.File>();

        public MinderContext() {
        }

        public MinderContext(DbContextOptions<MinderContext> options) : base(options) {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if (!optionsBuilder.IsConfigured) {
                optionsBuilder.UseSqlServer("");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}