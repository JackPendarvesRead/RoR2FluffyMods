using CustomCharacterBuilder.Infrastructure;
using R2API.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CustomCharacterBuilder.Logic
{
    public static class SkillAssemblyScanner
    {
        public static IEnumerable<ICustomSkill> GetSkillsFromAssembly(Assembly assembly)
        {
            return assembly.DefinedTypes
                .Where(typeInfo => typeof(ICustomSkill).IsAssignableFrom(typeInfo) && !typeInfo.IsInterface && !typeInfo.IsAbstract)
                .Select(typeInfo => (ICustomSkill)typeInfo.Instantiate());
        }
    }
}
