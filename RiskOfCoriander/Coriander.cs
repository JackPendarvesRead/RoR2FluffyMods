using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RiskOfCoriander
{
    public class Coriander : MonoBehaviour
    {
        public Coriander(CorianderType corianderType, float coarseness)
        {
            CorianderType = corianderType;
            Coarseness = coarseness;
        }

        public float Coarseness { get; private set; }
        public CorianderType CorianderType { get; private set; }
    }
}
