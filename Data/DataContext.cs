using Boardology.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boardology.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base (options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Upvote> Upvotes { get; set; }
        public DbSet<Downvote> Downvotes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Upvote>()
                .HasKey(k => new { k.UpVoterId, k.GameId });

            builder.Entity<Upvote>()
                .HasOne(u => u.Game)
                .WithMany(u => u.Upvoters)
                .HasForeignKey(u => u.UpVoterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Downvote>()
                .HasKey(k => new { k.DownVoterId, k.GameId });

            builder.Entity<Downvote>()
                .HasOne(u => u.Game)
                .WithMany(u => u.Downvoters)
                .HasForeignKey(u => u.DownVoterId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
