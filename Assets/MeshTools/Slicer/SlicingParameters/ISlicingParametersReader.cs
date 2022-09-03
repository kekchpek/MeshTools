using System;
using System.Collections.Generic;
using UnityEngine;

namespace MeshTools.Slicer.SlicingParameters
{
    public interface ISlicingParametersReader
    {
        Vector3 CutForce { get; }
        IEnumerable<(Type componentType, Action<object, GameObject> componentInitializer, Func<GameObject, bool> precondition)>
            ComponentsForNewObjects { get; }
        Action<GameObject> HandlerForChangedObjects { get; }
        bool CreateCutPlanes { get; }
    }
}