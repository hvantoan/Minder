using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minder.Database.Enums;

namespace Minder.Database.Models {

    public class RegistrationInformation {
        public string Id { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string OTPCode { get; set; } = null!;
        public EVerifyType Type { get; set; }
        public DateTimeOffset CreateAt { get; set; }

        public virtual User? User { get; set; }
    }

    public class RegistrationInformationConfig : IEntityTypeConfiguration<RegistrationInformation> {

        public void Configure(EntityTypeBuilder<RegistrationInformation> builder) {
            builder.ToTable(nameof(RegistrationInformation));

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasMaxLength(32).IsRequired();
            builder.Property(o => o.Username).IsRequired();
            builder.Property(o => o.OTPCode).HasMaxLength(6).IsRequired();
            builder.Property(o => o.CreateAt).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();
        }
    }
}