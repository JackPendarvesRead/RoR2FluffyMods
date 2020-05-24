using BepInEx;
using CustomCharacterBuilder;
using CustomCharacterBuilder.Infrastructure;
using CustomCharacterBuilder.Logic;
using EntityStates.MyCustomCharacter;
using R2API.Utils;
using RoR2;
using System.Reflection;

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

            var characterInfo = new CharacterInformation(
                "MyCharacterName",
                "This is the description of my character",
                SkillAssemblyScanner.GetSkillsFromAssembly(Assembly.GetExecutingAssembly())
                );   
            
            var bodyInfo = new PrefabInfo("BodyName", PrefabConstants.CommandoBody);
            var displayInfo = new PrefabInfo("DisplayName", PrefabConstants.CommandoDisplay);
            CharacterCreator.Create<MyCustomCharacter>(characterInfo, bodyInfo, displayInfo);
        }       
    }
}
