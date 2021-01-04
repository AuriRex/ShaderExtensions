using ShaderExtensions.Managers;
using ShaderExtensions.Util;
using Zenject;

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
