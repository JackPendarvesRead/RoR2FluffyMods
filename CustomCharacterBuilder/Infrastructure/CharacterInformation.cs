using System.Collections.Generic;
using UnityEngine;

namespace CustomCharacterBuilder.Infrastructure
{
    public class CharacterInformation
    {
        public CharacterInformation(string name, string description, IEnumerable<ICustomSkill> skills)
        {
            Name = name;
            Description = description;
            Skills = skills;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public Color PrimaryColour { get; set; }
        public CustomBodyStats CustomStats { get; set; }
        public IEnumerable<ICustomSkill> Skills { get; set; }
    }
}
