using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using ConfigurationManager;
using BepConfigManagerTest.Infrastructure;
using BepConfigManagerTest.Drawers;

namespace BepConfigManagerTest
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.ConfigManagerTest", "ConfigManagerTest", "0.0.0")]
    public class ConfigManagerTest : BaseUnityPlugin
    {
        private static ConfigEntry<Macro> Macro01;
        private static ConfigEntry<Macro> Macro02;
        private static ConfigEntry<Macro> Macro03;
        //private static ConfigEntry<BepInEx.Configuration.KeyboardShortcut> kbs;

        public ConfigManagerTest()
        {
            TomlTypeConverter.AddConverter(typeof(MyClass), MyClass.GetTypeConverter());
            TomlTypeConverter.AddConverter(typeof(Macro), Macro.GetTypeConvert());
            ConfigurationManager.ConfigurationManager.RegisterCustomSettingDrawer(typeof(Macro), new MacroDrawer().Draw());
        }

        public void Awake()
        {
            const string macroSection = "Macroes";

            Macro01 = Config.AddSetting<Macro>(
                 macroSection,
                 "Macro 1",
                 new Macro
                 {
                     MacroString = "",
                     RepeatNumber = 1,
                     KeyboardShortcut = new BepInEx.Configuration.KeyboardShortcut(KeyCode.None)
                },
                new ConfigDescription("Description of Macro1")
                );

            Macro02 = Config.AddSetting<Macro>(
                 macroSection,
                 "Macro 2",
                 new Macro
                 {
                     MacroString = "",
                     RepeatNumber = 1,
                     KeyboardShortcut = new BepInEx.Configuration.KeyboardShortcut(KeyCode.None)
                 },
                new ConfigDescription("Description of Macro2")
                );

            Macro03 = Config.AddSetting<Macro>(
                 macroSection,
                 "Macro 3",
                 new Macro
                 {
                     MacroString = "",
                     RepeatNumber = 1,
                     KeyboardShortcut = new BepInEx.Configuration.KeyboardShortcut(KeyCode.None)
                 },
                new ConfigDescription("Description of Macro3")
                );

            //new ConfigButton(this).AddButtonConfig("Button", "button", "Button", () =>
            //{
            //    var x = new MyClass
            //    {
            //        kbs = new BepInEx.Configuration.KeyboardShortcut(KeyCode.Z),
            //        MyInt = 2000,
            //        MyString = "You pushed da button"
            //    };
            //    MacroConfig.Value = x;
            //});
        }    
    }
}
