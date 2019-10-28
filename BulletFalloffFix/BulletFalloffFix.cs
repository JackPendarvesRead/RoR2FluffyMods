﻿using BepInEx;
using BepInEx.Configuration;
using FluffyLabsConfigManagerTools.Util;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BulletFalloffFix
{
    [BepInPlugin("com.FluffyMods.BulletFalloffFix", "BulletFalloffFix", "2.0.0")]
    public class BulletFalloffFix : BaseUnityPlugin
    {
        private static ConfigEntry<float> FallOffStartDistance;
        private static ConfigEntry<float> FallOffEndDistance;

        public void Awake()
        {
            if (!RoR2Application.isModded)
            {
                RoR2Application.isModded = true;
            }

            #region ConfigSetup
            const string falloffDistanceSection = "Falloff Distance";
            const string presetSection = "Presets";

            var buttonUtil = new ButtonUtil(this.Config);
            buttonUtil.AddButtonConfig(presetSection, "Buttons", "", GetButtonDic());

            FallOffStartDistance = Config.AddSetting<float>(
                new ConfigDefinition(falloffDistanceSection, nameof(FallOffStartDistance)),
                40f,
                new ConfigDescription(
                    "Set the distance at which damage starts to fall off (default=25, recommended=40)"
                    )
                );

            FallOffEndDistance = Config.AddSetting<float>(
                new ConfigDefinition(falloffDistanceSection, nameof(FallOffEndDistance)),
                80f,
                new ConfigDescription(
                    "Set the distance at which damage reaches minimum (default=60, recommended=80)"
                    )
                );
            #endregion


            IL.RoR2.BulletAttack.DefaultHitCallback += BulletAttack_DefaultHitCallback;
        }

        private void BulletAttack_DefaultHitCallback(MonoMod.Cil.ILContext il)
        {
            var c = new ILCursor(il);
            c.GotoNext(
                x => x.MatchLdcR4(0.5f),  //I know this could be more robust but for now this works...
                x => x.MatchLdcR4(60f),
                x => x.MatchLdcR4(25f)
                );
            c.Index++;
            c.RemoveRange(2);
            c.Emit(OpCodes.Ldc_R4, FallOffEndDistance.Value);
            c.Emit(OpCodes.Ldc_R4, FallOffStartDistance.Value);
        }


        private Dictionary<string, Action> GetButtonDic()
        {
            return new Dictionary<string, Action>
            {
                { "Vanilla", SetVanillaConfig },
                { "Recommended", SetRecommenedConfig}
            };
        }
        private void SetVanillaConfig()
        {
            FallOffStartDistance.Value = BulletFalloffConstantValues.DefaultStart;
            FallOffEndDistance.Value = BulletFalloffConstantValues.DefaultEnd;
            Debug.Log("Default falloff values set");
        }
        private void SetRecommenedConfig()
        {
            FallOffStartDistance.Value = BulletFalloffConstantValues.RecommendedStart;
            FallOffEndDistance.Value = BulletFalloffConstantValues.RecommendedEnd;
            Debug.Log("Recommended falloff values set");
        }        
    }
}
