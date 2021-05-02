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

        internal ShaderAssetLoader(PluginConfig pluginConfig)
        {
            _pluginConfig = pluginConfig;
        }

        public void Initialize() => LoadShaders();

        public void Dispose()
        {

        }

        IEnumerable<string> shaderFiles = Enumerable.Empty<string>();

        static ShaderEffectData LoadShaderEffectAssetBundleFromPath(string path)
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(path);
            var loadAsset = bundle.LoadAsset<Material>("Assets/ShaderEffect.mat");
            var shaderEffectMetadataGOPrefab = bundle.LoadAsset<GameObject>("Assets/ShaderEffectMetadata.prefab");
            GameObject shaderEffectMetadataGO = UnityEngine.Object.Instantiate(shaderEffectMetadataGOPrefab);
            ShaderEffect shaderEffect = shaderEffectMetadataGO.GetComponent<ShaderEffect>();
            ShaderEffectData data = new ShaderEffectData(shaderEffect);
            GameObject.Destroy(shaderEffectMetadataGO);
            bundle.Unload(false);
            return data;
        }

        internal List<ShaderEffectData> ShaderEffectList { get; private set; } = new List<ShaderEffectData>();

        public ShaderEffectData GetShaderEffectByReferenceName(string name)
        {
            if (name == null) return null;
            foreach (ShaderEffectData sfx in ShaderEffectList)
            {
                if (sfx != null)
                {
                    if (sfx.ReferenceName.Equals(name))
                        return sfx;
                }
            }
            return null;
        }

        public ShaderEffectData GetShaderEffectByMaterial(Material mat)
        {
            if (mat == null) return null;
            foreach (ShaderEffectData sfx in ShaderEffectList)
            {
                if (sfx != null)
                {
                    if (sfx.Material.shader.Equals(mat.shader))
                        return sfx;
                }
            }
            return null;
        }

        public void LoadShaders()
        {
            Directory.CreateDirectory(Plugin.PluginAssetPath);

            shaderFiles = Directory.GetFiles(Plugin.PluginAssetPath, "*.bsfx");

            ShaderEffectList = new List<ShaderEffectData>();

            foreach (string sh in shaderFiles)
            {
                ShaderEffectData shaderEffect = null;

                try
                {
                    shaderEffect = LoadShaderEffectAssetBundleFromPath(sh);
                    Logger.log.Info("Loading Shader: " + sh);
                    LogShaderFX(shaderEffect);
                    ShaderEffectList.Add(shaderEffect);
                }
                catch (Exception ex)
                {
                    Logger.log.Error("Error loading shader \"" + sh + "\"! - " + ex.Message);
                    Logger.log.Error(ex.StackTrace);
                }

            }

        }

        internal void Reload()
        {

            Logger.log.Debug("Reloading the ShaderAssetLoader");

            Dispose();
            Initialize();
        }

        public static void LogShaderFX(ShaderEffectData shaderEffect)
        {
            Logger.log?.Debug("ShaderEffect.material: " + shaderEffect.Material);
            Logger.log?.Debug("ShaderEffect.referenceName: " + shaderEffect.ReferenceName);
            Logger.log?.Debug("ShaderEffect.name: " + shaderEffect.Name);
            Logger.log?.Debug("ShaderEffect.author: " + shaderEffect.Author);
            Logger.log?.Debug("ShaderEffect.description: " + shaderEffect.Description);
            Logger.log?.Debug("ShaderEffect.isScreenSpace: " + shaderEffect.IsScreenSpace);
            Logger.log?.Debug("ShaderEffect.previewImage: " + shaderEffect.PreviewImage);
        }

    }
}
