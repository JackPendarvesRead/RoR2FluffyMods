using BepInEx;
using CustomCharacterBuilder;
using CustomCharacterBuilder.Infrastructure;
using CustomCharacterBuilder.Logic;
using EntityStates.MyCustomCharacter;
using R2API.Utils;
using RoR2;

namespace CustomCharacterPlay
{
    [BepInDependency(CustomCharacterBuilderPlugin.PluginGuid, BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(PluginGuid, pluginName, pluginVersion)]
    public class CustomCharacterPlay : BaseUnityPlugin
    {        
        public const string PluginGuid = "com.FluffyMods." + pluginName;
        private const string pluginName = "CustomCharacterPlay";
        private const string pluginVersion = "1.0.0";

        public void Awake()
        {
            if (!RoR2Application.isModded)
            {
                RoR2Application.isModded = true;
            }

            var characterInfo = new CharacterInformation("MyCharacterName", "This is the description of my character");
            var bodyInfo = new PrefabInfo("BodyName", PrefabConstants.CommandoBody);
            var displayInfo = new PrefabInfo("DisplayName", PrefabConstants.CommandoDisplay);
            new CharacterCreator().Create<MyCustomCharacter>(characterInfo, bodyInfo, displayInfo);
        }       
    }
}
