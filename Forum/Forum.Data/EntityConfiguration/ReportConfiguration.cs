﻿namespace Forum.Data.EntityConfiguration
{
    using Forum.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ReportConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder
                .HasKey(r => r.Id);

            builder
                .HasOne(r => r.Post)
                .WithMany(p => p.Reports)
                .HasForeignKey(r => r.PostId);

            builder
                .HasOne(r => r.User)
                .WithMany(u => u.Reports)
                .HasForeignKey(r => r.UserId);
        }
    }
}