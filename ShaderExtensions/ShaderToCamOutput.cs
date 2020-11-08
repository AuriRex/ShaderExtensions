using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ShaderToCamOutput : MonoBehaviour
{
    [SerializeField]
    List<Material> camShaders;
    List<RenderTexture> temps;
    RenderTexture prev;
    private void Awake() {
        if (temps == null) {
            temps = new List<RenderTexture>();
            temps.Add(null);
        }
        if (prev == null) {
            //prev = new RenderTexture(1, 1, 1);
        }
        if (camShaders == null) {
            camShaders = new List<Material>();
        } else {
            foreach (Material mat in camShaders) {
                temps[temps.Count - 1] = new RenderTexture(1, 1, 1);
                temps.Add(null);
            }
        }

    }

    public void ClearAllMaterials() {
        temps = new List<RenderTexture>();

        temps.Add(null);
        /*
        foreach (Material mat in camShaders) {
            temps.Add(new RenderTexture(1, 1, 1));
        }*/
        camShaders = new List<Material>();
    }

    public void AddMaterial(Material material) {
        /*if (temps == null) {
            temps = new List<RenderTexture>();
            temps.Add(null);
        }
        if (camShaders == null) {
            camShaders = new List<Material>();
        }*/
        camShaders.Add(material);
        temps[temps.Count - 1] = new RenderTexture(1, 1, 1);
        temps.Add(null);
    }

    public bool Contains(Material mat) {
        foreach (Material material in camShaders) {
            if (material == mat) return true;
        }
        return false;
    }

    public void RemoveMaterial(Material mat) {
        camShaders.Remove(mat);
        temps[temps.Count - 2] = null;
        temps.Remove(temps[temps.Count - 1]);
    }

    /*Vector2 prevScale = new Vector2(1, -1);
    Vector2 prevOffset = new Vector2(0, 1);*/

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (prev == null || prev.width != source.width || prev.height != source.height) {
            prev = new RenderTexture(source.width, source.height, source.depth);
        }
        if (temps.Count > 1) {
            temps[0] = source;
            temps[temps.Count - 1] = destination;
            int i = 0;
            int propIndex;
            foreach (Material material in camShaders) {

                propIndex = material.shader.FindPropertyIndex("_PrevMainTex");

                if (propIndex != -1 && material.shader.GetPropertyType(propIndex) == ShaderPropertyType.Texture) {
                    material.SetTexture("_PrevMainTex", prev);
                }

                if (i + 1 != temps.Count - 1) {
                    if (temps[i + 1].height != source.height || temps[i + 1].width != source.width || temps[i + 1].depth != source.depth)
                        temps[i + 1] = new RenderTexture(source);
                }
                Graphics.Blit(temps[i], temps[i + 1], material);
                i++;
            }
        } else {
            Graphics.Blit(source, destination);
        }
        Graphics.Blit(destination, prev/*, prevScale, prevOffset*/); //Blit(Texture source, RenderTexture dest, Vector2 scale, Vector2 offset)
    }
}
