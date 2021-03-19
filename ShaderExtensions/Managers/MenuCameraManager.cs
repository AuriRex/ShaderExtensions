using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

namespace ShaderExtensions.Managers
{
    internal class MenuCameraManager : CameraManager
    {
        [Inject]
        internal MenuCameraManager(ShaderManager shaderManager) : base(shaderManager) {
            _shaderManager = shaderManager;
        }

        public override void Dispose() {
            Logger.log.Debug("Disposing CameraManager!");
            Clean();
            _shaderManager.CameraManager = null;
        }

    }
}
