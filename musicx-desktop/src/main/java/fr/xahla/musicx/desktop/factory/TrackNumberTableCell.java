package fr.xahla.musicx.desktop.factory;

import fr.xahla.musicx.desktop.extensions.controls.TextFields;
import fr.xahla.musicx.desktop.model.entity.Song;
import fr.xahla.musicx.desktop.service.save.EditSongService;
import javafx.scene.control.TableCell;
import javafx.scene.control.TextField;
import javafx.scene.input.KeyCode;
import javafx.scene.input.MouseButton;
import javafx.scene.input.MouseEvent;

/**
 * @author Cochetooo
 * @since 0.5.1
 */
public class TrackNumberTableCell extends TableCell<Song, Integer> {

    private TextField editNumberTextField;

    public TrackNumberTableCell() {
        this.setOnMouseClicked(this::handleMouseClick);
        this.setOnKeyPressed(event -> {
            if (null != editNumberTextField) {
                if (KeyCode.ENTER == event.getCode()) {
                    if (null == editNumberTextField.getText() || editNumberTextField.getText().isBlank()) {
                        super.commitEdit(null);
                    } else {
                        commitEdit(Short.parseShort(editNumberTextField.getText()));
                    }
                } else if (KeyCode.ESCAPE == event.getCode()) {
                    cancelEdit();
                }
            }
        });
    }

    private void handleMouseClick(final MouseEvent event) {
        if (event.getButton() == MouseButton.MIDDLE && 1 == event.getClickCount()) {
            startEdit();
        }
    }

    @Override public void startEdit() {
        super.startEdit();
        if (null == editNumberTextField) {
            editNumberTextField = new TextField(getText());
            TextFields.setIntegerTextFieldConstraint(editNumberTextField);
        } else {
            editNumberTextField.setText(getText());
        }

        setGraphic(editNumberTextField);
        editNumberTextField.selectAll();
        editNumberTextField.requestFocus();
    }

    @Override public void cancelEdit() {
        super.cancelEdit();
        setGraphic(null);
    }

    public void commitEdit(final Short newValue) {
        super.commitEdit(newValue.intValue());

        final var song = this.getTableRow().getItem();

        song.setTrackNumber(newValue);

        new EditSongService().execute(song, () -> setGraphic(null));
    }

    @Override protected void updateItem(Integer item, boolean empty) {
        textProperty().unbind();

        if (null == item || empty) {
            setText("");
        } else {
            textProperty().bind(this.getTableRow().getItem().trackNumberProperty().asString());
        }
    }

}
