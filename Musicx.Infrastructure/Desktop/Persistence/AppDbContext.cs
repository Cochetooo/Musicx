using Microsoft.EntityFrameworkCore;
using Musicx.Contracts.Dto.Responses;


namespace Musicx.Infrastructure.Desktop.Persistence;

/// <summary>
/// Database context for local songs.
/// </summary>
/// <since>0.6.0</since>
internal sealed class AppDbContext : DbContext
{
    public DbSet<OutAlbum> Albums { get; set; }
    public DbSet<OutArtist> Artists { get; set; }
    public DbSet<OutGenre> Genres { get; set; }
    public DbSet<OutLabel> Labels { get; set; }
    public DbSet<OutRelease> Releases { get; set; }
    public DbSet<OutSong> Songs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //optionsBuilder
        //    .UseSqlite($"Data Source={GetDatabasePath()}");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
    }
    
    private static string GetDatabasePath()
    {
        var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Musicx");
        Directory.CreateDirectory(folderPath);
        return Path.Combine(folderPath, "musicx.db");
    }
}