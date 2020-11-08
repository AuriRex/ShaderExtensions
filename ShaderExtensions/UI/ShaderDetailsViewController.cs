using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using UnityEngine;
using UnityEngine.Rendering;

namespace ShaderExtensions.UI
{
    internal class ShaderDetailsViewController : BSMLResourceViewController
    {
        public override string ResourceName => "ShaderExtensions.UI.Views.shaderDetails.bsml";

        [UIParams]
        BSMLParserParams parserParams;

        [UIAction("#post-parse")]
        public void PostParse() => SetupDetails(null);

        //private Dictionary<string, int> properties = new Dictionary<string, int>();
        private ShaderEffect _currentShaderFX;

        private bool _usesPreviousFrameData = false;
        private int _customPropertyCount = 0;

        private void SetupDetails(ShaderEffect sfx) {
            if (sfx == null) return;

            Material mat = sfx.material;
            _currentShaderFX = sfx;

            _usesPreviousFrameData = false;
            _customPropertyCount = 0;

            if (mat != null) {
                int propCount = mat.shader.GetPropertyCount();
                _customPropertyCount = propCount - 2;
                ShaderPropertyType spt;
                string propName = "";

                for (int i = 0; i < propCount; i++) {
                    spt = mat.shader.GetPropertyType(i);
                    propName = mat.shader.GetPropertyName(i);
                    if (propName.Equals("_MainTex")) continue;
                    if(propName.Equals("_PrevMainTex")) {
                        _usesPreviousFrameData = true;
                        continue;
                    }

                }
            }

        }

        internal void ShaderSelected(ShaderEffect sfx) => SetupDetails(sfx);

        internal void ShaderSelectionCleared() => SetupDetails(null);

    }
}
