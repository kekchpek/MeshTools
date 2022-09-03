using MeshTools.Slicer.SlicingParameters;
using UnityEngine;

namespace MeshTools.Slicer.SlicingStrategies.SlicingTools
{
    internal static class ColliderSlicingTool
    {

        public static void UpdateParameters(ISlicingParameters slicingParameters)
        {
            slicingParameters.AddHandlerForSourceObjects(HandleSourceObject);
            slicingParameters.AddComponentToCreatedObjects<MeshCollider>(HandleCreatedCollider, CheckIfAddCollider);
        }

        private static void HandleSourceObject(GameObject gameObject)
        {
            var collider = gameObject.GetComponent<Collider>();
            if (collider != null)
            {
                var meshCollider = collider as MeshCollider;
                if (meshCollider == null)
                {
                    var colliderMaterial = collider.material;
                    Object.Destroy(collider);
                    meshCollider = gameObject.AddComponent<MeshCollider>();
                    meshCollider.material = colliderMaterial;
                }

                meshCollider.sharedMesh = gameObject.GetComponent<MeshFilter>().mesh;
            }
        }

        private static bool CheckIfAddCollider(GameObject sourceObj)
        {
            return sourceObj.GetComponent<Collider>() != null;
        }

        private static void HandleCreatedCollider(object obj, GameObject sourceObj)
        {
            var collider = (MeshCollider) obj;
            var sourceCollider = sourceObj.GetComponent<Collider>();
            collider.sharedMesh = collider.GetComponent<MeshFilter>().mesh;
            collider.material = sourceCollider.material;
            collider.convex = true;
        }
    }
}