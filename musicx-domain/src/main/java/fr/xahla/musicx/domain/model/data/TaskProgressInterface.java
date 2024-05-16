package fr.xahla.musicx.domain.model.data;

/**
 * Interface for service with a progress task
 * @author Cochetooo
 */
public interface TaskProgressInterface {

    /* @var String name */

    String getName();

    /* @var Double currentProgress */

    double getCurrentProgress();

    /* @var Double totalProgress */

    double getTotalProgress();
}
