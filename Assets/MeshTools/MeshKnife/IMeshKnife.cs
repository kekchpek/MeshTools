using UnityEngine;

namespace MeshTools.MeshKnife
{
    public interface IMeshKnife
    {
        
        /// <summary>
        /// Cut specified mesh.
        /// </summary>
        /// <param name="cutPlane">A plane, that is cutting the mesh</param>
        /// <param name="cutMeshFilter">The mesh filter, that contains mesh to cut.</param>
        /// <param name="cutMaterial">The material of cut area ("inside" the mesh).</param>
        /// <param name="cutForce">The force applied to the created meshes (in opposite directions)</param>
        void Cut(Plane cutPlane, MeshFilter cutMeshFilter, Material cutMaterial, float cutForce);
    }
}