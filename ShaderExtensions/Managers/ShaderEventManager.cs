using CustomJSONData;
using CustomJSONData.CustomBeatmap;
using Heck.Animation;
using ShaderExtensions.Event;
using ShaderExtensions.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using TreeDict = System.Collections.Generic.IDictionary<string, object>;

namespace ShaderExtensions.Managers
{
    public class ShaderEventManager : IInitializable, IDisposable
    {
        public class Trees
        {
            public static dynamic At(TreeDict customData, string pointName)
            {
                if (customData.TryGetValue(pointName, out dynamic value))
                    return value;
                return null;
            }
        }

        private ShaderCore _shaderCore;
        private ShaderManager _shaderManager;
        private PluginConfig _pluginConfig;

        private DiContainer _container;

        private CustomEventCallbackController _customEventCallbackController;
        private BeatmapObjectSpawnController _beatmapObjectSpawnController;
        private IDifficultyBeatmap _difficultyBeatmap;
        private CustomEventCallbackController.CustomEventCallbackData _customEventCallbackData;

        public bool IsEnabled { get; private set; } = false;

        [Inject]
        internal ShaderEventManager(DiContainer Container, ShaderCore shaderCore, ShaderManager shaderManager, PluginConfig pluginConfig, [InjectOptional] BeatmapObjectSpawnController beatmapObjectSpawnController, [InjectOptional] IDifficultyBeatmap difficultyBeatmap)
        {
            _container = Container;

            _shaderCore = shaderCore;
            _shaderManager = shaderManager;
            _pluginConfig = pluginConfig;

            _beatmapObjectSpawnController = beatmapObjectSpawnController;
            _difficultyBeatmap = difficultyBeatmap;
        }

        internal void CustomEventCallbackInit(CustomEventCallbackController customEventCallbackController)
        {
            Logger.log.Debug("CustomEventCallbackController: " + customEventCallbackController);

            if (!_shaderCore.ShouldEnableShaderEvents(_difficultyBeatmap))
            {
                IsEnabled = false;
                Logger.log.Debug("Shader Extensions Requirement / Suggestion NOT set, disabling Shader Event Handler!");
                return;
            }

            IsEnabled = true;
            Logger.log.Debug("Shader Extensions Requirement / Suggestion set, enabling Shader Event Handler!");

            _customEventCallbackController = customEventCallbackController;
            _customEventCallbackData = _customEventCallbackController.AddCustomEventCallback(ShaderEventCallback);
        }

        public const string EventTypeShader = "Shader";
        public const string EventTypeShaderClear = "ShaderClear";

        private List<List<ShaderCommand>> _shaderCommandLists;

        private void ShaderEventCallback(CustomEventData customEventData)
        {
            if (customEventData.data == null) return;
            try
            {
                TreeDict eventData;
                switch (customEventData.type)
                {
                    case EventTypeShader:
                        //Logger.log?.Debug("Shader event received!");

                        eventData = new Dictionary<string, object>(customEventData.data as TreeDict);

                        object res = Trees.At(eventData, "_shaders");
                        List<object> shaders;
                        if (res != null)
                        {
                            shaders = res as List<object>;
                        }
                        else
                        {
                            shaders = new List<object>();
                        }

                        List<ShaderCommand> scList = new List<ShaderCommand>();

                        foreach (TreeDict shader in shaders)
                        {
                            scList.Add(new ShaderCommand(shader));
                        }

                        foreach (ShaderCommand sc in scList)
                        {

                            ShaderEffectData sfx = _shaderManager.GetShaderEffectByReferenceName(sc.ReferenceName);

                            if (sfx != null)
                            {

                                sc.ShaderEffectData = sfx;

                                Material mat = _shaderManager.GetMaterial(sc.ID, sfx);

                                if (mat == null)
                                {
                                    mat = _shaderManager.AddMaterial(sc.ID, sfx);
                                }

                                sc.Material = mat;

                                StartEventCoroutine(sc, customEventData.time);
                            }
                            else
                            {
                                Logger.log.Error($"Unknown Shader reference used: '{sc.ReferenceName}'!");
                            }

                        }
                        break;
                    case EventTypeShaderClear:
                        string clearId;
                        string refName;
                        eventData = new Dictionary<string, object>(customEventData.data as TreeDict);
                        clearId = Trees.At(eventData, "_clearID");
                        if (clearId == null)
                        {
                            clearId = Trees.At(eventData, "_clearId");
                        }
                        refName = Trees.At(eventData, "_ref");
                        Logger.log.Debug($"ShaderClear at : {customEventData.time / 60 * _beatmapObjectSpawnController.currentBpm}");
                        Logger.log.Debug($"_clearId : {clearId}");
                        Logger.log.Debug($"_ref : {refName}");
                        if (clearId != null)
                        {
                            if (clearId.Equals("*"))
                            {
                                List<Material> removedMats = _shaderManager.ClearAllMaterials();
                                if (removedMats.Count > 0)
                                {
                                    Logger.log.Debug($"Clearing all {removedMats.Count} Materials!");
                                    StopAllCoroutinesModifyingMaterials(removedMats);
                                }
                                break;
                            }
                            ShaderEffectData sfx = _shaderManager.GetShaderEffectByReferenceName(refName);

                            Logger.log.Debug($"sfx reference Name : {sfx?.ReferenceName}");

                            if (sfx != null)
                            {

                                if (!_shaderManager.RemoveMaterial(clearId, sfx))
                                {
                                    Logger.log.Notice($"Tried to remove a Shader with an ID that doesn't exist: '{clearId}' at time (in beats) {customEventData.time / 60 * _beatmapObjectSpawnController.currentBpm}!");
                                }

                            }
                            else
                            {
                                Logger.log.Debug($"trying to remove all with clearId");
                                List<Material> removedMats = _shaderManager.RemoveAllMaterialsStartingWithId(clearId);
                                if (removedMats.Count == 0)
                                {
                                    Logger.log.Notice($"Tried to remove all Shaders starting with an ID that doesn't exist: '{clearId}' at time (in beats) {customEventData.time / 60 * _beatmapObjectSpawnController.currentBpm}!");
                                }
                                else
                                {
                                    Logger.log.Debug($"Clearing {removedMats.Count} Materials with starting ID {clearId}!");
                                    StopAllCoroutinesModifyingMaterials(removedMats);
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error($"{nameof(ShaderEventManager)} encountered an exception at time (in beats) {customEventData.time / 60 * _beatmapObjectSpawnController.currentBpm}: {ex.Message}");
                Logger.log.Error(ex.StackTrace);
            }
        }

        internal void StartEventCoroutine(ShaderCommand shaderCommand, float startTime)
        {

            if (shaderCommand.Material == null)
            {
                Logger.log.Error("ShaderCommand with null material!");
                return;
            }

            shaderCommand.Properties.getProps().ForEach(sp => {
                float duration = sp.Duration;
                duration = 60f * duration / _beatmapObjectSpawnController.currentBpm;

                if (!shaderCommand.Material.HasProperty(sp.Property))
                {
                    Logger.log.Error("ShaderCommand \"" + shaderCommand.ID + "\" wants to set a nonexistant property \"" + sp.Property + "\" for material \"" + shaderCommand.ReferenceName + "\"!");
                    return;
                }

                sp.Coroutine = SharedCoroutineStarter.instance.StartCoroutine(ShaderEventCoroutine(sp, startTime, duration, sp.Easing));

            });

        }

        private IEnumerator ShaderEventCoroutine(ShaderProperty property, float startTime, float duration, Functions easing)
        {
            if (property == null)
            {
                Logger.log.Error("ShaderEventCoroutine received null ShaderProperty!");
                yield return null;
            }
            while (true)
            {
                float elapsedTime = _customEventCallbackController.AudioTimeSource.songTime - startTime;
                float time = Easings.Interpolate(Mathf.Min(elapsedTime / duration, 1f), easing);

                PointDefinition points = property.Points;

                if (points != null)
                {
                    switch (property.PropertyType)
                    {
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
                }
                else
                {
                    Logger.log.Error("ShaderEventCoroutine: ShaderCommand with id \"" + property.ParentCommand.ID + "\" and _ref \"" + property.ParentCommand.ReferenceName + "\" has invalid points at \"" + property.Property + "\"!");
                }

                if (elapsedTime < duration)
                {
                    yield return null;
                }
                else
                {
                    break;
                }
            }
            if (property.IsLast && property.ParentCommand.ClearAfterLastPropIsDone)
            {
                if (!_shaderManager.RemoveMaterial(property.ParentCommand.ID, property.ParentCommand.ShaderEffectData))
                {
                    Logger.log.Error($"Tried to remove a Shader with an ID that doesn't exist: '{property.ParentCommand.ID}' at time (in beats) {_customEventCallbackController.AudioTimeSource.songTime / 60 * _beatmapObjectSpawnController.currentBpm}!");
                }
                StopAllCoroutinesModifyingMaterials(new List<Material>() { property.ParentCommand.Material });
                Logger.log.Debug($"Material removed after last property stopped animating! ID: {property.ParentCommand.ID} - ref: {property.ParentCommand.ReferenceName}");
            }
        }

        /// <summary>
        /// Stops the Material Property modifying Coroutines for all given Materials in the List.
        /// WARNING: Passing null stops EVERY Coroutine for ALL Materials!
        /// </summary>
        /// <param name="mats">The Material List</param>
        private void StopAllCoroutinesModifyingMaterials(List<Material> mats)
        {
            foreach (List<ShaderCommand> shaderCommandList in _shaderCommandLists)
            {
                foreach (ShaderCommand shaderCommand in shaderCommandList)
                {
                    if (mats == null || mats.Contains(shaderCommand.Material))
                    {
                        shaderCommand.Properties.getProps().ForEach(sp => {
                            if (sp.Coroutine != null)
                            {
                                SharedCoroutineStarter.instance.StopCoroutine(sp.Coroutine);
                                sp.Coroutine = null;
                            }
                        });
                    }
                }
            }
        }

        public void Initialize()
        {
            CustomEventCallbackController.didInitEvent += CustomEventCallbackInit;
            _shaderCommandLists = new List<List<ShaderCommand>>();
        }

        public void Dispose()
        {
            CustomEventCallbackController.didInitEvent -= CustomEventCallbackInit;
            if (_customEventCallbackData != null)
            {
                _customEventCallbackController.RemoveBeatmapEventCallback(_customEventCallbackData);
            }
        }
    }
}
