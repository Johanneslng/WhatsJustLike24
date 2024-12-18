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


            // Configure the relationships for Games

            // Many-to-Many relationship for Games
            modelBuilder.Entity<GameIsLike>()
                .HasOne<Game>(gil => gil.GameA)
                .WithMany(g => g.IsLikeConnections)
                .HasForeignKey(gil => gil.GameIdA)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            modelBuilder.Entity<GameIsLike>()
                .HasOne<Game>(gil => gil.GameB)
                .WithMany() // No need to define this side if you don't have a navigation property back
                .HasForeignKey(gil => gil.GameIdB)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // One-to-One relationship between Game and GameDetails
            modelBuilder.Entity<Game>()
                .HasOne(g => g.GameDetails)
                .WithOne(gd => gd.Game)
                .HasForeignKey<GameDetails>(gd => gd.GameId);

            // One-to-Many relationship between GameIsLike and GameIsLikeDetails
            modelBuilder.Entity<GameIsLike>()
                .HasMany(gil => gil.GameIsLikeDetails)
                .WithOne(gild => gild.GameIsLike)
                .HasForeignKey(gild => gild.GameIsLikeId);


            // Configure the relationships for Books

            // Many-to-Many relationship for Books
            modelBuilder.Entity<BookIsLike>()
                .HasOne<Book>(bil => bil.BookA)
                .WithMany(b => b.IsLikeConnections)
                .HasForeignKey(bil => bil.BookIdA)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            modelBuilder.Entity<BookIsLike>()
                .HasOne<Book>(bil => bil.BookB)
                .WithMany() // No need to define this side if you don't have a navigation property back
                .HasForeignKey(bil => bil.BookIdB)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // One-to-One relationship between Book and BookDetails
            modelBuilder.Entity<Book>()
                .HasOne(b => b.BookDetails)
                .WithOne(bd => bd.Book)
                .HasForeignKey<BookDetails>(bd => bd.BookId);

            // One-to-Many relationship between BookIsLike and BookIsLikeDetails
            modelBuilder.Entity<BookIsLike>()
                .HasMany(bil => bil.BookIsLikeDetails)
                .WithOne(bild => bild.BookIsLike)
                .HasForeignKey(bild => bild.BookIsLikeId);


            // Configure the relationships for Shows

            // Many-to-Many relationship for Shows
            modelBuilder.Entity<ShowIsLike>()
                .HasOne<Show>(sil => sil.ShowA)
                .WithMany(s => s.IsLikeConnections)
                .HasForeignKey(sil => sil.ShowIdA)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            modelBuilder.Entity<ShowIsLike>()
                .HasOne<Show>(sil => sil.ShowB)
                .WithMany() // No need to define this side if you don't have a navigation property back
                .HasForeignKey(sil => sil.ShowIdB)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // One-to-One relationship between Book and BookDetails
            modelBuilder.Entity<Show>()
                .HasOne(s => s.ShowDetails)
                .WithOne(sd => sd.Show)
                .HasForeignKey<ShowDetails>(sd => sd.ShowId);

            // One-to-Many relationship between BookIsLike and BookIsLikeDetails
            modelBuilder.Entity<ShowIsLike>()
                .HasMany(sil => sil.ShowIsLikeDetails)
                .WithOne(sild => sild.ShowIsLike)
                .HasForeignKey(sild => sild.ShowIsLikeId);

            //modelBuilder.Entity<SimilarityByTitleDTO>().HasNoKey().ToView(null);

            modelBuilder.Entity<SimilarityByTitleDTO>().HasNoKey();

            modelBuilder.HasDbFunction(() => GetMovieSimilarityDetails(default!))
            .HasName("GetMovieSimilarityDetails")
            .HasSchema("dbo");

            modelBuilder.HasDbFunction(() => GetGameSimilarityDetails(default!))
            .HasName("GetGameSimilarityDetails")
            .HasSchema("dbo");

            modelBuilder.HasDbFunction(() => GetBookSimilarityDetails(default!))
            .HasName("GetBookSimilarityDetails")
            .HasSchema("dbo");

            modelBuilder.HasDbFunction(() => GetShowSimilarityDetails(default!))
            .HasName("GetShowSimilarityDetails")
            .HasSchema("dbo");


        }

        public IQueryable<SimilarityByTitleDTO> GetMovieSimilarityDetails(string title)
        {
            return FromExpression(() => GetMovieSimilarityDetails(title));
        }

        public IQueryable<SimilarityByTitleDTO> GetGameSimilarityDetails(string title)
        {
            return FromExpression(() => GetGameSimilarityDetails(title));
        }

        public IQueryable<SimilarityByTitleDTO> GetBookSimilarityDetails(string title)
        {
            return FromExpression(() => GetBookSimilarityDetails(title));
        }

        public IQueryable<SimilarityByTitleDTO> GetShowSimilarityDetails(string title)
        {
            return FromExpression(() => GetShowSimilarityDetails(title));
        }

        // DbSet properties for Movies
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieDetails> MovieDetails { get; set; }
        public DbSet<MovieIsLike> MovieIsLike { get; set; }
        public DbSet<IsLikeDetails> IsLikeDetails { get; set; }

        // DbSet properties for Games
        public DbSet<Game> Games { get; set; }
        public DbSet<GameDetails> GameDetails { get; set; }
        public DbSet<GameIsLike> GameIsLike { get; set; }
        public DbSet<GameIsLikeDetails> GameIsLikeDetails { get; set; }

        // DbSet properties for Books
        public DbSet<Book> Books { get; set; }
        public DbSet<BookDetails> BookDetails { get; set; }
        public DbSet<BookIsLike> BookIsLike { get; set; }
        public DbSet<BookIsLikeDetails> BookIsLikeDetails { get; set; }

        // DbSet properties for Shows
        public DbSet<Show> Shows { get; set; }
        public DbSet<ShowDetails> ShowDetails { get; set; }
        public DbSet<ShowIsLike> ShowIsLike { get; set; }
        public DbSet<ShowIsLikeDetails> ShowIsLikeDetails { get; set; }

        public DbSet<AppUser> AppUsers { get; set; }

    }
}
