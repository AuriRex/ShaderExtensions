using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShaderExtensions.Managers
{
    public interface ICameraManager
    {
        Action onCameraRefreshDone { get; set; }
        Camera[] GetCameras();
        void AddMaterial(Material mat);
        void RemoveMaterial(Material mat);
        void ApplyMaterials(List<Material> matList);
        void ClearAllMaterials();
        void Refresh();
    }
}
