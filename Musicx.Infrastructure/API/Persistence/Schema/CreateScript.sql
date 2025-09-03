/*
    Extensions
*/

create extension if not exists pg_trgm;
create extension if not exists citext;

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

\i 'Songs/TriggerCalculateRatingSong.sql'
\i 'Albums/TriggerCalculateRatingAlbum.sql'
\i 'Albums/TriggerGenerateSimplifiedGenreName.sql'
\i 'Artists/TriggerCalculateRatingArtist.sql'

\i 'Artists/TriggerCalculateGenresArtist.sql'
\i 'Artists/TriggerCalculateInfluencesArtist.sql'

CREATE OR REPLACE FUNCTION update_artist_calculated_fields(artistId BIGINT)
    RETURNS void AS
$$
BEGIN
    PERFORM update_artist_calculated_genres(artistId);
    PERFORM update_artist_calculated_influences(artistId);
END;
$$ LANGUAGE plpgsql;