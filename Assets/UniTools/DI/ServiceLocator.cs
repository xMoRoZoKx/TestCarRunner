using System;
using System.Collections.Generic;

public static class ServiceLocator
{
    private readonly static Dictionary<Type, Func<object>> _registrations = new();
    private readonly static Dictionary<Type, object> _singletons = new();

    public static void RegisterSingleton<T>(T implementation = default) where T : new()
    {
        RegisterSingleton<T, T>(implementation);
    }

    public static void RegisterSingleton<TInterface, TImplementation>(TImplementation implementation = default) where TImplementation : TInterface, new()
    {
        _registrations[typeof(TInterface)] = () =>
        {
            var instance = implementation ?? new TImplementation();
            _singletons[typeof(TInterface)] = instance;
            return instance;
        };

        _singletons[typeof(TInterface)] = implementation ?? new TImplementation();
    }

    public static void RegisterTransient<T>(Func<T> onCreate = null) where T : new()
    {
        RegisterTransient<T, T>(onCreate);
    }

    public static void RegisterTransient<TInterface, TImplementation>(Func<TImplementation> onCreate = null) where TImplementation : TInterface, new()
    {
        _registrations[typeof(TInterface)] = () => onCreate == default ? new TImplementation() : onCreate.Invoke();
    }

    public static TInterface Resolve<TInterface>()
    {
        if (_registrations.TryGetValue(typeof(TInterface), out var factory))
        {
            return (TInterface)factory();
        }
        throw new InvalidOperationException($"No registration for {typeof(TInterface)}");
    }
}
