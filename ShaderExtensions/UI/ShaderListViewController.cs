using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using ShaderExtensions.Event;
using ShaderExtensions.Managers;
using ShaderExtensions.UI.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Zenject;

namespace ShaderExtensions.UI
{
    [ViewDefinition("ShaderExtensions.UI.Views.shaderList.bsml")]
    [HotReload(RelativePathToLayout = @"Views\shaderList.bsml")]
    class ShaderListViewController : BSMLAutomaticViewController
    {
        private ShaderAssetLoader _shaderAssetLoader;
        private ShaderManager _shaderManager;

        public event Action<ShaderEffect> shaderSelected;
        public event Action shadersCleared;

        private Dictionary<Texture2D, Sprite> _spriteCache = null;
        private int _selection = -1;

        [Inject]
        public void Construct(ShaderAssetLoader shaderAssetLoader, ShaderManager shaderManager) {
            _shaderAssetLoader = shaderAssetLoader;
            _shaderManager = shaderManager;
        }

        [UIComponent("shader-list")]
        public CustomListTableData customListTableData = null;

        [UIComponent("shader-stack-list")]
        public CustomCellListTableData shaderStackList = null;

        [UIComponent("scroll-indicator")]
        public BSMLScrollIndicator scrollIndicator = null;

        private Coroutine _scrollIndicatorCoroutine;

        internal IEnumerator ScrollIndicatorAnimator(float startValue, float endValue, float lerpDuration = 1f, Action onDone = null) {
            float timeElapsed = 0f;
            while (timeElapsed < lerpDuration) {
                scrollIndicator.progress = Mathf.Lerp(startValue, endValue, Easings.EaseOutCubic(timeElapsed / lerpDuration));
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            scrollIndicator.progress = endValue;
            onDone?.Invoke();
        }

        [UIAction("update-scroll-indicator-up")]
        public void ScrollUp() => Scroll(true);

        [UIAction("update-scroll-indicator-down")]
        public void ScrollDown() => Scroll(false);

        public void Scroll(bool up) {
            TableView tableView = customListTableData.tableView;

            Tuple<int, int> range = tableView.GetVisibleCellsIdRange();

            float rangeUpper;
            float pageSize = range.Item2 - range.Item1;
            float numOfCells = tableView.numberOfCells;

            if (up) {
                rangeUpper = Mathf.Max(0, range.Item2 - pageSize);
            } else {
                rangeUpper = Mathf.Min(numOfCells, range.Item2 + pageSize);
            }

            float progress = (rangeUpper - pageSize) / (numOfCells - pageSize);

            if (_scrollIndicatorCoroutine != null) {
                tableView.StopCoroutine(_scrollIndicatorCoroutine);
            }

            _scrollIndicatorCoroutine = tableView.StartCoroutine(ScrollIndicatorAnimator(scrollIndicator.progress, progress, 0.3f, () => {
                tableView.StopCoroutine(_scrollIndicatorCoroutine);
                _scrollIndicatorCoroutine = null;
            }));
        }

        private async void UpdateScrollIndicator() {

            await SiraUtil.Utilities.AwaitSleep(10);

            TableView tableView = customListTableData.tableView;

            Tuple<int, int> range = tableView.GetVisibleCellsIdRange();

            int pageSize = range.Item2 - range.Item1;
            float numOfCells = tableView.numberOfCells;

            scrollIndicator.normalizedPageHeight = (pageSize * 1f) / numOfCells;
            scrollIndicator.progress = (range.Item2 - pageSize) / (numOfCells - pageSize);
        }

        [UIAction("shader-select")]
        public void Select(TableView _, int row) {
            _selection = row;
            ShaderEffect sfx = _shaderAssetLoader.ShaderEffectList[_selection];
            Logger.log.Info("Selected: " + sfx.name + " by " + sfx.author);
            shaderSelected?.Invoke(sfx);
        }

        [UIAction("active-shader-select")]
        public void ActiveShaderStackSelect(TableView _, ActiveShaderElement ase) {
            Logger.log.Debug($"selcted ase: {ase}");
            _shaderManager.RemoveMaterial(ase.ID);
            SetupActiveShaderStackList();
        }

        [UIAction("reload-shaders")]
        public void ReloadShaders() {
            _shaderAssetLoader.Reload();
            SetupShaderList();
            _shaderManager.RefreshCameraManager();
            _shaderManager.ClearAllMaterials();
            SetupActiveShaderStackList();
            UpdateScrollIndicator();
        }

        [UIAction("add-shader")]
        public void AddShader() {
            if (_selection > -1) {
                _shaderManager.RefreshCameraManager();
                _shaderManager.AddMaterial("preview", _shaderAssetLoader.ShaderEffectList[_selection]);
                SetupActiveShaderStackList();
            }
        }

        [UIAction("select-shader")]
        public void SelectShader() {
            if (_selection > -1) {
                _shaderManager.RefreshCameraManager();
                _shaderManager.ClearAllMaterials();
                _shaderManager.AddMaterial("preview", _shaderAssetLoader.ShaderEffectList[_selection]);
                SetupActiveShaderStackList();
            }
        }

        [UIAction("clear-shader")]
        public void ClearShader() {
            _shaderManager.RefreshCameraManager();
            _shaderManager.ClearAllMaterials();
            SetupActiveShaderStackList();
        }

        [UIAction("#post-parse")]
        public void PostParse() {
            SetupShaderList();
            SetupActiveShaderStackList();
            UpdateScrollIndicator();
        }

        public void SetupShaderList() {
            customListTableData.data.Clear();
            if (_spriteCache != null) {
                _spriteCache.Clear();
            } else {
                _spriteCache = new Dictionary<Texture2D, Sprite>();
            }

            _selection = -1;
            shadersCleared?.Invoke();

            List<ShaderEffect> sfxList = _shaderAssetLoader.ShaderEffectList;

            foreach (ShaderEffect sfx in sfxList) {
                Sprite icon = Util.SEUtilities.GetDefaultShaderIcon();

                if (sfx.previewImage != null && _spriteCache.TryGetValue(sfx.previewImage, out Sprite image)) {
                    icon = image;
                } else if (sfx.previewImage != null) {
                    icon = Sprite.Create(sfx.previewImage, new Rect(0.0f, 0.0f, sfx.previewImage.width, sfx.previewImage.height), new Vector2(0.5f, 0.5f), 100.0f);
                    _spriteCache.Add(sfx.previewImage, icon);
                }

                CustomListTableData.CustomCellInfo customCellInfo = new CustomListTableData.CustomCellInfo(sfx.name, sfx.author, icon);
                customListTableData.data.Add(customCellInfo);
            }

            customListTableData.tableView.ReloadData();
        }

        public void SetupActiveShaderStackList() {
            shaderStackList.data.Clear();
            Dictionary<string, Material> matCache = _shaderManager.MaterialCache;

            foreach (string id in matCache.Keys) {

                matCache.TryGetValue(id, out Material mat);

                ShaderEffect sfx = _shaderManager.GetShaderEffectByMaterial(mat);

                if (sfx == null) continue;

                /*Sprite icon = Util.SEUtilities.GetDefaultShaderIcon();

                if (sfx.previewImage != null && _spriteCache.TryGetValue(sfx.previewImage, out Sprite image)) {
                    icon = image;
                }

                //CustomListTableData.CustomCellInfo customCellInfo = new CustomListTableData.CustomCellInfo(sfx.name, sfx.author, icon);*/
                shaderStackList.data.Add(new ActiveShaderElement(sfx.referenceName, id));
            }

            shaderStackList.tableView.ReloadData();
        }

        public Sprite GetPreviewImage(ShaderEffect sfx) {
            if (sfx == null || sfx.previewImage == null) return Util.SEUtilities.GetDefaultShaderIcon();
            if (_spriteCache.TryGetValue(sfx.previewImage, out Sprite image)) {
                return image;
            }
            return Util.SEUtilities.GetDefaultShaderIcon();
        }
    }
}
