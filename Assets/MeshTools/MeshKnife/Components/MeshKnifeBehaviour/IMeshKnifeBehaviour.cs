namespace MeshTools.MeshKnife.Components.MeshKnifeBehaviour
{
    public interface IMeshKnifeBehaviour
    {
        bool BasePointsSet { get; }

        void CreateBasePoints();

        void Cut();
    }
}