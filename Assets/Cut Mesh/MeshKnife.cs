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
        List<Vector2> sourceMeshUv = new List<Vector2>();
        List<Vector3> sourceMeshNormals = new List<Vector3>();
        List<Vector3> newMeshVertices = new List<Vector3>();
        List<int> newMeshTriangles = new List<int>();
        List<Vector2> newMeshUv = new List<Vector2>();
        List<Vector3> newMeshNormals = new List<Vector3>();

        for (int i = 0; i < _cutMeshFilter.sharedMesh.triangles.Length; i+=3)
        {
            Vector3 v1 = _cutMeshFilter.sharedMesh.vertices[_cutMeshFilter.sharedMesh.triangles[i]];
            v1.Scale(_cutMeshFilter.transform.localScale);
            v1 += _cutMeshFilter.transform.position;
            Vector3 v2 = _cutMeshFilter.sharedMesh.vertices[_cutMeshFilter.sharedMesh.triangles[i + 1]];
            v2.Scale(_cutMeshFilter.transform.localScale);
            v2 += _cutMeshFilter.transform.position;
            Vector3 v3 = _cutMeshFilter.sharedMesh.vertices[_cutMeshFilter.sharedMesh.triangles[i + 2]];
            v3.Scale(_cutMeshFilter.transform.localScale);
            v3 += _cutMeshFilter.transform.position;
            Plane cutPlane = new Plane(_basePoints[0].position, _basePoints[1].position, _basePoints[2].position);
            Vector2 uv1 = _cutMeshFilter.sharedMesh.uv[_cutMeshFilter.sharedMesh.triangles[i]];
            Vector2 uv2 = _cutMeshFilter.sharedMesh.uv[_cutMeshFilter.sharedMesh.triangles[i + 1]];
            Vector2 uv3 = _cutMeshFilter.sharedMesh.uv[_cutMeshFilter.sharedMesh.triangles[i + 2]];
            Vector3 normal1 = _cutMeshFilter.sharedMesh.normals[_cutMeshFilter.sharedMesh.triangles[i]];
            Vector3 normal2 = _cutMeshFilter.sharedMesh.normals[_cutMeshFilter.sharedMesh.triangles[i + 1]];
            Vector3 normal3 = _cutMeshFilter.sharedMesh.normals[_cutMeshFilter.sharedMesh.triangles[i + 2]];
            bool sideV1 = cutPlane.GetSide(v1);
            bool sideV2 = cutPlane.GetSide(v2);
            bool sideV3 = cutPlane.GetSide(v3);
            if (sideV1 && sideV2 && sideV3)
            {
                sourceMeshVertices.Add(v1);
                sourceMeshVertices.Add(v2);
                sourceMeshVertices.Add(v3);
                sourceMeshNormals.Add(normal1);
                sourceMeshNormals.Add(normal2);
                sourceMeshNormals.Add(normal3);
                sourceMeshUv.Add(uv1);
                sourceMeshUv.Add(uv2);
                sourceMeshUv.Add(uv3);
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
                newMeshNormals.Add(normal1);
                newMeshNormals.Add(normal2);
                newMeshNormals.Add(normal3);
                newMeshUv.Add(uv1);
                newMeshUv.Add(uv2);
                newMeshUv.Add(uv3);
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
                List<Vector2> intersectionUv = new List<Vector2>();
                if (d1 > 0 && d1 < (v2 - v1).magnitude)
                {
                    intersectionPoints.Add(r1.origin + r1.direction * d1);
                    intersectionUv.Add(uv1 + (uv2 - uv1) * d1 / ((v2-v1).magnitude));
                }
                if (d2 > 0 && d2 < (v3 - v2).magnitude)
                {
                    intersectionPoints.Add(r2.origin + r2.direction * d2);
                    intersectionUv.Add(uv2 + (uv3 - uv2) * d2 / ((v3 - v2).magnitude));
                }
                if (d3 > 0 && d3 < (v1 - v3).magnitude)
                {
                    intersectionPoints.Add(r3.origin + r3.direction * d3);
                    intersectionUv.Add(uv3 + (uv1 - uv3) * d3 / ((v1 - v3).magnitude));
                }
                Debug.Assert(intersectionPoints.Count == 2);
                Vector3 intersectionNormal = Vector3.Cross(intersectionPoints[0] - intersectionPoints[1], intersectionPoints[0] - v1);
                
                sourceMeshVertices.AddRange(intersectionPoints);
                sourceMeshUv.AddRange(intersectionUv);
                sourceMeshNormals.Add(intersectionNormal);
                sourceMeshNormals.Add(intersectionNormal);

                newMeshVertices.AddRange(intersectionPoints);
                newMeshUv.AddRange(intersectionUv);
                newMeshNormals.Add(intersectionNormal);
                newMeshNormals.Add(intersectionNormal);

                void distributePoint(bool side, Vector3 point, Vector3 normal, Vector2 uv)
                {
                    if (side)
                    {
                        sourceMeshVertices.Add(point);
                        sourceMeshUv.Add(uv);
                        sourceMeshNormals.Add(normal);
                    }
                    else
                    {
                        newMeshVertices.Add(point);
                        newMeshUv.Add(uv);
                        newMeshNormals.Add(normal);
                    }
                }

                distributePoint(sideV1, v1, normal1, uv1);
                distributePoint(sideV2, v2, normal2, uv2);
                distributePoint(sideV3, v3, normal3, uv3);

                if (!sideV1 && sideV2 && sideV3)
                {
                    int c = newMeshVertices.Count;
                    newMeshTriangles.Add(c - 3);
                    newMeshTriangles.Add(c - 2);
                    newMeshTriangles.Add(c - 1);

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
                    int c = newMeshVertices.Count;
                    newMeshTriangles.Add(c - 3);
                    newMeshTriangles.Add(c - 2);
                    newMeshTriangles.Add(c - 1);

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
                    int c = newMeshVertices.Count;
                    newMeshTriangles.Add(c - 3);
                    newMeshTriangles.Add(c - 2);
                    newMeshTriangles.Add(c - 1);

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
                    int c = sourceMeshVertices.Count;
                    sourceMeshTriangles.Add(c - 3);
                    sourceMeshTriangles.Add(c - 2);
                    sourceMeshTriangles.Add(c - 1);

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
                    int c = sourceMeshVertices.Count;
                    sourceMeshTriangles.Add(c - 3);
                    sourceMeshTriangles.Add(c - 2);
                    sourceMeshTriangles.Add(c - 1);

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
                    int c = sourceMeshVertices.Count;
                    sourceMeshTriangles.Add(c - 3);
                    sourceMeshTriangles.Add(c - 2);
                    sourceMeshTriangles.Add(c - 1);

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
        _cutMeshFilter.sharedMesh.uv = sourceMeshUv.ToArray();
        _cutMeshFilter.sharedMesh.normals = sourceMeshNormals.ToArray();


        var sourceCollider = _cutMeshFilter.GetComponent<Collider>();
        MeshCollider sourceMeshCollider = null;

        if (sourceCollider != null)
        {
            DestroyImmediate(sourceCollider);
            sourceMeshCollider = _cutMeshFilter.gameObject.AddComponent<MeshCollider>();
            sourceMeshCollider.sharedMesh = _cutMeshFilter.sharedMesh;
            sourceMeshCollider.convex = true;
        }

        if (newMeshVertices != null && newMeshVertices.Count > 0)
        {
            GameObject newMeshGameObject = new GameObject("newMesh");
            MeshFilter newMeshFilter = newMeshGameObject.AddComponent<MeshFilter>();
            newMeshFilter.sharedMesh = Instantiate(m);
            newMeshFilter.sharedMesh.vertices = newMeshVertices.ToArray();
            newMeshFilter.sharedMesh.triangles = newMeshTriangles.ToArray();
            newMeshFilter.sharedMesh.uv = newMeshUv.ToArray();
            newMeshFilter.sharedMesh.normals = newMeshNormals.ToArray();
            MeshRenderer newMeshRenderer = newMeshGameObject.AddComponent<MeshRenderer>();
            newMeshRenderer.sharedMaterial = _cutMeshFilter.GetComponent<MeshRenderer>().sharedMaterial;
            if (sourceMeshCollider != null)
            {
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
}
