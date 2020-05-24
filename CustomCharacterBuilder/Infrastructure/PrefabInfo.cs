using System;
using System.Collections.Generic;
using System.Text;

namespace CustomCharacterBuilder.Infrastructure
{
    public class PrefabInfo
    {
        public string ResourceLocationString { get; set; }
        public string Name { get; set; }

        public PrefabInfo(string name, string resourceLocationString)
        {
            Name = name;
            ResourceLocationString = resourceLocationString;
        }
    }
}
