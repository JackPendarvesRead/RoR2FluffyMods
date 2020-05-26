using BepInEx;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using R2API.Utils;
using RoR2;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace TestStuff
{
    [BepInPlugin(PluginGuid, pluginName, pluginVersion)]
    public class TestStuff : BaseUnityPlugin
    {
        public const string PluginGuid = "com.FluffyMods." + pluginName;
        private const string pluginName = "TestStuff";
        private const string pluginVersion = "1.0.0";

        public void Awake()
        {
             var b = new CharacterBody();
            CreateHook<CharacterBody, float>(nameof(b.RecalculateStats), nameof(b.maxHealth), (maxHealth, body) =>
            {
                if (IsHumanPlayer(body))
                {
                    Debug.Log($"{body.name} is WORTHY! HAVE SOME HEALTH!");
                    return maxHealth + 500;
                }
                else
                {
                    Debug.Log($"{body.name} is UNWORTHY!");
                    return 1;
                }
            });

            CreateHook<CharacterBody, float>(nameof(b.RecalculateStats), nameof(b.attackSpeed), (attackSpeed, body) =>
            {
                if (IsHumanPlayer(body))
                {
                    Debug.Log($"{body.name} is WORTHY! HAVE SOME ATTACK SPEED!");
                    return attackSpeed + 500;
                }
                else
                {
                    Debug.Log($"{body.name} is UNWORTHY!");
                    return 1;
                }
            });

            CreateHook<CharacterBody, float>(nameof(b.RecalculateStats), nameof(b.moveSpeed), (moveSpeed, body) =>
            {
                if (IsHumanPlayer(body))
                {
                    Debug.Log($"{body.name} is WORTHY! HAVE SOME MOVE SPEED!");
                    return moveSpeed * 2;
                }
                else
                {
                    Debug.Log($"{body.name} is UNWORTHY!");
                    return 1;
                }
            });
        } 

        private void CreateHook<TOrig, TProperty>(string methodName, string propertyName, Func<TProperty, TOrig, TProperty> func)
        {
            void action(ILContext il)
            {
                var c = new ILCursor(il);
                c.GotoNext(x => x.MatchCallvirt<TOrig>("set_" + propertyName));
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate(func);
            }

            var config = new ILHookConfig { ManualApply = true };
            var hook = new ILHook(
                typeof(TOrig).GetMethod(methodName),
                new ILContext.Manipulator(action),
                config);
            hook.Apply();
        }

        private bool IsHumanPlayer(CharacterBody body)
        {
            return NetworkUser.readOnlyInstancesList                                    
                .Select(x => x.GetCurrentBody())                                    
                .Where(x => x == body)                                    
                .Any();
        }
    }
}
