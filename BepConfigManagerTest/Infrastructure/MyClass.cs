using BepInEx.Configuration;
using ConfigurationManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BepConfigManagerTest.Infrastructure
{ 
    [System.Serializable]
    public struct MyClass
    {
        public string MyString { get; set; }
        public KeyboardShortcut kbs { get; set; }
        public int MyInt { get; set; }

        public static TypeConverter GetTypeConverter()
        {
            return new TypeConverter
            {
                ConvertToString = (obj, type) =>
                {
                    try
                    {
                        var x = (MyClass)obj;
                        var serialisedKbs = x.kbs.Serialize();
                        return string.Join(Special.Delimiter.ToString(), x.MyInt.ToString(), x.MyString, serialisedKbs);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                        throw e;
                    }

                },
                ConvertToObject = (s, type) =>
                {
                    try
                    {
                        var split = s.Split(Special.Delimiter);
                        return new MyClass
                        {
                            MyInt = Int32.Parse(split[0]),
                            MyString = split[1],
                            kbs = KeyboardShortcut.Deserialize(split[2])
                        };
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                        throw e;
                    }
                }
            };
        }
    }
}