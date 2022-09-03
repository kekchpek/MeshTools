using System;
using System.Linq;
using MeshTools.MeshKnife;
using MeshTools.Slicer.SlicingParameters;
using UnityEngine;

namespace MeshTools.Slicer
{
    public class Slicer : ISlicer
    {

        public void Cut(GameObject gameObject, Plane cutPlane, ISlicingParameters slicingParameters)
        {
            if (slicingParameters is null)
            {
                throw new ArgumentNullException(nameof(slicingParameters));
            }
            if (slicingParameters is not ISlicingParametersReader parameters)
            {
                throw new ArgumentException(
                    $"{nameof(slicingParameters)} should implement {nameof(ISlicingParametersReader)}");
            }
            
            var meshTransform = gameObject.transform;
            var scale = meshTransform.lossyScale;
            var origin = meshTransform.position;
            var rotation = meshTransform.rotation;

            var meshRenderersInObject = gameObject.GetComponentsInChildren<MeshFilter>();

            var gameObjectBounds = GetBounds(gameObject);
            var biggerSide = cutPlane.GetSide(gameObjectBounds.center);

            foreach (var meshFilter in meshRenderersInObject)
            {
                var processingObject = meshFilter.gameObject;
                var meshToCut = meshFilter.mesh;
                StaticMeshKnife.Cut(cutPlane, meshToCut, scale, rotation, origin,
                    out var positiveSideMeshes,
                    out var negativeSideMeshes);
                Mesh mainMesh;
                Mesh[] otherMeshes;
                if (biggerSide)
                {
                    mainMesh = positiveSideMeshes.First();
                    otherMeshes = positiveSideMeshes.Skip(1).Concat(negativeSideMeshes).ToArray();
                }
                else
                {
                    mainMesh =negativeSideMeshes.First();
                    otherMeshes = negativeSideMeshes.Skip(1).Concat(positiveSideMeshes).ToArray();
                }
                meshFilter.mesh = mainMesh;
                parameters.HandlerForChangedObjects?.Invoke(processingObject);
                foreach (var mesh in otherMeshes)
                {
                    var createdGameObject = CreateCutMeshObject(
                        origin, scale,
                        -cutPlane.normal, rotation,
                        mesh, meshFilter.GetComponent<MeshRenderer>().material);
                    foreach (var componentData in parameters.ComponentsForNewObjects)
                    {
                        if (componentData.precondition == null || componentData.precondition.Invoke(processingObject))
                        {
                            var component = createdGameObject.AddComponent(componentData.componentType);
                            componentData.componentInitializer?.Invoke(component, processingObject);
                        }
                    }
                }
            }
        }

        private static Bounds GetBounds(GameObject gameObject)
        {
            var renderers = gameObject.GetComponentsInChildren<Renderer>();
            var bounds = renderers[0].bounds;
            for (var i = 1; i < renderers.Length; ++i)
                bounds.Encapsulate(renderers[i].bounds);
            return bounds;
        }

        private static GameObject CreateCutMeshObject(
            Vector3 position, Vector3 scale,
            Vector3 cutNormal, Quaternion rotation,
            Mesh mesh, Material material)
        {
            var newMeshGameObject = new GameObject("newMesh")
            {
                transform =
                {
                    position = position
                }
            };
            newMeshGameObject.transform.position += cutNormal * 0.01f;
            newMeshGameObject.transform.localScale = scale;
            newMeshGameObject.transform.rotation = rotation;
            var newMeshFilter = newMeshGameObject.AddComponent<MeshFilter>();
            newMeshFilter.sharedMesh = mesh;
            var newMeshRenderer = newMeshGameObject.AddComponent<MeshRenderer>();
            newMeshRenderer.sharedMaterial = material;
            return newMeshGameObject;
        }
    }
}