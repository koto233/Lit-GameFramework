using UnityEngine;

namespace Framework
{
    public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        // 单例实例
        private static T _instance;

        /// <summary>
        /// 取单例实例。如果场景中没有该实例，会自动创建一个新的 GameObject 挂载该组件
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<T>();
                    if (_instance == null)
                    {
                        var singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = typeof(T).Name;
                    }
                }
                return _instance;
            }
        }

        // <summary>
        ///在 Awake 中检查是否已有实例存在，保证单例唯一性
        //I </summary>
        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else
            {
                Destroy(this);
            }
        }
    }
}