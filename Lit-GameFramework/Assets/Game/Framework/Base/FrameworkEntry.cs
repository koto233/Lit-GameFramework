using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 框架入口
    /// </summary>
    public class FrameworkEntry : SingletonMono<FrameworkEntry>
    {
        private readonly LinkedList<ManagerBase> _managers = new LinkedList<ManagerBase>();

        #region 生命周期

        private void Update()
        {
            foreach (var manager in _managers)
            {
                manager.Update(Time.deltaTime, Time.unscaledDeltaTime);
            }
        }

        private void OnDestroy()
        {
            for (var current = _managers.Last; current != null; current = current.Previous)
            {
                current.Value.Shutdown();
            }

            _managers.Clear();
        }

        #endregion

        /// <summary>
        /// 获取指定类
        /// </summary>
        /// <typeparam name="T">类名</typeparam>
        /// <returns></returns>
        public T GetManager<T>() where T : ManagerBase
        {
            var managerType = typeof(T);
            foreach (var manager in _managers)
            {
                if (manager.GetType() == managerType)
                {
                    return (T)manager;
                }
            }

            return CreateManager(managerType) as T;
        }

        /// <summary>
        /// 创建模块管理器
        /// </summary>
        /// <param name="managerType">模块类名</param>
        /// <returns></returns>
        private ManagerBase CreateManager(Type managerType)
        {
            var manager = (ManagerBase)Activator.CreateInstance(managerType);
            if (manager == null)
            {
                Debug.LogError($"模块管理器创建失败 {managerType}");
                return null;
            }

            var current = _managers.First;
            while (current != null)
            {
                if (manager.Priority > current.Value.Priority)
                {
                    break;
                }

                current = current.Next;
            }

            if (current != null)
            {
                _managers.AddBefore(current, manager);
            }
            else
            {
                _managers.AddLast(manager);
            }

            manager.Init();
            return manager;
        }

        /// <summary>
        /// 移除销毁模块管理器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RemoveManager<T>() where T : ManagerBase
        {
            var managerType = typeof(T);
            var node = _managers.First;
            while (node != null)
            {
                if (node.Value.GetType() == managerType)
                {
                    node.Value.Shutdown();
                    _managers.Remove(node);
                    return;
                }

                node = node.Next;
            }
        }
    }
}