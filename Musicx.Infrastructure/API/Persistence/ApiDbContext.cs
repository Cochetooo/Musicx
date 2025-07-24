using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Domain.Models;

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
        
        // Déclaration manuelle de la table de jointure
        modelBuilder.Entity<AlbumGenre>()
            .HasKey(ag => new { ag.AlbumId, ag.GenreId, ag.Level });

        modelBuilder.Entity<AlbumGenre>()
            .HasOne(ag => ag.Album)
            .WithMany()
            .HasForeignKey(ag => ag.AlbumId);

        modelBuilder.Entity<AlbumGenre>()
            .HasOne(ag => ag.Genre)
            .WithMany()
            .HasForeignKey(ag => ag.GenreId);

        // PRIMARY genres
        modelBuilder.Entity<Album>()
            .HasMany(a => a.PrimaryGenres)
            .WithMany()
            .UsingEntity<AlbumGenre>(
                j => j
                    .HasOne(ag => ag.Genre)
                    .WithMany()
                    .HasForeignKey(ag => ag.GenreId),
                j => j
                    .HasOne(ag => ag.Album)
                    .WithMany()
                    .HasForeignKey(ag => ag.AlbumId),
                j =>
                {
                    j.ToTable("Album_Genre");
                    j.HasKey(ag => new { ag.AlbumId, ag.GenreId, ag.Level });
                    j.HasQueryFilter(ag => ag.Level == 0);
                });

        // INFLUENCE genres
        modelBuilder.Entity<Album>()
            .HasMany(a => a.InfluenceGenres)
            .WithMany()
            .UsingEntity<AlbumGenre>(
                j => j
                    .HasOne(ag => ag.Genre)
                    .WithMany()
                    .HasForeignKey(ag => ag.GenreId),
                j => j
                    .HasOne(ag => ag.Album)
                    .WithMany()
                    .HasForeignKey(ag => ag.AlbumId),
                j =>
                {
                    j.ToTable("Album_Genre");
                    j.HasKey(ag => new { ag.AlbumId, ag.GenreId, ag.Level });
                    j.HasQueryFilter(ag => ag.Level == 1);
                });
        
        // Déclaration manuelle de la table de jointure
        modelBuilder.Entity<SongGenre>()
            .HasKey(ag => new { ag.SongId, ag.GenreId, ag.Level });

        modelBuilder.Entity<SongGenre>()
            .HasOne(ag => ag.Song)
            .WithMany()
            .HasForeignKey(ag => ag.SongId);

        modelBuilder.Entity<SongGenre>()
            .HasOne(ag => ag.Genre)
            .WithMany()
            .HasForeignKey(ag => ag.GenreId);

        // PRIMARY genres
        modelBuilder.Entity<Song>()
            .HasMany(a => a.PrimaryGenres)
            .WithMany()
            .UsingEntity<SongGenre>(
                j => j
                    .HasOne(ag => ag.Genre)
                    .WithMany()
                    .HasForeignKey(ag => ag.GenreId),
                j => j
                    .HasOne(ag => ag.Song)
                    .WithMany()
                    .HasForeignKey(ag => ag.SongId),
                j =>
                {
                    j.ToTable("Song_Genre");
                    j.HasKey(ag => new { ag.SongId, ag.GenreId, ag.Level });
                    j.HasQueryFilter(ag => ag.Level == 0);
                });

        // INFLUENCE genres
        modelBuilder.Entity<Song>()
            .HasMany(a => a.InfluenceGenres)
            .WithMany()
            .UsingEntity<SongGenre>(
                j => j
                    .HasOne(ag => ag.Genre)
                    .WithMany()
                    .HasForeignKey(ag => ag.GenreId),
                j => j
                    .HasOne(ag => ag.Song)
                    .WithMany()
                    .HasForeignKey(ag => ag.SongId),
                j =>
                {
                    j.ToTable("Song_Genre");
                    j.HasKey(ag => new { ag.SongId, ag.GenreId, ag.Level });
                    j.HasQueryFilter(ag => ag.Level == 1);
                });
        
        modelBuilder.Entity<Genre>()
            .Ignore(g => g.ParentIds)
            .Ignore(g => g.ChildIds);
    }
}