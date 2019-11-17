using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using EntityStates;
using R2API.Utils;
using RoR2;
using System.Reflection;
using UnityEngine;

namespace RoR2FluffyMods
{
    [BepInPlugin(PluginGuid, pluginName, pluginVersion)]
    public class Test : BaseUnityPlugin
    {
        public const string PluginGuid = "com.FluffyMods." + pluginName;
        private const string pluginName = "Test";
        private const string pluginVersion = "1.0.0";

        private static readonly string text = "This is the original string";

        public void Awake()
        {
            Debug.Log("DOING THE THING HIAFNASKFNAL:FNMAS:KFJMAS:KLFJAS:");
            Logger.LogInfo(text);
            typeof(Test)
                .GetField("text", BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(null, "This is the new string");
            Logger.LogInfo(text);
        }
    }
}
