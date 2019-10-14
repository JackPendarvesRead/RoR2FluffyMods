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
    class ConditionalIntDrawer
    {
        public Action<SettingEntryBase> Draw()
        {
            return (seb) =>
            {
                GUILayout.BeginHorizontal();
                var x = (ConditionalInt)seb.Get();
                var condition = x.Condition;
                var newCondition = GUILayout.Toggle(condition, "Tick Here");
                if(condition != newCondition)
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
                            x.Value = Int32.Parse(result);
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
