using ShaderExtensions.Util;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace ShaderExtensions.Managers
{
    public class ShaderManager : IInitializable, IDisposable
    {
        private ShaderAssetLoader _shaderAssetLoader;
        private PluginConfig _pluginConfig;

        internal Dictionary<string, Material> MaterialCache { get; private set; }

        public ICameraManager CameraManager { get; private set; }

        [Inject]
        internal ShaderManager(ShaderAssetLoader shaderAssetLoader, PluginConfig pluginConfig)
        {
            _shaderAssetLoader = shaderAssetLoader;
            _pluginConfig = pluginConfig;
        }

        [Inject]
        internal void Construct(CameraManager cameraManager) => CameraManager = cameraManager;

        /// <summary>
        /// Refreshes all active cameras
        /// </summary>
        public void RefreshCameraManager() => CameraManager?.Refresh();

        /// <summary>
        /// Re-applies all added Materials to every rendering cameras
        /// </summary>
        public void Refresh()
        {
            RefreshCameraManager();
            CameraManager?.ClearAllMaterials();
            RefreshMaterials();
        }

        private void RefreshMaterials()
        {
            foreach (Material mat in MaterialCache.Values)
            {
                CameraManager?.AddMaterial(mat);
            }
        }

        /// <summary>
        /// Finds the ShaderEffect with the given reference name from all loaded shader files 
        /// </summary>
        /// <param name="name">the ShaderEffect reference name to look up</param>
        /// <returns>The ShaderEffect with given reference name or null</returns>
        public ShaderEffectData GetShaderEffectByReferenceName(string name) => _shaderAssetLoader.GetShaderEffectByReferenceName(name);

        /// <summary>
        /// Finds the ShaderEffect with the given material from all loaded shader files 
        /// </summary>
        /// <param name="mat">the ShaderEffects Material</param>
        /// <returns>The ShaderEffect with given reference name or null</returns>
        public ShaderEffectData GetShaderEffectByMaterial(Material mat) => _shaderAssetLoader.GetShaderEffectByMaterial(mat);

        private string GetFID(string id, ShaderEffectData sfx) => id + "_" + sfx.ReferenceName;

        /// <summary>
        /// Adds a Material based of the ShaderEffect sfx with the specified identifier id 
        /// </summary>
        /// <param name="id">the id assigned</param>
        /// <param name="sfx">the shader used</param>
        /// <returns>The created Material</returns>
        public Material AddMaterial(string id, ShaderEffectData sfx)
        {
            string fullId = GetFID(id, sfx);
            Material mat;
            if (!MaterialCache.ContainsKey(fullId))
            {
                mat = new Material(sfx.Material);
                MaterialCache.Add(fullId, mat);
            }
            else
            {
                return GetMaterial(id, sfx);
            }
            CameraManager?.AddMaterial(mat);
            return mat;
        }

        public List<Material> GetAllMaterials() => new List<Material>(MaterialCache.Values);

        /// <summary>
        /// Removes the Material with the specified identifier id and ShaderEffect sfx
        /// </summary>
        /// <param name="id">the id to look for</param>
        /// <param name="sfx">the shader to look for</param>
        /// <returns>If the Material has been removed</returns>
        public bool RemoveMaterial(string id, ShaderEffectData sfx) => RemoveMaterial(GetFID(id, sfx));

        internal bool RemoveMaterial(string fullId)
        {
            if (fullId == null) return false;
            if (MaterialCache.TryGetValue(fullId, out Material mat))
            {
                CameraManager?.RemoveMaterial(mat);
                return MaterialCache.Remove(fullId);
            }
            return false;
        }

        internal void OnGameStart()
        {
            // We have to wait a bit for the default Game Cameras to init
            SharedCoroutineStarter.instance.StartCoroutine(SEUtilities.DoAfter(0.1f, () => {
                Refresh();
            }));
        }

        internal void OnGameQuit()
        {
            CameraManager?.ClearAllMaterials();
            
            if (_pluginConfig.ClearEffectsOnLevelCompletion)
            {
                MaterialCache = new Dictionary<string, Material>();
            }
        }

        /// <summary>
        /// Removes all Materials that start with the specified identifier id
        /// </summary>
        /// <param name="id">the id to look for</param>
        /// <returns>A List of materials that have been removed</returns>
        public List<Material> RemoveAllMaterialsStartingWithId(string id)
        {
            Stack<string> removeStack = new Stack<string>();
            foreach (KeyValuePair<string, Material> entry in MaterialCache)
            {
                if (entry.Key.StartsWith(id, StringComparison.InvariantCulture))
                {
                    removeStack.Push(entry.Key);
                }
            }
            List<Material> removedMaterials = new List<Material>(removeStack.Count);
            while (removeStack.Count > 0)
            {
                removedMaterials.Add(GetMaterial(removeStack.Peek()));
                RemoveMaterial(removeStack.Pop());
            }
            return removedMaterials;
        }

        /// <summary>
        /// Returns the Material with the specified identifier id and ShaderEffect sfx
        /// </summary>
        /// <param name="id">the id to look for</param>
        /// <param name="sfx">the shader to look for</param>
        /// <returns>The Material or null</returns>
        internal Material GetMaterial(string id, ShaderEffectData sfx) => GetMaterial(GetFID(id, sfx));

        private Material GetMaterial(string fullId)
        {
            if (MaterialCache.ContainsKey(fullId))
            {
                if (MaterialCache.TryGetValue(fullId, out Material mat))
                {
                    return mat;
                }
            }
            return null;
        }

        /// <summary>
        /// Clears all Materials
        /// </summary>
        /// <returns>All the removed Materials</returns>
        public List<Material> ClearAllMaterials()
        {
            CameraManager?.ClearAllMaterials();
            List<Material> oldMaterials = new List<Material>(MaterialCache.Values);
            MaterialCache = new Dictionary<string, Material>();
            return oldMaterials;
        }

        public void Initialize() => MaterialCache = new Dictionary<string, Material>();

        public void Dispose() => MaterialCache = null;
    }
}
