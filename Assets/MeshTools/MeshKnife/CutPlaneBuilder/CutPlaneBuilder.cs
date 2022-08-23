using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MeshTools.MeshKnife.CutPlaneBuilder
{
    public class CutPlaneBuilder : ICutPlaneBuilder
    {
        
        private const string Tag = "CutPlane";

        private readonly IList<IList<Vector3>> _segments = new List<IList<Vector3>>();

        private Material _planeMaterial;

        public ICutPlaneBuilder AddEdge(Vector3 point1, Vector3 point2)
        {
            IList<Vector3> firstSegmentToExpand = null;
            IList<Vector3> secondSegmentToExpand = null;
            foreach (var segment in _segments)
            {
                if (segment.First() == point1 || segment.Last() == point1 ||
                    segment.First() == point2 || segment.Last() == point2)
                {
                    if (firstSegmentToExpand == null)
                    {
                        firstSegmentToExpand = segment;
                        continue;
                    }

                    secondSegmentToExpand = segment;
                    break;
                }
            }

            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            if (firstSegmentToExpand == null && secondSegmentToExpand == null)
            {
                _segments.Add(new List<Vector3>{point1, point2});
            }
            else if (firstSegmentToExpand != null && secondSegmentToExpand == null)
            {
                ExpandSegment(ref firstSegmentToExpand, point1, point2);
            }
            else if (firstSegmentToExpand != null && secondSegmentToExpand != null)
            {
                _segments.Remove(firstSegmentToExpand);
                _segments.Remove(secondSegmentToExpand);
                _segments.Add(ConcatSegments(firstSegmentToExpand, secondSegmentToExpand, point1, point2));
            }
            // ReSharper restore ConditionIsAlwaysTrueOrFalse

            return this;
        }

        private void ExpandSegment(ref IList<Vector3> segment, Vector3 point1, Vector3 point2)
        {
            if (segment.First() == point1)
            {
                segment.Insert(0, point2);
            }
            else if (segment.First() == point2)
            {
                segment.Insert(0, point1);
            }
            else if (segment.Last() == point2)
            {
                segment.Add(point1);
            }
            else if (segment.Last() == point1)
            {
                segment.Add(point2);
            }
            CheckSegmentCycled(ref segment);
        }

        private IList<Vector3> ConcatSegments(IList<Vector3> segment1, IList<Vector3> segment2, 
            Vector3 point1, Vector3 point2)
        {
            if (segment1.Last() == point1)
            {
                if (segment2.Last() == point2)
                {
                    return segment1.Concat(segment2.Reverse()).ToList();
                }

                if (segment2.First() == point2)
                {
                    return segment1.Concat(segment2).ToList();
                }
            }
            else if (segment1.First() == point1)
            {
                if (segment2.Last() == point2)
                {
                    return segment1.Reverse().Concat(segment2.Reverse()).ToList();
                }

                if (segment2.First() == point2)
                {
                    return segment1.Reverse().Concat(segment2).ToList();
                }
            }
            if (segment1.Last() == point2)
            {
                if (segment2.Last() == point1)
                {
                    return segment1.Concat(segment2.Reverse()).ToList();
                }

                if (segment2.First() == point1)
                {
                    return segment1.Concat(segment2).ToList();
                }
            }
            else if (segment1.First() == point2)
            {
                if (segment2.Last() == point1)
                {
                    return segment1.Reverse().Concat(segment2.Reverse()).ToList();
                }

                if (segment2.First() == point1)
                {
                    return segment1.Reverse().Concat(segment2).ToList();
                }
            }

            throw new InvalidOperationException("There are no points to concat segments!");
        }

        private void CheckSegmentCycled(ref IList<Vector3> segment)
        {
            // and remove one border point if it is
            if (segment.First() == segment.Last())
                segment.RemoveAt(0);
        }

        public ICutPlaneBuilder SetMaterial(Material material)
        {
            _planeMaterial = material;
            return this;
        }

        public GameObject Build()
        {
            if (_segments.Count != 1)
                throw new InvalidOperationException("Not enough edges to build a polygon!");
            var planeObj = new GameObject();
            var meshFilter = planeObj.AddComponent<MeshFilter>();
            var vertices = _segments.First().ToArray();
            var triangles = new List<int>();
            for (var i = 1; i < vertices.Length - 1; i++)
            {
                // one side
                triangles.Add(0);
                triangles.Add(i);
                triangles.Add(i + 1);
                
                // other side
                triangles.Add(0);
                triangles.Add(i + 1);
                triangles.Add(i);
            }
            var mesh = new Mesh
            {
                vertices = vertices,
                triangles = triangles.ToArray()
            };
            meshFilter.sharedMesh = mesh;
            var renderer = planeObj.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = _planeMaterial;
            return planeObj;
        }
    }
}