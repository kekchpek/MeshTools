using System.Collections.Generic;
using UnityEngine;

public class MeshKnife : MonoBehaviour
{

    [SerializeField] private Transform[] _basePoints;

    [SerializeField] private MeshFilter _cutMeshFilter;

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
        List<Vector3> sourceMeshVertices = new List<Vector3>();
        List<int> sourceMeshTriangles = new List<int>();
        List<Vector3> newMeshVertices = new List<Vector3>();
        List<int> newMeshTriangles = new List<int>();

        for (int i = 0; i < _cutMeshFilter.sharedMesh.triangles.Length; i+=3)
        {
            Vector3 v1 = _cutMeshFilter.sharedMesh.vertices[_cutMeshFilter.sharedMesh.triangles[i]];
            Vector3 v2 = _cutMeshFilter.sharedMesh.vertices[_cutMeshFilter.sharedMesh.triangles[i + 1]];
            Vector3 v3 = _cutMeshFilter.sharedMesh.vertices[_cutMeshFilter.sharedMesh.triangles[i + 2]];
            Plane cutPlane = new Plane(_basePoints[0].position, _basePoints[1].position, _basePoints[2].position);
            bool sideV1 = cutPlane.GetSide(v1);
            bool sideV2 = cutPlane.GetSide(v2);
            bool sideV3 = cutPlane.GetSide(v3);
            if (sideV1 && sideV2 && sideV3)
            {
                sourceMeshVertices.Add(v1);
                sourceMeshVertices.Add(v2);
                sourceMeshVertices.Add(v3);
                int c = sourceMeshVertices.Count;
                sourceMeshTriangles.Add(c - 3);
                sourceMeshTriangles.Add(c - 2);
                sourceMeshTriangles.Add(c - 1);
            }
            else if (!sideV1 && !sideV2 && !sideV3)
            {
                newMeshVertices.Add(v1);
                newMeshVertices.Add(v2);
                newMeshVertices.Add(v3);
                int c = newMeshVertices.Count;
                newMeshTriangles.Add(c - 3);
                newMeshTriangles.Add(c - 2);
                newMeshTriangles.Add(c - 1);
            }
            else
            {
                Ray r1 = new Ray(v1, v2 - v1);
                Ray r2 = new Ray(v2, v3 - v2);
                Ray r3 = new Ray(v3, v1 - v3);
                cutPlane.Raycast(r1, out float d1);
                cutPlane.Raycast(r2, out float d2);
                cutPlane.Raycast(r3, out float d3);
                List<Vector3> intersectionPoints = new List<Vector3>();
                if (d1 > 0 && d1 < (v2 - v1).magnitude)
                {
                    intersectionPoints.Add(r1.origin + r1.direction * d1);
                }
                if (d2 > 0 && d2 < (v3 - v2).magnitude)
                {
                    intersectionPoints.Add(r2.origin + r2.direction * d2);
                }
                if (d3 > 0 && d3 < (v1 - v3).magnitude)
                {
                    intersectionPoints.Add(r3.origin + r3.direction * d3);
                }
                Debug.Assert(intersectionPoints.Count == 2);
                sourceMeshVertices.AddRange(intersectionPoints);
                newMeshVertices.AddRange(intersectionPoints);
                if (!sideV1 && sideV2 && sideV3)
                {
                    newMeshVertices.Add(v1);
                    int c = newMeshVertices.Count;
                    newMeshTriangles.Add(c - 3);
                    newMeshTriangles.Add(c - 2);
                    newMeshTriangles.Add(c - 1);

                    sourceMeshVertices.Add(v2);
                    sourceMeshVertices.Add(v3);
                    int c1 = sourceMeshVertices.Count;
                    sourceMeshTriangles.Add(c1 - 1);
                    sourceMeshTriangles.Add(c1 - 2);
                    sourceMeshTriangles.Add(c1 - 3);
                    sourceMeshTriangles.Add(c1 - 1);
                    sourceMeshTriangles.Add(c1 - 3);
                    sourceMeshTriangles.Add(c1 - 4);
                }
                if (sideV1 && !sideV2 && sideV3)
                {
                    newMeshVertices.Add(v2);
                    int c = newMeshVertices.Count;
                    newMeshTriangles.Add(c - 3);
                    newMeshTriangles.Add(c - 2);
                    newMeshTriangles.Add(c - 1);

                    sourceMeshVertices.Add(v1);
                    sourceMeshVertices.Add(v3);
                    int c1 = sourceMeshVertices.Count;
                    sourceMeshTriangles.Add(c1 - 1);
                    sourceMeshTriangles.Add(c1 - 2);
                    sourceMeshTriangles.Add(c1 - 3);
                    sourceMeshTriangles.Add(c1 - 1);
                    sourceMeshTriangles.Add(c1 - 3);
                    sourceMeshTriangles.Add(c1 - 4);
                }
                if (sideV1 && sideV2 && !sideV3)
                {
                    newMeshVertices.Add(v3);
                    int c = newMeshVertices.Count;
                    newMeshTriangles.Add(c - 3);
                    newMeshTriangles.Add(c - 2);
                    newMeshTriangles.Add(c - 1);

                    sourceMeshVertices.Add(v1);
                    sourceMeshVertices.Add(v2);
                    int c1 = sourceMeshVertices.Count;
                    sourceMeshTriangles.Add(c1 - 1);
                    sourceMeshTriangles.Add(c1 - 2);
                    sourceMeshTriangles.Add(c1 - 3);
                    sourceMeshTriangles.Add(c1 - 1);
                    sourceMeshTriangles.Add(c1 - 3);
                    sourceMeshTriangles.Add(c1 - 4);
                }
                if (sideV1 && !sideV2 && !sideV3)
                {
                    sourceMeshVertices.Add(v1);
                    int c = sourceMeshVertices.Count;
                    sourceMeshTriangles.Add(c - 3);
                    sourceMeshTriangles.Add(c - 2);
                    sourceMeshTriangles.Add(c - 1);

                    newMeshVertices.Add(v2);
                    newMeshVertices.Add(v3);
                    int c1 = newMeshVertices.Count;
                    newMeshTriangles.Add(c1 - 1);
                    newMeshTriangles.Add(c1 - 2);
                    newMeshTriangles.Add(c1 - 3);
                    newMeshTriangles.Add(c1 - 2);
                    newMeshTriangles.Add(c1 - 3);
                    newMeshTriangles.Add(c1 - 4);
                }
                if (!sideV1 && sideV2 && !sideV3)
                {
                    sourceMeshVertices.Add(v2);
                    int c = sourceMeshVertices.Count;
                    sourceMeshTriangles.Add(c - 3);
                    sourceMeshTriangles.Add(c - 2);
                    sourceMeshTriangles.Add(c - 1);

                    newMeshVertices.Add(v1);
                    newMeshVertices.Add(v3);
                    int c1 = newMeshVertices.Count;
                    newMeshTriangles.Add(c1 - 1);
                    newMeshTriangles.Add(c1 - 2);
                    newMeshTriangles.Add(c1 - 3);
                    newMeshTriangles.Add(c1 - 2);
                    newMeshTriangles.Add(c1 - 3);
                    newMeshTriangles.Add(c1 - 4);
                }
                if (!sideV1 && !sideV2 && sideV3)
                {
                    sourceMeshVertices.Add(v3);
                    int c = sourceMeshVertices.Count;
                    sourceMeshTriangles.Add(c - 3);
                    sourceMeshTriangles.Add(c - 2);
                    sourceMeshTriangles.Add(c - 1);

                    newMeshVertices.Add(v1);
                    newMeshVertices.Add(v2);
                    int c1 = newMeshVertices.Count;
                    newMeshTriangles.Add(c1 - 1);
                    newMeshTriangles.Add(c1 - 2);
                    newMeshTriangles.Add(c1 - 3);
                    newMeshTriangles.Add(c1 - 2);
                    newMeshTriangles.Add(c1 - 3);
                    newMeshTriangles.Add(c1 - 4);
                }
            }
        }

        int sc = sourceMeshTriangles.Count;
        for (int i = 0; i < sc; i++)
        {
            sourceMeshTriangles.Add(sourceMeshTriangles[sc - i - 1]);
        }
        int sc1 = newMeshTriangles.Count;
        for (int i = 0; i < sc1; i++)
        {
            newMeshTriangles.Add(newMeshTriangles[sc1 - i - 1]);
        }

        Mesh m = new Mesh();
        m.Clear();
        _cutMeshFilter.sharedMesh = Instantiate(m);
        _cutMeshFilter.sharedMesh.vertices = sourceMeshVertices.ToArray();
        _cutMeshFilter.sharedMesh.triangles = sourceMeshTriangles.ToArray();

        GameObject newMeshGameObject = new GameObject("newMesh");
        MeshFilter newMeshFilter = newMeshGameObject.AddComponent<MeshFilter>();
        newMeshFilter.sharedMesh = Instantiate(m);
        newMeshFilter.sharedMesh.vertices = newMeshVertices.ToArray();
        newMeshFilter.sharedMesh.triangles = newMeshTriangles.ToArray();
        newMeshGameObject.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));


        var sourceCollider = _cutMeshFilter.GetComponent<Collider>();
        if (sourceCollider != null)
        {
            DestroyImmediate(sourceCollider);
            var sourceMeshCollider = _cutMeshFilter.gameObject.AddComponent<MeshCollider>();
            sourceMeshCollider.sharedMesh = _cutMeshFilter.sharedMesh;
            sourceMeshCollider.convex = true;
            var newMeshCollider = newMeshGameObject.AddComponent<MeshCollider>();
            newMeshCollider.sharedMesh = newMeshFilter.sharedMesh;
            newMeshCollider.convex = true;
        }

        var sourceRigidbody = _cutMeshFilter.GetComponent<Rigidbody>();
        if (sourceRigidbody != null)
        {
            newMeshGameObject.AddComponent<Rigidbody>();
        }

    }
}
