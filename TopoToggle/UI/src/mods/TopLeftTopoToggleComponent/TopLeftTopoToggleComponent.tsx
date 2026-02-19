import { Button } from "cs2/ui";
import ContourLinesSrc from "../../images/ContourLines.svg";
import { bindValue, trigger, useValue } from "cs2/api";
import mod from "../../../mod.json";
import styles from "./topBoundStyles.module.scss";
import classNames from "classnames";
import { useLocalization } from "cs2/l10n";
import locale from "../lang/en-US.json";

// These establishes the binding with C# side. Without C# side game ui will crash.
const ForceContourLines$ = bindValue(mod.id, "ForceContourLines", false);
const ShowTerrainElevation$ = bindValue(mod.id, "ShowTerrainElevation", false);
const TerrainElevation$ = bindValue(mod.id, "TerrainElevation", ":???");

export const TopLeftTopoToggleComponent = () => 
{

    const ForceContourLines = useValue(ForceContourLines$);    
    const ShowTerrainHitPosition = useValue(ShowTerrainElevation$);
    const TerrainElevation = useValue(TerrainElevation$);

    const { translate } = useLocalization();
    
    return     (
        <div className={styles.columnGroup}>
            <>
                <Button
                    src={ContourLinesSrc}
                    variant="floating"
                    className={classNames({ [styles.selected]: ForceContourLines }, styles.toggle)}
                    onSelect={() => trigger(mod.id, "ToggleContourLines")}
                />
                { ShowTerrainHitPosition && 
                    <div className={classNames(styles.smallSize, styles.relativePosition)}>{ translate("TopoToggle.Text_Label_[ElevationAbbreviation]" ,locale["TopoToggle.Text_Label_[ElevationAbbreviation]"])+ TerrainElevation}</div>
                }
            </> 
       </div>
    );

}