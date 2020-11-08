﻿using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace ShaderExtensions.UI
{
    internal class ShaderPropertyListViewController : BSMLResourceViewController
    {
        public override string ResourceName => "ShaderExtensions.UI.Views.shaderPropertyList.bsml";

        [UIComponent("shaderPropList")]
        public CustomListTableData customListTableData;

        [UIValue("keyboardValue")]
        public string keyboardValue = "";

        [UIValue("colorPickerValue")]
        public Color colorPickerValue = Color.white;

        int selection = -1;

        [UIParams]
        BSMLParserParams parserParams;

        [UIAction("shaderPropSelect")]
        public void Select(TableView tv, int row) {

            //Logger.log.Info("Selected Property: " + row);
            selection = row;
            if (currentMat != null) {
                string propName = properties.Keys.ToArray()[selection];
                int propID = properties[propName];
                Logger.log.Info("Selected Property: " + propName);
                if (currentMat.shader.GetPropertyType(propID) == ShaderPropertyType.Float || currentMat.shader.GetPropertyType(propID) == ShaderPropertyType.Range) {
                    keyboardValue = "" + currentMat.GetFloat(propName);
                    parserParams.EmitEvent("openKeyboard");
                } else if (currentMat.shader.GetPropertyType(propID) == ShaderPropertyType.Color) {
                    colorPickerValue = currentMat.GetColor(propName);
                    parserParams.EmitEvent("openColorPicker");
                } else {
                    keyboardValue = "";
                }

            }


        }

        [UIAction("#post-parse")]
        public void PostParse() => SetupList(null);

        private Dictionary<string, int> properties = new Dictionary<string, int>();
        private Material currentMat;

        private void SetupList(Material mat) {
            customListTableData.data.Clear();
            selection = -1;
            properties = new Dictionary<string, int>();
            currentMat = mat;

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
                        CustomListTableData.CustomCellInfo customCellInfo = new CustomListTableData.CustomCellInfo(propName + " : " + mat.GetFloat(propName), spt.ToString());
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
                        CustomListTableData.CustomCellInfo customCellInfo = new CustomListTableData.CustomCellInfo(propName, spt.ToString());
                        customListTableData.data.Add(customCellInfo);
                    }


                    properties.Add(propName, i);

                }
            }

            customListTableData.tableView.ReloadData();

        }

        internal void ShaderSelected(Material material) => SetupList(material);

        internal void ShaderSelectionCleared() => SetupList(null);

        [UIAction("keyboardEnter")]
        public void OnKeyboardEnterPressed(string text) {
            if (selection <= -1) return;

            if (currentMat != null) {
                string propName = properties.Keys.ToArray()[selection];
                int propID = properties[propName];
                if (currentMat.shader.GetPropertyType(propID) == ShaderPropertyType.Float || currentMat.shader.GetPropertyType(propID) == ShaderPropertyType.Range) {
                    float value = 1;
                    try {
                        value = float.Parse(text);
                    } catch (Exception) {
                        value = 1;
                    }
                    currentMat.SetFloat(propName, value);
                } else {
                    // Todo?
                }

            }

            SetupList(currentMat);

        }

        [UIAction("colorPickerDone")]
        public void OnColorPickerDone(Color col) {
            if (selection <= -1) return;

            if (currentMat != null) {

                string propName = properties.Keys.ToArray()[selection];
                int propID = properties[propName];
                if (currentMat.shader.GetPropertyType(propID) == ShaderPropertyType.Color) {
                    currentMat.SetColor(propName, col);
                }

            }

            SetupList(currentMat);
        }

        [UIAction("colorPickerCancel")]
        public void OnColorPickerCancel() => SetupList(currentMat);

    }
}
