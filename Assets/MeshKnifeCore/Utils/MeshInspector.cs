using MeshKnifeCore.Auxiliary;
using UnityEngine;

namespace MeshKnifeCore.Utils
{
    public class MeshInspector : MonoBehaviour
    {

        [SerializeField]
        [HideInInspector]
        private int _triangle;

        public int TriangleIndex => _triangle;

        [SerializeField]
        [HideInInspector]
        private int _vertex;

        public int VertexIndex => _vertex;

        [SerializeField]
        private MeshFilter _meshFilter;

        public void NextTriangle()
        {
            _triangle++;
            if (_triangle * 3 == _meshFilter.sharedMesh.triangles.Length)
                _triangle = 0;
        }

        public void NextVertex()
        {
            _vertex++;
            if (_vertex == _meshFilter.sharedMesh.vertices.Length)
                _vertex = 0;
        }

        private void OnDrawGizmos()
        {
            if (_meshFilter == null)
                return;

            var sharedMesh = _meshFilter.sharedMesh;
            var vertices = sharedMesh.vertices;
            var triangles = sharedMesh.triangles;

            var meshTransform = _meshFilter.transform;
            var scale = meshTransform.lossyScale;
            var origin = meshTransform.position;
            var rotation = meshTransform.rotation;
        

            var m = new Mesh();
            m.Clear();
            m.vertices = new[]
            {
                MathUtils.TransformVertexToScaledRotatedOrigin(vertices[triangles[_triangle * 3]], scale, rotation, origin),
                MathUtils.TransformVertexToScaledRotatedOrigin(vertices[triangles[_triangle * 3 + 1]], scale, rotation, origin),
                MathUtils.TransformVertexToScaledRotatedOrigin(vertices[triangles[_triangle * 3 + 2]], scale, rotation, origin)
            };
            m.triangles = new[]
            {
                0, 1, 2
            };
            m.normals = new Vector3[] { new(0, 1, 0), new(0, 1, 0), new(0, 1, 0) };
            Gizmos.color = new Color(0, 0, 1);
            Gizmos.DrawMesh(m);



            Gizmos.DrawSphere(
                MathUtils.TransformVertexToScaledRotatedOrigin(vertices[_vertex], scale, rotation, origin), 0.1f);
        }
    }
}
