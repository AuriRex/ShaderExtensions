using CustomJSONData;
using CustomJSONData.CustomBeatmap;
using ShaderExtensions.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace ShaderExtensions.Managers
{
    class ShaderCore : IInitializable, IDisposable
    {
        private PluginConfig _pluginConfig;

        public bool EnableShaderEvents { get; private set; } = false;

        internal ShaderCore(PluginConfig pluginConfig)
        {
            _pluginConfig = pluginConfig;
        }

        public void Initialize() => RegisterCapabilty();

        public void Dispose() => RegisterCapabilty(false);

        internal bool ShouldEnableShaderEvents(IDifficultyBeatmap difficultyBeatmap)
        {
            if (difficultyBeatmap == null) return false;
            if (difficultyBeatmap.beatmapData is CustomBeatmapData customBeatmapData)
            {
                IEnumerable<string> requirements = ((List<object>) Trees.at(customBeatmapData.beatmapCustomData, "_requirements"))?.Cast<string>();
                EnableShaderEvents = requirements?.Contains(Plugin.CAPABILITY) ?? false;
                if (!EnableShaderEvents)
                {
                    IEnumerable<string> suggestions = ((List<object>) Trees.at(customBeatmapData.beatmapCustomData, "_suggestions"))?.Cast<string>();
                    EnableShaderEvents = suggestions?.Contains(Plugin.CAPABILITY) ?? false;
                }
            }
            else
            {
                EnableShaderEvents = false;
            }
            return EnableShaderEvents;
        }

        internal void RegisterCapabilty(bool register = true)
        {
            if (register)
            {
                Logger.log.Info($"Registering the {Plugin.CAPABILITY} capabilty!");
                SongCore.Collections.RegisterCapability(Plugin.CAPABILITY);
            }
            else
            {
                Logger.log.Info($"Deregistering the {Plugin.CAPABILITY} capabilty!");
                SongCore.Collections.DeregisterizeCapability(Plugin.CAPABILITY);
            }
        }
    }
}
