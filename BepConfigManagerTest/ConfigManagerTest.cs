using BepInEx;
using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using UnityEngine;
using ConfigurationManager;

namespace BepConfigManagerTest
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.ConfigManagerTest", "ConfigManagerTest", "0.0.0")]
    public class ConfigManagerTest : BaseUnityPlugin
    {
        private static ConfigEntry<MyClass> MacroConfig;
        private static ConfigEntry<BepInEx.Configuration.KeyboardShortcut> kbs;

        public ConfigManagerTest()
        {
            TomlTypeConverter.AddConverter(typeof(MyClass), MyClass.GetTypeConverter());
            //ConfigurationManager.ConfigurationManager.RegisterCustomSettingDrawer(typeof(Macro), HarbDrawer.Create());
        }

        public void Awake()
        {
            kbs = Config.AddSetting<BepInEx.Configuration.KeyboardShortcut>(
               "keyboard",
               "Key",
               new BepInEx.Configuration.KeyboardShortcut(KeyCode.U),
               new ConfigDescription(
                   "My Description"));

            MacroConfig = Config.AddSetting<MyClass>(
                 "Custom",
                 "Key",
                 new MyClass
                 {
                     MyString = "DefaultString",
                     MyInt = 100,
                     kbs = new BepInEx.Configuration.KeyboardShortcut(KeyCode.A)
                },
                new ConfigDescription(
                    "My Description",
                    null,
                    new MyClassDrawer().Create()                
                ));
        }



        private void DoSomeStuff(SettingEntryBase seb)
        {
            GUILayout.Label("THIS IS A LABEL", GUILayout.ExpandWidth(true));
        }        
    }
}
