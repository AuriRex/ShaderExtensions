using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using ShaderExtensions.UI;
using ShaderExtensions.Util;
using System;
using Zenject;

namespace ShaderExtensions.Managers
{
    internal class MenuButtonManager : IInitializable, IDisposable
    {
        private readonly MenuButton _menuButton;
        //private readonly MenuButton _clearEffectButton;
        private readonly MainFlowCoordinator _mainFlowCoordinator;
        private readonly PluginConfig _pluginConfig;
        private readonly ShadersFlowCoordinator _shadersFlowCoordinator;
        private readonly ShaderManager _shaderManager;

        private bool _buttonIsRegistered = false;

        public MenuButtonManager(MainFlowCoordinator mainFlowCoordinator, PluginConfig pluginConfig, ShadersFlowCoordinator shadersFlowCoordinator, ShaderManager shaderManager)
        {
            _mainFlowCoordinator = mainFlowCoordinator;
            _pluginConfig = pluginConfig;
            _shadersFlowCoordinator = shadersFlowCoordinator;
            _shaderManager = shaderManager;
            _menuButton = new MenuButton("Shaders", "Inspect Screen Space Shaders Here!", ShowNotesFlow, true);
            //_clearEffectButton = new MenuButton("[SE] Clear", "Clear all camera effects", ClearAllMaterialsButton, true);
        }

        public void Initialize()
        {
            if (_pluginConfig.ShowMenuButton)
            {
                MenuButtons.instance.RegisterButton(_menuButton);
                //MenuButtons.instance.RegisterButton(_clearEffectButton);
                _buttonIsRegistered = true;
            }
        }

        public void Dispose()
        {
            if (MenuButtons.IsSingletonAvailable && _buttonIsRegistered)
            {
                MenuButtons.instance.UnregisterButton(_menuButton);
                //MenuButtons.instance.UnregisterButton(_clearEffectButton);
            }
        }

        private void ShowNotesFlow() => _mainFlowCoordinator.PresentFlowCoordinator(_shadersFlowCoordinator);

        private void ClearAllMaterialsButton() => _shaderManager.ClearAllMaterials();

    }
}
