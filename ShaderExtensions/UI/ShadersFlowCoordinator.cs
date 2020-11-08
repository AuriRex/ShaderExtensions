using BeatSaberMarkupLanguage;
using HMUI;
using System;

namespace ShaderExtensions.UI
{
    class ShadersFlowCoordinator : FlowCoordinator
    {

        private ShaderListViewController shaderListView;
        private ShaderPropertyListViewController shaderProperyListView;
        private ShaderDetailsViewController shaderDetailsView;

        public void Awake() {
            if (!shaderListView) {
                shaderListView = BeatSaberUI.CreateViewController<ShaderListViewController>();
            }

            if (!shaderProperyListView) {
                shaderProperyListView = BeatSaberUI.CreateViewController<ShaderPropertyListViewController>();
            }

            if (!shaderDetailsView) {
                shaderDetailsView = BeatSaberUI.CreateViewController<ShaderDetailsViewController>();
            }

            shaderListView.parent = this;

        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling) {

            try {
                if (firstActivation) {
                    SetTitle("Screen Space Shaders");
                    showBackButton = true;
                    ProvideInitialViewControllers(shaderListView, shaderDetailsView, shaderProperyListView); //, left, right);
                }
            } catch (Exception ex) {
                Logger.log.Error(ex);
            }

        }

        internal void ShaderSelectionCleared() => shaderProperyListView.ShaderSelectionCleared();

        protected override void BackButtonWasPressed(ViewController topViewController) => BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this, null, ViewController.AnimationDirection.Horizontal, false);

        public void ShaderSelected(ShaderEffect sfx) {

            shaderProperyListView.ShaderSelected(sfx.material);
            shaderDetailsView.ShaderSelected(sfx);

        }

    }
}
