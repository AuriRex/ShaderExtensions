using ShaderExtensions.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace ShaderExtensions.Managers
{
    public class ShaderManager : IInitializable, IDisposable
    {

        private ShaderAssetLoader _shaderAssetLoader;
        private PluginConfig _pluginConfig;

        private Dictionary<string, Material> _materialCache;

        private ICameraManager _currentCameraManager;
        private ICameraManager _menuCameraManager;
        public ICameraManager CameraManager {
            get => _currentCameraManager;
            internal set {
                if(_menuCameraManager == null) {
                    _menuCameraManager = value;
                    _currentCameraManager = value;
                } else {
                    if(value == null) {
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

        public void RefreshCameras() {
            CameraManager?.Refresh();
            RefreshMaterials();
        }

        private void RefreshMaterials() {
            foreach (Material mat in _materialCache.Values) {
                CameraManager?.AddMaterial(mat);
            }
        }

        public ShaderEffect GetShaderEffectByReferenceName(string name) {
            return _shaderAssetLoader.GetShaderEffectByReferenceName(name);
        }

        private string GetFID(string id, ShaderEffect sfx) {
            return sfx.referenceName + "_" + id;
        }

        public Material AddMaterial(string id, ShaderEffect sfx) {
            string fullId = GetFID(id, sfx);
            Material mat = null;
            if (!_materialCache.ContainsKey(fullId)) {
                mat = new Material(sfx.material);
                _materialCache.Add(fullId, mat);
            }else {
                return GetMaterial(id, sfx);
            }
            CameraManager?.AddMaterial(mat);
            return mat;
        }

        public void RemoveMaterial(string id, ShaderEffect sfx) {
            string fullId = GetFID(id, sfx);
            if (_materialCache.ContainsKey(fullId)) {
                if (_materialCache.TryGetValue(fullId, out Material mat)) {
                    CameraManager?.RemoveMaterial(mat);
                }
                _materialCache.Remove(fullId);
            }
        }

        internal Material GetMaterial(string id, ShaderEffect sfx) {
            string fullId = GetFID(id, sfx);
            if (_materialCache.ContainsKey(fullId)) {
                if (_materialCache.TryGetValue(fullId, out Material mat)) {
                    return mat;
                }
            }
            return null;
        }

        public void ClearAllMaterials() {
            CameraManager?.ClearAllMaterials();
            _materialCache = new Dictionary<string, Material>();
        }
        
        public void Initialize() {
            _materialCache = new Dictionary<string, Material>();
        }

        public void Dispose() {
            _materialCache = null;
        }
    }
}
