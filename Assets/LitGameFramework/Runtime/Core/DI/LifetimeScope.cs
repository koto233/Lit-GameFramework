using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LitGameFramework.Core
{
    /// <summary>
    /// LifetimeScope 是一个轻量级依赖注入容器（DI 容器）。
    /// 功能：
    /// 1. 管理全局或场景服务的生命周期
    /// 2. 支持单例服务（立即创建或延迟创建）
    /// 3. 支持瞬态服务（每次 Resolve 都创建新实例）
    /// 4. 支持构造函数自动注入（Constructor Injection）
    /// 5. 可 Dispose 清理 IDisposable 服务
    /// </summary>
    public sealed class LifetimeScope : IDisposable
    {
        // 字典：类型 -> 工厂方法
        // 工厂方法用于创建对象（单例或瞬态）
        private readonly Dictionary<Type, Func<object>> _factories = new();

        // 字典：类型 -> 已创建的单例实例
        private readonly Dictionary<Type, object> _singletons = new();

        // 多线程安全锁
        private readonly object _lock = new();

        // ------------------- 注册方法 -------------------

        /// <summary>
        /// 注册一个已经存在的实例（立即单例）。
        /// Resolve 时总是返回同一个实例。
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="instance">已有实例</param>
        public void RegisterInstance<TService>(TService instance) where TService : class
        {
            var t = typeof(TService);
            lock (_lock)
            {
                // 保存实例到单例字典
                _singletons[t] = instance;

                // 工厂方法直接返回该实例
                _factories[t] = () => _singletons[t];
            }
        }

        /// <summary>
        /// 注册一个延迟单例（Singleton）。
        /// 第一次 Resolve 时调用 factory 创建实例，之后重复使用。
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="factory">实例工厂方法</param>
        public void RegisterSingleton<TService>(Func<TService> factory) where TService : class
        {
            var t = typeof(TService);
            lock (_lock)
            {
                // 工厂方法封装延迟创建逻辑
                _factories[t] = () =>
                {
                    if (!_singletons.TryGetValue(t, out var inst))
                    {
                        // 第一次 Resolve 时创建实例
                        inst = factory();
                        _singletons[t] = inst;
                    }
                    return _singletons[t];
                };
            }
        }

        /// <summary>
        /// 注册瞬态服务（Transient）。
        /// 每次 Resolve 都创建新实例，不保存到单例字典。
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="factory">实例工厂方法</param>
        public void RegisterTransient<TService>(Func<TService> factory) where TService : class
        {
            var t = typeof(TService);
            lock (_lock)
            {
                // 工厂方法每次调用都会创建新实例
                _factories[t] = () => factory();
            }
        }

        // ------------------- Resolve 方法 -------------------

        /// <summary>
        /// 泛型方式获取服务实例。
        /// 如果没有注册，会尝试自动创建（Constructor Injection）。
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <returns>实例</returns>
        public T Resolve<T>() where T : class
        {
            return Resolve(typeof(T)) as T;
        }

        /// <summary>
        /// 非泛型方式获取服务实例。
        /// 优先使用已注册的工厂方法或单例，
        /// 没注册则尝试自动创建（具体类型才行）。
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <returns>实例对象</returns>
        private object Resolve(Type serviceType)
        {
            lock (_lock)
            {
                // 如果注册了工厂方法，直接调用
                if (_factories.TryGetValue(serviceType, out var fact))
                {
                    return fact();
                }
            }

            // 没有注册
            // 如果是具体类（非抽象、非接口），尝试构造函数注入自动创建
            if (!serviceType.IsAbstract && !serviceType.IsInterface)
            {
                return CreateWithConstructorInjection(serviceType);
            }

            // 既没有注册，也无法自动创建，抛出异常
            throw new InvalidOperationException($"Service not registered: {serviceType}");
        }

        /// <summary>
        /// 尝试获取服务实例，不抛异常。
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <param name="instance">输出实例</param>
        /// <returns>是否成功获取</returns>
        public bool TryResolve<T>(out T instance) where T : class
        {
            try
            {
                instance = Resolve<T>();
                return instance != null;
            }
            catch
            {
                instance = null;
                return false;
            }
        }

        // ------------------- 构造函数注入 -------------------

        /// <summary>
        /// 自动构造对象，使用构造函数注入依赖。
        /// 选用参数最多的公共构造函数。
        /// </summary>
        /// <param name="implType">实现类型</param>
        /// <returns>实例对象</returns>
        private object CreateWithConstructorInjection(Type implType)
        {
            // 找所有公共构造函数
            var ctors = implType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            // 选参数最多的构造函数
            var ctor = ctors.OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();
            if (ctor == null)
                throw new InvalidOperationException($"No public ctor for {implType}");

            // 递归 Resolve 构造参数
            var args = ctor.GetParameters()
                           .Select(p => Resolve(p.ParameterType))
                           .ToArray();

            // 创建实例
            var instance = Activator.CreateInstance(implType, args);

            // 可选：如果实现 IDisposable，可在 Dispose 时清理（单例会存储）
            return instance;
        }

        // ------------------- Dispose -------------------

        /// <summary>
        /// 释放所有单例服务，如果实现 IDisposable，会调用 Dispose。
        /// 并清空注册的工厂方法和单例字典。
        /// </summary>
        public void Dispose()
        {
            foreach (var obj in _singletons.Values)
            {
                if (obj is IDisposable d) d.Dispose();
            }

            _singletons.Clear();
            _factories.Clear();
        }
    }
}
