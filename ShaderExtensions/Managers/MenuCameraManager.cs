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
    internal class MenuCameraManager : ICameraManager, IInitializable, IDisposable
    {
        private ShaderManager _shaderManager;

        [Inject]
        internal MenuCameraManager(ShaderManager shaderManager) {
            _shaderManager = shaderManager;
        }

        private MainCamera[] _mainCameras = null;
        private List<ShaderToCamOutput> _shaderToCamOutputList;

        public Action onCameraRefreshDone { get; set; }

        public void Initialize() {
            _shaderManager.CameraManager = this;
            _shaderToCamOutputList = new List<ShaderToCamOutput>();
            Refresh();
        }
        public void Dispose() {
            foreach (MainCamera cam in _mainCameras) {
                ShaderToCamOutput stco = cam.gameObject.GetComponent<ShaderToCamOutput>();
                if (stco != null) {
                    UnityEngine.Object.Destroy(stco);
                }
            }
            _shaderManager.CameraManager = null;
        }

        public void OnCameraRefreshDone() {
            Logger.log.Debug("callback called!");

            _shaderToCamOutputList = new List<ShaderToCamOutput>();

            foreach (MainCamera cam in _mainCameras) {
                ShaderToCamOutput stco = cam.gameObject.GetComponent<ShaderToCamOutput>();
                if (stco == null) {
                    stco = cam.gameObject.AddComponent<ShaderToCamOutput>();
                }
                _shaderToCamOutputList.Add(stco);
            }

            onCameraRefreshDone?.Invoke();
        }

        public virtual MainCamera[] GetCameras() {
            return _mainCameras;
        }
        public virtual void AddMaterial(Material mat) {
            Logger.log.Debug("Adding a Material");
            foreach(ShaderToCamOutput stco in _shaderToCamOutputList) {
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
                if(stco.Contains(mat)) {
                    stco.RemoveMaterial(mat);
                }
            }
        }

        public virtual void ClearAllMaterials() {
            foreach (ShaderToCamOutput stco in _shaderToCamOutputList) {
                stco.ClearAllMaterials();
            }
        }

        public virtual void Refresh() {
            Logger.log.Debug("Starting Refresh Coroutine");
            SharedCoroutineStarter.instance.StartCoroutine(RefreshCameras(OnCameraRefreshDone));

        }

        public IEnumerator RefreshCameras(Action callback) {
            Logger.log.Debug("Refreshing Cameras");
            MainCamera[] mainCameras = null;
            yield return new WaitUntil(() => {
                mainCameras = UnityEngine.Object.FindObjectsOfType<MainCamera>();
                return mainCameras != null;
            });
            _mainCameras = mainCameras;
            callback();
        }
    }
}
