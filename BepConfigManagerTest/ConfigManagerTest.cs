using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using ConfigurationManager;
using BepConfigManagerTest.Infrastructure;
using BepConfigManagerTest.Drawers;
using System;

namespace BepConfigManagerTest
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.ConfigManagerTest", "ConfigManagerTest", "0.0.0")]
    public class ConfigManagerTest : BaseUnityPlugin
    {
        private static ConfigEntry<GenericConditional<float>> MyGenericFloat;
        private static ConfigEntry<GenericConditional<int>> MyGenericInt;
        private static ConfigEntry<GenericConditional<double>> MyGenericDouble;
        private static ConfigEntry<bool> MyBool;
        private static ConfigEntry<int> MyInt;
        private static ConfigEntry<ConditionalInt> conditionalInt;
        private static ConfigEntry<Macro> Macro01;
        private static ConfigEntry<Macro> Macro02;
        private static ConfigEntry<Macro> Macro03;
        //private static ConfigEntry<BepInEx.Configuration.KeyboardShortcut> kbs;

        public ConfigManagerTest()
        {
            TomlTypeConverter.AddConverter(typeof(MyClass), MyClass.GetTypeConverter());
            TomlTypeConverter.AddConverter(typeof(Macro), Macro.GetTypeConverter());
            TomlTypeConverter.AddConverter(typeof(ConditionalInt), ConditionalInt.GetTypeConverter());

            ConfigurationManager.ConfigurationManager.RegisterCustomSettingDrawer(typeof(Macro), new MacroDrawer().Draw());
            ConfigurationManager.ConfigurationManager.RegisterCustomSettingDrawer(typeof(ConditionalInt), new ConditionalIntDrawer().Draw());
        }

        public void Awake()
        {
            const string macroSection = "Macroes";
            const string otherSection = "AAAAAA";

            conditionalInt = Config.AddSetting<ConditionalInt>(
                otherSection,
                "ConditionalInt",
                new ConditionalInt { Condition=false, Value=0 },
                new ConfigDescription("Description of bool")
               );

            MyGenericFloat = Config.AddSetting<GenericConditional<float>>(
                "GENERIC",
                "float",
                new GenericConditional<float> { Condition = false, Value = 5.5f },
                new ConfigDescription("Description of bool")
               );

            MyGenericInt = Config.AddSetting<GenericConditional<int>>(
                "GENERIC",
                "int",
                new GenericConditional<int> { Condition = false, Value = 5 },
                new ConfigDescription("Description of bool")
               );
            MyGenericDouble = Config.AddSetting<GenericConditional<double>>(
                "GENERIC",
                "double",
                new GenericConditional<double> { Condition = false, Value = 5.55 },
                new ConfigDescription("Description of bool")
               );

            MyBool = Config.AddSetting<bool>(
                 otherSection,
                 "MyBool",
                 false,
                 new ConfigDescription("Description of bool")
                );

            MyInt = Config.AddSetting<int>(
                 otherSection,
                 "MyInt",
                 5,
                 new ConfigDescription(
                     "Description of int",                 
                     null,                 
                     new ConditionalFieldDrawer().Draw(MyBool)
                     ));

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
