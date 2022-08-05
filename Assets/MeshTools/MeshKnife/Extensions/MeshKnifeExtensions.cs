using UnityEngine;

namespace MeshTools.MeshKnife.Extensions
{
    public static class MeshKnifeExtensions
    {
        public static void Cut(this IMeshKnife meshKnife, MeshFilter meshFilter)
        {
            meshKnife.SetCuttingMesh(meshFilter);
            meshKnife.Cut();
        }
    }
}