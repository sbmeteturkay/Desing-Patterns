using UnityEngine;

namespace SMTD.Templalets.DesingPatterns.DependensyInversion.DependencyInjection.Demo
{
    public class ClassA:MonoBehaviour {
        ServiceA serviceA;
        [Inject] EnvirontmentSystem environtmentSystem;
        [Inject]
        public void Init(ServiceA serviceA)
        {
            this.serviceA = serviceA;
        }
        private void Start()
        {
            serviceA.Initiliaze("ServiceA initiliaized from classA");
            environtmentSystem.Initialize();
        }
    }
}
