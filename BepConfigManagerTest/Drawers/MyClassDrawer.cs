using BepConfigManagerTest.Infrastructure;
using BepInEx.Configuration;
using ConfigurationManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BepConfigManagerTest.Drawers
{
    public class MyClassDrawer
    {
        public Action<SettingEntryBase> Draw()
        {
            return (seb) =>
            {
                GUILayout.BeginVertical();
                DrawStringBox(seb);
                DrawIntBox(seb);
                DrawKeyboardShortcut(seb);
                GUILayout.EndVertical();
            };
        }

        private void DrawStringBox(SettingEntryBase seb)
        {
            GUILayout.Label("MyString");
            var myClass = (MyClass)seb.Get();
            var text = myClass.MyString;
            var result = GUILayout.TextField(text, GUILayout.ExpandWidth(true));
            if (result != text)
            {
                try
                {
                    Debug.Log(result);
                    myClass.MyString = result;
                    Debug.Log($"{myClass.MyInt.ToString()}, {myClass.MyString}, {myClass.kbs.ToString()}");
                    seb.Set(myClass);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
        }

        private void DrawIntBox(SettingEntryBase seb)
        {
            GUILayout.Label("MyInt");
            var myClass = (MyClass)seb.Get();
            var number = myClass.MyInt.ToString();
            var result = GUILayout.TextField(number, GUILayout.ExpandWidth(true));
            if (result != number)
            {
                try
                {
                    Debug.Log(result);
                    myClass.MyInt = Int32.Parse(result);
                    Debug.Log($"{myClass.MyInt.ToString()}, {myClass.MyString}, {myClass.kbs.ToString()}");
                    seb.Set(myClass);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
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
            var myClass = (MyClass)seb.Get();
            GUILayout.BeginHorizontal();
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
                            myClass.kbs = new BepInEx.Configuration.KeyboardShortcut(keyCodeList[0], keyCodeList.Skip(1).ToArray());
                        }
                        else
                        {
                            myClass.kbs = new BepInEx.Configuration.KeyboardShortcut(keyCodeList[0]);
                        }
                        seb.Set(myClass);
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
                if (GUILayout.Button(myClass.kbs.ToString(), GUILayout.ExpandWidth(true)))
                {
                    edittingKbs = true;
                }

                if (GUILayout.Button("Clear", GUILayout.ExpandWidth(false)))
                {
                    myClass.kbs = BepInEx.Configuration.KeyboardShortcut.Empty;
                    seb.Set(myClass);
                    edittingKbs = false;
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}