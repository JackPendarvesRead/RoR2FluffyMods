using BepInEx;
using BepInEx.Configuration;
using ConfigurationManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BepConfigManagerTest
{
    class ConfigButton
    {
        private readonly BaseUnityPlugin plugin;

        public ConfigButton(BaseUnityPlugin plugin)
        {
            this.plugin = plugin;
        }

        public void AddButtonConfig(ConfigDefinition configDefinition, string buttonName, Action buttonLogic)
        {
            AddButtonConfig(configDefinition.Section, configDefinition.Key, buttonName, buttonLogic);
        }
        public void AddButtonConfig(string section, string key, string buttonName, Action buttonLogic)
        {
            plugin.Config.AddSetting<string>(
                section,
                key,
                "",
                new BepInEx.Configuration.ConfigDescription(
                    "This is a button",
                    null,
                    ConfigButtonCreateActions.Create(buttonName, buttonLogic)
                    ));
        }

        public void AddMultipleButtonConfig(ConfigDefinition configDefinition, Dictionary<string, Action> buttonDic)
        {
            AddMultipleButtonConfig(configDefinition.Section, configDefinition.Key, buttonDic);
        }
        public void AddMultipleButtonConfig(string section, string key, Dictionary<string, Action> buttonDic)
        {
            plugin.Config.AddSetting<string>(
                section,
                key,
                "",
                new BepInEx.Configuration.ConfigDescription(
                    "This is a button",
                    null,
                    ConfigButtonCreateActions.CreateMultiple(buttonDic)
                    ));
        }
    }

    public static class ConfigButtonCreateActions
    {
        public static Action<SettingEntryBase> Create(string buttonName, Action buttonLogic)
        {
            return (seb) =>
            {
                bool PressButton()
                {
                    return GUILayout.Button(buttonName, GUILayout.ExpandWidth(true));
                }
                if (PressButton())
                {
                    buttonLogic();
                }
            };
        }

        public static Action<SettingEntryBase> CreateMultiple(Dictionary<string, Action> buttonDictionary)
        {
            return (seb) =>
            {
                foreach (var button in buttonDictionary)
                {
                    bool PressButton()
                    {
                        return GUILayout.Button(button.Key, GUILayout.ExpandWidth(true));
                    }
                    if (PressButton())
                    {
                        button.Value();
                    }
                }
            };
        }
    }
}
