using UnityEngine;
namespace SMTD.Templalets.DesingPatterns.DependencyInversion.ServiceLocator
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ServiceLocator))]
    public abstract class Bootstrapper : MonoBehaviour
    {
        ServiceLocator container;
        internal ServiceLocator Container => container.OrNull() ?? container ?? (container = GetComponent<ServiceLocator>());

        bool hasBeenBootstrapped;

        private void Awake()
        {
            BootstrapOnDemand();
        }
        public void BootstrapOnDemand()
        {
            if (hasBeenBootstrapped) return;
            hasBeenBootstrapped = true;
            Bootsrap();
        }
        protected abstract void Bootsrap();
    }
    [AddComponentMenu("ServiceLocator/Service Locator Global")]
    public class ServiceLocatorGlobalBootstrapper : Bootstrapper
    {
        [SerializeField] bool dontDestroyOnload = true;
        protected override void Bootsrap()
        {
            // configure as global
        }
    }

    [AddComponentMenu("ServiceLocator/Service Locator Scene")]
    public class ServiceLocatorSceneBootstrapper : Bootstrapper
    {
        [SerializeField] bool dontDestroyOnload = true;
        protected override void Bootsrap()
        {
            // configure as scene
        }
    }
}
