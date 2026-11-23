using System;
using UnityEngine;

public abstract class Service : MonoBehaviour
{
    protected void RegisterToServiceLocator<T>(T instance)
    {
        ServiceLocator.Instance.Register<T>(instance);
    }
    protected void UnregisterFromServiceLocator<T>()
    {
        ServiceLocator.Instance.Unregister<T>();
    }
}
