using BepInEx;
using System;

[BepInDependency("com.bepis.r2api")]
[BepInPlugin("com.FluffyMods.NAMEOFTHEMOD", "NAMEOFTHEMOD", "1.0.1")]
public class ComparableConfigWrapper<T> : BaseUnityPlugin
    where T : IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
{
    private static BepInEx.Configuration.ConfigWrapper<T> ConfigWrapper;
    private readonly T minValue;
    private readonly T maxValue;

    public T Value
    {
        get
        {
            if(ConfigWrapper.Value.CompareTo(maxValue) > 0)
            {
                return maxValue;
            }
            if(ConfigWrapper.Value.CompareTo(minValue) < 0)
            {
                return minValue;
            }
            return ConfigWrapper.Value;
        }
        set
        {
            ConfigWrapper.Value = value;
        }
    }

    public ComparableConfigWrapper(string sectionName, string key, string description, T defaultValue, T minValue, T maxValue)
    {
        ConfigWrapper = Config.Wrap(sectionName, key, description, defaultValue);
        this.minValue = minValue;
        this.maxValue = maxValue;
    }
}

