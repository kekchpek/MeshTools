using System;
using UnityEngine;

namespace MeshTools.Slicer.SlicingParameters
{
    public interface ISlicingParameters
    {

        static ISlicingParameters CreateNew()
        {
            return new SlicingParameters();
        }
        
        /// <summary>
        /// The force, that will be applied to created objects rigidbodies. 
        /// </summary>
        /// <param name="cutForce">Cut force value.</param>
        /// <returns>Updated cut parameters.</returns>
        ISlicingParameters AddCutForce(Vector3 cutForce);

        /// <summary>
        /// Adds a specified component to the created objects.
        /// </summary>
        /// <param name="componentInitializer">Initializer that accepts created component as the first argument,
        /// and source object after handling, as the second argument.</param>
        /// <param name="precondition">The condition indicates if the component should be added.
        /// Accept the source cut object as a parameter.</param>
        /// <returns>Updated cut parameters.</returns>
        ISlicingParameters AddComponentToCreatedObjects<T>(Action<object, GameObject> componentInitializer,
            Func<GameObject, bool> precondition = null) where T : Component;

        /// <summary>
        /// Handler for source cut objects with meshes after cut operation.
        /// </summary>
        /// <returns></returns>
        ISlicingParameters AddHandlerForSourceObjects(Action<GameObject> handler);

    }
}