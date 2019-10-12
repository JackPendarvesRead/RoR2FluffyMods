using ConfigurationManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepConfigManagerTest.Drawers
{
    public interface IDrawer
    {
        Action<SettingEntryBase> Draw();
    }
}
