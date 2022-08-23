using System;
using System.Linq;
using UnityEngine;

namespace MeshTools.MeshKnife.Components.MeshKnifeBehaviour
{
    public class MeshKnifeBehaviour : MonoBehaviour, IMeshKnifeBehaviour
    {
        [SerializeField] private Transform[] _basePoints;

        /// <summary>
        /// Mesh to cut.
        /// </summary>
        [SerializeField] private MeshFilter _cutMeshFilter;

        [SerializeField] private Material _cutMaterial;

        [SerializeField] private float _cutForce;

        public bool BasePointsSet => _basePoints is {Length: 3} && _basePoints.All(x => x != null);

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
                StaticMeshKnife
                    .Cut(new Plane(_basePoints[0].position, _basePoints[1].position, _basePoints[2].position),
                        _cutMeshFilter,
                        _cutMaterial, _cutForce);
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