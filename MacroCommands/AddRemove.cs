//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace MacroCommands
//{
//    class AddRemove
//    {

//        private void StarT()
//        {
//            var bUtil = new ButtonUtil(this.Config);
//            bUtil.AddButtonConfig("Advanced",
//                "Add Macro",
//                "Press this button to add additional macro ConfigEntry",
//                GetButtonDictionary(),
//                false,
//                new ConfigurationManagerAttributes { HideDefaultButton = true, IsAdvanced = true });
//        }

//        private Dictionary<string, Action> GetButtonDictionary()
//        {
//            return new Dictionary<string, Action>
//            {
//                { "Add", AddNewMacro }
//            };
//        }
//        private void AddNewMacro()
//        {
//            if (Macros.Count < 100)
//            {
//                var mUtil = new MacroUtil(this.Config);
//                var number = (Macros.Count + 1).ToString("00");
//                var macro = mUtil.AddMacroConfig(macroSection, $"Macro {number}", "Type Macro into the black box. Commands are seperated by ';'",
//                    new ConfigurationManagerAttributes { IsAdvanced = true, HideDefaultButton = true });
//                Macros.Add(macro);
//            }
//        }
//    }
//}
