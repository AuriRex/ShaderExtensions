using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
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
    [ViewDefinition("ShaderExtensions.UI.Views.shaderPropertyList.bsml")]
    [HotReload(RelativePathToLayout = @"Views\shaderPropertyList.bsml")]
    internal class ShaderPropertyListViewController : BSMLResourceViewController
    {
        public override string ResourceName => "ShaderExtensions.UI.Views.shaderPropertyList.bsml";

        [UIComponent("shader-prop-list")]
        public CustomListTableData customListTableData = null!;

        [UIValue("keyboard-value")]
        public string keyboardValue = string.Empty;

        [UIValue("color-picker-value")]
        public Color colorPickerValue = Color.white;


        public delegate void FloatValueEnterCallback(float val);

        public delegate void CancelCallback();

        #region Range_Modal

        [UIComponent("range-modal-root")]
        protected ModalView rangeModalRoot = null!;

        [UIComponent("range-slider")]
        protected SliderSetting slider = null!;

        private float _rangeSliderValue = 0;
        [UIValue("range-slider-value")]
        protected float RangeSliderValue {
            get => _rangeSliderValue;
            set {
                _rangeSliderValue = value;
                NotifyPropertyChanged(nameof(RangeSliderValue));
            }
        }

        private float _rangeSliderMin = 0;
        [UIValue("range-slider-min")]
        protected float RangeSliderMin {
            get => _rangeSliderMin;
            set {
                _rangeSliderMin = value;
                NotifyPropertyChanged(nameof(RangeSliderMin));
            }
        }

        private float _rangeSliderMax = 1;
        [UIValue("range-slider-max")]
        protected float RangeSliderMax {
            get => _rangeSliderMax;
            set {
                _rangeSliderMax = value;
                NotifyPropertyChanged(nameof(RangeSliderMax));
            }
        }

        private string _rangePropertyName = string.Empty;
        [UIValue("range-property-name")]
        public string RangePropertyName {
            get => _rangePropertyName;
            set {
                _rangePropertyName = value;
                NotifyPropertyChanged(nameof(RangePropertyName));
            }
        }

        [UIAction("range-use-num")]
        public void RangeUseNumpad() {
            CloseRangeModal();
            OpenNumpad(RangePropertyName, RangeSliderValue, OnFloatValueEnter, _range_currentCancelCallback);
        }

        [UIAction("range-event-enter")]
        public void RangeEventEnter() {
            _range_currentCallback(RangeSliderValue);
            CloseRangeModal();
        }

        private bool _range_cancelOnClickoff = true;
        private FloatValueEnterCallback _range_currentCallback;
        private CancelCallback _range_currentCancelCallback;

        public void OpenRangeModal(string description, float initialValue, float minValue, float maxValue, FloatValueEnterCallback callback, CancelCallback cancelCallback) => OpenRangeModal(description, initialValue, minValue, maxValue, callback, cancelCallback, true);
        public void OpenRangeModal(string description, float initialValue, float minValue, float maxValue, FloatValueEnterCallback callback, CancelCallback cancelCallback, bool cancelOnClickoff) {
            rangeModalRoot.blockerClickedEvent += RangeModalRoot_blockerClickedEvent;
            _range_cancelOnClickoff = cancelOnClickoff;
            _range_currentCallback = callback;
            _range_currentCancelCallback = cancelCallback;
            RangePropertyName = description;

            RangeSliderMin = minValue;
            RangeSliderMax = maxValue;
            RangeSliderValue = initialValue;

            parserParams.EmitEvent("show-range-modal");
        }

        private void RangeModalRoot_blockerClickedEvent() {
            if (!_range_cancelOnClickoff) {
                RangeEventEnter();
            } else {
                _range_currentCancelCallback();
            }
        }

        public void CloseRangeModal() {
            parserParams.EmitEvent("hide-range-modal");
        }

        #endregion

        #region Numpad_Modal

        [UIComponent("numpad-modal-root")]
        protected ModalView numpadModalRoot = null!;

        // Events:
        // show-numpad-modal
        // hide-numpad-modal

        private string _numpadPropertyName = string.Empty;
        [UIValue("numpad-property-name")]
        protected string NumpadPropertyName {
            get => _numpadPropertyName;
            set {
                _numpadPropertyName = value;
                NotifyPropertyChanged(nameof(NumpadPropertyName));
            }
        }

        private string _numpadValue = string.Empty;
        [UIValue("numpad-value")]
        protected string NumpadValue {
            get => _numpadValue;
            set {
                _numpadValue = value;
                NotifyPropertyChanged(nameof(NumpadValue));
            }
        }

        private string _numpadSign = string.Empty;
        [UIValue("numpad-sign")]
        protected string NumpadSign {
            get => _numpadSign;
            set {
                _numpadSign = value;
                NotifyPropertyChanged(nameof(NumpadSign));
            }
        }

        private string _numpadSignButton = "-";
        [UIValue("numpad-sign-button")]
        protected string NumpadSignButton {
            get => _numpadSignButton;
            set {
                _numpadSignButton = value;
                NotifyPropertyChanged(nameof(NumpadSignButton));
            }
        }

        private bool _isNumpadValuePositive;
        public bool NumpadValuePositive {
            get => _isNumpadValuePositive;
            set {
                _isNumpadValuePositive = value;
                NumpadSign = value ? string.Empty : "-";
                NumpadSignButton = value ? "-" : "+";
            }
        }

        [UIAction("num-event-one")]
        public void NumEventOne() => NumpadAddCharacter('1');
        [UIAction("num-event-two")]
        public void NumEventTwo() => NumpadAddCharacter('2');
        [UIAction("num-event-three")]
        public void NumEventThree() => NumpadAddCharacter('3');
        [UIAction("num-event-four")]
        public void NumEventFour() => NumpadAddCharacter('4');
        [UIAction("num-event-five")]
        public void NumEventFive() => NumpadAddCharacter('5');
        [UIAction("num-event-six")]
        public void NumEventSix() => NumpadAddCharacter('6');
        [UIAction("num-event-seven")]
        public void NumEventSeven() => NumpadAddCharacter('7');
        [UIAction("num-event-eight")]
        public void NumEventEight() => NumpadAddCharacter('8');
        [UIAction("num-event-nine")]
        public void NumEventNine() => NumpadAddCharacter('9');
        [UIAction("num-event-zero")]
        public void NumEventZero() => NumpadAddCharacter('0');
        [UIAction("num-event-period")]
        public void NumEventPeriod() => NumpadAddCharacter('.');

        private void NumpadAddCharacter(char c) {
            if (c == '.') {
                if (!NumpadValue.Contains('.')) {
                    NumpadValue += c;
                }
            } else if (c == '0') {
                if(NumpadValue.Length > 0) {
                    NumpadValue += c;
                }
            } else {
                NumpadValue += c;
            }
        }

        [UIAction("num-event-minus")]
        public void NumEventSign() {
            NumpadValuePositive = !NumpadValuePositive;
        }

        [UIAction("num-event-back")]
        public void NumEventBack() {
            string val = NumpadValue;
            if(val.Length > 0) {
                NumpadValue = val.Substring(0, val.Length - 1);
            }
        }

        [UIAction("num-event-enter")]
        public void NumEventEnter() {
            string val = NumpadValue;
            if(float.TryParse(NumpadValuePositive ? val : '-'+val, out float result)) {
                _numpad_currentCallback(result);
            } else {
                _numpad_currentCallback(0.5f);
            }
            CloseNumpad();
        }

        [UIAction("num-event-clear")]
        public void NumEventClear() {
            NumpadValue = string.Empty;
        }

        private void NumpadModalRoot_blockerClickedEvent() {
            if(!_numpad_cancelOnClickoff) {
                NumEventEnter();
            } else {
                _numpad_currentCancelCallback();
            }
        }

        private bool _numpad_cancelOnClickoff = true;
        private FloatValueEnterCallback _numpad_currentCallback;
        private CancelCallback _numpad_currentCancelCallback;

        public void OpenNumpad(string description, float initialValue, FloatValueEnterCallback callback, CancelCallback cancelCallback) => OpenNumpad(description, initialValue, callback, cancelCallback, true);
        public void OpenNumpad(string description, float initialValue, FloatValueEnterCallback callback, CancelCallback cancelCallback, bool cancelOnClickoff) {
            numpadModalRoot.blockerClickedEvent += NumpadModalRoot_blockerClickedEvent;
            _numpad_cancelOnClickoff = cancelOnClickoff;
            string val = initialValue.ToString();
            if(val.StartsWith("-")) {
                NumpadValuePositive = false;
                val = val.Replace("-", string.Empty);
            } else {
                NumpadValuePositive = true;
            }
            NumpadValue = val;
            NumpadPropertyName = description;
            _numpad_currentCallback = callback;
            _numpad_currentCancelCallback = cancelCallback;
            parserParams.EmitEvent("show-numpad-modal");
        }

        public void CloseNumpad() {
            parserParams.EmitEvent("hide-numpad-modal");
            numpadModalRoot.blockerClickedEvent -= NumpadModalRoot_blockerClickedEvent;
        }

        #endregion

        public int selection { get; private set; } = -1;

        private Dictionary<string, int> _properties = new Dictionary<string, int>();

        private Material _currentMat;


        [UIParams]
        BSMLParserParams parserParams = null!;

        [UIAction("#post-parse")]
        public void PostParse() => SetupList(null);

        internal void ShaderSelected(ShaderEffect sfx) => SetupList(sfx.material);

        internal void ShaderSelectionCleared() => SetupList(null);

        private void SetupList(Material mat) {
            customListTableData.data.Clear();
            customListTableData.tableView.ClearSelection();
            selection = -1;
            _properties = new Dictionary<string, int>();
            _currentMat = mat;

            CloseNumpad();
            parserParams.EmitEvent("hide-color-picker");
            parserParams.EmitEvent("hide-keyboard");
            CloseRangeModal();

            if (mat != null) {
                int propCount = mat.shader.GetPropertyCount();

                ShaderPropertyType spt;
                string propName;

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

        [UIAction("shader-prop-select")]
        public void Select(TableView _, int row) {
            //Logger.log.Info("Selected Property: " + row);
            selection = row;

            if (_currentMat != null) {
                string propName = _properties.Keys.ToArray()[selection];
                int propID = _properties[propName];
                Logger.log.Debug("Selected Property: " + propName);
                if (_currentMat.shader.GetPropertyType(propID) == ShaderPropertyType.Float) {
                    float currentValue = _currentMat.GetFloat(propName);
                    OpenNumpad(propName, currentValue, OnFloatValueEnter, OnCanceled);
                } else if (_currentMat.shader.GetPropertyType(propID) == ShaderPropertyType.Range) {
                    float currentValue = _currentMat.GetFloat(propName);

                    Vector2 rangeLimits = _currentMat.shader.GetPropertyRangeLimits(propID);

                    OpenRangeModal(propName, currentValue, rangeLimits.x, rangeLimits.y, OnFloatValueEnter, OnCanceled);
                } else if (_currentMat.shader.GetPropertyType(propID) == ShaderPropertyType.Color) {
                    colorPickerValue = _currentMat.GetColor(propName);
                    parserParams.EmitEvent("show-color-picker");
                } else {
                    keyboardValue = "";
                }

            }

        }

        public void OnFloatValueEnter(float val) {
            if (selection <= -1) return;

            if (_currentMat != null) {
                string propName = _properties.Keys.ToArray()[selection];
                int propID = _properties[propName];
                if (_currentMat.shader.GetPropertyType(propID) == ShaderPropertyType.Float || _currentMat.shader.GetPropertyType(propID) == ShaderPropertyType.Range) {
                    _currentMat.SetFloat(propName, val);
                }
            }

            SetupList(_currentMat);
        }
        
        public void OnCanceled() {
            SetupList(_currentMat);
        }

        [UIAction("keyboard-enter")]
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

        [UIAction("color-picker-done")]
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

        [UIAction("color-picker-cancel")]
        public void OnColorPickerCancel() => SetupList(_currentMat);

        [UIAction("range-slider-update")]
        public void OnRangeSliderUpdate(float value) {

        }

    }
}
