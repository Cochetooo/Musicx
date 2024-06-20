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
import javafx.scene.shape.SVGPath;

/**
 * @author Cochetooo
 * @since 0.5.1
 */
public class StarRating extends Pane {

    private final static SVGPath emptyStar = new SVGPath();
    private final static SVGPath halfStar = new SVGPath();
    private final static SVGPath fullStar = new SVGPath();

    static {
        emptyStar.setContent("M9.15316 5.40838C10.4198 3.13613 11.0531 2 12 2C12.9469 2 13.5802 3.13612 14.8468 5.40837L15.1745 5.99623C15.5345 6.64193 15.7144 6.96479 15.9951 7.17781C16.2757 7.39083 16.6251 7.4699 17.3241 7.62805L17.9605 7.77203C20.4201 8.32856 21.65 8.60682 21.9426 9.54773C22.2352 10.4886 21.3968 11.4691 19.7199 13.4299L19.2861 13.9372C18.8096 14.4944 18.5713 14.773 18.4641 15.1177C18.357 15.4624 18.393 15.8341 18.465 16.5776L18.5306 17.2544C18.7841 19.8706 18.9109 21.1787 18.1449 21.7602C17.3788 22.3417 16.2273 21.8115 13.9243 20.7512L13.3285 20.4768C12.6741 20.1755 12.3469 20.0248 12 20.0248C11.6531 20.0248 11.3259 20.1755 10.6715 20.4768L10.0757 20.7512C7.77268 21.8115 6.62118 22.3417 5.85515 21.7602C5.08912 21.1787 5.21588 19.8706 5.4694 17.2544L5.53498 16.5776C5.60703 15.8341 5.64305 15.4624 5.53586 15.1177C5.42868 14.773 5.19043 14.4944 4.71392 13.9372L4.2801 13.4299C2.60325 11.4691 1.76482 10.4886 2.05742 9.54773C2.35002 8.60682 3.57986 8.32856 6.03954 7.77203L6.67589 7.62805C7.37485 7.4699 7.72433 7.39083 8.00494 7.17781C8.28555 6.96479 8.46553 6.64194 8.82547 5.99623L9.15316 5.40838Z");
        halfStar.setContent("M11.9997 1C11.6059 1 11.2487 1.2312 11.0874 1.59053L8.27041 7.86702L1.43062 8.60661C1.03903 8.64895 0.708778 8.91721 0.587066 9.2918C0.465355 9.66639 0.574861 10.0775 0.866772 10.342L5.96556 14.9606L4.55534 21.6942C4.4746 22.0797 4.62768 22.4767 4.94632 22.7082C5.26497 22.9397 5.68983 22.9626 6.03151 22.7667L11.9997 19.3447V1Z");
        fullStar.setContent("M9.15316 5.40838C10.4198 3.13613 11.0531 2 12 2C12.9469 2 13.5802 3.13612 14.8468 5.40837L15.1745 5.99623C15.5345 6.64193 15.7144 6.96479 15.9951 7.17781C16.2757 7.39083 16.6251 7.4699 17.3241 7.62805L17.9605 7.77203C20.4201 8.32856 21.65 8.60682 21.9426 9.54773C22.2352 10.4886 21.3968 11.4691 19.7199 13.4299L19.2861 13.9372C18.8096 14.4944 18.5713 14.773 18.4641 15.1177C18.357 15.4624 18.393 15.8341 18.465 16.5776L18.5306 17.2544C18.7841 19.8706 18.9109 21.1787 18.1449 21.7602C17.3788 22.3417 16.2273 21.8115 13.9243 20.7512L13.3285 20.4768C12.6741 20.1755 12.3469 20.0248 12 20.0248C11.6531 20.0248 11.3259 20.1755 10.6715 20.4768L10.0757 20.7512C7.77268 21.8115 6.62118 22.3417 5.85515 21.7602C5.08912 21.1787 5.21588 19.8706 5.4694 17.2544L5.53498 16.5776C5.60703 15.8341 5.64305 15.4624 5.53586 15.1177C5.42868 14.773 5.19043 14.4944 4.71392 13.9372L4.2801 13.4299C2.60325 11.4691 1.76482 10.4886 2.05742 9.54773C2.35002 8.60682 3.57986 8.32856 6.03954 7.77203L6.67589 7.62805C7.37485 7.4699 7.72433 7.39083 8.00494 7.17781C8.28555 6.96479 8.46553 6.64194 8.82547 5.99623L9.15316 5.40838Z");
    }

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
            final var star = new Label();
            star.setGraphic(emptyStar);
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
                star.setGraphic(fullStar);
                star.getStyleClass().add("selected");
            } else if (i == fullStars && hasHalfStar) {
                star.setGraphic(halfStar);
                star.getStyleClass().add("half-selected");
            } else {
                star.setGraphic(emptyStar);
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
