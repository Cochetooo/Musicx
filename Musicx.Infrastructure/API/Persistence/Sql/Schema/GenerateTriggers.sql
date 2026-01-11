/*
    Triggers 
 */
 
\i 'Genres/TriggerInsertGenreClosure.sql'

\i 'Songs/TriggerCalculateRatingSong.sql'

\i 'Albums/TriggerCalculateRatingAlbum.sql'
\i 'Albums/TriggerGenerateSimplifiedGenreName.sql'
\i 'Albums/TriggerSetTotalDurationDiscTrack.sql'

\i 'Artists/TriggerCalculateRatingArtist.sql'
\i 'Artists/TriggerCalculateGenresArtist.sql'