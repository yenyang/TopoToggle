import { bindValue, trigger, useValue } from "cs2/api";
import styles from "./TopoPanelComponent.module.scss";
import { Panel, Portal } from "cs2/ui";
import { VanillaComponentResolver } from "mods/VanillaComponentResolver/VanillaComponentResolver";
import mod from "../../../mod.json";
import { useLocalization } from "cs2/l10n";
import { game } from "cs2/bindings";
import ContourLinesSrc from "../../images/ContourLines.svg";

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
            {!isPhotoMode && !HideTopoTogglePanel && (
                <Portal>
                    <Panel                 
                        id = "TopoTogglePanel"    
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
                                    
                                    // Not an ideal solution.
                                    let icon = document.getElementById('TopoTogglePanel');
                                    if (icon) 
                                    {
                                        console.log("icon.offsetLeft" + icon.offsetLeft);
                                        console.log("icon.offsetTop" + icon.offsetTop);
                                        console.log("icon.offsetWidth" + icon.offsetWidth);
                                        console.log("icon.offsetHeight" + icon.offsetHeight);                                        
                                        console.log("icon.parentElement?.offsetLeft" + icon.parentElement?.offsetLeft);                                                 
                                        console.log("icon.parentElement?.offsetTop" + icon.parentElement?.offsetTop);                                                 
                                        console.log("icon.parentElement?.offsetWidth" + icon.parentElement?.offsetWidth);                                                 
                                        console.log("icon.parentElement?.offsetHeight" + icon.parentElement?.offsetHeight);
                                        // The additionals are manually calculated.
                                        // Initial Position is sometimes overriden by game.
                                        trigger(mod.id, "SetPanelPosition", { x: (icon.offsetLeft) / window.innerWidth + .004, y: (icon.offsetTop) / window.innerHeight + .049});
                                    }

                                    
                                }} 
                                src={ContourLinesSrc}
                                focusKey={VanillaComponentResolver.instance.FOCUS_DISABLED}>                          
                            </VanillaComponentResolver.instance.ToolButton>
                        </div>
                    </Panel>
                </Portal>
            )}
        </>
    );
}