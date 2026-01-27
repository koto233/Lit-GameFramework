using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LitGameFramework.Core
{
    public sealed class GameRoot : MonoBehaviour
    {
        public static GameRoot I { get; private set; }

        public LifetimeScope GlobalScope { get; private set; }
        // 管理所有场景 Scope
        private Dictionary<Scene, LifetimeScope> _sceneScopes = new();
        private void Awake()
        {
            if (I != null)
            {
                Destroy(gameObject);
                return;
            }

            I = this;
            DontDestroyOnLoad(gameObject);
            GlobalScope = new LifetimeScope();
            // 注册全局 Installer
            var installers = FindObjectsOfType<MonoInstaller>();
            foreach (var installer in installers)
            {
                if (installer.IsGlobal)
                    installer.Install(GlobalScope);
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }


        private void OnDestroy()
        {
            GlobalScope.Dispose();
            foreach (var scope in _sceneScopes.Values)
                scope.Dispose();

            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var scope = new LifetimeScope();
            _sceneScopes[scene] = scope;

            var installers = FindObjectsOfType<MonoInstaller>();
            foreach (var installer in installers)
                if (!installer.IsGlobal)
                    installer.Install(scope);
        }

        private void OnSceneUnloaded(Scene scene)
        {
            if (_sceneScopes.TryGetValue(scene, out var scope))
            {
                scope.Dispose();
                _sceneScopes.Remove(scene);
            }
        }

        // 获取当前场景 Scope
        public LifetimeScope GetSceneScope(Scene scene)
        {
            _sceneScopes.TryGetValue(scene, out var scope);
            return scope;
        }
    }
}
