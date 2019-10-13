using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BepConfigManagerTest.Infrastructure
{
    class Macro
    {
        public string MacroString { get; set; }
        public int RepeatNumber { get; set; }
        public BepInEx.Configuration.KeyboardShortcut KeyboardShortcut { get; set; }

        public static TypeConverter GetTypeConvert()
        {
            return new TypeConverter
            {
                ConvertToString = (obj, type) =>
                {
                    try
                    {
                        var macro = (Macro)obj;
                        var kb = macro.KeyboardShortcut.Serialize();
                        return macro.MacroString + Special.Delimiter + macro.RepeatNumber.ToString() + Special.Delimiter + kb;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                        throw;
                    }
                },
                ConvertToObject = (s, type) =>
                {
                    try
                    {
                        var split = s.Split(Special.Delimiter);
                        return new Macro
                        {
                            MacroString = split[0],
                            RepeatNumber = Int32.Parse(split[1]),
                            KeyboardShortcut = BepInEx.Configuration.KeyboardShortcut.Deserialize(split[2])
                        };
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                        throw;
                    }
                }
            };
        }
    }
}
