using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using BS_Utils.Utilities;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace ShaderExtensions.UI
{
    internal class ShaderPropertyListViewController : BSMLResourceViewController
    {
        public override string ResourceName => "ShaderExtensions.UI.Views.shaderPropertyList.bsml";

        [UIComponent("shader-prop-list")]
        public CustomListTableData customListTableData;

        [UIValue("keyboardValue")]
        public string keyboardValue = "";

        [UIValue("colorPickerValue")]
        public Color colorPickerValue = Color.white;

        public int selection { get; private set; } = -1;

        private Dictionary<string, int> _properties = new Dictionary<string, int>();
        
        private Material _currentMat;


        [UIParams]
        BSMLParserParams parserParams;

        [UIAction("#post-parse")]
        public void PostParse() => SetupList(null);

        internal void ShaderSelected(Material material) => SetupList(material);

        internal void ShaderSelectionCleared() => SetupList(null);

        private void SetupList(Material mat) {
            customListTableData.data.Clear();
            customListTableData.tableView.ClearSelection();
            selection = -1;
            _properties = new Dictionary<string, int>();
            _currentMat = mat;

            if (mat != null) {
                int propCount = mat.shader.GetPropertyCount();

                ShaderPropertyType spt;
                string propName = "";

                for (int i = 0; i < propCount; i++) {
                    spt = mat.shader.GetPropertyType(i);
                    propName = mat.shader.GetPropertyName(i);
                    if (propName.Equals("_MainTex") || propName.Equals("_PrevMainTex")) continue;

                    //Texture2D tex = new Texture2D(16,16);

                    /*if(spt == ShaderPropertyType.Texture || spt == ShaderPropertyType.Color) {

                    }*/

                    if (spt == ShaderPropertyType.Float || spt == ShaderPropertyType.Range) {
                        CustomListTableData.CustomCellInfo customCellInfo = new CustomListTableData.CustomCellInfo(propName + " : " + mat.GetFloat(propName), spt.ToString(), Util.SEUtilities.GetDefaultFloatIcon());
                        customListTableData.data.Add(customCellInfo);
                    } else if (spt == ShaderPropertyType.Color) {
                        Texture2D tex = new Texture2D(1, 1);
                        tex.SetPixel(0, 0, mat.GetColor(propName));
                        tex.Apply();
                        Sprite icon = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                        CustomListTableData.CustomCellInfo customCellInfo = new CustomListTableData.CustomCellInfo(propName, spt.ToString(), icon);
                        customListTableData.data.Add(customCellInfo);
                    } else if (spt == ShaderPropertyType.Texture) {
                        Texture2D tex = mat.GetTexture(propName) as Texture2D;
                        Sprite icon = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                        CustomListTableData.CustomCellInfo customCellInfo = new CustomListTableData.CustomCellInfo(propName, spt.ToString(), icon);
                        customListTableData.data.Add(customCellInfo);
                    } else {
                        // Vector
                        CustomListTableData.CustomCellInfo customCellInfo = new CustomListTableData.CustomCellInfo(propName, spt.ToString(), Util.SEUtilities.GetDefaultVectorIcon());
                        customListTableData.data.Add(customCellInfo);
                    }


                    _properties.Add(propName, i);

                }
            }

            customListTableData.tableView.ReloadData();

        }

        [UIAction("shaderPropSelect")]
        public void Select(TableView tv, int row) {
            //Logger.log.Info("Selected Property: " + row);
            selection = row;

            if (_currentMat != null) {
                string propName = _properties.Keys.ToArray()[selection];
                int propID = _properties[propName];
                Logger.log.Info("Selected Property: " + propName);
                if (_currentMat.shader.GetPropertyType(propID) == ShaderPropertyType.Float || _currentMat.shader.GetPropertyType(propID) == ShaderPropertyType.Range) {
                    keyboardValue = "" + _currentMat.GetFloat(propName);
                    parserParams.EmitEvent("openKeyboard");
                } else if (_currentMat.shader.GetPropertyType(propID) == ShaderPropertyType.Color) {
                    colorPickerValue = _currentMat.GetColor(propName);
                    parserParams.EmitEvent("openColorPicker");
                } else {
                    keyboardValue = "";
                }

            }

        }

        [UIAction("keyboardEnter")]
        public void OnKeyboardEnterPressed(string text) {
            if (selection <= -1) return;

            if (_currentMat != null) {
                string propName = _properties.Keys.ToArray()[selection];
                int propID = _properties[propName];
                if (_currentMat.shader.GetPropertyType(propID) == ShaderPropertyType.Float || _currentMat.shader.GetPropertyType(propID) == ShaderPropertyType.Range) {
                    float value = 1;
                    try {
                        value = float.Parse(text);
                    } catch (Exception) {
                        value = 1;
                    }
                    _currentMat.SetFloat(propName, value);
                } else {
                    // Todo?
                }

            }

            SetupList(_currentMat);

        }

        [UIAction("colorPickerDone")]
        public void OnColorPickerDone(Color col) {
            if (selection <= -1) return;

            if (_currentMat != null) {

                string propName = _properties.Keys.ToArray()[selection];
                int propID = _properties[propName];
                if (_currentMat.shader.GetPropertyType(propID) == ShaderPropertyType.Color) {
                    _currentMat.SetColor(propName, col);
                }

            }

            SetupList(_currentMat);
        }

        [UIAction("colorPickerCancel")]
        public void OnColorPickerCancel() => SetupList(_currentMat);

    }
}
