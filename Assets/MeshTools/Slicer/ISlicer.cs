using MeshTools.Slicer.SlicingParameters;
using UnityEngine;

namespace MeshTools.Slicer
{
    public interface ISlicer
    {
        public void Cut(GameObject gameObject, Plane cutPlane, ISlicingParameters slicingParameters);
    }
}