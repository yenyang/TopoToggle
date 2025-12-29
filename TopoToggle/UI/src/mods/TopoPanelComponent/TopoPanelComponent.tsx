import { bindValue, trigger, useValue } from "cs2/api";
import styles from "./TopoPanelComponent.module.scss";
import { Panel, Portal } from "cs2/ui";
import { VanillaComponentResolver } from "mods/VanillaComponentResolver/VanillaComponentResolver";
import mod from "../../../mod.json";
import { useLocalization } from "cs2/l10n";
import { game } from "cs2/bindings";
import ContourLinesSrc from "../../images/ContourLines.svg";
import { CSSProperties, useMemo, useRef, useState } from "react";
import classNames from "classnames";

// These establishes the binding with C# side. Without C# side game ui will crash.
const ForceContourLines$ = bindValue(mod.id, "ForceContourLines", false);
const HideTopoTogglePanel$ = bindValue(mod.id, "HideTopoTogglePanel", false);
const PanelPosition$ = bindValue(mod.id, "PanelPosition", {x: 250, y: 250});
const RecheckPanelPosition$ = bindValue(mod.id, "RecheckPanelPosition", false);
const ShowTerrainElevation$ = bindValue(mod.id, "ShowTerrainElevation", false);
const TerrainElevation$ = bindValue(mod.id, "TerrainElevation", 0);

export const TopoPanelComponent = () => {
    // These get the value of the bindings.
    const ForceContourLines = useValue(ForceContourLines$);
    const isPhotoMode = useValue(game.activeGamePanel$)?.__Type == game.GamePanelType.PhotoMode;
    const HideTopoTogglePanel = useValue(HideTopoTogglePanel$);
    const PanelPosition = useValue(PanelPosition$);
    const RecheckPanelPosition = useValue(RecheckPanelPosition$);
    const ShowTerrainHitPosition = useValue(ShowTerrainElevation$);
    const TerrainElevation = useValue(TerrainElevation$);

    // translation handling. Translates using locale keys that are defined in C# or fallback string here.    
    const { translate } = useLocalization();

    // Getting a consistent panel position is a genuine pain. This JS overrides panel position directly using the value saved directly from previous panel position.
    let panel = document.getElementById('TopoTogglePanel');
    if (panel && RecheckPanelPosition) 
    {
        panel.setAttribute("style", "left: " + PanelPosition.x + "px; top: " + PanelPosition.y + "px;");
    }
    else if (!panel)
    {
        trigger(mod.id, "CheckPanelPosition");
    }

    // The panel should always be there, but will be invisible with no mouse events if it should be hidden.
    return (
        <>
            <Portal>
                <Panel                 
                    id = "TopoTogglePanel"    
                    draggable
                    className={ (isPhotoMode || HideTopoTogglePanel)? classNames(styles.panel, styles.hidden) : styles.panel}
                    header={
                            <>
                                <div className={ ShowTerrainHitPosition? styles.columnGroup : classNames(styles.hidden, styles.columnGroup)}>
                                    <div className={styles.smallSize}>{ TerrainElevation == 0? "E: ???" : "E: " +TerrainElevation.toFixed(2) }</div>
                                </div>
                                <div className={ ShowTerrainHitPosition? classNames(styles.hidden, styles.absolutePosition) : styles.absolutePosition}>TOPO</div>
                            </>} // This is intentionally not translatable.   
                    // Setting initial position here doesn't match up with the panel position that is saved. so use default and override it using JS above.  
                    onMouseUp=
                    {() => 
                        {
                            let panel = document.getElementById('TopoTogglePanel');
                            if (panel) 
                            {
                                trigger(mod.id, "SetPanelPosition", { x: (panel.offsetLeft) , y: (panel.offsetTop) });
                            }
                        }   
                    }         
                    >
                    <div className={styles.panelSection}>
                        <VanillaComponentResolver.instance.ToolButton
                            className={VanillaComponentResolver.instance.toolButtonTheme.button} 
                            selected={ForceContourLines}
                            tooltip={translate("Options.OPTION_DESCRIPTION[InputSettings.Gamepad.Tool/Toggle Contour Lines/binding]", "Toggles the topographic contour lines on and off.")}
                            onSelect={() => 
                            { 
                                trigger(mod.id, "ToggleContourLines");
                            }} 
                            src={ContourLinesSrc}
                            focusKey={VanillaComponentResolver.instance.FOCUS_DISABLED}>                          
                        </VanillaComponentResolver.instance.ToolButton>
                    </div>
                </Panel>
            </Portal>
        </>
    );
}