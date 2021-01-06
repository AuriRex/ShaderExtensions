using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using ShaderExtensions.Managers;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace ShaderExtensions.UI
{
    [ViewDefinition("ShaderExtensions.UI.Views.shaderList.bsml")]
    [HotReload(RelativePathToLayout = @"Views\shaderList.bsml")]
    class ShaderListViewController : BSMLAutomaticViewController
    {
        private ShaderAssetLoader _shaderAssetLoader;
        private ShaderManager _shaderManager;

        public event Action<ShaderEffect> shaderSelected;
        public event Action shadersCleared;

        private Dictionary<Texture2D, Sprite> _spriteCache = null;
        private int _selection = -1;

        [Inject]
        public void Construct(ShaderAssetLoader shaderAssetLoader, ShaderManager shaderManager) {
            _shaderAssetLoader = shaderAssetLoader;
            _shaderManager = shaderManager;
        }

        [UIComponent("shader-list")]
        public CustomListTableData customListTableData = null;

        [UIComponent("shader-stack-list")]
        public CustomListTableData shaderStackList = null;

        [UIAction("shader-select")]
        public void Select(TableView _, int row) {
            _selection = row;
            ShaderEffect sfx = _shaderAssetLoader.ShaderEffectList[_selection];
            Logger.log.Info("Selected: " + sfx.name + " by " + sfx.author);
            shaderSelected?.Invoke(sfx);
        }

        [UIAction("reload-shaders")]
        public void ReloadShaders() {
            _shaderAssetLoader.Reload();
            SetupShaderList();
            _shaderManager.RefreshCameraManager();
            _shaderManager.ClearAllMaterials();
            SetupActiveShaderStackList();
        }

        [UIAction("add-shader")]
        public void AddShader() {
            if (_selection > -1) {
                _shaderManager.RefreshCameraManager();
                _shaderManager.AddMaterial("preview", _shaderAssetLoader.ShaderEffectList[_selection]);
                SetupActiveShaderStackList();
            }
        }

        [UIAction("select-shader")]
        public void SelectShader() {
            if (_selection > -1) {
                _shaderManager.RefreshCameraManager();
                _shaderManager.ClearAllMaterials();
                _shaderManager.AddMaterial("preview", _shaderAssetLoader.ShaderEffectList[_selection]);
                SetupActiveShaderStackList();
            }
        }

        [UIAction("clear-shader")]
        public void ClearShader() {
            _shaderManager.RefreshCameraManager();
            _shaderManager.ClearAllMaterials();
            SetupActiveShaderStackList();
        }

        [UIAction("#post-parse")]
        public void PostParse() {
            SetupShaderList();
            SetupActiveShaderStackList();
        }

        public void SetupShaderList() {
            customListTableData.data.Clear();
            if (_spriteCache != null) {
                _spriteCache.Clear();
            } else {
                _spriteCache = new Dictionary<Texture2D, Sprite>();
            }

            _selection = -1;
            shadersCleared?.Invoke();

            List<ShaderEffect> sfxList = _shaderAssetLoader.ShaderEffectList;

            foreach (ShaderEffect sfx in sfxList) {
                Sprite icon = Util.SEUtilities.GetDefaultShaderIcon();

                if (sfx.previewImage != null && _spriteCache.TryGetValue(sfx.previewImage, out Sprite image)) {
                    icon = image;
                } else if (sfx.previewImage != null) {
                    icon = Sprite.Create(sfx.previewImage, new Rect(0.0f, 0.0f, sfx.previewImage.width, sfx.previewImage.height), new Vector2(0.5f, 0.5f), 100.0f);
                    _spriteCache.Add(sfx.previewImage, icon);
                }

                CustomListTableData.CustomCellInfo customCellInfo = new CustomListTableData.CustomCellInfo(sfx.name, sfx.author, icon);
                customListTableData.data.Add(customCellInfo);
            }

            customListTableData.tableView.ReloadData();
        }

        public void SetupActiveShaderStackList() {
            shaderStackList.data.Clear();
            List<Material> matList = _shaderManager.GetAllMaterials();

            foreach(Material mat in matList) {
                ShaderEffect sfx = _shaderManager.GetShaderEffectByMaterial(mat);

                if (sfx == null) continue;

                Sprite icon = Util.SEUtilities.GetDefaultShaderIcon();

                if (sfx.previewImage != null && _spriteCache.TryGetValue(sfx.previewImage, out Sprite image)) {
                    icon = image;
                }

                CustomListTableData.CustomCellInfo customCellInfo = new CustomListTableData.CustomCellInfo(sfx.name, sfx.author, icon);
                shaderStackList.data.Add(customCellInfo);
            }

            shaderStackList.tableView.ReloadData();
        }

        public Sprite GetPreviewImage(ShaderEffect sfx) {
            if (sfx == null || sfx.previewImage == null) return Util.SEUtilities.GetDefaultShaderIcon();
            if (_spriteCache.TryGetValue(sfx.previewImage, out Sprite image)) {
                return image;
            }
            return Util.SEUtilities.GetDefaultShaderIcon();
        }
    }
}
