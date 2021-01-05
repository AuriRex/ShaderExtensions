using BeatSaberMarkupLanguage;
using HMUI;
using ShaderExtensions.Managers;
using ShaderExtensions.Util;
using Zenject;

namespace ShaderExtensions.UI
{
    class ShadersFlowCoordinator : FlowCoordinator
    {
        private MainFlowCoordinator _mainFlow;
        private ShaderListViewController _shaderListView;
        private ShaderPropertyListViewController _shaderProperyListView;
        private ShaderDetailsViewController _shaderDetailsView;

        private PluginConfig _pluginConfig;
        private ShaderManager _shaderManager;

        [Inject]
        public void Construct(MainFlowCoordinator mainFlow, ShaderListViewController shaderListViewController, ShaderPropertyListViewController shaderPropertyListViewController, ShaderDetailsViewController shaderDetailsViewController, PluginConfig pluginConfig, ShaderManager shaderManager) {
            _mainFlow = mainFlow;
            _shaderListView = shaderListViewController;
            _shaderProperyListView = shaderPropertyListViewController;
            _shaderDetailsView = shaderDetailsViewController;
            _pluginConfig = pluginConfig;
            _shaderManager = shaderManager;
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling) {
            if (firstActivation) {
                SetTitle("Screen Space Shaders");
                showBackButton = true;
            }
            ProvideInitialViewControllers(_shaderListView, _shaderDetailsView, _shaderProperyListView);
            _shaderListView.shaderSelected += _shaderDetailsView.ShaderSelected;
            _shaderListView.shaderSelected += _shaderProperyListView.ShaderSelected;
            _shaderListView.shadersCleared += _shaderProperyListView.ShaderSelectionCleared;
        }

        protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling) {
            _shaderListView.shaderSelected -= _shaderDetailsView.ShaderSelected;
            _shaderListView.shaderSelected -= _shaderProperyListView.ShaderSelected;
            _shaderListView.shadersCleared -= _shaderProperyListView.ShaderSelectionCleared;
            base.DidDeactivate(removedFromHierarchy, screenSystemDisabling);
        }

        protected override void BackButtonWasPressed(ViewController topViewController) {
            if(_pluginConfig.ClearPreviewEffects) {
                _shaderManager.ClearAllMaterials();
            }
            _mainFlow.DismissFlowCoordinator(this, null);
        } 


    }
}
