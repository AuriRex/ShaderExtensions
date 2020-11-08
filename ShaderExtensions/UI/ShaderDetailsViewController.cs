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
        private ShaderEffect currentShaderFX;

        private void SetupDetails(ShaderEffect sfx) {
            if (sfx == null) return;

            Material mat = sfx.material;
            currentShaderFX = sfx;

            if (mat != null) {
                int propCount = mat.shader.GetPropertyCount();

                ShaderPropertyType spt;
                string propName = "";

                for (int i = 0; i < propCount; i++) {
                    spt = mat.shader.GetPropertyType(i);
                    propName = mat.shader.GetPropertyName(i);
                    if (propName.Equals("_MainTex") || propName.Equals("_PrevMainTex")) continue;



                }
            }

        }

        internal void ShaderSelected(ShaderEffect sfx) => SetupDetails(sfx);

        internal void ShaderSelectionCleared() => SetupDetails(null);

    }
}
