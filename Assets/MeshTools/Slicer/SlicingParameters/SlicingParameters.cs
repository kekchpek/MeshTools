using System;
using System.Collections.Generic;
using UnityEngine;

namespace MeshTools.Slicer.SlicingParameters
{
    public class SlicingParameters : ISlicingParameters, ISlicingParametersReader
    {

        private readonly List<(Type, Action<object, GameObject>, Func<GameObject, bool>)> _componentsForNewObjects = new();

        public Vector3 CutForce { get; private set; }
        public IEnumerable<(Type, Action<object, GameObject>, Func<GameObject, bool>)> ComponentsForNewObjects 
            => _componentsForNewObjects.ToArray();
        public Action<GameObject> HandlerForChangedObjects { get; private set; }
        public bool CreateCutPlanes { get; private set; } = true;

        public ISlicingParameters AddCutForce(Vector3 cutForce)
        {
            CutForce += cutForce;
            return this;
        }
        
        public ISlicingParameters AddComponentToCreatedObjects<T>(Action<object, GameObject> componentInitializer,
            Func<GameObject, bool> precondition) where T : Component
        {
            _componentsForNewObjects.Add((typeof(T), componentInitializer, precondition));
            return this;
        }

        public ISlicingParameters DoNotCreateCutPlanes()
        {
            CreateCutPlanes = false;
            return this;
        }

        public ISlicingParameters AddHandlerForSourceObjects(Action<GameObject> handler)
        {
            HandlerForChangedObjects += handler;
            return this;
        }
    }
}