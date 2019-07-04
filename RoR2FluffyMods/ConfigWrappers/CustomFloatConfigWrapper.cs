using BepInEx;

[BepInDependency("com.bepis.r2api")]
[BepInPlugin("com.FluffyMods.NAMEOFTHEMOD", "NAMEOFTHEMOD", "1.0.1")]
public class CustomFloatConfigWrapper : BaseUnityPlugin
{
    private static BepInEx.Configuration.ConfigWrapper<float> ConfigWrapper;
    private readonly float minValue;
    private readonly float maxValue;

    public float Value
    {
        get
        {
            if (ConfigWrapper.Value > maxValue)
            {
                return maxValue;
            }
            if (ConfigWrapper.Value < minValue)
            {
                return minValue;
            }
            return ConfigWrapper.Value;
        }
        set
        {
            if (ConfigWrapper.Value != value)
            {
                ConfigWrapper.Value = value;
            }
        }
    }

    public CustomFloatConfigWrapper(string sectionName, string key, string description, float defaultValue, float minValue, float maxValue)
    {
        ConfigWrapper = Config.Wrap(sectionName, key, description, defaultValue);
        this.minValue = minValue;
        this.maxValue = maxValue;
    }
}


