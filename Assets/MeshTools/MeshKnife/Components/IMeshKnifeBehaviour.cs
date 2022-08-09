namespace MeshTools.MeshKnife.Components
{
    public interface IMeshKnifeBehaviour
    {
        bool BasePointsSet { get; }

        void CreateBasePoints();

        void Cut();
    }
}