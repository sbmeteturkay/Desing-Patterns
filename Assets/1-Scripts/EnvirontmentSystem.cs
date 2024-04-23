using UnityEngine;

namespace SMTD.Templalets.DesingPatterns.DependensyInversion.DependencyInjection.Demo
{ 
    public interface IEnvirontmentSystem
    {
        void Initialize();
        IEnvirontmentSystem ProvideEnvironmentSystem();
    }
    public class EnvirontmentSystem:MonoBehaviour,IDependencyProvider, IEnvirontmentSystem
    {
        [Provide]
        EnvirontmentSystem ProvideEnvironmentSystem() { return this; }
        public void Initialize()
        {
            Debug.Log("EnvironmentSystem.Initialize()");

        }

        IEnvirontmentSystem IEnvirontmentSystem.ProvideEnvironmentSystem()
        {
            throw new System.NotImplementedException();
        }
    }
}
