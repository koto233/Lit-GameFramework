namespace LitGameFramework.Core
{
    /// <summary>
    /// DI 中枢：
    /// - 持有 Global / Scene Scope
    /// - 提供统一 Resolve
    /// - 不暴露 Scope 本身
    /// </summary>
    internal sealed class ResolverHub : IResolver
    {
        private readonly LifetimeScope _globalScope;
        private LifetimeScope _sceneScope;

        public ResolverHub()
        {
            _globalScope = new LifetimeScope();
        }

        #region Composition API（只给 Root 用）

        internal LifetimeScope GlobalScope => _globalScope;
        internal LifetimeScope SceneScope => _sceneScope;

        internal void BeginSceneScope()
        {
            _sceneScope?.Dispose();
            _sceneScope = new LifetimeScope(_globalScope);
        }

        internal void EndSceneScope()
        {
            _sceneScope?.Dispose();
            _sceneScope = null;
        }
        internal void EndAllScope()
        {
            _sceneScope?.Dispose();
            _sceneScope = null;
            _globalScope.Dispose();
        }
        #endregion

        #region Resolve

        public T Resolve<T>() where T : class
        {
            if (_sceneScope != null)
                return _sceneScope.Resolve<T>();
            return _globalScope.Resolve<T>();
        }

        internal bool TryResolve<T>(out T instance) where T : class
        {
            if (_sceneScope != null)
                return _sceneScope.TryResolve(out instance);
            return _globalScope.TryResolve(out instance);
        }

        #endregion
    }
}
