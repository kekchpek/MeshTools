using MeshTools.Slicer.SlicingParameters;
using UnityEngine;

namespace MeshTools.Slicer.SlicingStrategies
{
    public class SlicingStrategy : ISlicingStrategy
    {
        private readonly ISlicer _slicer;
        private readonly ISlicingParameters _parameters;

        public SlicingStrategy(ISlicer slicer, ISlicingParameters parameters)
        {
            _slicer = slicer;
            _parameters = parameters;
        }

        public void Cut(GameObject gameObject, Plane cutPlane)
        {
            _slicer.Cut(gameObject, cutPlane, _parameters);
        }
    }
}