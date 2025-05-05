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
        modelBuilder.Entity<Artist>()
            .HasDiscriminator<string>("Discriminator")
            .HasValue<Artist>("Artist")
            .HasValue<BandArtist>("Band")
            .HasValue<PersonArtist>("Person");
    }
}