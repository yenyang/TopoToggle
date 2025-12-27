import { ModRegistrar } from "cs2/modding";
import { VanillaComponentResolver } from "mods/VanillaComponentResolver/VanillaComponentResolver";
import { TopoPanelComponent } from "mods/editorWaterToolPrefabSelection/TopoPanelComponent";
import mod from "../mod.json";

const register: ModRegistrar = (moduleRegistry) => {
      // The vanilla component resolver is a singleton that helps extrant and maintain components from game that were not specifically exposed.
      VanillaComponentResolver.setRegistry(moduleRegistry);
      
     // console.log('mr', moduleRegistry);

     moduleRegistry.append('Game', TopoPanelComponent);
     moduleRegistry.append('Editor', TopoPanelComponent);     

     // This is just to verify using UI console that all the component registriations was completed.
     console.log(mod.id + " UI module registrations completed.");
}

export default register;