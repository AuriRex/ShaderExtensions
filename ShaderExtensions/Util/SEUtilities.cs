using BS_Utils.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShaderExtensions.Util
{
    class SEUtilities
    {

        private static Texture2D shaderTex2D = UIUtilities.LoadTextureFromResources("ShaderExtensions.Resources.Icons.shader.png");
        private static Sprite shaderSprite = null;
        public static Sprite GetDefaultShaderIcon() {
            if(shaderSprite != null) {
                return shaderSprite;
            }

            shaderSprite = Sprite.Create(shaderTex2D, new Rect(0.0f, 0.0f, shaderTex2D.width, shaderTex2D.height), new Vector2(0.5f, 0.5f), 100.0f);

            return shaderSprite;
        }

        private static Texture2D floatTex2D = UIUtilities.LoadTextureFromResources("ShaderExtensions.Resources.Icons.float.png");
        private static Sprite floatSprite = null;
        public static Sprite GetDefaultFloatIcon() {
            if (floatSprite != null) {
                return floatSprite;
            }

            floatSprite = Sprite.Create(floatTex2D, new Rect(0.0f, 0.0f, floatTex2D.width, floatTex2D.height), new Vector2(0.5f, 0.5f), 100.0f);

            return floatSprite;
        }

        private static Texture2D vectorTex2D = UIUtilities.LoadTextureFromResources("ShaderExtensions.Resources.Icons.vector.png");
        private static Sprite vectorSprite = null;
        public static Sprite GetDefaultVectorIcon() {
            if (vectorSprite != null) {
                return vectorSprite;
            }

            vectorSprite = Sprite.Create(vectorTex2D, new Rect(0.0f, 0.0f, vectorTex2D.width, vectorTex2D.height), new Vector2(0.5f, 0.5f), 100.0f);

            return vectorSprite;
        }

    }
}
