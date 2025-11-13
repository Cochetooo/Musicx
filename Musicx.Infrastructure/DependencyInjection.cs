using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Auth;
using Musicx.Application.Api.Interfaces.Workers;
using Musicx.Application.Api.Services.Auth;
using Musicx.Application.Desktop.Interfaces.Persistence;
using Musicx.Application.Desktop.Interfaces.UseCases.LocalLibrary;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Application.Shared.Interfaces.Providers.ExternalMusicData;
using Musicx.Application.Shared.Interfaces.UseCases.ExternalMusicData;
using Musicx.Application.Web.Interfaces.Models.Auth;
using Musicx.Application.Web.Interfaces.UseCases;
using Musicx.Application.Web.Interfaces.UseCases.Specifics;
using Musicx.Contracts.Dto.Requests;
using Musicx.Infrastructure.API.Auth;
using Musicx.Infrastructure.API.Persistence;
using Musicx.Infrastructure.API.Persistence.Builders;
using Musicx.Infrastructure.API.Workers;
using Musicx.Infrastructure.Desktop.Persistence;
using Musicx.Infrastructure.Desktop.Persistence.Caches;
using Musicx.Infrastructure.Desktop.Persistence.Repositories;
using Musicx.Infrastructure.Desktop.Services.LocalLibrary;
using Musicx.Infrastructure.Shared.Logging;
using Musicx.Infrastructure.Shared.Providers.ExternalMusicData;
using Musicx.Infrastructure.Shared.UseCases.ExternalMusicData;
using Musicx.Infrastructure.Web.UseCases;
using Musicx.Infrastructure.Web.UseCases.Specifics;

namespace Musicx.Infrastructure;

public static class DependencyInjection
{
    public static ILoggingBuilder AddLog4Net(this ILoggingBuilder builder, string configFile = "log4net.config")
    {
        builder.AddProvider(new Log4NetLoggerProvider(configFile));
        return builder;
    }
    
    /// <summary>
    /// Add common module services and use cases.
    /// </summary>
    public static IServiceCollection AddMusicxInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Providers
        services.AddScoped<HttpClient>();

        services.AddScoped<IExternalMusicDataProvider, LastFmApiProvider>();
        services.AddScoped<IExternalMusicDataProvider, DeezerApiProvider>();
        services.AddScoped<IExternalMusicDataProvider, ItunesApiProvider>();

        services.AddScoped<ExternalMusicDataService>();

        // Use cases
        services.AddScoped<IFetchArtistInfoUseCase, UcFetchArtistInfo>();
        services.AddScoped<IFetchAlbumInfoUseCase, UcFetchAlbumInfo>();
        
        return services;
    }
    
    /// <summary>
    /// Add desktop module services and use cases.
    /// </summary>
    public static IServiceCollection AddMusicxDesktop(this IServiceCollection services)
    {
        // Database
        services.AddDbContext<DbContext, AppDbContext>();
        
        // Caches
        services.AddScoped<ISongCache, SongCache>();
        services.AddScoped<IAlbumCache, AlbumCache>();
        services.AddScoped<IArtistCache, ArtistCache>();
        services.AddScoped<IReleaseCache, ReleaseCache>();
        services.AddScoped<ILabelCache, LabelCache>();
        services.AddScoped<IGenreCache, GenreCache>();
        
        // Repositories
        
        services.AddScoped<IBatchImportRepository, BatchImportRepository>();
        
        // Use cases
        services.AddScoped<IReadAudioFileUseCase, UcReadAudioFile>();
        
        return services;
    }
    
    /// <summary>
    /// Add Api module services and use cases.
    /// </summary>
    public static IServiceCollection AddMusicxApi(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        services.AddSingleton<IDbConnectionProvider, NpgsqlConnectionProvider>();
        
        // SQL Builder
        services.AddScoped(typeof(SqlBuilder<InArtist>), typeof(ArtistSqlBuilder));
        services.AddScoped(typeof(SqlBuilder<InAlbum>), typeof(AlbumSqlBuilder));
        services.AddScoped(typeof(SqlBuilder<InGenre>), typeof(GenreSqlBuilder));
        services.AddScoped(typeof(SqlBuilder<InSong>), typeof(SongSqlBuilder));
        services.AddScoped(typeof(SqlBuilder<InPermission>), typeof(PermissionSqlBuilder));
        services.AddScoped(typeof(SqlBuilder<InRole>), typeof(RoleSqlBuilder));
        services.AddScoped(typeof(SqlBuilder<InTag>), typeof(TagSqlBuilder));
        services.AddScoped(typeof(SqlBuilder<InUser>), typeof(UserSqlBuilder));
        services.AddScoped(typeof(SqlBuilder<InUserAlbumAttribute>), typeof(UserAlbumAttrSqlBuilder));

        // Repositories
        services.AddScoped<Application.Api.Interfaces.Persistence.ISongRepository, API.Persistence.Repositories.SongRepository>();
        services.AddScoped<Application.Api.Interfaces.Persistence.IAlbumRepository, API.Persistence.Repositories.AlbumRepository>();
        services.AddScoped<Application.Api.Interfaces.Persistence.IArtistRepository, API.Persistence.Repositories.ArtistRepository>();
        services.AddScoped<Application.Api.Interfaces.Persistence.IGenreRepository, API.Persistence.Repositories.GenreRepository>();
        services.AddScoped<Application.Api.Interfaces.Persistence.IPermissionRepository, API.Persistence.Repositories.PermissionRepository>();
        services.AddScoped<Application.Api.Interfaces.Persistence.IRoleRepository, API.Persistence.Repositories.RoleRepository>();
        services.AddScoped<Application.Api.Interfaces.Persistence.ITagRepository, API.Persistence.Repositories.TagRepository>();
        services.AddScoped<Application.Api.Interfaces.Persistence.IUserRepository, API.Persistence.Repositories.UserRepository>();
        services.AddScoped<Application.Api.Interfaces.Persistence.IUserAlbumAttrsRepository, API.Persistence.Repositories.UserAlbumAttrRepository>();
        
        // Audit
        services.AddScoped<IAuditPublisher, RabbitMqAuditPublisher>();
        
        // Auth
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
        services.AddScoped<ITokenValidator, JwtTokenValidator>();
        
        services.AddScoped<IAuthService, AuthService>();

        services.AddMusicxWeb();
        
        return services;
    }
    
    /// <summary>
    /// Add web module services and use cases.
    /// </summary>
    public static IServiceCollection AddMusicxWeb(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGetUseCase<>), typeof(UcGet<>));
        services.AddScoped(typeof(ISaveUseCase<>), typeof(UcSave<>));
        services.AddScoped(typeof(ISaveAllUseCase<>), typeof(UcSaveAll<>));
        services.AddScoped(typeof(IDeleteUseCase<>), typeof(UcDelete<>));
        services.AddScoped(typeof(IListUseCase<>), typeof(UcList<>));
        services.AddScoped(typeof(IFindInUseCase<>), typeof(UcFindIn<>));
        services.AddScoped(typeof(ICountUseCase<>), typeof(UcCount<>));
        
        services.AddScoped<IGetAlbumAttributesByAlbum, UcGetAlbumAttrByAlbum>();
        services.AddScoped<IGetAlbumAttributesByUser, UcGetAlbumAttrByUser>();
        services.AddScoped<IGetAlbumByArtistUseCase, UcGetAlbumByArtist>();
        services.AddScoped<IGetAlbumByChartUseCase, UcGetAlbumByChart>();
        services.AddScoped<IGetAlbumByGenreUseCase, UcGetAlbumByGenre>();
        services.AddScoped<IGetSongByAlbumUseCase, UcGetSongByAlbum>();
        services.AddScoped(typeof(IGetRatingDistribByUser<>), typeof(UcGetRatingDistribByUser<>));

        services.AddScoped<IAuthSignInUseCase, UcAuthSignIn>();
        
        return services;
    }
}