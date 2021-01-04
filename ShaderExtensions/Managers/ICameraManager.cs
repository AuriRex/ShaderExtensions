using System.Collections.Generic;
using UnityEngine;

namespace ShaderExtensions.Managers
{
    public interface ICameraManager
    {
        void AddMaterial(Material mat);
        void RemoveMaterial(Material mat);
        void ApplyMaterials(List<Material> matList);
        void ClearAllMaterials();
        void Refresh();
    }
}
