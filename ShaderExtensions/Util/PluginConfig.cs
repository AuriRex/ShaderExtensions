namespace ShaderExtensions.Util
{
    public class PluginConfig
    {
        public virtual bool ClearEffectsOnLevelCompletion { get; set; } = true;

        public virtual bool ClearPreviewEffects { get; set; } = true;

        public virtual bool ShowMenuButton { get; set; } = true;

        public virtual bool DebugLogging { get; set; } = false;

    }
}
