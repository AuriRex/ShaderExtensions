using BeatSaberMarkupLanguage.MenuButtons;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Utilities;
using ShaderExtensions.Util;
using ShaderExtensions.Installers;
using ShaderExtensions.UI;
using SiraUtil.Zenject;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;

namespace ShaderExtensions
{

    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        public static string PluginAssetPath => Path.Combine(UnityGame.InstallPath, "CustomShaders");

        [Init]
        public Plugin(IPALogger logger, Config config, Zenjector zenjector) {
            Logger.log = logger;
            zenjector.OnApp<ShaderExtensionsCoreInstaller>().WithParameters(config.Generated<PluginConfig>());
            zenjector.OnMenu<ShaderExtensionsMenuInstaller>();
            zenjector.OnGame<ShaderExtensionsGameInstaller>().ShortCircuitForTutorial();
        }

        [OnEnable, OnDisable]
        public void OnState() {

        }
    }
}
