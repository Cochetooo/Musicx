using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Auth;
using Musicx.Application.Api.Interfaces.Workers;
using Musicx.Application.Desktop.Interfaces.Persistence;
using Musicx.Application.Desktop.Interfaces.UseCases.LocalLibrary;
using Musicx.Application.Shared.Interfaces.Providers.ExternalMusicData;
using Musicx.Application.Shared.Interfaces.UseCases.ExternalMusicData;
using Musicx.Application.Web.Interfaces.UseCases;
using Musicx.Application.Web.Interfaces.UseCases.Specifics;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Album;
using Musicx.Contracts.Dto.Requests.Artist;
using Musicx.Contracts.Dto.Requests.Event;
using Musicx.Contracts.Dto.Requests.Genre;
using Musicx.Contracts.Dto.Requests.Label;
using Musicx.Contracts.Dto.Requests.Security;
using Musicx.Contracts.Dto.Requests.Song;
using Musicx.Contracts.Dto.Requests.Tag;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Infrastructure.API.Auth;
using Musicx.Infrastructure.API.Persistence;
using Musicx.Infrastructure.API.Persistence.Builders;
using Musicx.Infrastructure.API.Persistence.Builders.Album;
using Musicx.Infrastructure.API.Persistence.Builders.Artist;
using Musicx.Infrastructure.API.Persistence.Builders.Event;
using Musicx.Infrastructure.API.Persistence.Builders.Genre;
using Musicx.Infrastructure.API.Persistence.Builders.Label;
using Musicx.Infrastructure.API.Persistence.Builders.Security;
using Musicx.Infrastructure.API.Persistence.Builders.Song;
using Musicx.Infrastructure.API.Persistence.Builders.Tag;
using Musicx.Infrastructure.API.Persistence.Builders.User;
using Musicx.Infrastructure.API.Persistence.Connection;
using Musicx.Infrastructure.API.Persistence.Repositories;
using Musicx.Infrastructure.API.Persistence.Repositories.Album;
using Musicx.Infrastructure.API.Persistence.Repositories.Artist;
using Musicx.Infrastructure.API.Persistence.Repositories.Genre;
using Musicx.Infrastructure.API.Persistence.Repositories.Security;
using Musicx.Infrastructure.API.Persistence.Repositories.Song;
using Musicx.Infrastructure.API.Persistence.Repositories.Tag;
using Musicx.Infrastructure.API.Persistence.Repositories.User;
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
        services.AddScoped(typeof(SqlBuilder<InAlbumGenre>), typeof(AlbumGenreSqlBuilder));
        services.AddScoped(typeof(SqlBuilder<InAlbumInfluence>), typeof(AlbumInfluenceSqlBuilder));
        services.AddScoped(typeof(SqlBuilder<InEvent>), typeof(EventSqlBuilder));
        services.AddScoped(typeof(SqlBuilder<InEventArtist>), typeof(EventArtistSqlBuilder));
        services.AddScoped(typeof(SqlBuilder<InEventUser>), typeof(EventUserSqlBuilder));
        services.AddScoped(typeof(SqlBuilder<InFacet>), typeof(FacetSqlBuilder));
        services.AddScoped(typeof(SqlBuilder<InGenre>), typeof(GenreSqlBuilder));
        services.AddScoped(typeof(SqlBuilder<InGenreAlias>), typeof(GenreAliasSqlBuilder));
        services.AddScoped(typeof(SqlBuilder<InGenreFacet>), typeof(GenreFacetSqlBuilder));
        services.AddScoped(typeof(SqlBuilder<InGenreRelation>), typeof(GenreRelationSqlBuilder));
        services.AddScoped(typeof(SqlBuilder<InLabel>), typeof(LabelSqlBuilder));
        services.AddScoped(typeof(SqlBuilder<InSong>), typeof(SongSqlBuilder));
        services.AddScoped(typeof(SqlBuilder<InPermission>), typeof(PermissionSqlBuilder));
        services.AddScoped(typeof(SqlBuilder<InRole>), typeof(RoleSqlBuilder));
        services.AddScoped(typeof(SqlBuilder<InTag>), typeof(TagSqlBuilder));
        services.AddScoped(typeof(SqlBuilder<InUser>), typeof(UserSqlBuilder));
        services.AddScoped(typeof(SqlBuilder<InUserAlbumAttribute>), typeof(UserAlbumAttrSqlBuilder));

        // Repositories
        services.AddScoped<Application.Api.Interfaces.Persistence.Repositories.Song.ISongRepository, SongRepository>();
        services.AddScoped<Application.Api.Interfaces.Persistence.Repositories.Album.IAlbumRepository, AlbumRepository>();
        services.AddScoped<Application.Api.Interfaces.Persistence.Repositories.Album.IAlbumGenreRepository, AlbumGenreRepository>();
        services.AddScoped<Application.Api.Interfaces.Persistence.Repositories.Album.IAlbumInfluenceRepository, AlbumInfluenceRepository>();
        services.AddScoped<Application.Api.Interfaces.Persistence.Repositories.Artist.IArtistRepository, ArtistRepository>();
        services.AddScoped<Application.Api.Interfaces.Persistence.Repositories.Genre.IGenreRepository, GenreRepository>();
        services.AddScoped<Application.Api.Interfaces.Persistence.Repositories.Security.IPermissionRepository, PermissionRepository>();
        services.AddScoped<Application.Api.Interfaces.Persistence.Repositories.Security.IRoleRepository, RoleRepository>();
        services.AddScoped<Application.Api.Interfaces.Persistence.Repositories.Tag.ITagRepository, TagRepository>();
        services.AddScoped<Application.Api.Interfaces.Persistence.Repositories.User.IUserRepository, UserRepository>();
        services.AddScoped<Application.Api.Interfaces.Persistence.Repositories.User.IUserAlbumAttrsRepository, UserAlbumAttrRepository>();
        
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
        services.AddScoped(typeof(IGetUseCase<,>), typeof(UcGet<,>));
        services.AddScoped(typeof(ISaveUseCase<>), typeof(UcSave<>));
        services.AddScoped(typeof(ISaveAllUseCase<>), typeof(UcSaveAll<>));
        services.AddScoped(typeof(IDeleteUseCase<>), typeof(UcDelete<>));
        services.AddScoped(typeof(IListUseCase<,>), typeof(UcList<,>));
        services.AddScoped(typeof(IFindInUseCase<,>), typeof(UcFindIn<,>));
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