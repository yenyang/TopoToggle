import { ModRegistrar } from "cs2/modding";
import { VanillaComponentResolver } from "mods/VanillaComponentResolver/VanillaComponentResolver";
import { TopoPanelComponent } from "mods/TopoPanelComponent/TopoPanelComponent";
import mod from "../mod.json";
import { ToolOptionsVisibility } from "mods/ToolOptionsVisible/toolOptionsVisible";
import { TopLeftTopoToggleComponent } from "mods/TopLeftTopoToggleComponent/TopLeftTopoToggleComponent";
import { TopRightTopoToggleComponent } from "mods/TopRightTopoToggleComponent/TopRightTopoToggleComponent";
import { BottomRightTopoToggleComponent } from "mods/BottomRightTopoToggleComponent/BottomRightTopoToggleComponent";
import { UniversalModButtonTopoToggleComponent } from "mods/UniversalModButtonTopoToggleComponent/UniversalModButtonTopoToggleComponent";

const register: ModRegistrar = (moduleRegistry) => {
      // The vanilla component resolver is a singleton that helps extrant and maintain components from game that were not specifically exposed.
      VanillaComponentResolver.setRegistry(moduleRegistry);
      
     // console.log('mr', moduleRegistry);

     moduleRegistry.append('Game', TopoPanelComponent);
     moduleRegistry.append('Editor', TopoPanelComponent);     
     moduleRegistry.append('GameTopLeft', TopLeftTopoToggleComponent);
     moduleRegistry.append('GameTopRight', TopRightTopoToggleComponent);
     moduleRegistry.append('GameBottomRight', BottomRightTopoToggleComponent);
     moduleRegistry.append('UniversalModMenu', UniversalModButtonTopoToggleComponent);

     // Ensures tool option is visible for water tool in the editor.
      moduleRegistry.extend("game-ui/game/components/tool-options/tool-options-panel.tsx", 'useToolOptionsVisible', ToolOptionsVisibility);


     // This is just to verify using UI console that all the component registriations was completed.
     console.log(mod.id + " UI module registrations completed.");
}

export default register;