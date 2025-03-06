using System.IO;
using Microsoft.EntityFrameworkCore;
using MusicxApi.Models;

namespace Musicx.Database;

public class AppDbContext : DbContext
{
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Album> Albums { get; set; }
    public DbSet<Song> Songs { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Label> Labels { get; set; }

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
}