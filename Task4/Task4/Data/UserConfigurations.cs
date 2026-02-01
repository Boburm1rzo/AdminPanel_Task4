using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task4.Models;

namespace Task4.Data;

public class UserConfigurations : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder
            .Property(u => u.Name)
            .HasMaxLength(256)
            .IsRequired();

        builder
            .Property(u => u.Email)
            .HasMaxLength(256)
            .UseCollation("SQL_Latin1_General_CP1_CI_AS")
            .IsRequired();

        builder
            .Property(u => u.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
    }
}
