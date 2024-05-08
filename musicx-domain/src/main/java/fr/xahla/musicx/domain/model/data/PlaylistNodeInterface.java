package fr.xahla.musicx.domain.model.data;

import java.util.List;

public interface PlaylistNodeInterface {

    /* @var String name */

    String getName();
    PlaylistNodeInterface setName(final String name);

    /* @var PlaylistFolderInterface parent */

    PlaylistNodeInterface getParent();
    PlaylistNodeInterface setParent(final PlaylistNodeInterface parent);

    /* @var PlaylistFolderInterface[] children */

    List<PlaylistNodeInterface> getChildren();
    PlaylistNodeInterface setChildren(final List<PlaylistNodeInterface> children);

}
