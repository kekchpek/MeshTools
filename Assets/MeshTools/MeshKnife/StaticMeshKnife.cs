using UnityEngine;

namespace MeshTools.MeshKnife
{
    public static class StaticMeshKnife
    {
        private static readonly IMeshKnife MeshKnife = new MeshKnife();

        /// <summary>
        /// Cut specified mesh.
        /// </summary>
        /// <param name="cutPlane">A plane, that is cutting the mesh. In global coordinates.</param>
        /// <param name="cutMesh">Mesh to cut.</param>
        /// <param name="meshScale">Indicates mesh scale for global world coordinates.</param>
        /// <param name="meshRotation">Indicates mesh rotation for global world coordinates.</param>
        /// <param name="meshOrigin">Indicates mesh origin for global world coordinates.</param>
        /// <param name="meshesFromPositiveSide">All meshes that are place on positive side of a cut plane.</param>
        /// <param name="meshesFromNegativeSide">All meshes that are place on negative side of a cut plane.</param>
        public static void Cut(Plane cutPlane, Mesh cutMesh, Vector3 meshScale, Quaternion meshRotation,
            Vector3 meshOrigin, out Mesh[] meshesFromPositiveSide, out Mesh[] meshesFromNegativeSide)
        {
            MeshKnife.Cut(cutPlane, cutMesh, meshScale, meshRotation, meshOrigin, out meshesFromPositiveSide, out meshesFromNegativeSide);
        }
    }
}