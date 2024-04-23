using UnityEngine;

namespace SMTD.Templalets.DesingPatterns.DependensyInversion.DependencyInjection.Demo
{
    public class ClassB:MonoBehaviour {
    
        [Inject]ServiceA serviceA;
        [Inject]ServiceB serviceB;
        FactoryA factoryA;

        [Inject]
        public void Init(FactoryA factoryA)
        {
            this.factoryA = factoryA;
        }
        private void Start()
        {
            serviceA.Initiliaze("ServiceA initiliaized from ClassB");
            serviceB.Initiliaze("ServiceB initiliaized from ClassB");
            factoryA.CreateServiceA().Initiliaze("Service initilaized from FactoryA");
        }
    }
}
