using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using ShaderExtensions.Managers;
using ShaderExtensions.UI.Elements;
using ShaderExtensions.Util;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace ShaderExtensions.UI
{
    [ViewDefinition("ShaderExtensions.UI.Views.shaderList.bsml")]
    [HotReload(RelativePathToLayout = @"Views\shaderList.bsml")]
    class ShaderListViewController : BSMLAutomaticViewController
    {
        public event Action<ShaderEffectData> shaderSelected;
        public event Action shadersCleared;

        private ShaderAssetLoader _shaderAssetLoader;
        private ShaderManager _shaderManager;

        private Dictionary<Texture2D, Sprite> _spriteCache = null;
        private int _selection = -1;

        [Inject]
        public void Construct(ShaderAssetLoader shaderAssetLoader, ShaderManager shaderManager)
        {
            _shaderAssetLoader = shaderAssetLoader;
            _shaderManager = shaderManager;
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (shaderStackList != null)
            {
                SetupActiveShaderStackList();
            }
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
        }

        [UIComponent("shader-list")]
        protected CustomListTableData customListTableData = null;

        [UIComponent("shader-stack-list")]
        protected CustomCellListTableData shaderStackList = null;

        [UIComponent("scroll-indicator")]
        protected BSMLScrollIndicator scrollIndicator = null;

        private Coroutine _scrollIndicatorCoroutine = null!;

        [UIAction("update-scroll-indicator-up")]
        protected void ScrollUp() => SEUtilities.ScrollTheScrollIndicator(true, customListTableData.tableView, scrollIndicator, _scrollIndicatorCoroutine);

        [UIAction("update-scroll-indicator-down")]
        protected void ScrollDown() => SEUtilities.ScrollTheScrollIndicator(false, customListTableData.tableView, scrollIndicator, _scrollIndicatorCoroutine);

        [UIAction("shader-select")]
        protected void Select(TableView _, int row)
        {
            _selection = row;
            ShaderEffectData sfx = _shaderAssetLoader.ShaderEffectList[_selection];
            Logger.log.Info("Selected: " + sfx.Name + " by " + sfx.Author);
            shaderSelected?.Invoke(sfx);
        }

        [UIAction("active-shader-select")]
        protected void ActiveShaderStackSelect(TableView _, ActiveShaderElement ase)
        {
            Logger.log.Debug($"selcted ase: {ase}");
            _shaderManager.RemoveMaterial(ase.ID);
            SetupActiveShaderStackList();
        }

        [UIAction("reload-shaders")]
        protected void ReloadShaders()
        {
            _shaderAssetLoader.Reload();
            SetupShaderList();
            _shaderManager.RefreshCameraManager();
            _shaderManager.ClearAllMaterials();
            SetupActiveShaderStackList();
            SEUtilities.UpdateScrollIndicator(customListTableData.tableView, scrollIndicator);
        }

        [UIAction("add-shader")]
        protected void AddShader()
        {
            if (_selection > -1)
            {
                _shaderManager.RefreshCameraManager();
                _shaderManager.AddMaterial("preview", _shaderAssetLoader.ShaderEffectList[_selection]);
                SetupActiveShaderStackList();
            }
        }

        [UIAction("select-shader")]
        protected void SelectShader()
        {
            if (_selection > -1)
            {
                _shaderManager.RefreshCameraManager();
                _shaderManager.ClearAllMaterials();
                _shaderManager.AddMaterial("preview", _shaderAssetLoader.ShaderEffectList[_selection]);
                SetupActiveShaderStackList();
            }
        }

        [UIAction("clear-shader")]
        protected void ClearShader()
        {
            _shaderManager.RefreshCameraManager();
            _shaderManager.ClearAllMaterials();
            SetupActiveShaderStackList();
        }

        [UIAction("#post-parse")]
        protected void PostParse()
        {
            SetupShaderList();
            SetupActiveShaderStackList();
        }

        protected void SetupShaderList()
        {
            customListTableData.data.Clear();
            if (_spriteCache != null)
            {
                _spriteCache.Clear();
            }
            else
            {
                _spriteCache = new Dictionary<Texture2D, Sprite>();
            }

            _selection = -1;
            shadersCleared?.Invoke();

            List<ShaderEffectData> sfxList = _shaderAssetLoader.ShaderEffectList;

            foreach (ShaderEffectData sfx in sfxList)
            {
                Sprite icon = SEUtilities.GetDefaultShaderIcon();

                if (sfx.PreviewImage != null && _spriteCache.TryGetValue(sfx.PreviewImage, out Sprite image))
                {
                    icon = image;
                }
                else if (sfx.PreviewImage != null)
                {
                    icon = Sprite.Create(sfx.PreviewImage, new Rect(0.0f, 0.0f, sfx.PreviewImage.width, sfx.PreviewImage.height), new Vector2(0.5f, 0.5f), 100.0f);
                    _spriteCache.Add(sfx.PreviewImage, icon);
                }

                CustomListTableData.CustomCellInfo customCellInfo = new CustomListTableData.CustomCellInfo(sfx.Name, sfx.Author, icon);
                customListTableData.data.Add(customCellInfo);
            }

            customListTableData.tableView.ReloadData();
            SEUtilities.UpdateScrollIndicator(customListTableData.tableView, scrollIndicator, true);
        }

        protected void SetupActiveShaderStackList()
        {
            shaderStackList.data.Clear();
            shaderStackList.tableView.ClearSelection();

            Dictionary<string, Material> matCache = _shaderManager.MaterialCache;

            foreach (string id in matCache.Keys)
            {

                matCache.TryGetValue(id, out Material mat);

                ShaderEffectData sfx = _shaderManager.GetShaderEffectByMaterial(mat);

                if (sfx == null) continue;

                /*Sprite icon = Util.SEUtilities.GetDefaultShaderIcon();

                if (sfx.previewImage != null && _spriteCache.TryGetValue(sfx.previewImage, out Sprite image)) {
                    icon = image;
                }

                //CustomListTableData.CustomCellInfo customCellInfo = new CustomListTableData.CustomCellInfo(sfx.name, sfx.author, icon);*/
                shaderStackList.data.Add(new ActiveShaderElement(sfx.ReferenceName, id));
            }

            shaderStackList.tableView.ReloadData();
        }

        public Sprite GetPreviewImage(ShaderEffectData sfx)
        {
            if (sfx == null || sfx.PreviewImage == null) return SEUtilities.GetDefaultShaderIcon();
            if (_spriteCache.TryGetValue(sfx.PreviewImage, out Sprite image))
            {
                return image;
            }
            return Util.SEUtilities.GetDefaultShaderIcon();
        }
    }
}
