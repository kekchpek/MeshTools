using MeshTools.Slicer.SlicingParameters;
using UnityEngine;

namespace MeshTools.Slicer.SlicingStrategies
{
    public interface ISlicingStrategy
    {
        void Cut(GameObject gameObject, Plane cutPlane);
    }
}