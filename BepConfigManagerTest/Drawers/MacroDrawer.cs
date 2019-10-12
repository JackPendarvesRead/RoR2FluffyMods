using BepConfigManagerTest.Infrastructure;
using ConfigurationManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace BepConfigManagerTest.Drawers
{
    class MacroDrawer : IDrawer
    {
        public Action<SettingEntryBase> Draw()
        {
            return (seb) =>
            {
                var macro = (Macro)seb.Get();
                var before = seb.ObjToStr(macro);
                DrawStringBox(seb, macro);
                DrawKeyboardShortcut(seb, macro);                
            };
        }

        private void DrawStringBox(SettingEntryBase seb, Macro macro)
        {
            if(seb.ObjToStr != null
                && seb.StrToObj != null)
            {
                var text = macro.Value;
                var result = GUILayout.TextField(text, GUILayout.Width(180.0f), GUILayout.Height(80.0f));
                if (result != text)
                {
                    macro.Value = result;
                    seb.Set(macro);
                }
            }
        }

        private bool edit = false;
        private readonly IEnumerable<KeyCode> _keysToCheck = BepInEx.Configuration.KeyboardShortcut.AllKeyCodes.Except(new[] { KeyCode.Mouse0 }).ToArray();
        private void DrawKeyboardShortcut(SettingEntryBase seb, Macro macro)
        {
            if (!edit)
            {
                edit = GUILayout.Button(macro.KeyboardShortcut.ToString(), GUILayout.ExpandWidth(true));
            }             
            if (edit)
            {
                GUILayout.Label("Press a key", GUILayout.ExpandWidth(true));
                GUIUtility.keyboardControl = -1;

                foreach (var key in _keysToCheck)
                {

                    if (Input.GetKeyDown(key))
                    {
                        macro.KeyboardShortcut = new BepInEx.Configuration.KeyboardShortcut(key);
                        seb.Set(macro);
                        //macro.KeyboardShortcut = new BepInEx.Configuration.KeyboardShortcut(key, _keysToCheck.Where(Input.GetKey).ToArray());
                        break;
                    }
                }                
                if (GUILayout.Button("Cancel", GUILayout.ExpandWidth(false)))
                {
                     edit = false;
                }
            }
            else
            {               
                if (GUILayout.Button("Clear", GUILayout.ExpandWidth(false)))
                {
                    macro.KeyboardShortcut = BepInEx.Configuration.KeyboardShortcut.Empty;
                }
            }
        }
    }
}
