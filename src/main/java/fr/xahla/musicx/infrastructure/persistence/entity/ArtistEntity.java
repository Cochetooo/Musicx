package fr.xahla.musicx.infrastructure.persistence.entity;

import fr.xahla.musicx.api.data.ArtistInterface;
import jakarta.persistence.*;

import java.util.List;

@Entity
@Table(name="artists")
public class ArtistEntity implements ArtistInterface {
    @Id
    @GeneratedValue(strategy= GenerationType.AUTO)
    @Column(name="artist_id")
    private Long id;

    private String name;

    @Override
    public Long getId() {
        return this.id;
    }

    @Override
    public ArtistEntity setId(Long id) {
        this.id = id;
        return this;
    }

    @Override
    public String getName() {
        return this.name;
    }

    @Override
    public ArtistEntity setName(String name) {
        this.name = name;
        return this;
    }

    public ArtistEntity set(ArtistInterface artistInterface) {
        if (null != artistInterface.getId() && 0 != artistInterface.getId()) {
            this.setId(artistInterface.getId());
        }

        if (null != artistInterface.getName()) {
            this.setName(artistInterface.getName());
        }

        return this;
    }
}
