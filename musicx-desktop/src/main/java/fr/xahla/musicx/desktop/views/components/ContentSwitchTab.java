package fr.xahla.musicx.desktop.views.components;

import fr.xahla.musicx.desktop.config.FxmlComponent;
import fr.xahla.musicx.desktop.helper.FxmlHelper;
import javafx.geometry.Side;
import javafx.scene.control.Tab;
import javafx.scene.control.TabPane;
import javafx.scene.control.Tooltip;
import javafx.scene.layout.Pane;
import org.kordamp.ikonli.javafx.FontIcon;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

import static fr.xahla.musicx.desktop.context.DesktopContext.globalResourceBundle;

/**
 * @author Cochetooo
 * @since 0.5.0
 */
public final class ContentSwitchTab extends Pane {

    /**
     * @param name
     * @param fxmlComponent
     * @param resourceText The text to be shown for the tab, set by a resource bundle key
     * @param icon
     * @param iconScale
     * @param tooltip
     */
    public record TabInfo(
        String name,
        FxmlComponent fxmlComponent,
        String resourceText,
        String icon,
        double iconScale,
        String tooltip
    ) {}

    private final TabPane tabPane;
    private final Map<Tab, FxmlComponent> tabs;

    private Pane pageContainer;

    public ContentSwitchTab() {
        this.setPrefWidth(1200);

        this.tabPane = new TabPane();
        this.tabPane.setId("contentSwitchTab");
        this.tabPane.getStyleClass().add("base-padding");
        this.tabPane.setPrefWidth(this.getPrefWidth());
        this.setHeight(this.tabPane.getHeight());

        this.tabs = new HashMap<>();

        // The user needs to set manually the content pane
        this.pageContainer = new Pane();

        tabPane.getSelectionModel().selectedItemProperty().addListener(((observable, oldValue, newValue)
            -> onSelect(newValue)));

        this.getChildren().add(tabPane);
    }

    public void setPageContainer(final Pane pageContainer) {
        this.pageContainer = pageContainer;
    }

    public void setTabs(final List<TabInfo> tabsInfo) {
        this.tabs.clear();

        tabsInfo.forEach(tabInfo -> {
            final var tab = new Tab();
            tab.setStyle("-fx-padding: 10px");
            tab.setId(tabInfo.name());

            if (null != tabInfo.resourceText()) {
                tab.setText(globalResourceBundle().getString(tabInfo.resourceText()));
            }

            if (null != tabInfo.icon()) {
                final var icon = new FontIcon();
                icon.setScaleX(tabInfo.iconScale());
                icon.setScaleY(tabInfo.iconScale());
                icon.setIconLiteral(tabInfo.icon());

                tab.setGraphic(icon);
            }

            if (null != tabInfo.tooltip()) {
                final var tooltip = new Tooltip();
                tooltip.setText(tabInfo.tooltip());

                tab.setTooltip(tooltip);
            }

            tabs.put(tab, tabInfo.fxmlComponent());
        });

        tabPane.getTabs().setAll(tabs.keySet());
    }

    public void setSide(final Side side) {
        this.tabPane.setSide(side);
    }

    public void setClosingPolicy(final TabPane.TabClosingPolicy closingPolicy) {
        this.tabPane.setTabClosingPolicy(closingPolicy);
    }

    public void setDragPolicy(final TabPane.TabDragPolicy dragPolicy) {
        this.tabPane.setTabDragPolicy(dragPolicy);
    }

    private void onSelect(final Tab newTab) {
        tabs.forEach((tab, fxmlComponent) -> {
            if (newTab == tab) {
                pageContainer.getChildren().setAll(FxmlHelper.getComponent(fxmlComponent.getFilepath()));
            }
        });
    }

}
