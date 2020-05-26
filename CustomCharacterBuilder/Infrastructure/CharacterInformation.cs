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
        public IEnumerable<ICustomSkill> Skills { get; set; }
        public string UnlockableName { get; set; }
        public Color PrimaryColour { get; set; }
        public CustomBodyStats CustomStats { get; set; }
        public GameObject PreferredPod { get; set; }
        public Texture PortraitIcon { get; set; }
    }
}
