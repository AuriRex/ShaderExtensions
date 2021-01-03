using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace ShaderExtensions.Managers
{
    class ShaderManager : IInitializable, IDisposable
    {

        private ShaderAssetLoader _shaderAssetLoader;

        private Dictionary<string, Material> _materialCache;

        private ICameraManager _cameraManager;
        public ICameraManager CameraManager {
            get { return _cameraManager; }
            internal set {
                if(value == null && _cameraManager != null) {
                    _cameraManager.onCameraRefreshDone -= OnCameraRefreshDone;
                } else {
                    value.onCameraRefreshDone += OnCameraRefreshDone;
                }
                _cameraManager = value;
            }
        }

        [Inject]
        internal ShaderManager(ShaderAssetLoader shaderAssetLoader) {
            _shaderAssetLoader = shaderAssetLoader;
        }

        public void RefreshCameras() {
            CameraManager?.Refresh();
        }

        public void AddMaterial(string id, ShaderEffect sfx) {
            string fullId = sfx.referenceName + "_" + id;
            if (!_materialCache.ContainsKey(fullId)) {
                _materialCache.Add(fullId, new Material(sfx.material));
            }
            if(_materialCache.TryGetValue(fullId, out Material mat)) {
                CameraManager?.AddMaterial(mat);
            }
        }

        public void RemoveMaterial(string id, ShaderEffect sfx) {
            string fullId = sfx.referenceName + "_" + id;
            if (_materialCache.ContainsKey(fullId)) {
                if (_materialCache.TryGetValue(fullId, out Material mat)) {
                    CameraManager?.RemoveMaterial(mat);
                }
                _materialCache.Remove(fullId);
            }
        }

        public void ClearAllMaterials() {
            CameraManager?.ClearAllMaterials();
            _materialCache = new Dictionary<string, Material>();
        }
        
        public void Initialize() {
            _materialCache = new Dictionary<string, Material>();
        }

        public void Dispose() {
            
        }
        
        private void OnCameraRefreshDone() {
            Logger.log.Debug("callback on ShaderManager");
            foreach (Material mat in _materialCache.Values) {
                CameraManager?.AddMaterial(mat);
            }
        }
    }
}
