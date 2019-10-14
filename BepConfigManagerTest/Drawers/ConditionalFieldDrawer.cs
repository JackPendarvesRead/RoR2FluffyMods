using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Configuration;
using ConfigurationManager;
using UnityEngine;

namespace BepConfigManagerTest.Drawers
{
    class ConditionalFieldDrawer
    {
        public Action<SettingEntryBase> Draw(ConfigEntry<bool> condition)
        {
            return (seb) =>
            {
                if (condition.Value)
                {
                    DrawIntBox(seb);
                }
                else
                {
                    GUILayout.Label("DISABLED");
                }
            };            
        }

        private void DrawIntBox(SettingEntryBase seb)
        {
            GUILayout.BeginHorizontal();
            var number = seb.Get();
            var result = GUILayout.TextField(number.ToString(), GUILayout.ExpandWidth(true));
            if (result != number.ToString())
            {
                try
                {
                    seb.Set(Int32.Parse(result));
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}
