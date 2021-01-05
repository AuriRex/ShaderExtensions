﻿using BeatSaberMarkupLanguage.Attributes;
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
        //public override string ResourceName => "ShaderExtensions.UI.Views.shaderList.bsml";

        private ShaderAssetLoader _shaderAssetLoader;
        private ShaderManager _shaderManager;

        public Action<ShaderEffect> shaderSelected;
        public Action shadersCleared;

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
        public void Select(TableView tv, int row) {
            selection = row;
            ShaderEffect sfx = _shaderAssetLoader.ShaderEffectList[selection];
            Logger.log.Info("Selected: " + sfx.name + " by " + sfx.author);
            shaderSelected?.Invoke(sfx);
        }

        Dictionary<Texture2D, Sprite> Images = null;

        public Sprite GetPreviewImage(ShaderEffect sfx) {
            if(sfx == null || sfx.previewImage == null) return Util.SEUtilities.GetDefaultShaderIcon();
            if (Images.TryGetValue(sfx.previewImage, out Sprite image)) {
                return image;
            }
            return Util.SEUtilities.GetDefaultShaderIcon();
        }

        [UIAction("#post-parse")]
        public void SetupList() {
            customListTableData.data.Clear();
            if (Images != null) {
                Images.Clear();
            } else {
                Images = new Dictionary<Texture2D, Sprite>();
            }

            selection = -1;
            shadersCleared?.Invoke();

            // Loop thorugh all shaders and add to list

            List<ShaderEffect> sfxList = _shaderAssetLoader.ShaderEffectList;

            foreach (ShaderEffect sfx in sfxList) {

                /*if (sfx.previewImage == null) {
                    sfx.previewImage = defaultImage;
                }*/
                Sprite icon = Util.SEUtilities.GetDefaultShaderIcon();

                if (sfx.previewImage != null && Images.ContainsKey(sfx.previewImage)) {
                    Images.TryGetValue(sfx.previewImage, out Sprite image);
                    icon = image;
                } else if (sfx.previewImage != null) {
                    icon = Sprite.Create(sfx.previewImage, new Rect(0.0f, 0.0f, sfx.previewImage.width, sfx.previewImage.height), new Vector2(0.5f, 0.5f), 100.0f);
                    Images.Add(sfx.previewImage, icon);
                }

                CustomListTableData.CustomCellInfo customCellInfo = new CustomListTableData.CustomCellInfo(sfx.name, sfx.author, icon);
                customListTableData.data.Add(customCellInfo);
            }

            customListTableData.tableView.ReloadData();

        }


        [UIAction("reload-shaders")]
        public void ReloadShaders() {

            _shaderAssetLoader.Reload();
            SetupList();
            _shaderManager.RefreshCameraManager();
            _shaderManager.ClearAllMaterials();
        }

        int selection = -1;

        [UIAction("add-shader")]
        public void AddShader() {

            if (selection > -1) {

                _shaderManager.RefreshCameraManager();
                _shaderManager.AddMaterial("preview", _shaderAssetLoader.ShaderEffectList[selection]);

            }

        }

        [UIAction("select-shader")]
        public void SelectShader() {

            if (selection > -1) {
                _shaderManager.RefreshCameraManager();
                _shaderManager.ClearAllMaterials();
                _shaderManager.AddMaterial("preview", _shaderAssetLoader.ShaderEffectList[selection]);
            }

        }

        [UIAction("clear-shader")]
        public void ClearShader() {
            _shaderManager.RefreshCameraManager();
            _shaderManager.ClearAllMaterials();
        }

    }
}
