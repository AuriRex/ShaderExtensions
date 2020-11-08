using CustomJSONData;
using CustomJSONData.CustomBeatmap;
using NoodleExtensions.Animation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TreeDict = System.Collections.Generic.IDictionary<string, object>;


namespace ShaderExtensions.Event
{
    public class ShaderEventController : MonoBehaviour
    {
        private BeatmapObjectSpawnController _beatmapObjectSpawnController;

        public static ShaderEventController Instance { get; private set; }

        public CustomEventCallbackController CustomEventCallbackController { get; private set; }

        public BeatmapObjectSpawnController BeatmapObjectSpawnController {
            get {
                if (_beatmapObjectSpawnController == null) {
                    _beatmapObjectSpawnController = Resources.FindObjectsOfTypeAll<BeatmapObjectSpawnController>().First();
                }

                return _beatmapObjectSpawnController;
            }
        }

        internal static void CustomEventCallbackInit(CustomEventCallbackController customEventCallbackController) {
            if (Instance != null) {
                Destroy(Instance);
            }

            Logger.log.Info("CustomEventCallbackController: " + customEventCallbackController);

            Instance = customEventCallbackController.gameObject.AddComponent<ShaderEventController>();

            Instance.CustomEventCallbackController = customEventCallbackController;
            Instance.CustomEventCallbackController.AddCustomEventCallback(Instance.ShaderEventCallback);
        }

        private static List<Material> materials;

        public void ShaderEventCallback(CustomEventData customEventData) {
            if (customEventData.data == null) return;
            if (customEventData.type == "Shader") {
                if (materials == null) {
                    materials = new List<Material>();
                }
                Logger.log?.Info("Shader event received!");
                try {
                    bool clear = false;
                    TreeDict eventData = new Dictionary<string, object>(customEventData.data as TreeDict);
                    //Logger.log?.Info("nice1");

                    object res = Trees.At(eventData, "_shaders");
                    List<object> shaders;
                    if (res != null) {
                        shaders = res as List<object>;
                    } else {
                        shaders = new List<object>();
                    }

                    dynamic tmp = Trees.At(eventData, "_clearAll");
                    if (tmp != null && tmp.GetType() == typeof(bool)) {
                        clear = tmp;
                    }


                    /*try {
                        clear = (bool)tmp;
                    } catch (Exception) { }*/

                    //Logger.log?.Info("nice2");

                    List<ShaderCommand> scList = new List<ShaderCommand>();

                    foreach (TreeDict shader in shaders) {
                        /*new List<string>(shader.Keys).ForEach(s => {
                            Logger.log.Info(s + ": " + shader[s]);
                        });*/

                        scList.Add(new ShaderCommand(shader));
                    }

                    if (clear) {
                        materialQueue = new List<Material>();
                        ShaderExtensionsController.instance.ClearShaders();
                    }

                    foreach (ShaderCommand sc in scList) {

                        if (materialLookUp.ContainsKey(sc.id)) {
                            if (materialLookUp.TryGetValue(sc.id, out Material mat)) {
                                sc.mat = mat;
                                if (sc.clear) {
                                    MQRemove(mat);
                                } else {
                                    MQAdd(mat);
                                }

                            }

                        } else {
                            // Add Material to LookUp

                            ShaderEffect sfx = ShaderExtensionsController.instance.GetShaderEffectByReferenceName(sc.reference);

                            if (sfx != null) {

                                Material mat = new Material(sfx.material);

                                sc.mat = mat;

                                materialLookUp.Add(sc.id, mat);
                                if (sc.clear) {
                                    MQRemove(mat);
                                } else {
                                    MQAdd(mat);
                                }
                            }

                        }

                    }

                    foreach (ShaderCommand sc in scList) {
                        StartEventCoroutine(sc, customEventData.time);
                    }

                    foreach (Material mat in materialQueue) {
                        ShaderExtensionsController.instance.AddNewMaterial(mat);
                    }


                } catch (Exception ex) {
                    Logger.log.Error("ShaderEventController encountered an exception: " + ex.Message);
                    Logger.log.Error(ex.StackTrace);
                }


            }
        }

        private List<Material> materialQueue = new List<Material>();
        private void MQAdd(Material mat) {
            if (!materialQueue.Contains(mat)) {
                materialQueue.Add(mat);
            }
        }
        private void MQRemove(Material mat) {
            if (materialQueue.Contains(mat)) {
                materialQueue.Remove(mat);
            }
        }
        // identification -> material
        Dictionary<string, Material> materialLookUp = new Dictionary<string, Material>();


        internal static void StartEventCoroutine(ShaderCommand shaderCommand, float startTime) {

            if (shaderCommand.mat == null) {
                Logger.log.Error("ShaderCommand with null material!");
                return;
            }

            shaderCommand.properties.getProps().ForEach(sp => {
                float duration = sp.duration;
                duration = 60f * duration / Instance.BeatmapObjectSpawnController.currentBpm;

                if (!shaderCommand.mat.HasProperty(sp.property)) {
                    Logger.log.Error("ShaderCommand \"" + shaderCommand.id + "\" wants to set a nonexistant property \"" + sp.property + "\" for material \"" + shaderCommand.reference + "\"!");
                    return;
                }

                // TODO: this probably wont work the way it's supposed to because a new Object is created each time and sp.Coroutine will always be null
                if (sp.Coroutine != null) {
                    Instance.StopCoroutine(sp.Coroutine);
                    sp.Coroutine = null;
                }

                sp.Coroutine = Instance.StartCoroutine(ShaderEventCoroutine(sp, startTime, duration, sp.easing));

            });

        }

        private static IEnumerator ShaderEventCoroutine(ShaderProperty property, float startTime, float duration, Functions easing) {
            if (property == null) {
                Logger.log.Error("ShaderEventCoroutine received null ShaderProperty!");
                yield return null;
            }
            while (true) {
                float elapsedTime = Instance.CustomEventCallbackController._audioTimeSource.songTime - startTime;
                float time = Easings.Interpolate(Mathf.Min(elapsedTime / duration, 1f), easing);

                PointDefinition points = property.points;

                if (points != null) {
                    switch (property.PropertyType) {
                        case PropertyType.Linear:
                            property.SetValue(points.InterpolateLinear(time));
                            break;

                        case PropertyType.Vector3:
                            property.Value = points.Interpolate(time);
                            break;

                        case PropertyType.Vector4:
                            property.Value = points.InterpolateVector4(time);
                            break;

                        case PropertyType.Quaternion:
                            property.Value = points.InterpolateQuaternion(time);
                            break;
                    }
                } else {
                    Logger.log.Error("ShaderEventCoroutine: ShaderCommand with id \"" + property.parent.id + "\" and _ref \"" + property.parent.reference + "\" has invalid points at \"" + property.property + "\"!");
                }

                if (elapsedTime < duration) {
                    yield return null;
                } else {
                    break;
                }
            }
        }


    }
}
