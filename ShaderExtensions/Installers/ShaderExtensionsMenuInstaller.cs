using ShaderExtensions.Managers;
using ShaderExtensions.UI;
using SiraUtil;
using Zenject;

namespace ShaderExtensions.Installers
{
    internal class ShaderExtensionsMenuInstaller : Installer<ShaderExtensionsMenuInstaller>
    {
        public override void InstallBindings() {
            Container.Bind<ShaderPropertyListViewController>().FromNewComponentAsViewController().AsSingle();
            Container.Bind<ShaderListViewController>().FromNewComponentAsViewController().AsSingle();
            Container.Bind<ShaderDetailsViewController>().FromNewComponentAsViewController().AsSingle();

            Container.Bind<ShadersFlowCoordinator>().FromNewComponentOnNewGameObject(nameof(ShadersFlowCoordinator)).AsSingle();

            Container.BindInterfacesTo<MenuButtonManager>().AsSingle();

            Container.BindInterfacesAndSelfTo<MenuCameraManager>().AsSingle();
        }
    }
}
