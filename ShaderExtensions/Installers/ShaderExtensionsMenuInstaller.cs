using ShaderExtensions.Managers;
using ShaderExtensions.UI;
using SiraUtil;
using Zenject;

namespace ShaderExtensions.Installers
{
    internal class ShaderExtensionsMenuInstaller : Installer<ShaderExtensionsMenuInstaller>
    {
        public override void InstallBindings() {
            Container.BindViewController<ShaderPropertyListViewController>();
            Container.BindViewController<ShaderListViewController>();
            Container.BindViewController<ShaderDetailsViewController>();
            Container.BindFlowCoordinator<ShadersFlowCoordinator>();

            Container.BindInterfacesTo<MenuButtonManager>().AsSingle();

            Container.BindInterfacesAndSelfTo<CameraManager>().AsSingle();
        }
    }
}
