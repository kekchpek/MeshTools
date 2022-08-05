using UnityEngine;

namespace MeshTools.Auxiliary
{
    public static class MathUtils
    {
        private const float Epsilon = 0.00001f;
        
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

        public static bool CompareVectors(Vector3 one, Vector3 two)
        {
            return
                Mathf.Abs(one.x - two.x) < Epsilon &&
                Mathf.Abs(one.y - two.y) < Epsilon &&
                Mathf.Abs(one.z - two.z) < Epsilon;
        }
    }
}
