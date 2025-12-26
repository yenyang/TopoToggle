import { bindValue, trigger, useValue } from "cs2/api";
import styles from "./TopoPanelComponent.module.scss";
import { Panel, Portal } from "cs2/ui";
import { VanillaComponentResolver } from "mods/VanillaComponentResolver/VanillaComponentResolver";
import mod from "../../../mod.json";
import { useLocalization } from "cs2/l10n";
import { game, Number2 } from "cs2/bindings";
import ContourLinesSrc from "../../images/ContourLines.svg";
import { useState } from "react";

// These establishes the binding with C# side. Without C# side game ui will crash.
const ForceContourLines$ = bindValue(mod.id, "ForceContourLines", false);
const HideTopoTogglePanel$ = bindValue(mod.id, "HideTopoTogglePanel", false);
const PanelPosition$ = bindValue(mod.id, "PanelPosition", {x: 0.085, y: 0.875});

export const TopoPanelComponent = () => {
    // These get the value of the bindings.
    const ForceContourLines = useValue(ForceContourLines$);
    const isPhotoMode = useValue(game.activeGamePanel$)?.__Type == game.GamePanelType.PhotoMode;
    const HideTopoTogglePanel = useValue(HideTopoTogglePanel$);
    const PanelPosition = useValue(PanelPosition$);
    // translation handling. Translates using locale keys that are defined in C# or fallback string here.
    
    const { translate } = useLocalization();
         
    // This either returns an empty JSX component or a small panel with TOPO title and a button to toggle contour lines.
    return (
        <>
            <div id = "TopoTogglePanel">
                {!isPhotoMode && !HideTopoTogglePanel && (
                    <Portal>
                        <Panel                            
                            draggable
                            className={styles.panel}
                            header={"TOPO"} // This is intentionally not translatable.
                            initialPosition={PanelPosition}                       
                            >
                            <div className={styles.panelSection}>
                                <VanillaComponentResolver.instance.ToolButton
                                    className={VanillaComponentResolver.instance.toolButtonTheme.button} 
                                    selected={ForceContourLines}
                                    tooltip={translate("Options.OPTION_DESCRIPTION[InputSettings.Gamepad.Tool/Toggle Contour Lines/binding]", "Toggles the topographic contour lines on and off.")}
                                    onSelect={() => 
                                    { 
                                        trigger(mod.id, "ToggleContourLines");
                                        trigger(mod.id, "SetPanelPosition", PanelPosition);
                                    }} 
                                    src={ContourLinesSrc}
                                    focusKey={VanillaComponentResolver.instance.FOCUS_DISABLED}>                          
                                </VanillaComponentResolver.instance.ToolButton>
                            </div>
                        </Panel>
                    </Portal>
                )}
            </div>
        </>
    );
}