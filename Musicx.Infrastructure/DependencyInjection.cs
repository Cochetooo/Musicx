using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Auth;
using Musicx.Application.Api.Interfaces.Caching;
using Musicx.Application.Api.Interfaces.DataViews;
using Musicx.Application.Api.Interfaces.PatchNotes;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Album;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Artist;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Genre;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Security;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Song;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Tag;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.User;
using Musicx.Application.Api.Interfaces.Storage;
using Musicx.Application.Api.Interfaces.Workers;
using Musicx.Application.Desktop;
using Musicx.Application.Desktop.Interfaces.Library;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Application.Shared.Interfaces.Localization;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Application.Shared.Interfaces.Providers.ExternalMusicData;
using Musicx.Application.Shared.Interfaces.UseCases.ExternalMusicData;
using Musicx.Application.Shared.Interfaces.UseCases.PatchNotes;
using Musicx.Application.Shared.Interfaces.UseCases.Security;
using Musicx.Application.Shared.Interfaces.UseCases.User.AlbumAttribute;
using Musicx.Application.Shared.Interfaces.UseCases.User.Avatar;
using Musicx.Application.Shared.Interfaces.UseCases.User.Ratings;
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
using Musicx.Infrastructure.API.Caching;
using Musicx.Infrastructure.API.DataViews;
using Musicx.Infrastructure.API.PatchNotes;
using Musicx.Infrastructure.API.Persistence.Builders;
using Musicx.Infrastructure.API.Persistence.Builders.Album;
using Musicx.Infrastructure.API.Persistence.Builders.Artist;
using Musicx.Infrastructure.API.Persistence.Builders.Core.Commands;
using Musicx.Infrastructure.API.Persistence.Builders.Event;
using Musicx.Infrastructure.API.Persistence.Builders.Genre;
using Musicx.Infrastructure.API.Persistence.Builders.Label;
using Musicx.Infrastructure.API.Persistence.Builders.Security;
using Musicx.Infrastructure.API.Persistence.Builders.Song;
using Musicx.Infrastructure.API.Persistence.Builders.Tag;
using Musicx.Infrastructure.API.Persistence.Builders.User;
using Musicx.Infrastructure.API.Persistence.Connection;
using Musicx.Infrastructure.API.Persistence.Repositories.Album;
using Musicx.Infrastructure.API.Persistence.Repositories.Artist;
using Musicx.Infrastructure.API.Persistence.Repositories.Genre;
using Musicx.Infrastructure.API.Persistence.Repositories.Security;
using Musicx.Infrastructure.API.Persistence.Repositories.Song;
using Musicx.Infrastructure.API.Persistence.Repositories.Tag;
using Musicx.Infrastructure.API.Persistence.Repositories.User;
using Musicx.Infrastructure.API.Storage;
using Musicx.Infrastructure.API.Workers;
using Musicx.Infrastructure.Desktop.Persistence.Sqlite;
using Musicx.Infrastructure.Desktop.Services.Library;
using Musicx.Infrastructure.Shared.Clients;
using Musicx.Infrastructure.Shared.Logging;
using Musicx.Infrastructure.Shared.Providers.ExternalMusicData;
using Musicx.Infrastructure.Shared.Services.Localization;
using Musicx.Infrastructure.Shared.UseCases.ExternalMusicData;
using Musicx.Infrastructure.Shared.UseCases.PatchNotes;
using Musicx.Infrastructure.Shared.UseCases.Security;
using Musicx.Infrastructure.Shared.UseCases.User.AlbumAttribute;
using Musicx.Infrastructure.Shared.UseCases.User.Avatar;
using Musicx.Infrastructure.Shared.UseCases.User.Ratings;
using Musicx.Infrastructure.Shared.UseCases.User.Ratings.Export;

namespace Musicx.Infrastructure;

public static class DependencyInjection
{
    public static ILoggingBuilder AddLog4Net(this ILoggingBuilder builder, string configFile = "log4net.config")
    {
        builder.AddProvider(new Log4NetLoggerProvider(configFile));
        return builder;
    }

    public static IServiceCollection AddMusicxLocalization(this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        var descriptor = new ServiceDescriptor(typeof(ITranslationService), typeof(ResxTranslationService), lifetime);
        services.Add(descriptor);

        return services;
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
        services.AddScoped<IExternalMusicDataProvider, WikipediaApiProvider>();

        services.AddScoped<ExternalMusicDataService>();

        // Use cases
        services.AddScoped<IFetchArtistInfoClientService, UcFetchArtistInfo>();
        services.AddScoped<IFetchAlbumInfoClientService, UcFetchAlbumInfo>();
        
        return services;
    }
    
    /// <summary>
    /// Add desktop module services and use cases.
    /// </summary>
    public static IServiceCollection AddMusicxDesktop(this IServiceCollection services)
    {
        services.AddMusicxDesktopApp();
        services.AddMusicxLocalization(ServiceLifetime.Singleton);

        services.AddSingleton(new HttpClient(new HttpClientHandler
        {
            UseCookies = true,
            CookieContainer = new CookieContainer()
        })
        {
            BaseAddress = new Uri(Environment.GetEnvironmentVariable("MUSICX_API_URL") ?? "http://localhost:7287")
        });

        services.AddSingleton<SqliteLibraryDatabase>();
        services.AddSingleton<InMemoryImportProgressPublisher>();
        services.AddScoped<IImportProgressPublisher>(sp => sp.GetRequiredService<InMemoryImportProgressPublisher>());

        services.AddScoped<ILibraryProfileRepository, SqliteLibraryProfileRepository>();
        services.AddScoped<ILocalLibraryRepository, SqliteLocalLibraryRepository>();
        services.AddScoped<IAudioMetadataReader, AtlAudioMetadataReader>();
        services.AddScoped<IEncyclopediaMatcher, WebApiEncyclopediaMatcher>();

        services.AddScoped<IApiClient, ApiClient>();
        
        services.AddScoped<IFindAlbumAttributesByAlbumService, FindAlbumAttrByAlbumService>();
        services.AddScoped<IFindAlbumAttributesByUserService, FindAlbumAttrByUserService>();
        services.AddScoped<IFindAlbumAttributesByAlbumUserService, FindAlbumAttrByAlbumUserService>();
        services.AddScoped(typeof(IFindRatingDistribByUserService<>), typeof(FindRatingDistribByUserService<>));
        services.AddScoped<IFindUserYearlyRatingsService, FindUserYearlyRatingsService>();
        services.AddScoped<IFindUserGenreRatingsService, FindUserGenreRatingsService>();
        services.AddScoped<IExportUserRatingsService, ExportUserRatingsService>();
        services.AddScoped<IUserRatingsExportFormatter, CsvUserRatingsExportFormatter>();
        services.AddScoped<IUserRatingsExportFormatter, XlsxUserRatingsExportFormatter>();
        services.AddScoped<IUserRatingsExportFormatter, JsonUserRatingsExportFormatter>();
        services.AddScoped<ISaveAvatarService,SaveAvatarService>();

        services.AddScoped<IPatchNotesService, PatchNotesService>();

        services.AddScoped<IAuthUserSavePasswordService, AuthUserSavePasswordService>();
        services.AddScoped<IAuthSignInService, AuthSignInService>();
        
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
        services.AddScoped<ManyToManySyncService>();
        
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
        services.AddScoped(typeof(SqlBuilder<InUserArtistAttribute>), typeof(UserArtistAttrSqlBuilder));
        services.AddScoped(typeof(SqlBuilder<InUserAlbumAttribute>), typeof(UserAlbumAttrSqlBuilder));
        services.AddScoped(typeof(SqlBuilder<InUserSongAttribute>), typeof(UserSongAttrSqlBuilder));

        // Repositories
        services.AddScoped<ISongRepository, SongRepository>();
        services.AddScoped<IAlbumRepository, AlbumRepository>();
        services.AddScoped<IAlbumGenreRepository, AlbumGenreRepository>();
        services.AddScoped<IAlbumInfluenceRepository, AlbumInfluenceRepository>();
        services.AddScoped<IArtistRepository, ArtistRepository>();
        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddScoped<IGenreRelationRepository, GenreRelationRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserArtistAttrsRepository, UserArtistAttrRepository>();
        services.AddScoped<IUserAlbumAttrsRepository, UserAlbumAttrRepository>();
        services.AddScoped<IUserSongAttrsRepository, UserSongAttrRepository>();
        
        // Data Views / Cache abstractions
        services.AddScoped<IDataViewCacheProvider, NoOpDataViewCacheProvider>();
        services.AddScoped<IDataViewCacheKeyFactory, DataViewCacheKeyFactory>();
        services.AddScoped<IAlbumDataViewBuilder, AlbumDataViewBuilder>();
        services.AddScoped<IArtistDataViewBuilder, ArtistDataViewBuilder>();
        services.AddScoped<IGenreDataViewBuilder, GenreDataViewBuilder>();
        services.AddScoped<IUserDataViewBuilder, UserDataViewBuilder>();
        
        // Audit
        services.AddScoped<IAuditPublisher, RabbitMqAuditPublisher>();
        
        // Auth
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
        services.AddScoped<ITokenValidator, JwtTokenValidator>();
        
        services.AddScoped<IAuthService, AuthService>();

        // Storage
        
        services.AddScoped<IAvatarStorage, LocalAvatarStorage>();
        services.AddScoped<IArtworkStorage, LocalArtworkStorage>();
        
        // Patch Notes

        services.AddScoped<IPatchNoteService, FileSystemPatchNoteService>();

        services.AddMusicxWeb();
        
        return services;
    }
    
    /// <summary>
    /// Add web module services and use cases.
    /// </summary>
    public static IServiceCollection AddMusicxWeb(this IServiceCollection services)
    {
        services.AddScoped<IApiClient, ApiClient>();
        
        services.AddScoped<IFindAlbumAttributesByAlbumService, FindAlbumAttrByAlbumService>();
        services.AddScoped<IFindAlbumAttributesByUserService, FindAlbumAttrByUserService>();
        services.AddScoped<IFindAlbumAttributesByAlbumUserService, FindAlbumAttrByAlbumUserService>();
        services.AddScoped(typeof(IFindRatingDistribByUserService<>), typeof(FindRatingDistribByUserService<>));
        services.AddScoped<IFindUserYearlyRatingsService, FindUserYearlyRatingsService>();
        services.AddScoped<IFindUserGenreRatingsService, FindUserGenreRatingsService>();
        services.AddScoped<IExportUserRatingsService, ExportUserRatingsService>();
        services.AddScoped<IUserRatingsExportFormatter, CsvUserRatingsExportFormatter>();
        services.AddScoped<IUserRatingsExportFormatter, XlsxUserRatingsExportFormatter>();
        services.AddScoped<IUserRatingsExportFormatter, JsonUserRatingsExportFormatter>();
        services.AddScoped<ISaveAvatarService,SaveAvatarService>();

        services.AddScoped<IPatchNotesService, PatchNotesService>();

        services.AddScoped<IAuthUserSavePasswordService, AuthUserSavePasswordService>();
        services.AddScoped<IAuthSignInService, AuthSignInService>();
        
        return services;
    }
}