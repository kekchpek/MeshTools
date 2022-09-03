namespace MeshTools.Slicer.SlicingStrategies.Builder
{
    public interface ISlicingStrategyBuilder
    {
        ISlicingStrategyBuilder SetSlicer(ISlicer slicer);
        ISlicingStrategyBuilder SliceColliders();
        ISlicingStrategyBuilder SliceRigidbodies();
        ISlicingStrategy Build();
    }
}