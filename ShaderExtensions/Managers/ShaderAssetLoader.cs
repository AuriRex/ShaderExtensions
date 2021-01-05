using ShaderExtensions.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Zenject;

namespace ShaderExtensions.Managers
{
    class ShaderAssetLoader : IInitializable, IDisposable
    {
        private PluginConfig _pluginConfig;

        internal ShaderAssetLoader(PluginConfig pluginConfig) {
            _pluginConfig = pluginConfig;
        }

        public void Initialize() => LoadShaders();

        public void Dispose() {

        }

        IEnumerable<string> shaderFiles = Enumerable.Empty<string>();

        static ShaderEffect LoadShaderEffectAssetBundleFromPath(string path) {
            AssetBundle bundle = AssetBundle.LoadFromFile(path);
            var loadAsset = bundle.LoadAsset<Material>("Assets/ShaderEffect.mat");
            var shaderEffectMetadataGOPrefab = bundle.LoadAsset<GameObject>("Assets/ShaderEffectMetadata.prefab");
            Logger.log.Info("shaderEffectMetadataGOPrefab: " + shaderEffectMetadataGOPrefab);
            GameObject shaderEffectMetadataGO = UnityEngine.Object.Instantiate(shaderEffectMetadataGOPrefab);
            Logger.log.Info("shaderEffectMetadataGO: " + shaderEffectMetadataGO);
            ShaderEffect shaderEffect = shaderEffectMetadataGO.GetComponent<ShaderEffect>();
            Logger.log.Info("shaderEffect: " + shaderEffect);
            GameObject.DontDestroyOnLoad(shaderEffectMetadataGO);
            bundle.Unload(false);
            //GameObject.DestroyImmediate(shaderEffectMetadataGO);
            return shaderEffect;
        }

        internal List<ShaderEffect> ShaderEffectList { get; private set; } = new List<ShaderEffect>();

        public ShaderEffect GetShaderEffectByReferenceName(string name) {
            if (name == null) return null;
            foreach (ShaderEffect sfx in ShaderEffectList) {
                if (sfx != null) {
                    if (sfx.referenceName.Equals(name))
                        return sfx;
                }
            }
            return null;
        }

        public void LoadShaders() {
            Directory.CreateDirectory(Plugin.PluginAssetPath);

            shaderFiles = Directory.GetFiles(Plugin.PluginAssetPath, "*.bsfx");

            ShaderEffectList = new List<ShaderEffect>();

            foreach (string sh in shaderFiles) {
                ShaderEffect shaderEffect = null;

                try {
                    shaderEffect = LoadShaderEffectAssetBundleFromPath(sh);
                    Logger.log.Info("Loading Shader: " + sh);
                    LogShaderFX(shaderEffect);
                    ShaderEffectList.Add(shaderEffect);
                } catch (Exception ex) {
                    Logger.log.Error("Error loading shader \"" + sh + "\"! - " + ex.Message);
                    Logger.log.Error(ex.StackTrace);
                }

            }

        }

        internal void Reload() {

            Logger.log.Debug("Reloading the ShaderAssetLoader");

            Dispose();
            Initialize();
        }

        public static void LogShaderFX(ShaderEffect shaderEffect) {
            Logger.log?.Info("ShaderEffect.material: " + shaderEffect.material);
            Logger.log?.Info("ShaderEffect.referenceName: " + shaderEffect.referenceName);
            Logger.log?.Info("ShaderEffect.name: " + shaderEffect.name);
            Logger.log?.Info("ShaderEffect.author: " + shaderEffect.author);
            Logger.log?.Info("ShaderEffect.description: " + shaderEffect.description);
            Logger.log?.Info("ShaderEffect.isScreenSpace: " + shaderEffect.isScreenSpace);
            Logger.log?.Info("ShaderEffect.previewImage: " + shaderEffect.previewImage);
        }

    }
}
