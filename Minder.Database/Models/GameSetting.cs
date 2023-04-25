using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minder.Database.Enums;

namespace Minder.Database.Models {

    public class GameSetting {
        public string Id { get; set; } = null!;
        public string? UserId { get; set; }
        public string? TeamId { get; set; }
        public string GameTypes { get; set; } = string.Empty;
        public string? GameTime { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public double Radius { get; set; }
        public string Positions { get; set; } = string.Empty;

        public ERank Rank { get; set; }
        public int Point { get; set; }

        public virtual User? User { get; set; }
        public virtual Team? Team { get; set; }
    }

    public class GameSettingConfig : IEntityTypeConfiguration<GameSetting> {

        public void Configure(EntityTypeBuilder<GameSetting> builder) {
            builder.ToTable(nameof(GameSetting));

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasMaxLength(32);
            builder.Property(o => o.UserId).HasMaxLength(32);

            builder.Property(o => o.Longitude).HasColumnType("decimal(18,15)");
            builder.Property(o => o.Latitude).HasColumnType("decimal(18,15)");

            builder.HasOne(o => o.User).WithOne(o => o.GameSetting).HasForeignKey<GameSetting>(o => o.UserId);
            builder.HasOne(o => o.Team).WithOne(o => o.GameSetting).HasForeignKey<GameSetting>(o => o.TeamId);
        }
    }
}