using ShaderExtensions.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ShaderExtensions
{
    /// <summary>
    /// Monobehaviours (scripts) are added to GameObjects.
    /// For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
    /// </summary>
    public class ShaderExtensionsController : MonoBehaviour
    {
        public static ShaderExtensionsController instance { get; private set; }

        static ShaderEffect LoadShaderEffectAssetBundleFromPath(string path) {
            AssetBundle bundle = AssetBundle.LoadFromFile(path);
            var loadAsset = bundle.LoadAsset<Material>("Assets/ShaderEffect.mat");
            var shaderEffectMetadataGOPrefab = bundle.LoadAsset<GameObject>("Assets/ShaderEffectMetadata.prefab");
            Logger.log.Info("shaderEffectMetadataGOPrefab: " + shaderEffectMetadataGOPrefab);
            GameObject shaderEffectMetadataGO = Instantiate(shaderEffectMetadataGOPrefab);
            Logger.log.Info("shaderEffectMetadataGO: " + shaderEffectMetadataGO);
            ShaderEffect shaderEffect = shaderEffectMetadataGO.GetComponent<ShaderEffect>();
            Logger.log.Info("shaderEffect: " + shaderEffect);
            GameObject.DontDestroyOnLoad(shaderEffectMetadataGO);
            bundle.Unload(false);
            //GameObject.DestroyImmediate(shaderEffectMetadataGO);
            return shaderEffect;
        }

        internal List<ShaderEffect> shaderEffectList = new List<ShaderEffect>();

        public ShaderEffect GetShaderEffectByReferenceName(string name) {
            foreach (ShaderEffect sfx in shaderEffectList) {
                if (sfx != null) {
                    if (sfx.referenceName.Equals(name))
                        return sfx;
                }
            }
            return null;
        }

        List<ShaderToCamOutput> stco_list;

        public void OnLevelStart() {
            Logger.log.Info("OnLevelStart()");
            camToLookFor = GAMECAM;
            StartCoroutine(GetPlayerCamera());
            //GetCustomEventController();
        }


        IEnumerable<string> shaderFiles = Enumerable.Empty<string>();

        public void LoadShaders() {
            shaderFiles = Directory.GetFiles(Plugin.PluginAssetPath, "*.bsfx");

            shaderEffectList = new List<ShaderEffect>();

            foreach (string sh in shaderFiles) {
                ShaderEffect shaderEffect = null;

                try {
                    shaderEffect = LoadShaderEffectAssetBundleFromPath(sh);
                    Logger.log.Info("Loading Shader: " + sh);
                    LogShaderFX(shaderEffect);
                    shaderEffectList.Add(shaderEffect);
                } catch (Exception ex) {
                    Logger.log.Error("Error loading shader \"" + sh + "\"! - " + ex.Message);
                    Logger.log.Error(ex.StackTrace);
                }

            }

        }

        private void LogShaderFX(ShaderEffect shaderEffect) {
            Logger.log?.Info("ShaderEffect.material: " + shaderEffect.material);
            Logger.log?.Info("ShaderEffect.referenceName: " + shaderEffect.referenceName);
            Logger.log?.Info("ShaderEffect.name: " + shaderEffect.name);
            Logger.log?.Info("ShaderEffect.author: " + shaderEffect.author);
            Logger.log?.Info("ShaderEffect.description: " + shaderEffect.description);
            Logger.log?.Info("ShaderEffect.isScreenSpace: " + shaderEffect.isScreenSpace);
            Logger.log?.Info("ShaderEffect.previewImage: " + shaderEffect.previewImage);
        }

        #region Monobehaviour Messages
        /// <summary>
        /// Only ever called once, mainly used to initialize variables.
        /// </summary>
        private void Awake() {
            // For this particular MonoBehaviour, we only want one instance to exist at any time, so store a reference to it in a static property
            //   and destroy any that are created while one already exists.
            if (instance != null) {
                Logger.log?.Warn($"Instance of {this.GetType().Name} already exists, destroying.");
                GameObject.DestroyImmediate(this);
                return;
            }

            stco_list = new List<ShaderToCamOutput>();
            materialsToAdd = new List<Material>();

            CustomJSONData.CustomEventCallbackController.customEventCallbackControllerInit += ShaderEventController.CustomEventCallbackInit;
            /*customEventCallbackControllerInit += (CustomEventCallbackController customEventCallbackController) => {

            };*/

            GameObject.DontDestroyOnLoad(this); // Don't destroy this object on scene changes
            instance = this;
            Logger.log?.Debug($"{name}: Awake()");





            Logger.log?.Info("Cool Shader Variables Log spam UwU");

            LoadShaders();

            //ShaderEffect shaderEffect = LoadShaderEffectAssetBundleFromPath("fancy_color.bsfx"); // TODO: remove this bs
            //Material cancer = shaderEffect.material;

            //LogShaderFX(shaderEffect);

            //material = cancer;
            //Logger.log?.Info("cancer: " + cancer);
            //Renderer mainCameraRenderer = Camera.main.GetComponent<Renderer>();
            //mainCameraRenderer.material = cancer;

            Camera[] cams = FindObjectsOfType<Camera>();

            //MainCamera mainCamera = GameObject.FindObjectOfType<Camera>();
            ShaderToCamOutput stco;
            foreach (Camera cam in cams) {
                if (cam != null) {
                    if (cam.name.Equals("UnityIPADebugger")) continue;
                    if (cam.name.Contains(".cfg")) continue;

                    Logger.log?.Info("Adding Script to camera: " + cam);
                    stco = cam.gameObject.AddComponent<ShaderToCamOutput>();
                    //stco.addMaterial(cancer);
                    stco_list.Add(stco);
                } else {
                    Logger.log?.Info("skipping null camera");
                }
            }

            camToLookFor = MENUCAM;
            StartCoroutine(GetPlayerCamera());


            // CustomEventCallbackController

            //Logger.log?.Info("MainCamera: " + mainCamera);
            //Logger.log?.Info("MainCameraGO: " + mainCamera.gameObject);

            //ShaderToCamOutput stco = mainCamera.gameObject.AddComponent<ShaderToCamOutput>();
            //Logger.log?.Info("stco: " + stco);

            //GameObject.DestroyImmediate(go);
            //GameObject.DontDestroyOnLoad(go);
            Logger.log?.Info("Shader Test Instantiated");

        }

        internal void AddNewMaterial(Material mat) {
            foreach (ShaderToCamOutput stco in stco_list) {
                if (!stco.Contains(mat)) {
                    stco.AddMaterial(mat);
                }
            }
        }

        internal void RemoveMaterial(Material mat) {
            foreach (ShaderToCamOutput stco in stco_list) {
                if (stco.Contains(mat)) {
                    stco.RemoveMaterial(mat);
                }
            }
        }

        internal List<Material> materialsToAdd;

        public void AddShader(ShaderEffect sfx) {
            Material mat = new Material(sfx.material);

            AddMaterial(mat);
        }

        internal void AddMaterial(Material mat) {
            materialsToAdd.Add(mat);

            foreach (ShaderToCamOutput stco in stco_list) {
                stco.AddMaterial(mat);
            }
        }

        public void ClearShaders() {
            foreach (ShaderToCamOutput stco in stco_list) {
                stco.ClearAllMaterials();
            }
            materialsToAdd = new List<Material>();
        }

        GameObject mainCamera = null;
        GameObject mainGameCamera = null;

        private static string MENUCAM = "MenuMainCamera";
        private static string GAMECAM = "MainCamera";

        string camToLookFor = MENUCAM;
        IEnumerator GetPlayerCamera() {
            GameObject playerCamera = null;
            MainCamera mainCam = null;
            MainCamera[] mainCameras = null;
            yield return new WaitUntil(() => {
                //mainCam = GameObject.FindObjectOfType<MainCamera>();
                mainCameras = FindObjectsOfType<MainCamera>();
                //playerCamera = GameObject.FindObjectOfType<MainCamera>();//GameObject.Find("MenuCore/Wrapper/Origin/MenuMainCamera");
                foreach (MainCamera mc in mainCameras) {
                    if (mc.gameObject.name.Equals(camToLookFor)) {
                        mainCam = mc;
                    }
                }

                return mainCam != null;
            });

            playerCamera = mainCam.gameObject;

            if (camToLookFor.Equals(GAMECAM)) {
                mainGameCamera = playerCamera;
            } else {
                mainCamera = playerCamera;
            }

            Logger.log?.Info($"Main camera found: {playerCamera.name}");
            ShaderToCamOutput stco = playerCamera.GetComponent<ShaderToCamOutput>();
            if (stco == null) {
                stco = playerCamera.AddComponent<ShaderToCamOutput>();
            }

            Logger.log?.Info($"# of materials to add: {materialsToAdd.Count}");
            if (materialsToAdd != null && materialsToAdd.Count > 0) {
                foreach (Material mat in materialsToAdd) {
                    stco.AddMaterial(mat);
                }
            }
            //stco.addMaterial(material);
            stco_list.Add(stco);
            Logger.log?.Info("Main camera prepped and ready to go!");
        }

        /// <summary>
        /// Only ever called once on the first frame the script is Enabled. Start is called after any other script's Awake() and before Update().
        /// </summary>
        private void Start() {

        }

        /// <summary>
        /// Called every frame if the script is enabled.
        /// </summary>
        private void Update() {
            if (Input.GetKeyDown(KeyCode.K)) {
                ClearShaders();
            }
        }

        /// <summary>
        /// Called every frame after every other enabled script's Update().
        /// </summary>
        private void LateUpdate() {

        }

        /// <summary>
        /// Called when the script becomes enabled and active
        /// </summary>
        private void OnEnable() {

        }

        /// <summary>
        /// Called when the script becomes disabled or when it is being destroyed.
        /// </summary>
        private void OnDisable() {

        }

        /// <summary>
        /// Called when the script is being destroyed.
        /// </summary>
        private void OnDestroy() {
            Logger.log?.Debug($"{name}: OnDestroy()");
            instance = null; // This MonoBehaviour is being destroyed, so set the static instance property to null.

        }
        #endregion
    }
}
