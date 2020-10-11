using UnityEngine;
using System.Collections;

public class MeshInspector : MonoBehaviour
{

    [SerializeField]
    [HideInInspector]
    private int _triangle;

    [SerializeField]
    private MeshFilter _meshFilter;

    public bool Inialized => _meshFilter != null;

    public void NextTriangle()
    {
        _triangle++;
        if (_triangle * 3 == _meshFilter.sharedMesh.triangles.Length)
            _triangle = 0;
    }

    private void OnDrawGizmos()
    {
        if (_meshFilter == null)
            return;

        Vector3[] vertices = _meshFilter.sharedMesh.vertices;
        int[] triangles = _meshFilter.sharedMesh.triangles;

        Mesh m = new Mesh();
        m.Clear();
        m.vertices = new Vector3[]
        {
            MathUtils.transformVertexFromScaledOrigin(vertices[triangles[_triangle * 3]], _meshFilter.transform.localScale, _meshFilter.transform.position),
            MathUtils.transformVertexFromScaledOrigin(vertices[triangles[_triangle * 3 + 1]], _meshFilter.transform.localScale, _meshFilter.transform.position),
            MathUtils.transformVertexFromScaledOrigin(vertices[triangles[_triangle * 3 + 2]], _meshFilter.transform.localScale, _meshFilter.transform.position)
        };
        m.triangles = new int[]
        {
            0, 1, 2
        };
        m.normals = new Vector3[] { new Vector3(0, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 0) };
        Gizmos.color = new Color(0, 0, 1);
        Gizmos.DrawMesh(m);
    }
}
