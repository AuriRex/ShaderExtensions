using ShaderExtensions.Util;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace ShaderExtensions.Managers
{
    public class ShaderManager : IInitializable, IDisposable {
        private ShaderAssetLoader _shaderAssetLoader;
        private PluginConfig _pluginConfig;

        private Dictionary<string, Material> _materialCache;

        private ICameraManager _currentCameraManager;
        private ICameraManager _menuCameraManager;
        public ICameraManager CameraManager {
            get => _currentCameraManager;
            internal set {
                if (_menuCameraManager == null) {
                    _menuCameraManager = value;
                    _currentCameraManager = value;
                } else {
                    if (value == null) {
                        _currentCameraManager = _menuCameraManager;
                        OnMenuCameraManagerReset();
                    } else {
                        _currentCameraManager = value;
                        value.Refresh();
                        (value as CameraManager).ClearAllMaterials();
                    }
                }
                RefreshMaterials();
            }
        }

        [Inject]
        internal ShaderManager(ShaderAssetLoader shaderAssetLoader, PluginConfig pluginConfig) {
            _shaderAssetLoader = shaderAssetLoader;
            _pluginConfig = pluginConfig;
        }

        private void OnMenuCameraManagerReset() {
            CameraManager?.ClearAllMaterials();
            if (_pluginConfig.ClearEffectsOnLevelCompletion) {
                _materialCache = new Dictionary<string, Material>();
            }
        }

        /// <summary>
        /// Re-applies all added Materials to every rendering cameras
        /// </summary>
        public void RefreshCameraManager() {
            CameraManager?.Refresh();
            CameraManager?.ClearAllMaterials();
            RefreshMaterials();
        }

        private void RefreshMaterials() {
            foreach (Material mat in _materialCache.Values) {
                CameraManager?.AddMaterial(mat);
            }
        }

        /// <summary>
        /// Finds the ShaderEffect with the given reference name from all loaded shader files 
        /// </summary>
        /// <param name="name">the ShaderEffect to look up</param>
        /// <returns>The ShaderEffect with given reference name or null</returns>
        public ShaderEffect GetShaderEffectByReferenceName(string name) => _shaderAssetLoader.GetShaderEffectByReferenceName(name);

        private string GetFID(string id, ShaderEffect sfx) => id + "_" + sfx.referenceName;

        /// <summary>
        /// Adds a Material based of the ShaderEffect sfx with the specified identifier id 
        /// </summary>
        /// <param name="id">the id assigned</param>
        /// <param name="sfx">the shader use</param>
        /// <returns>The created Material</returns>
        public Material AddMaterial(string id, ShaderEffect sfx) {
            string fullId = GetFID(id, sfx);
            Material mat;
            if (!_materialCache.ContainsKey(fullId)) {
                mat = new Material(sfx.material);
                _materialCache.Add(fullId, mat);
            } else {
                return GetMaterial(id, sfx);
            }
            CameraManager?.AddMaterial(mat);
            return mat;
        }

        /// <summary>
        /// Removes the Material with the specified identifier id and ShaderEffect sfx
        /// </summary>
        /// <param name="id">the id to look for</param>
        /// <param name="sfx">the shader to look for</param>
        /// <returns>If the Material has been removed</returns>
        public bool RemoveMaterial(string id, ShaderEffect sfx) {
            return RemoveMaterial(GetFID(id, sfx));
        }

        private bool RemoveMaterial(string fullId) {
            if (fullId == null) return false;
            if (_materialCache.TryGetValue(fullId, out Material mat)) {
                CameraManager?.RemoveMaterial(mat);
                return _materialCache.Remove(fullId);
            }
            return false;
        }

        /// <summary>
        /// Removes all Materials that start with the specified identifier id
        /// </summary>
        /// <param name="id">the id to look for</param>
        /// <returns>A List of materials that have been removed</returns>
        public List<Material> RemoveAllMaterialsStartingWithId(string id) {
            Stack<string> removeStack = new Stack<string>();
            foreach (KeyValuePair<string, Material> entry in _materialCache) {
                if(entry.Key.StartsWith(id, StringComparison.InvariantCulture)) {
                    removeStack.Push(entry.Key);
                }
            }
            List<Material> removedMaterials = new List<Material>(removeStack.Count);
            while(removeStack.Count > 0) {
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
        internal Material GetMaterial(string id, ShaderEffect sfx) {
            return GetMaterial(GetFID(id, sfx));
        }

        private Material GetMaterial(string fullId) {
            if (_materialCache.ContainsKey(fullId)) {
                if (_materialCache.TryGetValue(fullId, out Material mat)) {
                    return mat;
                }
            }
            return null;
        }

        /// <summary>
        /// Clears all Materials
        /// </summary>
        /// <returns>All the removed Materials</returns>
        public List<Material> ClearAllMaterials() {
            CameraManager?.ClearAllMaterials();
            List<Material> oldMaterials = new List<Material>(_materialCache.Values);
            _materialCache = new Dictionary<string, Material>();
            return oldMaterials;
        }

        public void Initialize() => _materialCache = new Dictionary<string, Material>();

        public void Dispose() => _materialCache = null;
    }
}
