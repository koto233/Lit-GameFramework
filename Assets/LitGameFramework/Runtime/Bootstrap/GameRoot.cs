using UnityEngine;
using UnityEngine.SceneManagement;

namespace LitGameFramework.Core
{
    /// <summary>
    /// 游戏根节点：
    /// - 提供全局生命周期 Scope
    /// - 管理当前场景的生命周期 Scope
    /// - 作为 Composition Root（对象创建入口）
    /// </summary>
    public sealed class GameRoot : MonoBehaviour
    {
        public static GameRoot I { get; private set; }

        /// <summary>
        /// 跨场景常驻服务（UI、配置、网络等）
        /// </summary>
        internal LifetimeScope GlobalScope { get; private set; }

        /// <summary>
        /// 当前场景级服务（Controller、Scene System 等）
        /// </summary>
        internal LifetimeScope CurrentSceneScope { get; private set; }

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

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;

            CurrentSceneScope?.Dispose();
            GlobalScope?.Dispose();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            CurrentSceneScope?.Dispose();
            CurrentSceneScope = new LifetimeScope();
        }

        private void OnSceneUnloaded(Scene scene)
        {
            CurrentSceneScope?.Dispose();
            CurrentSceneScope = null;
        }

        /// <summary>
        /// 统一入口（不直接暴露 Scope）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public T Get<T>() where T : class
        {
            if (CurrentSceneScope != null && CurrentSceneScope.TryResolve<T>(out var sceneService))
                return sceneService;

            return GlobalScope.Resolve<T>();
        }
    }
}
// UI / Button / View 里（可以偷懒）
// GameRoot.I.Get<UIManager>();


// 这是允许的，别纠结。

// Manager / 系统代码里（别偷懒）
// public UIManager(Config config) { }


// 依赖写在构造函数里，不要直接去拿

// 哪些地方“绝对别乱用”？

// ❌ 在 Manager / 系统里写：

// GameRoot.I.Get<XXX>();


// 看到这种，说明结构开始乱了。
// [ Installer / Entry ]
//         │
//         │  （这里负责创建 & 组装）
//         ▼
// [ GlobalScope / SceneScope ]
//         │
//         │  构造函数传依赖
//         ▼
// [ Manager / Controller ]
//         │
//         │  提供高层接口
//         ▼
// [ UI / Button / View ]
//         │
//         │  可以直接用
//         ▼
//  GameRoot.I.Get<T>()
