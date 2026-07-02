using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<PostTag> PostTags { get; set; }
    public DbSet<PostLike> PostLikes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(x => x.Id);
            //entity.HasIndex(x => x.Email);
            entity.OwnsOne(x => x.Email, email =>
            {
                email.Property(e => e.Value).HasColumnName("UserEmail").IsRequired();
            });
            entity.OwnsOne(t => t.Password, password =>
            {
                password.Property(p => p.Hash).HasColumnName("Hash").IsRequired();
                password.Property(p => p.Salt).HasColumnName("Salt").IsRequired();
            });
            entity.Property(p => p.FullName).IsRequired().HasMaxLength(100);
            entity.HasQueryFilter(q => !q.IsDeleted); // TODO: Not suitable for admin pannel (IgnoreQueryFilters)
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.OwnsOne(x => x.PostContent, content =>
            {
                content.Property(p => p.Value).HasColumnName("Content").IsRequired();
            });
            entity.Property(p => p.PostTitle).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Status).HasConversion<string>();
            entity.HasOne(t => t.Author)
            .WithMany(t => t.Posts)
            .HasForeignKey(t => t.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        });

        modelBuilder.Entity<PostLike>(entity =>
        {
            entity.HasKey(t => t.Id);

            entity.HasOne(t => t.User)
            .WithMany(t => t.LikedPosts)
            .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Post)
            .WithMany(t => t.LikedBy)
            .HasForeignKey(t => t.PostId)
            .OnDelete(DeleteBehavior.Restrict);

        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.HasOne(t => t.User)
            .WithMany(t => t.Comments)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(t => t.Post)
            .WithMany(t => t.Comments)
            .HasForeignKey(t => t.PostId)
            .OnDelete(DeleteBehavior.Restrict);

        });

        modelBuilder.Entity<PostTag>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.HasOne(t => t.Post)
            .WithMany(t => t.PostTags)
            .HasForeignKey(t => t.PostId)
            .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(t => t.Tag)
            .WithMany(t => t.PostTags)
            .HasForeignKey(t => t.TagId)
            .OnDelete(DeleteBehavior.Restrict);
        });

        base.OnModelCreating(modelBuilder);
    }


}
