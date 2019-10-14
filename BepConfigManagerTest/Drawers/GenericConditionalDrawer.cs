using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepConfigManagerTest.Infrastructure;
using BepInEx.Configuration;
using ConfigurationManager;
using UnityEngine;

namespace BepConfigManagerTest.Drawers
{
    class GenericConditionalDrawer<T>
        where T : struct, IConvertible
    {
        public Action<SettingEntryBase> Draw()
        {
            return (seb) =>
            {
                GUILayout.BeginHorizontal();
                var x = (GenericConditional<T>)seb.Get();
                var condition = x.Condition;

                string label = "Disabled";
                if (x.Condition)
                {
                    label = "Enabled";
                }
                var newCondition = GUILayout.Toggle(condition, label, GUILayout.Width(90f));
                if (condition != newCondition)
                {
                    x.Condition = newCondition;
                    seb.Set(x);
                }
                if (condition)
                {
                    var number = x.Value;
                    var result = GUILayout.TextField(number.ToString(), GUILayout.ExpandWidth(true));
                    if (result != number.ToString())
                    {
                        try
                        {
                            x.Value = (T)Convert.ChangeType(result, typeof(T));
                            seb.Set(x);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(ex);
                        }
                    }
                }
                GUILayout.EndHorizontal();
            };
        }
    }
}
