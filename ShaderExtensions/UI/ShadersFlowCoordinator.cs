using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using HMUI;

namespace ShaderExtensions.UI
{
    class ShadersFlowCoordinator : FlowCoordinator
    {

        private ShaderListViewController shaderListView;
        private ShaderPropertyListViewController shaderProperyListView;

        public void Awake() {
            if (!shaderListView) {
                shaderListView = BeatSaberUI.CreateViewController<ShaderListViewController>();
            }

            if (!shaderProperyListView) {
                shaderProperyListView = BeatSaberUI.CreateViewController<ShaderPropertyListViewController>();
            }

            shaderListView.parent = this;

        }

        protected override void DidActivate(bool firstActivation, ActivationType activationType) {

            try {
                if (firstActivation) {
                    title = "Screen Space Shaders";
                    showBackButton = true;
                    ProvideInitialViewControllers(shaderListView, null, shaderProperyListView); //, left, right);
                }
            } catch (Exception ex) {
                Logger.log.Error(ex);
            }

        }

        internal void ShaderSelectionCleared() {
            shaderProperyListView.ShaderSelectionCleared();
        }

        protected override void BackButtonWasPressed(ViewController topViewController) {

            BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this, null, false);

        }

        public void ShaderSelected(ShaderEffect sfx) {

            shaderProperyListView.ShaderSelected(sfx.material);

        }

    }
}
