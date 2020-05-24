using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace CustomCharacterBuilder.Infrastructure
{
    public class CharacterInformation
    {
        public CharacterInformation(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public Color PrimaryColour { get; set; }
        public CustomBodyStats CustomStats { get; set; }
    }
}
