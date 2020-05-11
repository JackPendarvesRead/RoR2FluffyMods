using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RiskOfCoriander
{
    public static class CorrianderExtension
    {
        public static void AddCorriander(this GameObject gameObject)
        {
            var coriander = gameObject.AddComponent<Coriander>();
            coriander.Coarseness = RiskOfCoriander.Coarseness.Value;
            coriander.CorianderType = RiskOfCoriander.CorianderFreshness.Value;
        }
    }
}
