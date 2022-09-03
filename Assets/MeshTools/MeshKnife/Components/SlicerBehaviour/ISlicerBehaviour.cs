namespace MeshTools.MeshKnife.Components.MeshKnifeBehaviour
{
    public interface ISlicerBehaviour
    {
        bool BasePointsSet { get; }

        void CreateBasePoints();

        void Cut();
    }
}