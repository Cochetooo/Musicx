package fr.xahla.musicx.domain.service.library;

import fr.xahla.musicx.api.data.LibraryInterface;
import fr.xahla.musicx.domain.model.Album;
import fr.xahla.musicx.domain.model.Artist;
import fr.xahla.musicx.domain.model.Song;
import fr.xahla.musicx.api.data.SongInterface;
import fr.xahla.musicx.domain.repository.LibraryRepositoryInterface;
import fr.xahla.musicx.domain.service.ProgressListenerInterface;
import fr.xahla.musicx.domain.service.library.data.LocalSong;
import org.jaudiotagger.audio.AudioFileIO;
import org.jaudiotagger.tag.FieldKey;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.Callable;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.atomic.AtomicInteger;
import java.util.logging.Logger;

public class ScanFolderAsLibraryService {

    private final LibraryRepositoryInterface libraryManager;
    private final ProgressListenerInterface progressListener;
    private final Logger logger;
    private final ExecutorService executor;

    public record Request(LibraryInterface library) { }

    public ScanFolderAsLibraryService(
        final Logger logger,
        final LibraryRepositoryInterface libraryManager,
        final ProgressListenerInterface progressListener
    ) {
        this.logger = logger;
        this.libraryManager = libraryManager;
        this.progressListener = progressListener;
        this.executor = Executors.newFixedThreadPool(Runtime.getRuntime().availableProcessors());
    }

    public List<SongInterface> execute(ScanFolderAsLibraryService.Request request) {
        var path = request.library().getFolderPaths();

        var directory = Paths.get(path);

        var songs = new ArrayList<SongInterface>();

        try (var fileStream = Files.walk(directory)) {
            var audioFiles = fileStream.filter((filepath) -> {
                if (!filepath.toFile().isFile()) {
                    return false;
                }

                var filename = filepath.getFileName().toString();
                var lastDotIndex = filename.lastIndexOf('.');
                var extension = filename.substring(lastDotIndex + 1);

                return extension.equals("mp3") || extension.equals("ogg")
                    || extension.equals("wav");
            }).toList();

            var fileNumbers = audioFiles.size();
            var count = new AtomicInteger();

            if (fileNumbers > 0) {
                audioFiles.forEach(filepath -> {
                    if (!filepath.toFile().isFile()) {
                        return;
                    }

                    var filename = filepath.getFileName().toString();
                    var lastDotIndex = filename.lastIndexOf('.');
                    var extension = filename.substring(lastDotIndex + 1);

                    if (!extension.equals("mp3") && !extension.equals("ogg")
                        && !extension.equals("wav")) {
                        return;
                    }

                    var futureSong = executor.submit(() -> retrieveSongFromFile(filepath));

                    SongInterface song;
                    try {
                        song = futureSong.get();
                    } catch (InterruptedException | ExecutionException e) {
                        throw new RuntimeException(e);
                    }

                    if (null != song) {
                        songs.add(song);

                        count.getAndIncrement();
                        progressListener.updateProgress(count.get(), fileNumbers);
                    }
                });

                executor.shutdown();
            } else {
                // Exit process instantly by making the progress 100% if empty library
                progressListener.updateProgress(1, 1);
            }
        } catch (IOException exception) {
            exception.printStackTrace();
        }

        if (null != request.library().getId() && 0 != request.library().getId()) {
            request.library().set(this.libraryManager.findOneById(request.library().getId()));
        }

        return songs;
    }

    private SongInterface retrieveSongFromFile(Path filepath) {
        Song song;

        try {
            var audioFile = AudioFileIO.read(filepath.toFile());
            var localSong = new LocalSong(audioFile, logger);

            var artist = new Artist()
                .setId(null)
                .setName(localSong.getArtistName());

            var album = new Album()
                .setId(null)
                .setName(localSong.getAlbumName())
                .setReleaseYear(localSong.getYear());

            if (localSong.getAlbumArtist().equals(artist.getName())) {
                album.setArtist(artist);
            } else {
                var albumArtist = new Artist()
                    .setId(null)
                    .setName(localSong.getAlbumArtist());

                album.setArtist(albumArtist);
            }

            song = new Song()
                .setTitle(localSong.getTitle())
                .setDuration(localSong.getDuration())
                .setBitRate(localSong.getBitRate())
                .setSampleRate(localSong.getSampleRate())
                .setId(null)
                .setArtist(artist)
                .setAlbum(album)
                .setFilePath(filepath.toAbsolutePath().toString());
        } catch (Exception e) {
            throw new RuntimeException(e);
        }

        return song;
    }

}
