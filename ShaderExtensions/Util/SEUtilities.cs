using System.IO;
using System.Reflection;
using UnityEngine;

namespace ShaderExtensions.Util
{
    class SEUtilities
    {

        private static Texture2D shaderTex2D = LoadTextureRaw(LoadFromResource("ShaderExtensions.Resources.Icons.shader.png"));
        private static Sprite shaderSprite = null;

        /// <summary>
        /// Loads and caches the default icon for Shader files
        /// </summary>
        public static Sprite GetDefaultShaderIcon() {
            if (shaderSprite != null) {
                return shaderSprite;
            }

            shaderSprite = Sprite.Create(shaderTex2D, new Rect(0.0f, 0.0f, shaderTex2D.width, shaderTex2D.height), new Vector2(0.5f, 0.5f), 100.0f);

            return shaderSprite;
        }

        private static Texture2D floatTex2D = LoadTextureRaw(LoadFromResource("ShaderExtensions.Resources.Icons.float.png"));
        private static Sprite floatSprite = null;

        /// <summary>
        /// Loads and caches the default icon for material properties of the type Float
        /// </summary>
        public static Sprite GetDefaultFloatIcon() {
            if (floatSprite != null) {
                return floatSprite;
            }

            floatSprite = Sprite.Create(floatTex2D, new Rect(0.0f, 0.0f, floatTex2D.width, floatTex2D.height), new Vector2(0.5f, 0.5f), 100.0f);

            return floatSprite;
        }

        private static Texture2D vectorTex2D = LoadTextureRaw(LoadFromResource("ShaderExtensions.Resources.Icons.vector.png"));
        private static Sprite vectorSprite = null;

        /// <summary>
        /// Loads and caches the default icon for material properties of the type Vector
        /// </summary>
        public static Sprite GetDefaultVectorIcon() {
            if (vectorSprite != null) {
                return vectorSprite;
            }

            vectorSprite = Sprite.Create(vectorTex2D, new Rect(0.0f, 0.0f, vectorTex2D.width, vectorTex2D.height), new Vector2(0.5f, 0.5f), 100.0f);

            return vectorSprite;
        }

        /// <summary>
        /// Loads an Texture2D from byte[]
        /// </summary>
        /// <param name="file"></param>
        public static Texture2D LoadTextureRaw(byte[] file) {
            if (file.Length > 0) {
                Texture2D texture = new Texture2D(2, 2);
                if (texture.LoadImage(file)) {
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
        public static byte[] GetResource(Assembly assembly, string resourcePath) {
            Stream stream = assembly.GetManifestResourceStream(resourcePath);
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, (int) stream.Length);
            return data;
        }
    }
}
