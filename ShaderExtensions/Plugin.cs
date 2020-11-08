using BeatSaberMarkupLanguage.MenuButtons;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Utilities;
using ShaderExtensions.UI;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;

namespace ShaderExtensions
{

    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin instance { get; private set; }
        internal static string Name => "ShaderExtensions";

        public static string PluginAssetPath => Path.Combine(UnityGame.InstallPath, "CustomShaders");

        private static readonly MenuButton clearEffectButton = new MenuButton("[SE] Clear", "Clear all camera effects", ClearAllMaterialsButton, true);

        [Init]
        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
        /// Only use [Init] with one Constructor.
        /// </summary>
        public void Init(IPALogger logger) {
            instance = this;
            Logger.log = logger;
            Logger.log.Debug("Logger initialized.");
        }

        #region BSIPA Config
        [Init]
        public void InitWithConfig(Config conf) {
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Logger.log.Debug("Config loaded");
        }
        #endregion

        [OnStart]
        public void OnApplicationStart() {
            Logger.log.Debug("OnApplicationStart");

            //SceneManager.activeSceneChanged += this.OnActiveSceneChanged;

            BS_Utils.Utilities.BSEvents.gameSceneLoaded += OnGameSceneLoaded;
            BS_Utils.Utilities.BSEvents.earlyMenuSceneLoadedFresh += OnEarlyMenuSceneLoadedFresh;
            BS_Utils.Utilities.BSEvents.menuSceneLoaded += OnMenuSceneLoaded;
        }

        bool first = true;

        public void OnMenuSceneLoaded() {
            Logger.log.Info("Menu Scene loaded");
            if (first) {
                first = false;
            } else {
                ShaderExtensionsController.instance.ClearShaders();
            }
        }

        private void OnGameSceneLoaded() => ShaderExtensionsController.instance.OnLevelStart();

        private void OnEarlyMenuSceneLoadedFresh(ScenesTransitionSetupDataSO so) {
            MenuButtons.instance.RegisterButton(clearEffectButton);
            SettingsUI.Enable();
            new GameObject("ShaderExtensionsController").AddComponent<ShaderExtensionsController>();
        }

        [OnExit]
        public void OnApplicationQuit() => Logger.log.Debug("OnApplicationQuit");

        private static void ClearAllMaterialsButton() => ShaderExtensionsController.instance.ClearShaders();

    }
}
