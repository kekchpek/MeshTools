using System.Collections.Generic;
using System.Linq;
using MeshTools.Auxiliary;
using MeshTools.MeshKnife;
using UnityEngine;
using UnityEngine.Assertions;

namespace MeshTools.MeshTools.MeshKnife
{
    public class MeshKnife : MonoBehaviour, IMeshKnife
    {

        const float Epsilon = 0.00001f;

        private readonly struct VertexData
        {
            public Vector3 Coordinates { get; }
            public Vector3 Normal { get; }
            public Vector2 UV { get; }
            public bool Side { get; }

            /// <summary>
            /// Creates a data of a vertex in the context of mesh cutting.
            /// </summary>
            /// <param name="coordinates">Coordinates of the point.</param>
            /// <param name="normal">Normal of the point.</param>
            /// <param name="uv">UV coordinates of the point.</param>
            /// <param name="side">Side of the point.</param>
            public VertexData(Vector3 coordinates, Vector3 normal, Vector2 uv, bool side)
            {
                Coordinates = coordinates;
                Normal = normal;
                UV = uv;
                Side = side;
            }

            public static bool RaycastPlane(VertexData start, VertexData end, Plane plane, Vector3 normal, out VertexData intersection)
            {
                var dir = end.Coordinates - start.Coordinates;
                var r = new Ray(start.Coordinates, dir);
                if (plane.Raycast(r, out var d) && d > 0 && d <= dir.magnitude)
                {
                    intersection = new VertexData(start.Coordinates + dir.normalized * d,
                        normal,
                        Vector3.Lerp(start.UV, end.UV, d / dir.magnitude),
                        plane.GetSide(start.Coordinates + dir.normalized * d));
                    return true;
                }
                else
                {
                    Assert.IsTrue(!(plane.GetSide(start.Coordinates) ^ plane.GetSide(end.Coordinates)));
                    intersection = default;
                    return false;
                }
            }

            public override bool Equals(object obj)
            {
                return obj != null && obj.GetHashCode() == GetHashCode();
            }

            public override int GetHashCode()
            {
                return Normal.GetHashCode() + UV.GetHashCode() + Coordinates.GetHashCode() + Side.GetHashCode();
            }
        }

        [SerializeField] private Transform[] _basePoints;

        /// <summary>
        /// Mesh to cut.
        /// </summary>
        [SerializeField] private MeshFilter _cutMeshFilter;

        [SerializeField] private Material _cutMaterial;

        [SerializeField] private float _cutForce;

        public bool Initialized => _basePoints is {Length: 3};

        private void OnDrawGizmos()
        {
            if (_basePoints == null)
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

        public void Initialize()
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

        public void SetCuttingMesh(MeshFilter meshFilter)
        {
            _cutMeshFilter = meshFilter;
        }

        public void Cut()
        {
            var partOneVertices = new List<VertexData>();
            var partOneTriangles = new List<int>();

            var partTwoVertices = new List<VertexData>();
            var partTwoTriangles = new List<int>();

            var cutPlane = new Plane(_basePoints[0].position, _basePoints[1].position, _basePoints[2].position);

            var cutPlaneBuilder = new global::MeshTools.MeshKnife.CutPlaneBuilder.CutPlaneBuilder();

            var meshTransform = _cutMeshFilter.transform;
            var scale = meshTransform.lossyScale;
            var origin = meshTransform.position;
            var rotation = meshTransform.rotation;

            var sourceMesh = _cutMeshFilter.sharedMesh;
            
            // Pass through all triangles of the mesh
            for (var i = 0; i < sourceMesh.triangles.Length; i+=3)
            {
                var polygon = new List<VertexData>(); // triangle of vertex data that can become pentagon
                
                // convert simple coordinate triangle to the VertexData
                for(var j = 0; j < 3; j++)
                {
                    var vertexIndex = sourceMesh.triangles[i + j];
                    var coordinates = sourceMesh.vertices[vertexIndex];
                    var realCoordinates = MathUtils.TransformVertexToScaledRotatedOrigin(coordinates, scale, rotation, origin);
                    var vertex = new VertexData(
                        realCoordinates,
                        sourceMesh.normals[vertexIndex],
                        sourceMesh.uv[vertexIndex],
                        cutPlane.GetSide(realCoordinates));
                    polygon.Add(vertex);
                }
                
                if (polygon[0].Side && polygon[1].Side && polygon[2].Side)
                {
                    AddTriangle(ref partOneVertices, ref partOneTriangles, polygon.ToArray());
                }
                else if (!polygon[0].Side && !polygon[1].Side && !polygon[2].Side)
                {
                    AddTriangle(ref partTwoVertices, ref partTwoTriangles, polygon.ToArray());
                }
                else // triangle intersects cut plane
                {
                    // normal for new two points of cut triangle
                    var intersectionNormal = -Vector3.Cross(
                        polygon[0].Coordinates - polygon[1].Coordinates,
                        polygon[0].Coordinates - polygon[2].Coordinates).normalized;
                    
                    var intersectionIndices = new List<int>();

                    // find intersection points
                    var intersectionPoints = new List<Vector3>();
                    for (var j = 0; j < polygon.Count; j++)
                    {
                        var next = (j + 1) % polygon.Count;
                        if (VertexData.RaycastPlane(polygon[j], polygon[next], cutPlane, intersectionNormal, out var intersection))
                        {
                            polygon.Insert(j + 1, intersection);
                            intersectionIndices.Add(j + 1);
                            intersectionPoints.Add(intersection.Coordinates);
                            j++;
                        }
                    }

                    cutPlaneBuilder.AddEdge(
                        MathUtils.TransformVertexFromScaledRotatedOrigin(intersectionPoints[0], scale, Quaternion.identity, origin),
                        MathUtils.TransformVertexFromScaledRotatedOrigin(intersectionPoints[1], scale, Quaternion.identity, origin));

                    Debug.Assert(polygon.Count == 5);

                    // create triangles for new meshes
                    var basePoint = polygon[intersectionIndices[0]];
                    for (var j = 1; j < polygon.Count - 1; j++)
                    {
                        var index = (intersectionIndices[0] + j) % polygon.Count;
                        var nextIndex = (index + 1) % polygon.Count;
                        var current = polygon[index];
                        var next = polygon[nextIndex];
                        if (current.Side && index != intersectionIndices[1] || index == intersectionIndices[1] && next.Side)
                        {
                            AddTriangle(ref partOneVertices, ref partOneTriangles,
                                new[] { basePoint, current, next });
                        }
                        else
                        {
                            AddTriangle(ref partTwoVertices, ref partTwoTriangles,
                                new[] { basePoint, current, next });
                        }
                    }
                }
            }
        
            for (var i = 0; i < partTwoVertices.Count; i++)
            {
                var vertex = partTwoVertices[i];
                vertex = new VertexData(MathUtils.TransformVertexFromScaledRotatedOrigin(vertex.Coordinates, scale, Quaternion.identity, origin),
                    vertex.Normal,
                    vertex.UV,
                    vertex.Side);
                partTwoVertices[i] = vertex;
            }
            for (var i = 0; i < partOneVertices.Count; i++)
            {
                var vertex = partOneVertices[i];
                vertex = new VertexData(MathUtils.TransformVertexFromScaledRotatedOrigin(vertex.Coordinates, scale, Quaternion.identity, origin),
                    vertex.Normal,
                    vertex.UV,
                    vertex.Side);
                partOneVertices[i] = vertex;
            }

            if (partTwoVertices.Count > 0 && partOneVertices.Count > 0)
            {
                var createCollider = _cutMeshFilter.GetComponent<Collider>() != null;
                var createRigidbody = _cutMeshFilter.GetComponent<Rigidbody>() != null;
                var material = _cutMeshFilter.GetComponent<MeshRenderer>().sharedMaterial;
                var physicMaterial = _cutMeshFilter.GetComponent<Collider>()?.sharedMaterial;

                cutPlaneBuilder.SetMaterial(_cutMaterial);
                var cutPlaneOneObj = cutPlaneBuilder.Build();
                var cutPlaneTwoObj = cutPlaneBuilder.Build();

                CreateCutMeshObject(partTwoVertices, partTwoTriangles,
                    origin, scale,
                    -cutPlane.normal, _cutForce,
                    createCollider, createRigidbody,
                    material, physicMaterial,
                    cutPlaneOneObj);
                CreateCutMeshObject(partOneVertices, partOneTriangles,
                    origin, scale,
                    cutPlane.normal, _cutForce,
                    createCollider, createRigidbody,
                    material, physicMaterial,
                    cutPlaneTwoObj);
                DestroyImmediate(_cutMeshFilter.gameObject);
            }

        }

        private static void CreateCutMeshObject(List<VertexData> vertices, List<int> triangles,
            Vector3 position, Vector3 scale,
            Vector3 cutNormal, float cutForce,
            bool createCollider, bool createRigidbody,
            Material material, PhysicMaterial physicMaterial,
            GameObject cutPlane)
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
            var newMeshFilter = newMeshGameObject.AddComponent<MeshFilter>();
            newMeshFilter.sharedMesh = new Mesh
            {
                vertices = vertices.Select(v => v.Coordinates).ToArray(),
                triangles = triangles.ToArray(),
                uv = vertices.Select(v => v.UV).ToArray(),
                normals = vertices.Select(v => v.Normal).ToArray()
            };
            var newMeshRenderer = newMeshGameObject.AddComponent<MeshRenderer>();
            newMeshRenderer.sharedMaterial = material;
            if (createCollider)
            {
                var newMeshCollider = newMeshGameObject.AddComponent<MeshCollider>();
                newMeshCollider.sharedMesh = newMeshFilter.sharedMesh;
                newMeshCollider.sharedMaterial = physicMaterial;
                newMeshCollider.convex = true;
            }

            if (createRigidbody)
            {
                var newMeshRigidbody = newMeshGameObject.AddComponent<Rigidbody>();
                newMeshRigidbody.velocity = cutNormal * cutForce;
            }

            cutPlane.transform.parent = newMeshGameObject.transform;
            cutPlane.transform.localPosition = Vector3.zero;
            cutPlane.transform.rotation = Quaternion.identity;
            cutPlane.transform.localScale = Vector3.one;
        }

        private static void AddTriangle(ref List<VertexData> vertices, ref List<int> triangles, IReadOnlyCollection<VertexData> triangle)
        {
            Debug.Assert(triangle.Count == 3);
            foreach (var point in triangle)
            {
                var index = vertices.FindIndex(v =>
                    v.Coordinates == point.Coordinates &&
                    v.UV == point.UV &&
                    v.Normal == point.Normal);
                if (index < 0)
                {
                    vertices.Add(point);
                    triangles.Add(vertices.Count - 1);
                }
                else
                {
                    triangles.Add(index);
                }
            }
        }
    }
}
