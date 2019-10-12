using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepConfigManagerTest.Infrastructure
{
    class Macro
    {
        public string Value { get; set; }
        public BepInEx.Configuration.KeyboardShortcut KeyboardShortcut { get; set; }

        public static TypeConverter GetTypeConvert()
        {
            return new TypeConverter
            {
                ConvertToString = (obj, type) =>
                {
                    var macro = (Macro)obj;
                    var kb = macro.KeyboardShortcut.Serialize();
                    return macro.Value + "\0" + kb;
                },
                ConvertToObject = (s, type) =>
                {
                    var split = s.Split('\0');
                    return new Macro
                    {
                        Value = split[0],
                        KeyboardShortcut = BepInEx.Configuration.KeyboardShortcut.Deserialize(split[1])
                    };
                }
            };
        }
    }
}
