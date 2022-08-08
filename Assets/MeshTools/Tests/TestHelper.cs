using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSubstitute;
using Zenject;

namespace MeshTools.Tests
{
    public static class TestHelper
    {

        /// <summary>
        /// Creates a container filled with substitutes and one specified real object.
        /// </summary>
        /// <param name="createdObj">Created object</param>
        /// <typeparam name="T">The type of real object that should be created.</typeparam>
        /// <returns>The container filled with substitutes and one specified real object.</returns>
        public static DiContainer CreateContainerFor<T>(out T createdObj)
        {
            return CreateContainerFor(out createdObj, new Dictionary<Type, object>());
        }

        /// <summary>
        /// Creates a container filled with substitutes and one specified real object.
        /// </summary>
        /// <param name="createdObj">Created object</param>
        /// <param name="explicitDependencies">Explicitly specified dependencies of created object.</param>
        /// <typeparam name="T">The type of real object that should be created.</typeparam>
        /// <returns>The container filled with substitutes and one specified real object.</returns>
        public static DiContainer CreateContainerFor<T>(out T createdObj, IReadOnlyDictionary<Type, object> explicitDependencies)
        {
            var type = typeof(T);
            var container = new DiContainer();
            var constructorInfos = type.GetConstructors(
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (constructorInfos.Length != 1)
            {
                throw new InvalidOperationException(
                    "Can not determine what constructor should be used for mocks creating");
            }
            foreach (var argType in constructorInfos.First().GetParameters().Select(x => x.ParameterType))
            {
                if (!explicitDependencies.ContainsKey(argType))
                {
                    // IInstantiator should be rebind because it is bind by default
                    if (argType == typeof(IInstantiator))
                    {
                        container.Rebind(argType)
                            .FromInstance(Substitute.For(new[] {argType}, Array.Empty<object>()))
                            .AsSingle();
                    }
                    else
                    {
                        container.Bind(argType)
                            .FromInstance(Substitute.For(new[] {argType}, Array.Empty<object>()))
                            .AsSingle();
                    }
                }
                else
                {
                    container.Bind(argType)
                        .FromInstance(explicitDependencies[argType])
                        .AsSingle();
                }
            }
            container.Bind<T>().ToSelf().AsSingle();
            createdObj = container.Resolve<T>();
            return container;
        }
    }
}