import { Button } from "cs2/ui";
import ContourLinesSrc from "../../images/ContourLines.svg";
import { bindValue, trigger, useValue } from "cs2/api";
import mod from "../../../mod.json";
import styles from "../BottomRightTopoToggleComponent/bottomRightStyles.module.scss";
import classNames from "classnames";
import { useLocalization } from "cs2/l10n";
import locale from "../lang/en-US.json";
import { getModule } from "cs2/modding";
import { GameToggleOptions } from "Domain/GameToggleOptions";

// These establishes the binding with C# side. Without C# side game ui will crash.
const ForceContourLines$ = bindValue(mod.id, "ForceContourLines", false);
const ShowTerrainElevation$ = bindValue(mod.id, "ShowTerrainElevation", false);
const TerrainElevation$ = bindValue(mod.id, "TerrainElevation", ":???");
const GameToggleOption$ = bindValue(mod.id, "GameToggleOption", GameToggleOptions.FloatingPanel);
const HideTopoTogglePanel$ = bindValue(mod.id, "HideTopoTogglePanel", false);

const rightMenuStyles = getModule("game-ui/game/components/right-menu/right-menu.module.scss", "classes");

const rightMenuButtonStyles = getModule("game-ui/game/components/right-menu/right-menu-button.module.scss", "classes");

export const BottomRightTopoToggleComponent = () => 
{
    const ForceContourLines = useValue(ForceContourLines$);    
    const ShowTerrainHitPosition = useValue(ShowTerrainElevation$);
    const TerrainElevation = useValue(TerrainElevation$);
    const GameToggleOption = useValue(GameToggleOption$);    
    const HideTopoTogglePanel = useValue(HideTopoTogglePanel$);

    const { translate } = useLocalization();
    
    return     (
        <>
            {GameToggleOption == GameToggleOptions.BottomRight && !HideTopoTogglePanel && (
                <div className={classNames(styles.bottomRightMargins, ShowTerrainHitPosition? styles.overrideHeight: "")}>
                    <div className={rightMenuStyles.item}>
                            <Button
                                src={ContourLinesSrc}
                                variant="icon"
                                className={classNames( ForceContourLines? "selected": "" , rightMenuButtonStyles.button)}
                                onSelect={() => trigger(mod.id, "ToggleContourLines")}
                            />                
                        
                    </div>
                    { ShowTerrainHitPosition && 
                            <div className={classNames(styles.smallSize, styles.absolutePosition)}>{ translate("TopoToggle.Text_Label_[ElevationAbbreviation]" ,locale["TopoToggle.Text_Label_[ElevationAbbreviation]"])+ TerrainElevation}</div>
                    }
                </div>
            )}
        </>
    );

}