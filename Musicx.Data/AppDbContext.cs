using Microsoft.EntityFrameworkCore;
using Musicx.Data.Entities;

namespace Musicx.Data;

/// <summary>
/// 
/// </summary>
/// <since>0.6.0</since>
public class AppDbContext : DbContext
{
    public DbSet<ArtistEntity> Artists { get; set; }
    public DbSet<AlbumEntity> Albums { get; set; }
    public DbSet<SongEntity> Songs { get; set; }
    public DbSet<GenreEntity> Genres { get; set; }
    public DbSet<LabelEntity> Labels { get; set; }

    private static string GetDatabasePath()
    {
        string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Musicx");
        Directory.CreateDirectory(folderPath);
        return Path.Combine(folderPath, "musicx.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseLazyLoadingProxies()
            .UseSqlite($"Data Source={GetDatabasePath()}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ArtistEntity>()
            .HasDiscriminator<string>("Discriminator")
            .HasValue<BandArtistEntity>("Band")
            .HasValue<PersonArtistEntity>("Person");
    }
}