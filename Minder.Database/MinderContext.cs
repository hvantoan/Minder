﻿using Microsoft.EntityFrameworkCore;
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
        public virtual DbSet<TeamRejected> TeamRejecteds => Set<TeamRejected>();

        // Stadium

        public virtual DbSet<Stadium> Stadiums => Set<Stadium>();

        //Chat

        public virtual DbSet<Group> Groups => Set<Group>();
        public virtual DbSet<Participant> Participants => Set<Participant>();
        public virtual DbSet<Message> Messages => Set<Message>();
        public virtual DbSet<Models.File> Files => Set<Models.File>();

        // Match

        public virtual DbSet<Match> Matches => Set<Match>();
        public virtual DbSet<MatchSetting> MatchSettings => Set<MatchSetting>();
        public virtual DbSet<MatchParticipant> MatchParticipants => Set<MatchParticipant>();

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