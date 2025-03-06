// Infrastructure/Data/Configurations/CandidateConfiguration.cs
using InterviewSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InterviewSystem.Infrastructure.Data.Configurations;

public class CandidateConfiguration : IEntityTypeConfiguration<Candidate>
{
    public void Configure(EntityTypeBuilder<Candidate> builder)
    {
        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(c => new { c.InterviewId, c.Email })
            .IsUnique()
            .HasFilter("([IsDeleted] = 0)");

        builder.Property(c => c.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(c => c.Interview)
            .WithMany(i => i.Candidates)
            .HasForeignKey(c => c.InterviewId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}