using UnityEngine;

namespace SMTD.Templalets.DesingPatterns.DependensyInversion.DependencyInjection
{
    public class Provider:MonoBehaviour, IDependencyProvider {

        [Provide]
        public ServiceA ProviderServideA (){ return new ServiceA (); }
        [Provide]
        public ServiceB ProviderServideB() {  return new ServiceB (); }
        [Provide]
        public FactoryA FactoryServideA() { return new FactoryA ();}


    }
    
    public class ServiceA {
        public void Initiliaze(string message = null)
        {
            Debug.Log($"ServiceA.Initilaize({message})");
        }
    }    

    public class ServiceB {
        public void Initiliaze(string message = null)
        {
            Debug.Log($"ServiceB.Initilaize({message})");
        }
    }
    
    public class FactoryA
    {
        ServiceA cachedServiceA;

        public ServiceA CreateServiceA()
        {
            if ( cachedServiceA == null )
            {
                cachedServiceA = new ServiceA ();
            }
            return cachedServiceA;
        }
    }
}
