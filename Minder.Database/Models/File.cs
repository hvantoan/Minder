using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minder.Database.Enums;
using System;

namespace Minder.Database.Models {

    public partial class File {
        public string Id { get; set; } = null!;
        public EFile Type { get; set; }
        public string? ItemId { get; set; }
        public string Name { get; set; } = null!;
        public string Path { get; set; } = null!;
        public string UploadBy { get; set; } = null!;
        public DateTimeOffset UploadDate { get; set; } = DateTimeOffset.Now;
        public virtual User? User { get; set; }
    }

    public class FileConfig : IEntityTypeConfiguration<File> {

        public void Configure(EntityTypeBuilder<File> builder) {
            builder.ToTable(nameof(File));

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasMaxLength(32);

            builder.Property(o => o.Type).HasMaxLength(20);
            builder.Property(o => o.ItemId).HasMaxLength(32);
            builder.Property(o => o.Name).HasMaxLength(255).IsRequired();
            builder.Property(o => o.Path).HasMaxLength(8000);
            builder.Property(o => o.UploadBy).HasMaxLength(32).IsRequired();
            builder.Property(o => o.UploadDate).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();

            //fk
            builder.HasOne(o => o.User).WithMany(o => o.Files).HasForeignKey(o => o.UploadBy);

            // index
            builder.HasIndex(o => o.Type);
        }
    }
}