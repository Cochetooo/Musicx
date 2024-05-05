package fr.xahla.musicx.api.model;

import java.util.List;

public class BandArtistDto extends ArtistDto {

    private List<Long> memberIds;

    public List<Long> getMemberIds() {
        return memberIds;
    }

    public BandArtistDto setMemberIds(final List<Long> memberIds) {
        this.memberIds = memberIds;
        return this;
    }
}
