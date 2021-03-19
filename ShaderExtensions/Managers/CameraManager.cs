using IPA.Loader;
using System;
using System.Collections.Generic;
using System.Reflection;
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

        public Camera[] Cameras { get; private set; } = null;
        protected List<ShaderToCamOutput> _shaderToCamOutputList;

        public virtual void Initialize() {
            Logger.log.Debug("Initializing CameraManager!");
            _shaderToCamOutputList = new List<ShaderToCamOutput>();
        }
        public virtual void Dispose() {
            Logger.log.Debug("Disposing CameraManager!");
            Clean();
        }

        private void GetOrAddShaderBehaviours() {
            _shaderToCamOutputList = new List<ShaderToCamOutput>();
            foreach (Camera cam in Cameras) {
                Logger.log.Debug(cam.name);
                if (cam.name.EndsWith(".cfg")) continue;
                ShaderToCamOutput stco = cam.gameObject.GetComponent<ShaderToCamOutput>();
                if (stco == null) {
                    stco = cam.gameObject.AddComponent<ShaderToCamOutput>();
                }
                _shaderToCamOutputList.Add(stco);
            }
        }

        public virtual void AddMaterial(Material mat) {
            foreach (ShaderToCamOutput stco in _shaderToCamOutputList) {
                if (stco == null) continue;
                if (!stco.Contains(mat)) {
                    stco.AddMaterial(mat);
                }
            }
        }

        public virtual void ApplyMaterials(List<Material> matList) {
            foreach (Material mat in matList) {
                AddMaterial(mat);
            }
        }

        public virtual void RemoveMaterial(Material mat) {
            foreach (ShaderToCamOutput stco in _shaderToCamOutputList) {
                if (stco == null) continue;
                if (stco.Contains(mat)) {
                    stco.RemoveMaterial(mat);
                }
            }
        }

        public virtual void ClearAllMaterials() {
            foreach (ShaderToCamOutput stco in _shaderToCamOutputList) {
                if (stco == null) continue;
                stco.ClearAllMaterials();
            }
        }

        public virtual void Refresh() {
            PluginMetadata cameraTwo = PluginManager.GetPluginFromId("Camera2");
            if (cameraTwo != null) {
                List<Camera> cameras = new List<Camera>();
                Type cam2Type = cameraTwo?.Assembly.GetType("Camera2.Behaviours.Cam2");
                MonoBehaviour[] allCam2s = GameObject.FindObjectsOfType(cam2Type) as MonoBehaviour[];
                foreach (MonoBehaviour cam2 in allCam2s) {
                    var camera = cam2Type.GetProperty("UCamera", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(cam2, null) as Camera;
                    if (camera != null) {
                        cameras.Add(camera);
                    }
                }
                cameras.AddRange(Camera.allCameras);
                Cameras = cameras.ToArray();
            } else {
                Cameras = Camera.allCameras;
            }
            GetOrAddShaderBehaviours();
        }

        internal virtual void Clean() {
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
