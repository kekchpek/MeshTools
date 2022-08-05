using UnityEngine;

namespace MeshTools.MeshKnife
{
    public interface IMeshKnife
    {
        bool Initialized { get; }
        
        void Initialize();

        void SetCuttingMesh(MeshFilter meshFilter);
        
        void Cut();
    }
}