package fr.xahla.musicx.domain.service.genreList;

import fr.xahla.musicx.api.model.GenreDto;
import fr.xahla.musicx.api.repository.searchCriterias.GenreSearchCriterias;
import fr.xahla.musicx.domain.application.AbstractContext;
import fr.xahla.musicx.domain.application.SettingsInterface;
import fr.xahla.musicx.domain.database.HibernateLoader;
import fr.xahla.musicx.domain.helper.JsonHelper;
import fr.xahla.musicx.domain.manager.AudioPlayerManagerInterface;
import fr.xahla.musicx.domain.manager.LibraryManagerInterface;
import fr.xahla.musicx.domain.model.data.LibraryInterface;
import fr.xahla.musicx.domain.model.enums.RepeatMode;
import fr.xahla.musicx.domain.model.enums.ShuffleMode;
import fr.xahla.musicx.domain.repository.data.LibraryRepositoryInterface;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.logging.Logger;

import org.json.JSONObject;

import static fr.xahla.musicx.domain.application.AbstractContext.env;
import static fr.xahla.musicx.domain.application.AbstractContext.logger;
import static fr.xahla.musicx.domain.repository.GenreRepository.genreRepository;

public final class ImportGenresFromJson {

    private int nbCreated;

    private JSONObject jsonContent;

    public void execute(final String jsonPath) {
        jsonContent = JsonHelper.loadJsonFromFile(jsonPath);

        for (final var entry : jsonContent.toMap().entrySet()) {
            if (entry.getValue() instanceof final List<?> array) {
                saveGenre(entry.getKey(), array.stream().map(Object::toString).toList());
            }
        }
    }

    private Long saveGenre(final String name, final List<String> parents) {
        final var parentIds = new ArrayList<Long>();

        if (!genreRepository().findByCriteria(Map.of(
            GenreSearchCriterias.NAME, name
        )).isEmpty()) {
            return 0L;
        }

        if (!parents.isEmpty()) {
            parents.forEach(parentName -> {
                final var exists = genreRepository().findByCriteria(Map.of(
                    GenreSearchCriterias.NAME, parentName
                ));

                if (!exists.isEmpty()) {
                    parentIds.add(exists.getFirst().getId());
                } else {
                    final var parentGenre = jsonContent.getJSONArray(parentName).toList().stream()
                        .map(Object::toString).toList();

                    parentIds.add(saveGenre(parentName, parentGenre));
                }
            });
        }

        final var genreDto = GenreDto.builder()
            .name(name)
            .parentIds(parentIds)
            .build();

        genreRepository().save(genreDto);

        nbCreated++;

        return genreDto.getId();
    }

    public static void main(final String[] args) {
        new Context();
        new HibernateLoader();

        final var service = new ImportGenresFromJson();
        service.execute(env("GENRES_URL"));

        logger().info("Nb created: " + service.nbCreated);
    }

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

}
