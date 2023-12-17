using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace grate.Infrastructure;

public class Factory : IFactory
{
    private readonly IServiceProvider _provider;
    private readonly Dictionary<object, Type> _services = new();

    public Factory(IServiceProvider provider)
    {
        _provider = provider;
    }

    public void AddService<TKey>(TKey name, Type service)
    {
        if (name == null) throw new ArgumentNullException(nameof(name));
        _services.Add(name, service);
    }

    public void AddService<TKey, TValue>(TKey name)
    {
        if (name == null) throw new ArgumentNullException(nameof(name));
        _services.Add(name, typeof(TValue));
    }

    public TValue GetService<TKey, TValue>(TKey key)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        return (TValue)_provider.GetRequiredService(_services[key]);
    }
}
