using SMTD.Templalets.DesingPatterns.Singleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SMTD.Templalets.DesingPatterns.DependensyInversion.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Field|AttributeTargets.Method)]
    public sealed class InjectAttribute : Attribute
    {
        public InjectAttribute() { }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ProvideAttribute: Attribute { 
        public ProvideAttribute() { }
    }
    public interface IDependencyProvider { }
    [DefaultExecutionOrder(-1000)]

    public class Injector : Singleton<Injector>
    {
        const BindingFlags k_bindingflags= BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        readonly Dictionary<Type,object> registry = new Dictionary<Type,object>();

        protected override void Awake()
        {
            base.Awake();  
            var providers=FindMonobehaviors().OfType<IDependencyProvider>();
            foreach (var provider in providers)
            {
                RegisterProvider(provider);
            }

            //find all injectable objecys and inject their dependencies
            var injectables = FindMonobehaviors().Where(IsInjectable);
            foreach (var injectable in injectables)
            {
                Inject(injectable);
            }
        }

        private void Inject(object instance)
        {
            var type  = instance.GetType();
            var injectableFields = type.GetFields(k_bindingflags).Where(member=>Attribute.IsDefined(member,typeof(InjectAttribute)));

            foreach (var injectableField in injectableFields)
            {
                var fieldType = injectableField.FieldType;
                var resolvedInstance = Resolve(fieldType);
                if (resolvedInstance == null)
                {
                    throw new Exception($"Failed to resolve {fieldType.Name} for {type.Name}");
                }
                injectableField.SetValue(instance, resolvedInstance);
                Debug.Log($"Field Injected {fieldType.Name} into {type.Name}");
            }

            var injectableMethods = type.GetMethods(k_bindingflags).Where(member=>Attribute.IsDefined(member,typeof(InjectAttribute)));

            foreach (var injectableMethod in injectableMethods)
            {
                var requiredParameterts = injectableMethod.GetParameters().Select(parameter => parameter.ParameterType).ToArray();
                var resolvedInstances = requiredParameterts.Select(Resolve).ToArray();
                if (resolvedInstances.Any(resolvedInstances => resolvedInstances == null))
                {
                    throw new Exception($"Failed to inject {type.Name} for {injectableMethod.Name}");
                }
                injectableMethod.Invoke(instance, resolvedInstances);
                Debug.Log($"Method Injected {injectableMethod.Name} into {type.Name}");
            }
        }
        object Resolve(Type type)
        {
            registry.TryGetValue(type, out object resolvedInstance);
            return resolvedInstance;
        }
        static bool IsInjectable(MonoBehaviour obj)
        {
            var members = obj.GetType().GetMembers(k_bindingflags);
            return members.Any(member=>Attribute.IsDefined(member,typeof(InjectAttribute)));
        }
        private void RegisterProvider(IDependencyProvider provider)
        {
            var methods = provider.GetType().GetMethods(k_bindingflags);
            foreach (var method in methods)
            {
                if (!Attribute.IsDefined(method, typeof(ProvideAttribute))) continue;
                var returnType = method.ReturnType;
                var providedInstance = method.Invoke(provider, null);
                if(providedInstance != null)
                {
                    registry.Add(returnType, providedInstance);
                    Debug.Log($"Registered {returnType.Name} from {provider.GetType().Name}");
                }
                else
                {
                    throw new Exception($"Provider{provider.GetType().Name} returned null for {returnType.Name}");
                }
            }
        }

        static MonoBehaviour[] FindMonobehaviors()
        {
            return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.InstanceID);
        }
    }
}
