using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ShaderEffect : MonoBehaviour
{
    public string referenceName = "";

    public string name = "";
    public string author = "";
    public string description = "";

    public bool isScreenSpace = true;
    public Texture2D previewImage;

    public Material material;
}


