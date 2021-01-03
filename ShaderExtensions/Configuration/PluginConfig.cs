namespace ShaderExtensions.Configuration
{
    public class PluginConfig
    {
        public virtual int IntValue { get; set; } = 42; // Must be 'virtual' if you want BSIPA to detect a value change and save the config automatically.

    }
}
