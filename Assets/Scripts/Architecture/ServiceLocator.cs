using UnityEngine;
using System.Collections.Generic;
using System;

public class ServiceLocator : Singleton<ServiceLocator>
{
    private Dictionary<Type, object> services = new Dictionary<Type, object>();
    public void Register<T>(T serviceInstance)
    {
        if(!services.ContainsKey(typeof(T)))
        {
            services.Add(typeof(T), serviceInstance);
        }
        else
        {
            services[typeof(T)] = serviceInstance;
        }
    }

    public void Unregister<T>()
    {
        if(services.ContainsKey(typeof(T)))
        {
            services.Remove(typeof(T));
        }
    }

    public T GetService<T>()
    {
        if(services.TryGetValue(typeof(T), out object serviceObject))
        {
            return (T)serviceObject;
        }
        else
        {
            Debug.LogWarning($"Service of type {typeof(T)} has not been registered");
            return default;
        }
    }

    public bool TryGetService<T>(out T service)
    {
        if(services.TryGetValue(typeof(T), out object serviceObject))
        {
            service = (T)serviceObject;
            return true;
        }
        else
        {
            service = default;
            return false;
        }
    }
}
