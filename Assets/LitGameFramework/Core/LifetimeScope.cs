using System;
using System.Collections.Generic;

namespace LitGameFramework.Core
{
    public sealed class LifetimeScope : IResolver, IDisposable
    {
        private readonly Dictionary<Type, object> _instances = new();
        private bool _disposed;
        private readonly LifetimeScope _parent;
        public LifetimeScope(LifetimeScope parent = null) => _parent = parent;
        public void Register<T>(T instance) where T : class
        {
            _instances[typeof(T)] = instance;
        }

        public T Resolve<T>() where T : class
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(LifetimeScope));

            if (_instances.TryGetValue(typeof(T), out var obj))
                return obj as T;
            if (_parent != null)
            {
                return _parent.Resolve<T>();
            }
            throw new InvalidOperationException($"{typeof(T)} 未注册");
        }

        internal bool TryResolve<T>(out T instance) where T : class
        {
            if (_instances.TryGetValue(typeof(T), out var obj))
            {
                instance = obj as T;
                return true;
            }
            if (_parent != null)
            {
                return _parent.TryResolve(out instance);
            }
            instance = null;
            return false;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            foreach (var obj in _instances.Values)
                if (obj is IDisposable d) d.Dispose();

            _instances.Clear();
        }
    }

}
