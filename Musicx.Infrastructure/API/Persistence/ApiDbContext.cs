using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Musicx.Application.Shared.Interfaces.Common;


namespace Musicx.Infrastructure.API.Persistence;

public sealed class ApiDbContext(DbContextOptions<ApiDbContext> options) : DbContext(options)
{
    public DbSet<Album> Albums { get; set; }
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Label> Labels { get; set; }
    public DbSet<Release> Releases { get; set; }
    public DbSet<Song> Songs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Artist>()
            .HasDiscriminator<string>("Discriminator")
            .HasValue<Artist>("Artist")
            .HasValue<BandArtist>("Band")
            .HasValue<PersonArtist>("Person");
        
        // 🎵 Album - Primary Genres
        modelBuilder.Entity<Album>()
            .HasMany(a => a.PrimaryGenres)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "Album_Genre",
                j => j
                    .HasOne<Genre>()
                    .WithMany()
                    .HasForeignKey("GenreId")
                    .OnDelete(DeleteBehavior.Restrict),
                j => j
                    .HasOne<Album>()
                    .WithMany()
                    .HasForeignKey("AlbumId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j.ToTable("Album_Genre")
            );

        // 🎵 Album - Influence Genres
        modelBuilder.Entity<Album>()
            .HasMany(a => a.InfluenceGenres)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "Album_Influence",
                j => j
                    .HasOne<Genre>()
                    .WithMany()
                    .HasForeignKey("GenreId")
                    .OnDelete(DeleteBehavior.Restrict),
                j => j
                    .HasOne<Album>()
                    .WithMany()
                    .HasForeignKey("AlbumId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j.ToTable("Album_Influence")
            );

        // 🎶 Song - Primary Genres
        modelBuilder.Entity<Song>()
            .HasMany(s => s.PrimaryGenres)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "Song_Genre",
                j => j
                    .HasOne<Genre>()
                    .WithMany()
                    .HasForeignKey("GenreId")
                    .OnDelete(DeleteBehavior.Restrict),
                j => j
                    .HasOne<Song>()
                    .WithMany()
                    .HasForeignKey("SongId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j.ToTable("Song_Genre")
            );

        // 🎶 Song - Influence Genres
        modelBuilder.Entity<Song>()
            .HasMany(s => s.InfluenceGenres)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "Song_Influence",
                j => j
                    .HasOne<Genre>()
                    .WithMany()
                    .HasForeignKey("GenreId")
                    .OnDelete(DeleteBehavior.Restrict),
                j => j
                    .HasOne<Song>()
                    .WithMany()
                    .HasForeignKey("SongId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j.ToTable("Song_Influence")
            );

        modelBuilder.Entity<Genre>()
            .HasMany(g => g.Parents)
            .WithMany(g => g.Children)
            .UsingEntity<Dictionary<string, object>>(
                "ChildrenGenre_ParentGenre",
                j => j.HasOne<Genre>()
                    .WithMany()
                    .HasForeignKey("ParentId")
                    .OnDelete(DeleteBehavior.Restrict),
                j => j.HasOne<Genre>()
                    .WithMany()
                    .HasForeignKey("ChildrenId")
                    .OnDelete(DeleteBehavior.Restrict),
                j =>
                {
                    j.HasKey("ParentId", "ChildrenId");
                    j.ToTable("ChildrenGenre_ParentGenre");
                }
            );
    }
}