// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using Aveva.ApplicationFramework;

namespace PmlUnit
{
    interface ServiceProvider
    {
        T GetService<T>() where T : class;
    }

    class ProxyServiceProvider : ServiceProvider
    {
        private readonly IServiceProvider Provider;

        public ProxyServiceProvider(IServiceProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            Provider = provider;
        }

        public T GetService<T>() where T : class
        {
            return (T)Provider.GetService(typeof(T));
        }
    }

#if E3D_21
    class DependencyResolverServiceProvider : ServiceProvider
    {
        private readonly IDependencyResolver Resolver;

        public DependencyResolverServiceProvider(IDependencyResolver resolver)
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));
            Resolver = resolver;
        }

        public T GetService<T>() where T : class
        {
            return Resolver.GetImplementationOf<T>();
        }
    }
#endif
}
