using System;
using UnityEngine;

public static class MathUtils
{
    public static Vector3 transformVertexFromScaledOrigin(Vector3 vertex, Vector3 scale, Vector3 origin)
    {
        Vector3 v = vertex;
        v.Scale(scale);
        v += origin;
        return v;
    }

    public static Vector3 transformVertexToScaledOrigin(Vector3 vertex, Vector3 scale, Vector3 origin)
    {
        Vector3 v = vertex;
        v -= origin;
        Vector3 revertedScale = new Vector3(1f / scale.x, 1f / scale.y, 1f / scale.z);
        v.Scale(revertedScale);
        return v;
    }
}
