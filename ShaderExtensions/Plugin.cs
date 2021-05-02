using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Utilities;
using ShaderExtensions.Installers;
using ShaderExtensions.Util;
using SiraUtil.Zenject;
using System.IO;
using IPALogger = IPA.Logging.Logger;

namespace ShaderExtensions
{

    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        public static string PluginAssetPath => Path.Combine(UnityGame.InstallPath, "CustomShaders");

        internal const string CAPABILITY = "Shader Extensions";

        internal const string HARMONY_ID = "dev.aurirex.shaderextensions";

        [Init]
        public Plugin(IPALogger logger, Config config, Zenjector zenjector)
        {
            Logger.log = logger;
            zenjector.OnApp<ShaderExtensionsCoreInstaller>().WithParameters(config.Generated<PluginConfig>());
            zenjector.OnMenu<ShaderExtensionsMenuInstaller>();
            zenjector.OnGame<ShaderExtensionsGameInstaller>(false).ShortCircuitForTutorial();
        }

        [OnEnable]
        public void OnEnable()
        {

        }

        [OnDisable]
        public void OnDisable()
        {

        }
    }
}
