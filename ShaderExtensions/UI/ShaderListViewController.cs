using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        [UIAction("#post-parse")]
        public void SetupList() {
            customListTableData.data.Clear();
            selection = -1;
            parent.ShaderSelectionCleared();
            // Loop thorugh all shaders and add to list

            List<ShaderEffect> sfxList = ShaderExtensionsController.instance.shaderEffectList;

            foreach (ShaderEffect sfx in sfxList) {
                CustomListTableData.CustomCellInfo customCellInfo = new CustomListTableData.CustomCellInfo(sfx.name, sfx.author, sfx.previewImage);
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

            if(selection > -1) {

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
        public void ClearShader() {
            ShaderExtensionsController.instance.ClearShaders();
        }

    }
}
