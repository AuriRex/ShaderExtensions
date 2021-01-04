using CustomJSONData;
using CustomJSONData.CustomBeatmap;
using NoodleExtensions.Animation;
using ShaderExtensions.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;
using TreeDict = System.Collections.Generic.IDictionary<string, object>;

namespace ShaderExtensions.Managers
{
    public class ShaderEventManager : IInitializable, IDisposable
    {
        private ShaderManager _shaderManager;

        private CustomEventCallbackController _customEventCallbackController;
        private BeatmapObjectSpawnController _beatmapObjectSpawnController;
        private CustomEventCallbackController.CustomEventCallbackData _customEventCallbackData;

        [Inject]
        internal ShaderEventManager(ShaderManager shaderManager, BeatmapObjectSpawnController beatmapObjectSpawnController) {
            _shaderManager = shaderManager;
            _beatmapObjectSpawnController = beatmapObjectSpawnController;
        }

        internal void CustomEventCallbackInit(CustomEventCallbackController customEventCallbackController) {
            Logger.log.Debug("CustomEventCallbackController: " + customEventCallbackController);

            _customEventCallbackController = customEventCallbackController;
            _customEventCallbackData = _customEventCallbackController.AddCustomEventCallback(ShaderEventCallback);
        }

        private void ShaderEventCallback(CustomEventData customEventData) {
            if (customEventData.data == null) return;
            if (customEventData.type == "Shader") {
                Logger.log?.Info("Shader event received!");
                try {
                    bool clear = false;
                    TreeDict eventData = new Dictionary<string, object>(customEventData.data as TreeDict);

                    object res = Trees.At(eventData, "_shaders");
                    List<object> shaders;
                    if (res != null) {
                        shaders = res as List<object>;
                    } else {
                        shaders = new List<object>();
                    }

                    // this is stupid -> remove
                    dynamic tmp = Trees.At(eventData, "_clearAll");
                    if (tmp != null && tmp.GetType() == typeof(bool)) {
                        clear = tmp;
                    }

                    List<ShaderCommand> scList = new List<ShaderCommand>();

                    foreach (TreeDict shader in shaders) {
                        scList.Add(new ShaderCommand(shader));
                    }

                    foreach (ShaderCommand sc in scList) {

                        ShaderEffect sfx = _shaderManager.GetShaderEffectByReferenceName(sc.Reference);

                        if(sfx != null) {

                            string id = "";

                            Material mat = _shaderManager.GetMaterial(id, sfx);

                            if(mat == null) {
                                mat = _shaderManager.AddMaterial(id, sfx);
                            }

                            sc.material = mat;

                            StartEventCoroutine(sc, customEventData.time);
                        } else {
                            Logger.log.Error($"Unknown Shader reference used: '{sc.Reference}'!");
                        }

                    }

                } catch (Exception ex) {
                    Logger.log.Error("ShaderEventController encountered an exception: " + ex.Message);
                    Logger.log.Error(ex.StackTrace);
                }


            }
        }

        internal void StartEventCoroutine(ShaderCommand shaderCommand, float startTime) {

            if (shaderCommand.material == null) {
                Logger.log.Error("ShaderCommand with null material!");
                return;
            }

            shaderCommand.Properties.getProps().ForEach(sp => {
                float duration = sp.Duration;
                duration = 60f * duration / _beatmapObjectSpawnController.currentBpm;

                if (!shaderCommand.material.HasProperty(sp.Property)) {
                    Logger.log.Error("ShaderCommand \"" + shaderCommand.ID + "\" wants to set a nonexistant property \"" + sp.Property + "\" for material \"" + shaderCommand.Reference + "\"!");
                    return;
                }

                // TODO: this probably wont work the way it's supposed to because a new Object is created each time and sp.Coroutine will always be null
                if (sp.Coroutine != null) {
                    SharedCoroutineStarter.instance.StopCoroutine(sp.Coroutine);
                    sp.Coroutine = null;
                }

                sp.Coroutine = SharedCoroutineStarter.instance.StartCoroutine(ShaderEventCoroutine(sp, startTime, duration, sp.Easing));

            });

        }

        private IEnumerator ShaderEventCoroutine(ShaderProperty property, float startTime, float duration, Functions easing) {
            if (property == null) {
                Logger.log.Error("ShaderEventCoroutine received null ShaderProperty!");
                yield return null;
            }
            while (true) {
                float elapsedTime = _customEventCallbackController._audioTimeSource.songTime - startTime;
                float time = Easings.Interpolate(Mathf.Min(elapsedTime / duration, 1f), easing);

                PointDefinition points = property.Points;

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
                    Logger.log.Error("ShaderEventCoroutine: ShaderCommand with id \"" + property.ParentCommand.ID + "\" and _ref \"" + property.ParentCommand.Reference + "\" has invalid points at \"" + property.Property + "\"!");
                }

                if (elapsedTime < duration) {
                    yield return null;
                } else {
                    break;
                }
            }
        }

        public void Initialize() {
            CustomEventCallbackController.customEventCallbackControllerInit += CustomEventCallbackInit;
        }

        public void Dispose() {
            CustomEventCallbackController.customEventCallbackControllerInit -= CustomEventCallbackInit;
            if (_customEventCallbackData != null) {
                _customEventCallbackController.RemoveBeatmapEventCallback(_customEventCallbackData);
            }
        }
    }
}
