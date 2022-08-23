using UnityEngine;

namespace MeshTools.MeshKnife
{
    public static class StaticMeshKnife
    {
        private static readonly IMeshKnife MeshKnife = new MeshKnife();

        public static void Cut(Plane cutPlane, MeshFilter cutMeshFilter, Material cutMaterial, float cutForce)
        {
            MeshKnife.Cut(cutPlane, cutMeshFilter, cutMaterial, cutForce);
        }
    }
}