﻿using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using ShaderExtensions.Util;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

namespace ShaderExtensions.UI
{
    [ViewDefinition("ShaderExtensions.UI.Views.shaderDetails.bsml")]
    [HotReload(RelativePathToLayout = @"Views\shaderDetails.bsml")]
    internal class ShaderDetailsViewController : BSMLAutomaticViewController
    {
        private PluginConfig _pluginConfig;
        private ShaderListViewController _shaderListViewController;

        private ShaderEffectData _currentShaderEffect;
        private int _customPropertyCount = 0;

        [Inject]
        public void Construct(PluginConfig pluginConfig, ShaderListViewController shaderListViewController)
        {
            _pluginConfig = pluginConfig;
            _shaderListViewController = shaderListViewController;
        }

        [UIComponent("shader-description")]
        protected TextPageScrollView shaderDescription = null;

        [UIComponent("shader-icon")]
        private ImageView _shaderIcon = null!;

        [UIValue("clear-on-beat")]
        protected bool ClearOnBeat
        {
            get => _pluginConfig.ClearEffectsOnLevelCompletion;
            set => _pluginConfig.ClearEffectsOnLevelCompletion = value;
        }

        [UIValue("clear-on-back-button")]
        protected bool ClearOnBackButton
        {
            get => _pluginConfig.ClearPreviewEffects;
            set => _pluginConfig.ClearPreviewEffects = value;
        }

        private string _shadername = string.Empty;
        [UIValue("name")]
        protected string ShaderName
        {
            get => _shadername;
            set
            {
                _shadername = value;
                NotifyPropertyChanged(nameof(ShaderName));
            }
        }

        private string _authorName = string.Empty;
        [UIValue("author")]
        protected string AuthorName
        {
            get => _authorName;
            set
            {
                _authorName = value;
                NotifyPropertyChanged(nameof(AuthorName));
            }
        }

        private string _shaderReferenceName = string.Empty;
        [UIValue("shader-reference-name")]
        protected string ShaderReferenceName
        {
            get => _shaderReferenceName;
            set
            {
                _shaderReferenceName = value;
                NotifyPropertyChanged(nameof(ShaderReferenceName));
            }
        }

        private string _shaderPropertyCountText = string.Empty;
        [UIValue("shader-property-count")]
        protected string ShaderPropertyCountText
        {
            get => _shaderPropertyCountText;
            set
            {
                _shaderPropertyCountText = value;
                NotifyPropertyChanged(nameof(ShaderPropertyCountText));
            }
        }

        [UIAction("#post-parse")]
        protected void PostParse() => SetupDetails(null);

        private void SetupDetails(ShaderEffectData sfx)
        {
            if (sfx == null)
            {
                _shaderIcon.sprite = SEUtilities.GetDefaultShaderIcon();
                shaderDescription.SetText("Select a Shader!");
                return;
            }

            Material mat = sfx.Material;
            _currentShaderEffect = sfx;

            //_usesPreviousFrameData = false;
            _customPropertyCount = 0;

            if (mat != null)
            {
                int propCount = mat.shader.GetPropertyCount();
                _customPropertyCount = propCount;
                ShaderPropertyType spt;
                string propName = "";

                for (int i = 0; i < propCount; i++)
                {
                    spt = mat.shader.GetPropertyType(i);
                    propName = mat.shader.GetPropertyName(i);
                    if (propName.Equals("_MainTex"))
                    {
                        _customPropertyCount--;
                        continue;
                    }
                    if (propName.Equals("_PrevMainTex"))
                    {
                        _customPropertyCount--;
                        //_usesPreviousFrameData = true;
                        continue;
                    }

                }
            }

            ShaderName = sfx.Name;
            AuthorName = sfx.Author;
            ShaderReferenceName = "<color=#901212>" + sfx.ReferenceName;
            ShaderPropertyCountText = "Property count : " + _customPropertyCount;
            _shaderIcon.sprite = _shaderListViewController.GetPreviewImage(_currentShaderEffect);
            shaderDescription.SetText(sfx.Description);

        }

        internal void ShaderSelected(ShaderEffectData sfx) => SetupDetails(sfx);

        internal void ShaderSelectionCleared() => SetupDetails(null);
    }
}
