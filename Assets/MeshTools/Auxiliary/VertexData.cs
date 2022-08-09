using UnityEngine;
using UnityEngine.Assertions;

namespace MeshTools.Auxiliary
{
    internal readonly struct VertexData
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
}