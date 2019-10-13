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
                GUILayout.BeginVertical();
                DrawStringBox(seb);
                DrawIntBox(seb);
                DrawKeyboardShortcut(seb);
                GUILayout.Label("_____________________________________"); //I know this is ugly but its a temp solution
                GUILayout.EndVertical();
            };
        }

        
        private Texture2D GetBackground()
        {
            var background = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            background.SetPixel(0, 0, Color.black);
            background.Apply();
            return background;
        }
        private void DrawStringBox(SettingEntryBase seb)
        {
            var style = new GUIStyle
            {
                normal = new GUIStyleState { textColor = Color.white, background = GetBackground() },
                wordWrap = true                
            };

            GUILayout.BeginHorizontal();
            GUILayout.Label("MacroString");
            var macro = (Macro)seb.Get();
            var text = macro.MacroString;
            var result = GUILayout.TextField(text, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            if (result != text)
            {
                try
                {
                    macro.MacroString = result;
                    seb.Set(macro);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            GUILayout.EndHorizontal();
        }

        private void DrawIntBox(SettingEntryBase seb)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("RepeatCount");
            var macro = (Macro)seb.Get();
            var repeatCount = macro.RepeatNumber.ToString();
            var result = GUILayout.TextField(repeatCount, GUILayout.ExpandWidth(true));
            if (result != repeatCount)
            {
                try
                {
                    macro.RepeatNumber = Int32.Parse(result);
                    seb.Set(macro);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            GUILayout.EndHorizontal();
        }

        private static readonly IEnumerable<KeyCode> _keysToCheck =
            BepInEx.Configuration.KeyboardShortcut
            .AllKeyCodes
            .Except(new[] { KeyCode.Mouse0, KeyCode.None })
            .ToArray();

        private List<KeyCode> keyCodeList = new List<KeyCode>();
        private bool edittingKbs = false;
        private void DrawKeyboardShortcut(SettingEntryBase seb)
        {
            GUILayout.BeginHorizontal();
            var macro = (Macro)seb.Get();
            if (edittingKbs)
            {
                GUIUtility.keyboardControl = -1;
                Event e = Event.current;
                if (e.isKey
                    && _keysToCheck.Contains(e.keyCode)
                    && !keyCodeList.Contains(e.keyCode))
                {
                    keyCodeList.Add(e.keyCode);
                }

                if (keyCodeList.Count > 0)
                {
                    var sb = new StringBuilder();
                    foreach (var code in keyCodeList)
                    {
                        sb.Append(code.ToString());
                        if (keyCodeList.Last() != code)
                        {
                            sb.Append(" + ");
                        }
                    }
                    GUILayout.Label(sb.ToString(), GUILayout.ExpandWidth(true));
                }
                else
                {
                    GUILayout.Label("Press any key combination", GUILayout.ExpandWidth(true));
                }

                if (GUILayout.Button("OK", GUILayout.ExpandWidth(false)))
                {
                    if (keyCodeList.Count > 0)
                    {
                        if (keyCodeList.Count > 1)
                        {
                            macro.KeyboardShortcut = new BepInEx.Configuration.KeyboardShortcut(keyCodeList[0], keyCodeList.Skip(1).ToArray());
                        }
                        else
                        {
                            macro.KeyboardShortcut = new BepInEx.Configuration.KeyboardShortcut(keyCodeList[0]);
                        }
                        seb.Set(macro);
                    }
                    keyCodeList.Clear();
                    edittingKbs = false;
                }

                if (GUILayout.Button("Cancel", GUILayout.ExpandWidth(false)))
                {
                    keyCodeList.Clear();
                    edittingKbs = false;
                }
            }
            else
            {
                GUILayout.Label("Shortcut: ");
                if (GUILayout.Button(macro.KeyboardShortcut.ToString(), GUILayout.ExpandWidth(true)))
                {
                    edittingKbs = true;
                }

                if (GUILayout.Button("Clear", GUILayout.ExpandWidth(false)))
                {
                    macro.KeyboardShortcut = BepInEx.Configuration.KeyboardShortcut.Empty;
                    seb.Set(macro);
                    edittingKbs = false;
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}
