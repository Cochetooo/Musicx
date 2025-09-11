/*
    Extensions
*/

create extension if not exists pg_trgm;
create extension if not exists citext;

/*
    Create
 */

\i 'Artists/CreateArtistTable.sql'
\i 'Labels/CreateLabelTable.sql'
\i 'Genres/CreateGenreTable.sql'
\i 'Albums/CreateAlbumTable.sql'
\i 'Releases/CreateReleaseTable.sql'
\i 'Songs/CreateSongTable.sql'

\i 'Tags/CreateTagTable.sql'

\i 'Permissions/CreatePermissionTable.sql'
\i 'Roles/CreateRoleTable.sql'
\i 'Users/CreateUserTable.sql'

\i 'Audit/CreateAuditLogTable.sql'

/*
    Triggers 
 */

\i 'Songs/TriggerCalculateRatingSong.sql'

\i 'Albums/TriggerCalculateRatingAlbum.sql'
\i 'Albums/TriggerGenerateSimplifiedGenreName.sql'

\i 'Artists/TriggerCalculateRatingArtist.sql'
\i 'Artists/TriggerCalculateGenresArtist.sql'