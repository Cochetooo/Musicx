import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.model.ArtistDto;
import fr.xahla.musicx.api.model.GenreDto;
import fr.xahla.musicx.api.model.LabelDto;
import fr.xahla.musicx.domain.application.AbstractContext;
import fr.xahla.musicx.domain.application.SettingsInterface;
import fr.xahla.musicx.domain.manager.AudioPlayerManagerInterface;
import fr.xahla.musicx.domain.manager.LibraryManagerInterface;
import fr.xahla.musicx.domain.model.data.LibraryInterface;
import fr.xahla.musicx.domain.model.enums.RepeatMode;
import fr.xahla.musicx.domain.model.enums.ShuffleMode;
import fr.xahla.musicx.domain.repository.data.LibraryRepositoryInterface;

import java.time.LocalDate;
import java.util.List;
import java.util.logging.Logger;

import static fr.xahla.musicx.domain.application.AbstractContext.*;

public class MainTest {

    static class Context extends AbstractContext {
        protected Context() {
            super(Logger.getLogger(Context.class.getName()), new SettingsInterface() {
            }, new AudioPlayerManagerInterface() {
                @Override public double getCurrentTime() {
                    return 0;
                }

                @Override public RepeatMode getRepeatMode() {
                    return null;
                }

                @Override public ShuffleMode getShuffleMode() {
                    return null;
                }

                @Override public double getVolume() {
                    return 0;
                }

                @Override public boolean isMuted() {
                    return false;
                }

                @Override public boolean isPlaying() {
                    return false;
                }

                @Override public void setRepeatMode(final RepeatMode repeatMode) {

                }

                @Override public void setShuffleMode(final ShuffleMode shuffleMode) {

                }

                @Override public void setVolume(final double volume) {

                }

                @Override public void mute() {

                }

                @Override public void next() {

                }

                @Override public void pause() {

                }

                @Override public void previous() {

                }

                @Override public void resume() {

                }

                @Override public void seek(final double seconds) {

                }

                @Override public void stop() {

                }

                @Override public void togglePlaying() {

                }

                @Override public void toggleRepeat() {

                }

                @Override public void toggleShuffle() {

                }

                @Override public void updateSong() {

                }
            }, new LibraryManagerInterface() {
                @Override public LibraryInterface get() {
                    return null;
                }

                @Override public LibraryRepositoryInterface getRepository() {
                    return null;
                }

                @Override public void set(final LibraryInterface library) {

                }
            });
        }
    }

    public static void main(final String[] args) {
        final var context = new Context();

        final var album = new AlbumDto() {
            private String artworkUrl;
            private LocalDate releaseDate;

            @Override public Long getId() {
                return 0L;
            }

            @Override public AlbumDto setId(final Long id) {
                return null;
            }

            @Override public ArtistDto getArtist() {
                return new ArtistDto() {
                    @Override public Long getId() {
                        return 0L;
                    }

                    @Override public ArtistDto setId(final Long id) {
                        return null;
                    }

                    @Override public String getName() {
                        return "Katatonia";
                    }

                    @Override public ArtistDto setName(final String name) {
                        return null;
                    }

                    @Override public String getCountry() {
                        return "";
                    }

                    @Override public ArtistDto setCountry(final String country) {
                        return null;
                    }

                    @Override public String getArtworkUrl() {
                        return "";
                    }

                    @Override public ArtistDto setArtworkUrl(final String artworkUrl) {
                        return null;
                    }

                    @Override public ArtistDto set(final ArtistDto artistInterface) {
                        return null;
                    }
                };
            }

            @Override public AlbumDto setArtist(final ArtistDto artistInterface) {
                return null;
            }

            @Override public String getName() {
                return "Last Fair Deal Gone Down";
            }

            @Override public AlbumDto setName(final String name) {
                return null;
            }

            @Override public LocalDate getReleaseDate() {
                return this.releaseDate;
            }

            @Override public AlbumDto setReleaseDate(final LocalDate date) {
                this.releaseDate = date;
                return this;
            }

            @Override public List<GenreDto> getPrimaryGenres() {
                return List.of();
            }

            @Override public AlbumDto setPrimaryGenres(final List<GenreDto> genres) {
                return null;
            }

            @Override public List<GenreDto> getSecondaryGenres() {
                return List.of();
            }

            @Override public AlbumDto setSecondaryGenres(final List<GenreDto> genres) {
                return null;
            }

            @Override public Short getTrackTotal() {
                return 0;
            }

            @Override public AlbumDto setTrackTotal(final Short trackTotal) {
                return null;
            }

            @Override public Short getDiscTotal() {
                return 0;
            }

            @Override public AlbumDto setDiscTotal(final Short discTotal) {
                return null;
            }

            @Override public String getArtworkUrl() {
                return artworkUrl;
            }

            @Override public AlbumDto setArtworkUrl(final String artworkUrl) {
                this.artworkUrl = artworkUrl;
                return this;
            }

            @Override public LabelDto getLabel() {
                return null;
            }

            @Override public AlbumDto setLabel(final LabelDto label) {
                return null;
            }

            @Override public AlbumDto set(final AlbumDto albumInterface) {
                return null;
            }
        };

        iTunesApi().fetchAlbumFromExternal(album, true);

        logger().info("Album artwork URL: " + album.getArtworkUrl());
    }

}
