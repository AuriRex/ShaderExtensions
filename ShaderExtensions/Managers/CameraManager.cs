using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace ShaderExtensions.Managers
{
    internal class CameraManager : ICameraManager, IInitializable, IDisposable
    {
        private ShaderManager _shaderManager;

        [Inject]
        internal CameraManager(ShaderManager shaderManager) {
            _shaderManager = shaderManager;
        }

        public Camera[] Cameras { get; private set; } = null;
        private List<ShaderToCamOutput> _shaderToCamOutputList;

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

        private void OnCameraRefreshDone() {
            _shaderToCamOutputList = new List<ShaderToCamOutput>();

            foreach (Camera cam in Cameras) {
                ShaderToCamOutput stco = cam.gameObject.GetComponent<ShaderToCamOutput>();
                if (stco == null) {
                    stco = cam.gameObject.AddComponent<ShaderToCamOutput>();
                }
                _shaderToCamOutputList.Add(stco);
            }
        }

        public void AddMaterial(Material mat) {
            foreach (ShaderToCamOutput stco in _shaderToCamOutputList) {
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
                if (stco.Contains(mat)) {
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
            Cameras = Camera.allCameras;
            OnCameraRefreshDone();
        }

        internal void Clean() {
            if (Cameras != null) {
                foreach (Camera cam in Cameras) {
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
