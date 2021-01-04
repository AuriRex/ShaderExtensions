using ShaderExtensions.Managers;
using Zenject;

namespace ShaderExtensions.Installers
{
    internal class ShaderExtensionsGameInstaller : Installer<ShaderExtensionsGameInstaller>
    {
        public override void InstallBindings() {
            Container.BindInterfacesAndSelfTo<CameraManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<ShaderEventManager>().AsSingle();
        }
    }
}
