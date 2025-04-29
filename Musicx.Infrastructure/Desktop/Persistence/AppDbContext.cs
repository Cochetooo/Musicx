using Microsoft.EntityFrameworkCore;
using Musicx.Domain.Models;

namespace Musicx.Infrastructure.Persistence;

/// <summary>
/// Database context for local songs.
/// </summary>
/// <since>0.6.0</since>
internal sealed class AppDbContext : DbContext
{
    public DbSet<Album> Albums { get; set; }
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Label> Labels { get; set; }
    public DbSet<Release> Releases { get; set; }
    public DbSet<Song> Songs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlite($"Data Source={GetDatabasePath()}");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Artist>()
            .HasDiscriminator<string>("Discriminator")
            .HasValue<BandArtist>("Band")
            .HasValue<PersonArtist>("Person");
    }
    
    private static string GetDatabasePath()
    {
        var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Musicx");
        Directory.CreateDirectory(folderPath);
        return Path.Combine(folderPath, "musicx.db");
    }
}