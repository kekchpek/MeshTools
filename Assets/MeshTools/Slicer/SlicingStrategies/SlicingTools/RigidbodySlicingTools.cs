using MeshTools.Slicer.SlicingParameters;
using UnityEngine;

namespace MeshTools.Slicer.SlicingStrategies.SlicingTools
{
    public static class RigidbodySlicingTools 
    {
        
        public static void UpdateParameters(ISlicingParameters slicingParameters)
        {
            slicingParameters.AddComponentToCreatedObjects<Rigidbody>(HandleCreatedRigidbody, CheckIfAddRigidbody);
        }

        private static bool CheckIfAddRigidbody(GameObject sourceObj)
        {
            return sourceObj.GetComponent<Rigidbody>() != null;
        }
        
        private static void HandleCreatedRigidbody(object obj, GameObject sourceObj)
        {
            // TODO: calculate mass
        }
    }
}