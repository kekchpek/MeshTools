using UnityEngine;

namespace MeshTools.MeshKnife
{
    public static class StaticMeshKnife
    {
        private static readonly IMeshKnife _meshKnife = new MeshKnife();

        public static void Cut(Plane cutPlane, MeshFilter cutMeshFilter, Material cutMaterial, float cutForce)
        {
            _meshKnife.Cut(cutPlane, cutMeshFilter, cutMaterial, cutForce);
        }
    }
}