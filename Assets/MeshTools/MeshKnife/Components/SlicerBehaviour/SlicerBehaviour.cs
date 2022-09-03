using System;
using System.Linq;
using MeshTools.MeshKnife.Components.MeshKnifeBehaviour;
using MeshTools.Slicer.SlicingStrategies;
using MeshTools.Slicer.SlicingStrategies.Builder;
using UnityEngine;

namespace MeshTools.MeshKnife.Components.SlicerBehaviour
{
    public class SlicerBehaviour : MonoBehaviour, ISlicerBehaviour
    {
        [SerializeField] private Transform[] _basePoints;

        /// <summary>
        /// Object to cut.
        /// </summary>
        [SerializeField] private GameObject _cutObject;

        public bool BasePointsSet => _basePoints is {Length: 3} && _basePoints.All(x => x != null);
        
        private readonly ISlicingStrategy _slicingStrategy = new SlicingStrategyBuilder()
            .SetSlicer(new Slicer.Slicer())
            .SliceColliders()
            .SliceRigidbodies()
            .Build();

        public void CreateBasePoints()
        {
            _basePoints = new Transform[3];
            for (var i = 0; i < 3; i++)
            {
                _basePoints[i] = new GameObject($"BasePoint{i}").transform;
                var cachedTransform = transform;
                _basePoints[i].position = cachedTransform.position;
                _basePoints[i].parent = cachedTransform;
            }
        }

        public void Cut()
        {
            if (BasePointsSet)
            {
                var cutPlane = new Plane(_basePoints[0].position, _basePoints[1].position, _basePoints[2].position);
                _slicingStrategy.Cut(_cutObject, cutPlane);
            }
            else
            {
                throw new InvalidOperationException("Not possible to cut a mesh without base points for a cut plane.");
            }
        }

        private void OnDrawGizmos()
        {
            if (_basePoints == null || !_basePoints.Any() || _basePoints.Any(x => x == null))
                return;

            var m = new Mesh();
            m.Clear();
            m.vertices = new[]
            {
                _basePoints[0].position,
                _basePoints[1].position,
                _basePoints[2].position,
            };
            m.triangles = new[]
            {
                0, 1, 2,
                2, 1, 0
            };
            m.normals = new[] { Vector3.up, Vector3.up, Vector3.up };
            Gizmos.color = new Color(1, 0, 0);
            Gizmos.DrawLine(_basePoints[0].position, _basePoints[1].position);
            Gizmos.DrawLine(_basePoints[1].position, _basePoints[2].position);
            Gizmos.DrawLine(_basePoints[2].position, _basePoints[0].position);
            Gizmos.DrawMesh(m);
        }
    }
}