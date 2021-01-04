using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace ShaderExtensions.Managers
{
    internal class CameraManager : ICameraManager, IInitializable, IDisposable
    {
        protected ShaderManager _shaderManager;

        [Inject]
        internal CameraManager(ShaderManager shaderManager) {
            _shaderManager = shaderManager;
        }

        protected Camera[] _mainCameras = null;
        protected List<ShaderToCamOutput> _shaderToCamOutputList;

        public Action onCameraRefreshDone { get; set; }

        public void Initialize() {
            Logger.log.Debug("Initializing new CameraManager!");
            _shaderToCamOutputList = new List<ShaderToCamOutput>();
            Refresh();
            _shaderManager.CameraManager = this;
        }
        public void Dispose() {
            Logger.log.Debug("Disposing CameraManager!");
            Clean();
            _shaderManager.CameraManager = null;
        }

        public void OnCameraRefreshDone() {
            _shaderToCamOutputList = new List<ShaderToCamOutput>();

            foreach (Camera cam in _mainCameras) {
                ShaderToCamOutput stco = cam.gameObject.GetComponent<ShaderToCamOutput>();
                if (stco == null) {
                    stco = cam.gameObject.AddComponent<ShaderToCamOutput>();
                }
                _shaderToCamOutputList.Add(stco);
            }

            onCameraRefreshDone?.Invoke();
        }

        public Camera[] GetCameras() {
            return _mainCameras;
        }
        public void AddMaterial(Material mat) {
            foreach(ShaderToCamOutput stco in _shaderToCamOutputList) {
                if (!stco.Contains(mat)) {
                    stco.AddMaterial(mat);
                }
            }
        }

        public void ApplyMaterials(List<Material> matList) {
            foreach (Material mat in matList) {
                AddMaterial(mat);
            }
        }

        public void RemoveMaterial(Material mat) {
            foreach (ShaderToCamOutput stco in _shaderToCamOutputList) {
                if(stco.Contains(mat)) {
                    stco.RemoveMaterial(mat);
                }
            }
        }

        public void ClearAllMaterials() {
            foreach (ShaderToCamOutput stco in _shaderToCamOutputList) {
                stco.ClearAllMaterials();
            }
        }

        public void Refresh() {
            _mainCameras = Camera.allCameras;
            OnCameraRefreshDone();
        }

        internal void Clean() {
            if (_mainCameras != null) {
                foreach (Camera cam in _mainCameras) {
                    if (cam == null) continue;
                    ShaderToCamOutput stco = cam.gameObject.GetComponent<ShaderToCamOutput>();
                    if (stco != null) {
                        UnityEngine.Object.Destroy(stco);
                    }
                }
            }
        }
    }
}
