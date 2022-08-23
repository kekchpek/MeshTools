using System;
using MeshTools.MeshKnife.CutPlaneBuilder;
using ModestTree;
using NUnit.Framework;
using UnityEngine;
using Assert = NUnit.Framework.Assert;

namespace MeshTools.Tests.MeshKnife
{
    public class CutPlaneBuilderTests
    {

        [Test]
        public void CreatePlane_FourEdges_ReturnsPolygonWithFourEdges(
            [Values(new[]{2,1,0,3}, new[]{0,1,2,3}, new[]{3,2,1,0}, new[]{3,0,1,2})] int[] edgesOrder)
        {
            // Arrange
            TestHelper.CreateContainerFor<CutPlaneBuilder>(out var cutPlaneBuilder);


            var points = new Vector3[]
            {
                new(0f, 0f, 0f),
                new(1f, 1f, 0f),
                new(1f, 1f, 1f),
                new(0f, 1f, 1f),
            };
            
            var edges = new[]
            {
                (p1: points[0], p2: points[1]),
                (p1: points[3], p2: points[2]),
                (p1: points[2], p2: points[1]),
                (p1: points[0], p2: points[3]),
            };

            // Act
            foreach (var order in edgesOrder)
            {
                cutPlaneBuilder.AddEdge(edges[order].p1, edges[order].p2);
            }
            var plane = cutPlaneBuilder.Build();

            // Assert
            var mesh = plane.GetComponent<MeshFilter>().sharedMesh;
            var vertices = mesh.vertices;
            for (var i = 0; i < vertices.Length; i++)
            {
                var pointIndex = points.IndexOf(vertices[i]);
                var nextPointIndex = points.IndexOf(vertices[(i + 1) % vertices.Length]);
                var indexDifference = Math.Abs(nextPointIndex - pointIndex) % (points.Length - 2);
                Assert.AreEqual(1, indexDifference);
            }
        }
        

        [Test]
        public void CreatePlane_MaterialSpecified_MaterialSet()
        {
            // Arrange
            TestHelper.CreateContainerFor<CutPlaneBuilder>(out var cutPlaneBuilder);

            var edges = new[]
            {
                (p1: new Vector3(0f, 0f, 0f), p2: new Vector3(1f, 1f, 0f)),
                (p1: new Vector3(1f, 1f, 0f), p2: new Vector3(1f, 1f, 1f)),
                (p1: new Vector3(1f, 1f, 1f), p2: new Vector3(0f, 1f, 1f)),
                (p1: new Vector3(0f, 1f, 1f), p2: new Vector3(0f, 0f, 0f)),
            };

            var material = new Material(Shader.Find("Specular"));

            // Act
            cutPlaneBuilder.AddEdge(edges[2].p1, edges[2].p2);
            cutPlaneBuilder.AddEdge(edges[1].p2, edges[1].p1);
            cutPlaneBuilder.SetMaterial(material);
            cutPlaneBuilder.AddEdge(edges[0].p1, edges[0].p2);
            cutPlaneBuilder.AddEdge(edges[3].p2, edges[3].p1);
            
            var plane = cutPlaneBuilder.Build();

            // Assert
            var renderer = plane.GetComponent<Renderer>();
            Assert.AreEqual(material, renderer.sharedMaterial);
        }
    }
}
