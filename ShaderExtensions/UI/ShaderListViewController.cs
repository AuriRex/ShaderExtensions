using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using BS_Utils.Utilities;
using HMUI;
using System.Collections.Generic;
using UnityEngine;

namespace ShaderExtensions.UI
{
    class ShaderListViewController : BSMLResourceViewController
    {
        public override string ResourceName => "ShaderExtensions.UI.Views.shaderList.bsml";

        public ShadersFlowCoordinator parent;

        [UIComponent("shaderList")]
        public CustomListTableData customListTableData;

        [UIAction("shaderSelect")]
        public void Select(TableView tv, int row) {


            selection = row;
            ShaderEffect sfx = ShaderExtensionsController.instance.shaderEffectList[selection];
            Logger.log.Info("Selected: " + sfx.name + " by " + sfx.author);
            parent.ShaderSelected(sfx);

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
            parent.ShaderSelectionCleared();

            // Loop thorugh all shaders and add to list

            List<ShaderEffect> sfxList = ShaderExtensionsController.instance.shaderEffectList;

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

            ShaderExtensionsController.instance.LoadShaders();
            SetupList();

        }

        int selection = -1;

        [UIAction("addShader")]
        public void AddShader() {

            if (selection > -1) {

                ShaderExtensionsController.instance.AddShader(ShaderExtensionsController.instance.shaderEffectList[selection]);

            }

        }

        [UIAction("selectShader")]
        public void SelectShader() {

            if (selection > -1) {

                ShaderExtensionsController.instance.ClearShaders();
                ShaderExtensionsController.instance.AddShader(ShaderExtensionsController.instance.shaderEffectList[selection]);

            }

        }

        [UIAction("clearShader")]
        public void ClearShader() => ShaderExtensionsController.instance.ClearShaders();

    }
}
