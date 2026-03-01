import { Button } from "cs2/ui";
import ContourLinesSrc from "../../images/ContourLines.svg";
import { bindValue, trigger, useValue } from "cs2/api";
import mod from "../../../mod.json";
import styles from "../TopLeftTopoToggleComponent/topBoundStyles.module.scss";
import classNames from "classnames";
import { useLocalization } from "cs2/l10n";
import locale from "../lang/en-US.json";
import { GameToggleOptions } from "Domain/GameToggleOptions";

// These establishes the binding with C# side. Without C# side game ui will crash.
const ForceContourLines$ = bindValue(mod.id, "ForceContourLines", false);
const ShowTerrainElevation$ = bindValue(mod.id, "ShowTerrainElevation", false);
const TerrainElevation$ = bindValue(mod.id, "TerrainElevation", ":???");
const GameToggleOption$ = bindValue(mod.id, "GameToggleOption", GameToggleOptions.FloatingPanel);
const HideTopoTogglePanel$ = bindValue(mod.id, "HideTopoTogglePanel", false);

export const TopRightTopoToggleComponent = () => 
{
    const GameToggleOption = useValue(GameToggleOption$); 
    const ForceContourLines = useValue(ForceContourLines$);    
    const ShowTerrainHitPosition = useValue(ShowTerrainElevation$);
    const TerrainElevation = useValue(TerrainElevation$);
    const HideTopoTogglePanel = useValue(HideTopoTogglePanel$);

    const { translate } = useLocalization();
    
    return     (
        <>
            {GameToggleOption == GameToggleOptions.TopRight && !HideTopoTogglePanel && (
                <div className={styles.columnGroup}>
                    <>
                        <Button
                            src={ContourLinesSrc}
                            variant="floating"
                            className={classNames({ [styles.selected]: ForceContourLines }, styles.toggle)}
                            onSelect={() => trigger(mod.id, "ToggleContourLines")}
                        />
                        { ShowTerrainHitPosition && 
                            <div className={classNames(styles.smallSize, styles.absolutePosition)}>{ translate("TopoToggle.Text_Label_[ElevationAbbreviation]" ,locale["TopoToggle.Text_Label_[ElevationAbbreviation]"])+ TerrainElevation}</div>
                        }
                    </> 
                </div>
            )}
       </>
    );

}