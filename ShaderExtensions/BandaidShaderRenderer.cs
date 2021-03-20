using System.Collections.Generic;
using UnityEngine;

public class BandaidShaderRenderer : MonoBehaviour
{
    [SerializeField]
    private List<Material> _materialList;
    private RenderTexture _previousFrame;

    private void Awake()
    {
        if (_materialList == null)
        {
            _materialList = new List<Material>();
        }
    }

    public void ClearAllMaterials()
    {
        _materialList = new List<Material>();
    }

    public void AddMaterial(Material material)
    {
        _materialList.Add(material);
    }

    public bool Contains(Material mat) => _materialList.Contains(mat);

    public void RemoveMaterial(Material mat)
    {
        _materialList.Remove(mat);
    }

    public void OnDestroy()
    {
        if (_temporary)
        {
            _temporary.Release();
            _temporaryTwo.Release();
        }
        if (_previousFrame)
        {
            _previousFrame.Release();
        }
    }

    private RenderTexture _temporary;
    private RenderTexture _temporaryTwo;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_materialList.Count == 0)
        {
            RenderTexture.active = null;
            Graphics.Blit(source, destination);
            return;
        }

        if (!_previousFrame)
        {
            _previousFrame = new RenderTexture(source.width, source.height, source.depth, source.format, 0);
        }

        if (!_temporary)
        {
            _temporary = RenderTexture.GetTemporary(source.width, source.height, source.depth, source.format, RenderTextureReadWrite.Default, 1, source.memorylessMode, source.vrUsage);
            _temporaryTwo = RenderTexture.GetTemporary(source.width, source.height, source.depth, source.format, RenderTextureReadWrite.Default, 1, source.memorylessMode, source.vrUsage);
        }

        RenderTexture temptemp;
        for (int i = 0; i < _materialList.Count; i++)
        {
            Material mat = _materialList[i];
            mat.SetTexture("_PrevMainTex", _previousFrame);
            if (i == 0)
            {
                Graphics.Blit(source, _temporary, mat);
            }
            else
            {
                temptemp = _temporaryTwo;
                _temporaryTwo = _temporary;
                _temporary = temptemp;
                Graphics.Blit(_temporaryTwo, _temporary, mat);
            }
        }

        Graphics.Blit(_temporary, destination);
        Graphics.CopyTexture(_temporary, _previousFrame);
    }
}