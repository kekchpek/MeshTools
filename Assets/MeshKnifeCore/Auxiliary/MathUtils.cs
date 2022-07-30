﻿using UnityEngine;

namespace MeshKnifeCore.Auxiliary
{
    public static class MathUtils
    {
        public static Vector3 TransformVertexToScaledRotatedOrigin(Vector3 vertex, Vector3 scale, Quaternion rotation, Vector3 origin)
        {
            var v = vertex;
            v.Scale(scale);
            v = rotation * v;
            v += origin;
            return v;
        }

        public static Vector3 TransformVertexFromScaledRotatedOrigin(Vector3 vertex, Vector3 scale, Quaternion rotation, Vector3 origin)
        {
            var v = vertex;
            v -= origin;
            v = Quaternion.Inverse(rotation) * v;
            var revertedScale = new Vector3(1f / scale.x, 1f / scale.y, 1f / scale.z);
            v.Scale(revertedScale);
            return v;
        }
    }
}
