using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BepConfigManagerTest.Infrastructure
{
    struct ConditionalInt
    {
        public int Value { get; set; }
        public bool Condition { get; set; }

        public static TypeConverter GetTypeConverter()
        {
            return new TypeConverter
            {
                ConvertToObject = (s, type) =>
                {
                    try
                    {
                        var split = s.Split(Special.Delimiter);
                        return new ConditionalInt
                        {
                            Value = Int32.Parse(split[0]),
                            Condition = bool.Parse(split[1])
                        };
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                        throw;
                    }
                },
                ConvertToString = (obj, type) =>
                {
                    try
                    {
                        var x = (ConditionalInt)obj;
                        return string.Join(Special.Delimiter.ToString(), x.Value.ToString(), x.Condition.ToString());
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
