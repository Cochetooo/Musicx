package fr.xahla.musicx.domain.manager;

import fr.xahla.musicx.api.model.LabelDto;
import fr.xahla.musicx.api.repository.LabelRepositoryInterface;

import java.util.List;

public interface LabelListManagerInterface {

    List<? extends LabelDto> getLabels();
    LabelRepositoryInterface getRepository();

    void set(final List<? extends LabelDto> labels);

}
