using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using ShaderExtensions.Util;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

namespace ShaderExtensions.UI
{
    internal class ShaderDetailsViewController : BSMLResourceViewController
    {
        public override string ResourceName => "ShaderExtensions.UI.Views.shaderDetails.bsml";

        private PluginConfig _pluginConfig;
        private ShaderListViewController _shaderListViewController;


        [UIParams]
        BSMLParserParams parserParams = null;

        [UIValue("clear-on-beat")]
        public bool _clearOnBeat {
            get => _pluginConfig.ClearEffectsOnLevelCompletion;
            set => _pluginConfig.ClearEffectsOnLevelCompletion = value;
        }

        [UIValue("clear-on-back-button")]
        public bool _clearOnBackButton {
            get => _pluginConfig.ClearPreviewEffects;
            set => _pluginConfig.ClearPreviewEffects = value;
        }

        [UIComponent("shader-description")]
        public TextPageScrollView shaderDescription = null;

        [Inject]
        public void Construct(PluginConfig pluginConfig, ShaderListViewController shaderListViewController) {
            _pluginConfig = pluginConfig;
            _shaderListViewController = shaderListViewController;
        }

        [UIComponent("shader-icon")]
        private ClickableImage _shaderIcon;

        [UIAction("#post-parse")]
        public void PostParse() => SetupDetails(null);

        private ShaderEffect _currentShaderFX;

        private bool _usesPreviousFrameData = false;
        private int _customPropertyCount = 0;

        private void SetupDetails(ShaderEffect sfx) {
            if (sfx == null) {
                _shaderIcon.sprite = SEUtilities.GetDefaultShaderIcon();
                shaderDescription.SetText("Select a Shader!");
                return;
            }

            Material mat = sfx.material;
            _currentShaderFX = sfx;

            _usesPreviousFrameData = false;
            _customPropertyCount = 0;

            if (mat != null) {
                int propCount = mat.shader.GetPropertyCount();
                _customPropertyCount = propCount;
                ShaderPropertyType spt;
                string propName = "";

                for (int i = 0; i < propCount; i++) {
                    spt = mat.shader.GetPropertyType(i);
                    propName = mat.shader.GetPropertyName(i);
                    if (propName.Equals("_MainTex")) {
                        _customPropertyCount--;
                        continue;
                    }
                    if (propName.Equals("_PrevMainTex")) {
                        _customPropertyCount--;
                        _usesPreviousFrameData = true;
                        continue;
                    }

                }
            }

            _shaderIcon.sprite = _shaderListViewController.GetPreviewImage(_currentShaderFX);
            shaderDescription.SetText(sfx.description);

        }

        internal void ShaderSelected(ShaderEffect sfx) => SetupDetails(sfx);

        internal void ShaderSelectionCleared() => SetupDetails(null);

    }
}
