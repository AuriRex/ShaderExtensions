using ShaderExtensions.Managers;
using ShaderExtensions.UI;
using SiraUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
