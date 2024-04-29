package fr.xahla.musicx.domain.manager;

import fr.xahla.musicx.api.model.LabelInterface;
import fr.xahla.musicx.api.repository.LabelRepositoryInterface;

import java.util.List;

public interface LabelListManagerInterface {

    List<? extends LabelInterface> getLabels();
    LabelRepositoryInterface getRepository();

    void set(final List<? extends LabelInterface> labels);

}
