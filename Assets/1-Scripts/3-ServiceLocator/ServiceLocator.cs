using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
namespace SMTD.Templalets.DesingPatterns.DependencyInversion.ServiceLocator
{
    public class ServiceLocator : MonoBehaviour
    {
        static ServiceLocator global;
        static Dictionary<Scene, ServiceLocator> sceneContainers;

        readonly ServiceManager services = new();

        const string k_globalServiceLocatorName = "Service Locator [Gloabal]";
        const string k_sceneServiceLocatorName = "Service Locator [Scene]";
        public static ServiceLocator Global
        {
            get
            {
                if (global != null) return global;
                //bootstrap or initilaize the new instance of global as necessary
                if(FindFirstObjectByType<ServiceLocatorGlobalBootstrapper>() is { } found)
                {
                    found.BootstrapOnDemand();
                    return global;
                }

                var container= new GameObject(k_globalServiceLocatorName, typeof(ServiceLocator));
                container.AddComponent<ServiceLocatorGlobalBootstrapper>().BootstrapOnDemand();

                return global;
            }
        }
        static List<GameObject> tmpSceneGameObjects;
        public static ServiceLocator For(MonoBehaviour mb)
        {
            return mb.GetComponentInParent<ServiceLocator>().OrNull()??ForSceneOf(mb)??Global;
        }
        public static ServiceLocator ForSceneOf(MonoBehaviour mb)
        {
            Scene scene = mb.gameObject.scene;
            if(sceneContainers.TryGetValue(scene, out ServiceLocator container)&&container!=mb)
            {
                return container;
            }

            tmpSceneGameObjects.Clear();
            scene.GetRootGameObjects(tmpSceneGameObjects);
            foreach (GameObject go in tmpSceneGameObjects.Where(go=>go.GetComponent<ServiceLocatorSceneBootstrapper>()!=null))
            {
                if(go.TryGetComponent(out ServiceLocatorSceneBootstrapper bootstrapper) && bootstrapper.Container != mb)
                {
                    bootstrapper.BootstrapOnDemand();
                    return bootstrapper.Container;
                }
            }
            return Global;
        }
        public ServiceLocator Register<T>(T service)
        {
            services.Register(service);
            return this;
        }
        public ServiceLocator Get<T>(out T service) where T : class
        {
            if (TryGetService(out service)) return this;

            if(TryGetNextInHierarchy(out ServiceLocator container)){
                container.Get(out service);
                return this;
            }
            throw new ArgumentException($"ServiceLocator.Get: Service of type{typeof(T).FullName} not registered.");
        }
        public ServiceLocator Register(Type type, object service)
        {
            services.Register(type,service);
            return this;
        }
        bool TryGetService<T>(out T service) where T: class
        {
            return services.TryGet(out service);
        }
        bool TryGetNextInHierarchy(out ServiceLocator container)
        {
            if (this == global)
            {
                container = null;
                return false;
            }
            container = transform.parent.OrNull()?.GetComponentInParent<ServiceLocator>()
                                        .OrNull() ?? ForSceneOf(this);
            return container != null;
        }
    }
    public static class Extension
    {
        public static T OrNull<T>(this T obj) where T : UnityEngine.Object => obj ? obj : null;
    }
}
