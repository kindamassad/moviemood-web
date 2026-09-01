using Microsoft.EntityFrameworkCore;
using MovieMood.Models;

namespace MovieMood.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserList> UserLists => Set<UserList>();
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<MovieGenre> MovieGenres => Set<MovieGenre>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<CategoryMovie> CategoryMovies => Set<CategoryMovie>();
    public DbSet<CastMember> CastMembers => Set<CastMember>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<UserList>(entity =>
        {
            entity.HasIndex(ul => new { ul.UserId, ul.MovieId, ul.ListType, ul.ListName })
                .IsUnique()
                .HasDatabaseName("unique_user_movie_list");

            entity.HasOne(ul => ul.User)
                .WithMany(u => u.UserLists)
                .HasForeignKey(ul => ul.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<Movie>()
                .WithMany()
                .HasForeignKey(ul => ul.MovieId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MovieGenre>(entity =>
        {
            entity.HasKey(mg => new { mg.MovieId, mg.GenreId });
            entity.HasOne(mg => mg.Movie).WithMany(m => m.MovieGenres).HasForeignKey(mg => mg.MovieId);
            entity.HasOne(mg => mg.Genre).WithMany(g => g.MovieGenres).HasForeignKey(mg => mg.GenreId);
        });

        modelBuilder.Entity<CategoryMovie>(entity =>
        {
            entity.HasKey(cm => new { cm.CategoryId, cm.MovieId });
            entity.HasOne(cm => cm.Category).WithMany(c => c.CategoryMovies).HasForeignKey(cm => cm.CategoryId);
            entity.HasOne(cm => cm.Movie).WithMany(m => m.CategoryMovies).HasForeignKey(cm => cm.MovieId);
        });

        modelBuilder.Entity<CastMember>(entity =>
        {
            entity.HasOne(cm => cm.Movie).WithMany(m => m.CastMembers).HasForeignKey(cm => cm.MovieId);
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasOne(m => m.Genre).WithMany().HasForeignKey(m => m.GenreId);
        });
    }
}
