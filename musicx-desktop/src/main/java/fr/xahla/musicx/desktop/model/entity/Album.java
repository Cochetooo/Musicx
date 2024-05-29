package fr.xahla.musicx.desktop.model.entity;

import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.model.enums.ArtistRole;
import fr.xahla.musicx.api.model.enums.ReleaseType;
import fr.xahla.musicx.desktop.helper.ImageHelper;
import fr.xahla.musicx.domain.helper.StringHelper;
import javafx.beans.property.*;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.collections.ObservableMap;
import javafx.scene.image.Image;

import java.time.LocalDate;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import static fr.xahla.musicx.domain.application.AbstractContext.albumRepository;
import static fr.xahla.musicx.domain.application.AbstractContext.logger;
import static fr.xahla.musicx.domain.helper.StringHelper.str_is_null_or_blank;

/**
 * Defines the behaviour of an album for a JavaFX context.
 * @author Cochetooo
 * @since 0.1.0
 */
public class Album {

    public static final Image artworkPlaceholder
        = ImageHelper.loadImageFromResource("thumbnail-placeholder.png");

    private final AlbumDto dto;

    private final LongProperty id;

    private final StringProperty artworkUrl;
    private final StringProperty catalogNo;
    private final IntegerProperty discTotal;
    private final StringProperty name;
    private final ObjectProperty<LocalDate> releaseDate;
    private final IntegerProperty trackTotal;
    private final ObjectProperty<ReleaseType> type;

    private ObjectProperty<Artist> artist;
    private MapProperty<Artist, ArtistRole> creditArtists;
    private ObjectProperty<Label> label;
    private ListProperty<Genre> primaryGenres;
    private ListProperty<Genre> secondaryGenres;

    private ObjectProperty<Image> image;

    public Album(final AlbumDto album) {
        this.id = new SimpleLongProperty(album.getId());

        this.artworkUrl = new SimpleStringProperty(album.getArtworkUrl());
        this.catalogNo = new SimpleStringProperty(album.getCatalogNo());
        this.discTotal = new SimpleIntegerProperty(album.getDiscTotal());
        this.name = new SimpleStringProperty(album.getName());
        this.releaseDate = new SimpleObjectProperty<>(album.getReleaseDate());
        this.trackTotal = new SimpleIntegerProperty(album.getTrackTotal());
        this.type = new SimpleObjectProperty<>(album.getType());

        this.image = new SimpleObjectProperty<>();

        this.dto = album;
    }

    /**
     * @since 0.3.0
     */
    public AlbumDto getDto() {
        return dto;
    }

    // Getters - Setters

    public long getId() {
        return id.get();
    }

    public LongProperty idProperty() {
        return id;
    }

    public Album setId(final long id) {
        this.dto.setId(id);
        this.id.set(id);
        return this;
    }

    public String getArtworkUrl() {
        return artworkUrl.get();
    }

    public StringProperty artworkUrlProperty() {
        return artworkUrl;
    }

    public Album setArtworkUrl(final String artworkUrl) {
        this.dto.setArtworkUrl(artworkUrl);
        this.artworkUrl.set(artworkUrl);

        if (!StringHelper.str_is_null_or_blank(artworkUrl)) {
            this.setImage(new Image(artworkUrl));
        }

        return this;
    }

    public String getCatalogNo() {
        return catalogNo.get();
    }

    public StringProperty catalogNoProperty() {
        return catalogNo;
    }

    public Album setCatalogNo(final String catalogNo) {
        this.dto.setCatalogNo(catalogNo);
        this.catalogNo.set(catalogNo);
        return this;
    }

    public int getDiscTotal() {
        return discTotal.get();
    }

    public IntegerProperty discTotalProperty() {
        return discTotal;
    }

    public Album setDiscTotal(final short discTotal) {
        this.dto.setDiscTotal(discTotal);
        this.discTotal.set(discTotal);
        return this;
    }

    public String getName() {
        return name.get();
    }

    public StringProperty nameProperty() {
        return name;
    }

    public Album setName(final String name) {
        this.dto.setName(name);
        this.name.set(name);
        return this;
    }

    public LocalDate getReleaseDate() {
        return releaseDate.get();
    }

    public ObjectProperty<LocalDate> releaseDateProperty() {
        return releaseDate;
    }

    public Album setReleaseDate(final LocalDate releaseDate) {
        this.dto.setReleaseDate(releaseDate);
        this.releaseDate.set(releaseDate);
        return this;
    }

    public int getTrackTotal() {
        return trackTotal.get();
    }

    public IntegerProperty trackTotalProperty() {
        return trackTotal;
    }

    public Album setTrackTotal(final short trackTotal) {
        this.dto.setTrackTotal(trackTotal);
        this.trackTotal.set(trackTotal);
        return this;
    }

    public ReleaseType getType() {
        return type.get();
    }

    public ObjectProperty<ReleaseType> typeProperty() {
        return type;
    }

    public Album setType(final ReleaseType type) {
        this.dto.setType(type);
        this.type.set(type);
        return this;
    }

    public Artist getArtist() {
        if (null == artist) {
            artist = new SimpleObjectProperty<>();
            artist.set(new Artist(albumRepository().getArtist(dto)));
        }

        return artist.get();
    }

    public ObjectProperty<Artist> artistProperty() {
        return artist;
    }

    public Album setArtist(final Artist artist) {
        if (null == this.artist) {
            this.artist = new SimpleObjectProperty<>(artist);
        } else {
            this.artist.set(artist);
        }

        return this;
    }

    public ObservableMap<Artist, ArtistRole> getCreditArtists() {
        if (null == creditArtists) {
            creditArtists = new SimpleMapProperty<>(FXCollections.observableMap(new HashMap<>()));

            final var creditArtistsDto = albumRepository().getCreditArtists(dto);
            creditArtistsDto.forEach((artist, role) -> creditArtists.put(
                new Artist(artist),
                role
            ));
        }

        return creditArtists.get();
    }

    public MapProperty<Artist, ArtistRole> creditArtistsProperty() {
        return creditArtists;
    }

    public Album setCreditArtists(final Map<Artist, ArtistRole> creditArtists) {
        if (null == this.creditArtists) {
            this.creditArtists = new SimpleMapProperty<>(FXCollections.observableMap(creditArtists));
        } else {
            this.creditArtists.putAll(creditArtists);
        }

        return this;
    }

    public Label getLabel() {
        if (null == label) {
            label = new SimpleObjectProperty<>();
            label.set(new Label(albumRepository().getLabel(dto)));
        }

        return label.get();
    }

    public ObjectProperty<Label> labelProperty() {
        return label;
    }

    public Album setLabel(final Label label) {
        if (null == this.label) {
            this.label = new SimpleObjectProperty<>(label);
        } {
            this.label.set(label);
        }

        return this;
    }

    public ObservableList<Genre> getPrimaryGenres() {
        if (null == primaryGenres) {
            primaryGenres = new SimpleListProperty<>(FXCollections.observableList(new ArrayList<>()));

            final var primaryGenresDto = albumRepository().getPrimaryGenres(dto);
            primaryGenresDto.forEach(genre -> primaryGenres.add(new Genre(genre)));
        }

        return primaryGenres.get();
    }

    public ListProperty<Genre> primaryGenresProperty() {
        return primaryGenres;
    }

    public Album setPrimaryGenres(final List<Genre> primaryGenres) {
        if (null == this.primaryGenres) {
            this.primaryGenres = new SimpleListProperty<>(FXCollections.observableList(primaryGenres));
        } else {
            this.primaryGenres.setAll(primaryGenres);
        }

        return this;
    }

    public ObservableList<Genre> getSecondaryGenres() {
        if (null == secondaryGenres) {
            secondaryGenres = new SimpleListProperty<>(FXCollections.observableList(new ArrayList<>()));

            final var secondaryGenresDto = albumRepository().getSecondaryGenres(dto);
            secondaryGenresDto.forEach(genre -> secondaryGenres.add(new Genre(genre)));
        }

        return secondaryGenres.get();
    }

    public ListProperty<Genre> secondaryGenresProperty() {
        return secondaryGenres;
    }

    public Album setSecondaryGenres(final List<Genre> secondaryGenres) {
        if (null == this.secondaryGenres) {
            this.secondaryGenres = new SimpleListProperty<>(FXCollections.observableList(secondaryGenres));
        } else {
            this.secondaryGenres.setAll(secondaryGenres);
        }

        return this;
    }

    /**
     * @return the Image created from the artwork url of the album, otherwise the default placeholder Image if
     * artwork url is empty or null.
     */
    public Image getImage() {
        if (null == image.get()) {
            if (str_is_null_or_blank(this.getArtworkUrl())) {
                this.setImage(Album.artworkPlaceholder);
            }

            this.setImage(new Image(this.getArtworkUrl()));
        }

        return image.get();
    }

    public Album setImage(final Image image) {
        this.image.set(image);
        return this;
    }

    public ObjectProperty<Image> imageProperty() {
        if (null == image.get()) {
            if (str_is_null_or_blank(this.getArtworkUrl())) {
                this.setImage(Album.artworkPlaceholder);
            }

            this.setImage(new Image(this.getArtworkUrl()));
        }

        return image;
    }
}
