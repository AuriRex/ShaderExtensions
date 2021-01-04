using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using BS_Utils.Utilities;
using HMUI;
using ShaderExtensions.Managers;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace ShaderExtensions.UI
{
    class ShaderListViewController : BSMLResourceViewController
    {
        public override string ResourceName => "ShaderExtensions.UI.Views.shaderList.bsml";

        private ShaderAssetLoader _shaderAssetLoader;
        private ShaderManager _shaderManager;

        public Action<ShaderEffect> shaderSelected;
        public Action shadersCleared;

        [Inject]
        public void Construct(ShaderAssetLoader shaderAssetLoader, ShaderManager shaderManager) {
            _shaderAssetLoader = shaderAssetLoader;
            _shaderManager = shaderManager;
        }

        [UIComponent("shaderList")]
        public CustomListTableData customListTableData = null;

        [UIAction("shaderSelect")]
        public void Select(TableView tv, int row) {
            selection = row;
            ShaderEffect sfx = _shaderAssetLoader.ShaderEffectList[selection];
            Logger.log.Info("Selected: " + sfx.name + " by " + sfx.author);
            shaderSelected?.Invoke(sfx);
        }

        Dictionary<Texture2D, Sprite> Images = null;

        [UIAction("#post-parse")]
        public void SetupList() {
            customListTableData.data.Clear();
            if(Images != null) {
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
                } else if(sfx.previewImage != null) {
                    icon = Sprite.Create(sfx.previewImage, new Rect(0.0f, 0.0f, sfx.previewImage.width, sfx.previewImage.height), new Vector2(0.5f, 0.5f), 100.0f);
                    Images.Add(sfx.previewImage, icon);
                }

                CustomListTableData.CustomCellInfo customCellInfo = new CustomListTableData.CustomCellInfo(sfx.name, sfx.author, icon);
                customListTableData.data.Add(customCellInfo);
            }

            customListTableData.tableView.ReloadData();

        }


        [UIAction("reloadShaders")]
        public void ReloadShaders() {

            _shaderAssetLoader.Reload();
            SetupList();
            _shaderManager.RefreshCameras();
            _shaderManager.ClearAllMaterials();
        }

        int selection = -1;

        [UIAction("addShader")]
        public void AddShader() {

            if (selection > -1) {

                _shaderManager.RefreshCameras();
                _shaderManager.AddMaterial("preview", _shaderAssetLoader.ShaderEffectList[selection]);

            }

        }

        [UIAction("selectShader")]
        public void SelectShader() {

            if (selection > -1) {
                _shaderManager.RefreshCameras();
                _shaderManager.ClearAllMaterials();
                _shaderManager.AddMaterial("preview", _shaderAssetLoader.ShaderEffectList[selection]);
            }

        }

        [UIAction("clearShader")]
        public void ClearShader() {
            _shaderManager.RefreshCameras();
            _shaderManager.ClearAllMaterials();
        }

    }
}
