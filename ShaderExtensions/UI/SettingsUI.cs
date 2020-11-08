using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;

namespace ShaderExtensions.UI
{
    class SettingsUI
    {

        private static readonly MenuButton menuButton = new MenuButton("Shaders", "Change Screen Space Shaders Here!", ShadersMenuButtonPressed, true);

        public static ShadersFlowCoordinator shadersFlowCoordinator;

        public static void Enable() => MenuButtons.instance.RegisterButton(menuButton);

        private static void ShadersMenuButtonPressed() {
            if (!shadersFlowCoordinator) {
                shadersFlowCoordinator = BeatSaberUI.CreateFlowCoordinator<ShadersFlowCoordinator>();
            }

            BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(shadersFlowCoordinator, null, HMUI.ViewController.AnimationDirection.Horizontal, false, false);

        }

    }
}
