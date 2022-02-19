using Heck.Animation;
using HMUI;
using IPA.Loader;
//using ShaderExtensions.Event;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ShaderExtensions.Util
{
    class SEUtilities
    {
        /// <summary>
        /// Animates a Scroll Indicator
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="endValue"></param>
        /// <param name="verticalScrollIndicator"></param>
        /// <param name="lerpDuration"></param>
        /// <param name="onDone">Method to execute after it's done animating.</param>
        /// <returns></returns>
        public static IEnumerator ScrollIndicatorAnimator(float startValue, float endValue, VerticalScrollIndicator verticalScrollIndicator, float lerpDuration = 0.3f, Action onDone = null)
        {
            float timeElapsed = 0f;
            while (timeElapsed < lerpDuration)
            {
                verticalScrollIndicator.progress = Mathf.Lerp(startValue, endValue, Easings.EaseOutCubic(timeElapsed / lerpDuration));
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            verticalScrollIndicator.progress = endValue;
            onDone?.Invoke();
        }

        /// <summary>
        /// Scroll a ScrollIndicator
        /// </summary>
        /// <param name="up"></param>
        /// <param name="tableView"></param>
        /// <param name="verticalScrollIndicator"></param>
        /// <param name="coroutine"></param>
        public static void ScrollTheScrollIndicator(bool up, TableView tableView, VerticalScrollIndicator verticalScrollIndicator, Coroutine coroutine)
        {
            Tuple<int, int> range = tableView.GetVisibleCellsIdRange();

            float rangeUpper;
            float pageSize = range.Item2 - range.Item1;
            float numOfCells = tableView.numberOfCells;

            if (up)
            {
                rangeUpper = Mathf.Max(0, range.Item2 - pageSize);
            }
            else
            {
                rangeUpper = Mathf.Min(numOfCells, range.Item2 + pageSize);
            }

            float progress = (rangeUpper - pageSize) / (numOfCells - pageSize);

            if (coroutine != null)
            {
                tableView.StopCoroutine(coroutine);
            }

            coroutine = tableView.StartCoroutine(ScrollIndicatorAnimator(verticalScrollIndicator.progress, progress, verticalScrollIndicator, 0.3f, () => {
                tableView.StopCoroutine(coroutine);
                coroutine = null;
            }));
        }

        /// <summary>
        /// Update the Scroll Indicators Graphics
        /// </summary>
        /// <param name="tableView"></param>
        /// <param name="verticalScrollIndicator"></param>
        /// <param name="doTheWaitThing">if it should wait for 10 ms (for bsml to initialize things)</param>
        public static async void UpdateScrollIndicator(TableView tableView, VerticalScrollIndicator verticalScrollIndicator, bool doTheWaitThing = false)
        {

            if (doTheWaitThing)
                await SiraUtil.Extras.Utilities.AwaitSleep(10);

            Tuple<int, int> range = tableView.GetVisibleCellsIdRange();

            float pageSize = range.Item2 - range.Item1;
            float numOfCells = tableView.numberOfCells;

            verticalScrollIndicator.normalizedPageHeight = pageSize / numOfCells;
            verticalScrollIndicator.progress = (range.Item2 - pageSize) / (numOfCells - pageSize);
        }


        private static Texture2D _shaderTex2D = LoadTextureRaw(LoadFromResource("ShaderExtensions.Resources.Icons.Shader.png"));
        private static Sprite _shaderSprite = null;

        /// <summary>
        /// Loads and caches the default icon for Shader files
        /// </summary>
        public static Sprite GetDefaultShaderIcon()
        {
            if (_shaderSprite != null)
            {
                return _shaderSprite;
            }

            _shaderSprite = Sprite.Create(_shaderTex2D, new Rect(0.0f, 0.0f, _shaderTex2D.width, _shaderTex2D.height), new Vector2(0.5f, 0.5f), 100.0f);

            return _shaderSprite;
        }

        private static Texture2D _texture2dTex2D = LoadTextureRaw(LoadFromResource("ShaderExtensions.Resources.Icons.Texture2D.png"));
        private static Sprite _texture2dSprite = null;

        /// <summary>
        /// Loads and caches the default icon for Shader files
        /// </summary>
        public static Sprite GetDefaultTexture2DIcon()
        {
            if (_texture2dSprite != null)
            {
                return _texture2dSprite;
            }

            _texture2dSprite = Sprite.Create(_texture2dTex2D, new Rect(0.0f, 0.0f, _texture2dTex2D.width, _texture2dTex2D.height), new Vector2(0.5f, 0.5f), 100.0f);

            return _texture2dSprite;
        }

        private static Texture2D _floatTex2D = LoadTextureRaw(LoadFromResource("ShaderExtensions.Resources.Icons.Float.png"));
        private static Sprite _floatSprite = null;

        /// <summary>
        /// Loads and caches the default icon for material properties of the type Float
        /// </summary>
        public static Sprite GetDefaultFloatIcon()
        {
            if (_floatSprite != null)
            {
                return _floatSprite;
            }

            _floatSprite = Sprite.Create(_floatTex2D, new Rect(0.0f, 0.0f, _floatTex2D.width, _floatTex2D.height), new Vector2(0.5f, 0.5f), 100.0f);

            return _floatSprite;
        }

        private static Texture2D _rangeTex2D = LoadTextureRaw(LoadFromResource("ShaderExtensions.Resources.Icons.Range.png"));
        private static Sprite _rangeSprite = null;

        /// <summary>
        /// Loads and caches the default icon for material properties of the type Range
        /// </summary>
        public static Sprite GetDefaultRangeIcon()
        {
            if (_rangeSprite != null)
            {
                return _rangeSprite;
            }

            _rangeSprite = Sprite.Create(_rangeTex2D, new Rect(0.0f, 0.0f, _rangeTex2D.width, _rangeTex2D.height), new Vector2(0.5f, 0.5f), 100.0f);

            return _rangeSprite;
        }

        private static Texture2D _vector2Tex2D = LoadTextureRaw(LoadFromResource("ShaderExtensions.Resources.Icons.Vector2.png"));
        private static Sprite _vector2Sprite = null;
        private static Texture2D _vector3Tex2D = LoadTextureRaw(LoadFromResource("ShaderExtensions.Resources.Icons.Vector3.png"));
        private static Sprite _vector3Sprite = null;
        private static Texture2D _vector4Tex2D = LoadTextureRaw(LoadFromResource("ShaderExtensions.Resources.Icons.Vector4.png"));
        private static Sprite _vector4Sprite = null;

        /// <summary>
        /// Loads and caches the default icon for material properties of the type Vector2 / vector3 / vector4
        /// </summary>
        /// <param name="type">2, 3 or 4 - 3 is default</param>
        public static Sprite GetDefaultVectorIcon(int type)
        {
            switch (type)
            {
                case 2:
                    if (_vector2Sprite != null)
                    {
                        return _vector2Sprite;
                    }
                    _vector2Sprite = Sprite.Create(_vector2Tex2D, new Rect(0.0f, 0.0f, _vector2Tex2D.width, _vector2Tex2D.height), new Vector2(0.5f, 0.5f), 100.0f);
                    return _vector2Sprite;
                case 3:
                default:
                    if (_vector3Sprite != null)
                    {
                        return _vector3Sprite;
                    }
                    _vector3Sprite = Sprite.Create(_vector3Tex2D, new Rect(0.0f, 0.0f, _vector3Tex2D.width, _vector3Tex2D.height), new Vector2(0.5f, 0.5f), 100.0f);
                    return _vector3Sprite;
                case 4:
                    if (_vector4Sprite != null)
                    {
                        return _vector4Sprite;
                    }
                    _vector4Sprite = Sprite.Create(_vector4Tex2D, new Rect(0.0f, 0.0f, _vector4Tex2D.width, _vector4Tex2D.height), new Vector2(0.5f, 0.5f), 100.0f);
                    return _vector4Sprite;
            }
        }

        /// <summary>
        /// Loads a Texture2D from byte[]
        /// </summary>
        /// <param name="file"></param>
        public static Texture2D LoadTextureRaw(byte[] file)
        {
            if (file.Length > 0)
            {
                Texture2D texture = new Texture2D(2, 2);
                if (texture.LoadImage(file))
                {
                    return texture;
                }
            }

            return null;
        }

        /// <summary>
        /// Loads an embedded resource from the calling assembly
        /// </summary>
        /// <param name="resourcePath">Path to resource</param>
        public static byte[] LoadFromResource(string resourcePath) => GetResource(Assembly.GetCallingAssembly(), resourcePath);

        /// <summary>
        /// Loads an embedded resource from an assembly
        /// </summary>
        /// <param name="assembly">Assembly to load from</param>
        /// <param name="resourcePath">Path to resource</param>
        public static byte[] GetResource(Assembly assembly, string resourcePath)
        {
            Stream stream = assembly.GetManifestResourceStream(resourcePath);
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, (int) stream.Length);
            return data;
        }

        public static IEnumerator DoAfter(float time, Action action)
        {
            float start = Time.fixedTime;
            while (start + time > Time.fixedTime)
                yield return null;
            action?.Invoke();
            yield break;
        }

        public static bool AnyCameraModInstalled() => PluginManager.GetPluginFromId("Camera2") != null ? true : (PluginManager.GetPluginFromId("CameraPlus") != null);
    }
}
