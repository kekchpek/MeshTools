using System;
using MeshTools.Slicer.SlicingParameters;
using MeshTools.Slicer.SlicingStrategies.SlicingTools;

namespace MeshTools.Slicer.SlicingStrategies.Builder
{
    public class SlicingStrategyBuilder : ISlicingStrategyBuilder
    {
        private ISlicer _slicer;
        private readonly ISlicingParameters _parameters = ISlicingParameters.CreateNew();
        
        public ISlicingStrategyBuilder SetSlicer(ISlicer slicer)
        {
            _slicer = slicer;
            return this;
        }

        public ISlicingStrategyBuilder SliceColliders()
        {
            ColliderSlicingTool.UpdateParameters(_parameters);
            return this;
        }

        public ISlicingStrategyBuilder SliceRigidbodies()
        {
            RigidbodySlicingTools.UpdateParameters(_parameters);
            return this;
        }

        public ISlicingStrategy Build()
        {
            if (_slicer is null)
            {
                throw new InvalidOperationException("Slicer should be set!");
            }
            return new SlicingStrategy(_slicer, _parameters);
        }
    }
}