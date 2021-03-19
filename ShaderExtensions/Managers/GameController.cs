using System;
using Zenject;

namespace ShaderExtensions.Managers
{
    internal class GameController : IInitializable, IDisposable
    {
        private ShaderManager _shaderManager;

        [Inject]
        internal GameController(ShaderManager shaderManager){
            _shaderManager = shaderManager;
        }

        public void Dispose() => _shaderManager.OnGameQuit();

        public void Initialize() => _shaderManager.OnGameStart();
    }
}
