using System.Linq;
using UnityEngine;
namespace UIFramework.Core.Panel
{
    public abstract class UIBase : MonoBehaviour
    {

        #region 自动绑定字段
        private UIBind[] _binds;
        [Header("自动绑定脚本路径")]
        private string _pathPrefix = "Assets/";
        [SerializeField]
        private string _generatedScriptPath = $"UI/Generated";

        public string GeneratedScriptPath
            => string.IsNullOrEmpty(_generatedScriptPath)
                ? "Assets/UI/Generated"
                : _pathPrefix + _generatedScriptPath;
        #endregion

        public bool IsShown { get; private set; }


        protected virtual void Awake()
        {
            CacheBinds();
            GetUI(); // 子类 partial 中生成
        }
        internal void ShowInternal(object param)
        {
            IsShown = true;
            OnShowInternal(param);
        }

        internal void HideInternal()
        {
            IsShown = false;
            OnHide();
        }

        protected abstract void OnShowInternal(object param);
        protected virtual void OnHide() { }


        #region 自动绑定方法
        void CacheBinds()
        {
            _binds = GetComponentsInChildren<UIBind>(true)
            .OrderBy(b => b.Index)
            .ToArray();
        }

        protected T GetBind<T>(int index) where T : Component
        {
            if (_binds == null || index < 0 || index >= _binds.Length)
            {
                Debug.LogError($"[UIBase] Bind index invalid: {index}", this);
                return null;
            }

            var bind = _binds[index];
            var target = bind.Target;

            if (target == null)
            {
                Debug.LogError(
                    $"[UIBase] Bind index {index} ({bind.name}) has no target component",
                    bind
                );
                return null;
            }

            if (target is T t)
                return t;

            Debug.LogError(
                $"[UIBase] Component {typeof(T).Name} not found on bind index {index}, " +
                $"actual type is {target.GetType().Name}",
                bind
            );
            return null;
        }

        /// <summary>
        /// Editor 生成的 partial 会 override
        /// </summary>
        protected virtual void GetUI() { }

        #endregion


    }
}