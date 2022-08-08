using UnityEngine;

namespace MeshTools.MeshKnife.CutPlaneBuilder
{
    public interface ICutPlaneBuilder
    {
        ICutPlaneBuilder AddEdge(Vector3 point1, Vector3 point2);

        ICutPlaneBuilder SetMaterial(Material material);

        GameObject Build();
    }
}