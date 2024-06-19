package fr.xahla.musicx.desktop.views.components;

import javafx.beans.property.DoubleProperty;
import javafx.beans.property.IntegerProperty;
import javafx.beans.property.SimpleDoubleProperty;
import javafx.beans.property.SimpleIntegerProperty;
import javafx.fxml.FXML;
import javafx.scene.control.Label;
import javafx.scene.input.MouseEvent;
import javafx.scene.layout.HBox;
import javafx.scene.layout.Pane;

/**
 * @author Cochetooo
 * @since 0.5.1
 */
public class StarRating extends Pane {

    private final IntegerProperty maxStars = new SimpleIntegerProperty(5);
    private final DoubleProperty rating = new SimpleDoubleProperty(0);

    @FXML private HBox starsBox;

    public StarRating() {
        this(5);
    }

    public StarRating(final int maxStars) {
        this.maxStars.set(maxStars);
        initialize();
    }

    @FXML private void initialize() {
        starsBox = new HBox();
        starsBox.setSpacing(5);
        getChildren().add(starsBox);
        createStars();
        updateStarsDisplay(rating.get());

        rating.addListener((observable, oldValue, newValue)
            -> updateStarsDisplay(newValue.doubleValue()));

        maxStars.addListener(((observable, oldValue, newValue) -> {
            starsBox.getChildren().clear();
            createStars();
            updateStarsDisplay(rating.get());
        }));
    }

    private void createStars() {
        for (var i = 1; i <= maxStars.get(); i++) {
            final var star = new Label("★");
            star.getStyleClass().add("rating-star");
            final var starIndex = i;

            star.setOnMouseClicked(event -> handleMouseClick(event, starIndex));
            star.setOnMouseEntered(event -> updateStarsDisplay(starIndex - 0.5));
            star.setOnMouseExited(event -> updateStarsDisplay(rating.get()));

            starsBox.getChildren().add(star);
        }
    }

    private void handleMouseClick(final MouseEvent event, final int starIndex) {
        final var clickX = event.getX();
        final var star = (Label) event.getSource();
        final var width = star.getWidth();
        final var halfStarThreshold = width / 2;

        if (clickX <= halfStarThreshold) {
            setRating(starIndex - 0.5);
        } else {
            setRating(starIndex);
        }
    }

    private void setRating(final double rating) {
        this.rating.set(rating);
    }

    private void updateStarsDisplay(final double rating) {
        final var fullStars = (int) rating;
        final var hasHalfStar = rating - fullStars >= 0.5;

        for (var i = 0; i < starsBox.getChildren().size(); i++) {
            final var star = (Label) starsBox.getChildren().get(i);
            star.getStyleClass().removeAll("selected", "half-selected");

            if (i < fullStars) {
                star.getStyleClass().add("selected");
            } else if (i == fullStars && hasHalfStar) {
                star.getStyleClass().add("half-selected");
            }
        }
    }

    public int getMaxStars() {
        return maxStars.get();
    }

    public void setMaxStars(final int maxStars) {
        this.maxStars.set(maxStars);
    }

    public IntegerProperty maxStarsProperty() {
        return maxStars;
    }

    public double getRating() {
        return rating.get();
    }

    public DoubleProperty ratingProperty() {
        return rating;
    }
}
