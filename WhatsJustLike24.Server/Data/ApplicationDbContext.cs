using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using WhatsJustLike24.Server.Data.DTOs;
using WhatsJustLike24.Server.Data.Models;
using WhatsJustLike24.Server.Data.Identity;

namespace WhatsJustLike24.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext() : base()
        { }

        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Explicitly configure the many-to-many relationship
            modelBuilder.Entity<MovieIsLike>()
                .HasOne<Movie>(mil => mil.MovieA)
                .WithMany(m => m.IsLikeConnections)
                .HasForeignKey(mil => mil.MovieIdA)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            modelBuilder.Entity<MovieIsLike>()
                .HasOne<Movie>(mil => mil.MovieB)
                .WithMany() // No need to define this side if you don't have a navigation property back
                .HasForeignKey(mil => mil.MovieIdB)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // Configure the one-to-one relationship between Movie and MovieDetails
            modelBuilder.Entity<Movie>()
                .HasOne(m => m.MovieDetails)
                .WithOne(md => md.Movie)
                .HasForeignKey<MovieDetails>(md => md.MovieId);

            // Configure the one-to-many relationship between MovieIsLike and IsLikeDetails
            modelBuilder.Entity<MovieIsLike>()
                .HasMany(mil => mil.IsLikeDetails)
                .WithOne(ild => ild.MovieIsLike)
                .HasForeignKey(ild => ild.MovieIsLikeId);

            modelBuilder.Entity<SimilarityByTitleDTO>().HasNoKey().ToView(null);
        }


        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieDetails> MovieDetails { get; set; }
        public DbSet<MovieIsLike> MovieIsLike { get; set; }
        public DbSet<IsLikeDetails> IsLikeDetails { get; set; }

        public DbSet<AppUser> AppUsers { get; set; }

    }
}
