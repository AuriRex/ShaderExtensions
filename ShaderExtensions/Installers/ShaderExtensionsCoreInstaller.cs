using ShaderExtensions.Configuration;
using ShaderExtensions.Managers;
using Zenject;
using SiraUtil.Interfaces;

namespace ShaderExtensions.Installers
{
    internal class ShaderExtensionsCoreInstaller : Installer<PluginConfig, ShaderExtensionsCoreInstaller>
    {

        private readonly PluginConfig _pluginConfig;

        public ShaderExtensionsCoreInstaller(PluginConfig pluginConfig) {
            _pluginConfig = pluginConfig;
        }

        public override void InstallBindings() {

            Container.BindInstance(_pluginConfig).AsSingle();
            Container.BindInterfacesAndSelfTo<ShaderAssetLoader>().AsSingle();
            Container.BindInterfacesAndSelfTo<ShaderManager>().AsSingle();

        }
    }
}
