// Infrastructure/Data/Configurations/InterviewConfiguration.cs
using InterviewSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InterviewSystem.Infrastructure.Data.Configurations;

public class InterviewConfiguration : IEntityTypeConfiguration<Interview>
{
    public void Configure(EntityTypeBuilder<Interview> builder)
    {
        builder.Property(i => i.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.Question)
            .IsRequired();

        builder.Property(i => i.Details)
            .IsRequired();

        builder.Property(i => i.Duration)
            .IsRequired();

        builder.HasOne(i => i.Company)
            .WithMany(c => c.Interviews)
            .HasForeignKey(i => i.FirmId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(i => i.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");
    }
}