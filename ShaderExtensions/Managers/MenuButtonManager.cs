using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using ShaderExtensions.UI;
using System;
using Zenject;

namespace ShaderExtensions.Managers
{
    internal class MenuButtonManager : IInitializable, IDisposable
    {
        private readonly MenuButton _menuButton;
        private readonly MenuButton _clearEffectButton;
        private readonly MainFlowCoordinator _mainFlowCoordinator;
        private readonly ShadersFlowCoordinator _shadersFlowCoordinator;
        private readonly ShaderManager _shaderManager;

        public MenuButtonManager(MainFlowCoordinator mainFlowCoordinator, ShadersFlowCoordinator shadersFlowCoordinator, ShaderManager shaderManager) {
            _mainFlowCoordinator = mainFlowCoordinator;
            _shadersFlowCoordinator = shadersFlowCoordinator;
            _shaderManager = shaderManager;
            _menuButton = new MenuButton("Shaders", "Change Screen Space Shaders Here!", ShowNotesFlow, true);
            _clearEffectButton = new MenuButton("[SE] Clear", "Clear all camera effects", ClearAllMaterialsButton, true);
        }

        public void Initialize() {
            MenuButtons.instance.RegisterButton(_menuButton);
            MenuButtons.instance.RegisterButton(_clearEffectButton);
        }

        public void Dispose() {
            if (MenuButtons.IsSingletonAvailable) {
                MenuButtons.instance.UnregisterButton(_menuButton);
                MenuButtons.instance.UnregisterButton(_clearEffectButton);
            }
        }

        private void ShowNotesFlow() => _mainFlowCoordinator.PresentFlowCoordinator(_shadersFlowCoordinator);

        private void ClearAllMaterialsButton() => _shaderManager.ClearAllMaterials();

    }
}
