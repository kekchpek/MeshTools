using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshKnife : MonoBehaviour
{

    private struct VertexData
    {
        public Vector3 Coordinates { get; set; }
        public Vector3 Normal { get; set; }
        public Vector2 UV { get; set; }
        public bool Side { get; set; }

        public VertexData(Vector3 coordinates, Vector3 normal, Vector3 uv, bool side)
        {
            Coordinates = coordinates;
            Normal = normal;
            UV = uv;
            Side = side;
        }

        public VertexData(Mesh mesh, int vertexIntex, Plane plane)
        {
            if (vertexIntex > mesh.vertexCount)
                throw new ArgumentException($"There is no vertex with index {vertexIntex} in {nameof(mesh)}. {nameof(mesh)} vertex count is {mesh.vertexCount}");
            Coordinates = mesh.vertices[vertexIntex];
            Normal = mesh.normals[vertexIntex];
            UV = mesh.uv[vertexIntex];
            Side = plane.GetSide(Coordinates);
        }

        public static bool RaycastPlane(VertexData start, VertexData end, Plane plane, out VertexData intersection)
        {
            Vector3 dir = end.Coordinates - start.Coordinates;
            Vector2 uvDir = end.UV - start.UV;
            Ray r = new Ray(start.Coordinates, dir);
            plane.Raycast(r, out float d);
            if (d > 0 && d < dir.magnitude)
            {
                intersection = new VertexData(start.Coordinates + dir.normalized * d,
                    Vector3.zero,
                    Vector3.Lerp(start.UV, end.UV, d / dir.magnitude),
                    plane.GetSide(start.Coordinates + dir.normalized * d));
                return true;
            }
            else
            {
                intersection = default;
                return false;
            }
        }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == this.GetHashCode();
        }

        public override int GetHashCode()
        {
            return Normal.GetHashCode() + UV.GetHashCode() + Coordinates.GetHashCode();
        }
    }

    [SerializeField] private Transform[] _basePoints;

    [SerializeField] private MeshFilter _cutMeshFilter;

    [SerializeField] private Material _cutMaterial;

    [SerializeField] private float _cutForce;

    public bool Initialized => _basePoints != null && _basePoints.Length == 3;

    private void OnDrawGizmos()
    {
        if (_basePoints == null)
            return;

        Mesh m = new Mesh();
        m.Clear();
        m.vertices = new Vector3[]
        {
            _basePoints[0].position,
            _basePoints[1].position,
            _basePoints[2].position,
        };
        m.triangles = new int[]
        {
            0, 1, 2,
            2, 1, 0
        };
        m.normals = new Vector3[] { new Vector3(0, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 0) };
        Gizmos.color = new Color(1, 0, 0);
        Gizmos.DrawLine(_basePoints[0].position, _basePoints[1].position);
        Gizmos.DrawLine(_basePoints[1].position, _basePoints[2].position);
        Gizmos.DrawLine(_basePoints[2].position, _basePoints[0].position);
        Gizmos.DrawMesh(m);
    }

    public void Initialize()
    {
        _basePoints = new Transform[3];
        for (int i = 0; i < 3; i++)
        {
            _basePoints[i] = new GameObject($"BasePoint{i}").transform;
            _basePoints[i].position = transform.position;
            _basePoints[i].parent = transform;
        }
    }

    public void Cut()
    {
        List<VertexData> sourceVertices = new List<VertexData>();
        List<int> sourceMeshTriangles = new List<int>();

        List<VertexData> newVerticies = new List<VertexData>();
        List<int> newMeshTriangles = new List<int>();

        Plane cutPlane = new Plane(_basePoints[0].position, _basePoints[1].position, _basePoints[2].position);

        List<VertexData> intersectionPoints = new List<VertexData>();

        Vector3 scale = _cutMeshFilter.transform.localScale;
        Vector3 origin = _cutMeshFilter.transform.position;

        Mesh sourceMesh = _cutMeshFilter.sharedMesh;
        for (int i = 0; i < sourceMesh.triangles.Length; i+=3)
        {
            List<VertexData> polygon = new List<VertexData>();
            for(int j = 0; j < 3; j++)
            {
                VertexData vertex = new VertexData(sourceMesh, sourceMesh.triangles[i + j], cutPlane);
                vertex.Coordinates = MathUtils.transformVertexFromScaledOrigin(vertex.Coordinates, scale, origin);
                polygon.Add(vertex);
            }
            if (polygon[0].Side && polygon[1].Side && polygon[2].Side)
            {
                addTriangle(ref sourceVertices, ref sourceMeshTriangles, polygon.ToArray());
            }
            else if (!polygon[0].Side && !polygon[1].Side && !polygon[2].Side)
            {
                addTriangle(ref newVerticies, ref newMeshTriangles, polygon.ToArray());
            }
            else
            {
                Vector3 intersectionNormal = -Vector3.Cross(
                    polygon[0].Coordinates - polygon[1].Coordinates,
                    polygon[0].Coordinates - polygon[2].Coordinates).normalized;
                List<int> intersectionIndicies = new List<int>();
                for (int j = 0; j < polygon.Count; j++)
                {
                    int next = (j + 1) % polygon.Count;
                    if (VertexData.RaycastPlane(polygon[j], polygon[next], cutPlane, out VertexData intersection))
                    {
                        intersection.Normal = intersectionNormal;
                        polygon.Insert(j + 1, intersection);
                        intersectionIndicies.Add(j + 1);
                        if(!intersectionPoints.Contains(intersection))
                            intersectionPoints.Add(intersection);
                        j++;
                    }
                }
                Debug.Assert(polygon.Count == 5);

                VertexData basePoint = polygon[intersectionIndicies[0]];
                for (int j = 1; j < polygon.Count; j++)
                {
                    int index = (intersectionIndicies[0] + j) % polygon.Count;
                    int nextIndex = (index + 1) % polygon.Count;
                    VertexData current = polygon[index];
                    VertexData next = polygon[nextIndex];
                    if (current.Side && index != intersectionIndicies[1])
                    {
                        addTriangle(ref sourceVertices, ref sourceMeshTriangles,
                            new VertexData[] { basePoint, current, next });
                    }
                    else
                    {
                        addTriangle(ref newVerticies, ref newMeshTriangles,
                            new VertexData[] { basePoint, current, next });
                    }
                }
            }
        }
        
        for (int i = 0; i < newVerticies.Count; i++)
        {
            VertexData vertex = newVerticies[i];
            vertex = new VertexData(MathUtils.transformVertexToScaledOrigin(vertex.Coordinates, scale, origin),
                vertex.Normal,
                vertex.UV,
                vertex.Side);
            newVerticies[i] = vertex;
        }
        for (int i = 0; i < sourceVertices.Count; i++)
        {
            VertexData vertex = sourceVertices[i];
            vertex = new VertexData(MathUtils.transformVertexToScaledOrigin(vertex.Coordinates, scale, origin),
                vertex.Normal,
                vertex.UV,
                vertex.Side);
            sourceVertices[i] = vertex;
        }
        for (int i = 0; i < intersectionPoints.Count; i++)
        {
            VertexData vertex = intersectionPoints[i];
            vertex = new VertexData(MathUtils.transformVertexToScaledOrigin(vertex.Coordinates, scale, origin),
                vertex.Normal,
                vertex.UV,
                vertex.Side);
            intersectionPoints[i] = vertex;
        }

        if (newVerticies != null && newVerticies.Count > 0)
        {
            bool createCollider = _cutMeshFilter.GetComponent<Collider>() != null;
            bool createRigidbody = _cutMeshFilter.GetComponent<Rigidbody>() != null;
            Material material = _cutMeshFilter.GetComponent<MeshRenderer>().sharedMaterial;
            PhysicMaterial physicMaterial = _cutMeshFilter.GetComponent<Collider>()?.sharedMaterial;

            GameObject cutPlaneObj = createCutPlane(intersectionPoints, _cutMaterial);

            createCutMeshObject(newVerticies, newMeshTriangles,
                origin, scale,
                -cutPlane.normal, _cutForce,
                createCollider, createRigidbody,
                material, physicMaterial,
                cutPlaneObj);
            createCutMeshObject(sourceVertices, sourceMeshTriangles,
                origin, scale,
                cutPlane.normal, _cutForce,
                createCollider, createRigidbody,
                material, physicMaterial,
                cutPlaneObj);
            DestroyImmediate(cutPlaneObj);
            DestroyImmediate(_cutMeshFilter.gameObject);
        }

    }

    private static void createCutMeshObject(List<VertexData> vertices, List<int> triangles,
        Vector3 position, Vector3 scale,
        Vector3 cutNormal, float cutForce,
        bool createCollider, bool createRigidbody,
        Material material, PhysicMaterial physicMaterial,
        GameObject cutPlane)
    {
        GameObject newMeshGameObject = new GameObject("newMesh");
        newMeshGameObject.transform.position = position;
        newMeshGameObject.transform.position += cutNormal * 0.01f;
        newMeshGameObject.transform.localScale = scale;
        MeshFilter newMeshFilter = newMeshGameObject.AddComponent<MeshFilter>();
        newMeshFilter.sharedMesh = new Mesh();
        newMeshFilter.sharedMesh.vertices = vertices.Select(v => v.Coordinates).ToArray();
        newMeshFilter.sharedMesh.triangles = triangles.ToArray();
        newMeshFilter.sharedMesh.uv = vertices.Select(v => v.UV).ToArray();
        newMeshFilter.sharedMesh.normals = vertices.Select(v => v.Normal).ToArray();
        MeshRenderer newMeshRenderer = newMeshGameObject.AddComponent<MeshRenderer>();
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
            Rigidbody newMeshRigidbody = newMeshGameObject.AddComponent<Rigidbody>();
            newMeshRigidbody.velocity = cutNormal * cutForce;
        }

        GameObject newCutPlaneObj = Instantiate(cutPlane);
        newCutPlaneObj.transform.parent = newMeshGameObject.transform;
        newCutPlaneObj.transform.localPosition = Vector3.zero;
    }

    private static GameObject createCutPlane(List<VertexData> vertices, Material material)
    {
        GameObject cutPlaneObj = new GameObject("cutPlane");
        cutPlaneObj.transform.localPosition = Vector3.zero;
        MeshFilter sourceCutPlaneMeshFilter = cutPlaneObj.AddComponent<MeshFilter>();
        Mesh sourceCutPlaneMesh = new Mesh();
        sourceCutPlaneMesh.Clear();
        sourceCutPlaneMesh.vertices = vertices.Select(v => v.Coordinates).ToArray();
        List<int> sourceCutMeshTriangles = new List<int>();
        for (int i = 1; i < vertices.Count - 1; i++)
        {
            sourceCutMeshTriangles.Add(0);
            sourceCutMeshTriangles.Add(i);
            sourceCutMeshTriangles.Add(i + 1);
            sourceCutMeshTriangles.Add(i + 1);
            sourceCutMeshTriangles.Add(i);
            sourceCutMeshTriangles.Add(0);
        }
        sourceCutPlaneMesh.triangles = sourceCutMeshTriangles.ToArray();
        sourceCutPlaneMesh.normals = Enumerable.Repeat<Vector3>(
            Vector3.Cross(
                vertices[1].Coordinates - vertices[0].Coordinates,
                vertices[2].Coordinates - vertices[0].Coordinates).normalized,
            vertices.Count).ToArray();
        sourceCutPlaneMeshFilter.sharedMesh = sourceCutPlaneMesh;
        MeshRenderer sourceCutPlaneMeshRenderer = cutPlaneObj.AddComponent<MeshRenderer>();
        sourceCutPlaneMeshRenderer.material = material;
        return cutPlaneObj;
    }

    private static void addTriangle(ref List<VertexData> verticies, ref List<int> triangles, VertexData[] triangle)
    {
        Debug.Assert(triangle.Length == 3);
        for(int i = 0; i < triangle.Length; i++)
        {
            int index = verticies.IndexOf(triangle[i]);
            if (index < 0)
            {
                verticies.Add(triangle[i]);
                triangles.Add(verticies.Count - 1);
            }
            else
            {
                triangles.Add(index);
            }
        }
        verticies.Add(triangle[0]);
        verticies.Add(triangle[1]);
        verticies.Add(triangle[2]);
        int c = verticies.Count;
        triangles.Add(c - 3);
        triangles.Add(c - 2);
        triangles.Add(c - 1);
    }
}
