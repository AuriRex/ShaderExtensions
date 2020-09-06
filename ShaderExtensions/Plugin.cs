using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using UnityEngine.SceneManagement;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;
using System.IO;
using IPA.Utilities;
using BeatSaberMarkupLanguage.MenuButtons;
using ShaderExtensions.UI;

namespace ShaderExtensions
{

    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin instance { get; private set; }
        internal static string Name => "ShaderExtensions";

        public static string PluginAssetPath => Path.Combine(UnityGame.InstallPath, "CustomShaders");

        /*private static readonly MenuButton menuButton = new MenuButton("Reload shadertest", "Debug <3", DebugButtonPressed, true);*/
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
        public void InitWithConfig(Config conf)
        {
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Logger.log.Debug("Config loaded");
        }
        #endregion

        [OnStart]
        public void OnApplicationStart() {
            Logger.log.Debug("OnApplicationStart");

            SceneManager.activeSceneChanged += this.OnActiveSceneChanged;
        }

        Boolean first = true;

        public void OnActiveSceneChanged(Scene from, Scene to) {
            Logger.log.Info($"Shader Test SceneChanged: {from.name} to {to.name}");
            if (to.name.Equals("MenuViewControllers")) {
                Logger.log.Info("Shader Test MenuCore loaded");
                if(first) {
                    //MenuButtons.instance.RegisterButton(menuButton);
                    MenuButtons.instance.RegisterButton(clearEffectButton);
                    SettingsUI.Enable();
                    new GameObject("ShaderExtensionsController").AddComponent<ShaderExtensionsController>();
                    first = false;
                } else {
                    ShaderExtensionsController.instance.ClearShaders();
                }
                
            }
            if (to.name.Equals("GameCore")) {
                ShaderExtensionsController.instance.OnLevelStart();
            }
        }

        [OnExit]
        public void OnApplicationQuit() {
            Logger.log.Debug("OnApplicationQuit");

        }

        /*private static void DebugButtonPressed() {
            Logger.log.Debug("Debug button was pressed!");
            ShaderExtensionsController.instance.SetNewShader("fancy_color.bsfx");
        }*/

        private static void ClearAllMaterialsButton() {
            ShaderExtensionsController.instance.ClearShaders();
        }

    }
}
