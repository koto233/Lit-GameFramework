using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class FsmManager : ManagerBase
    {
        /// <summary>
        /// 所有状态机的字典（在这里，状态机接口的作用就显示出来了）
        /// </summary>
        private Dictionary<string, IFsm> _fsms;

        private List<IFsm> _tempFsms;
        public override int Priority => 60;

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Init()
        {
            _tempFsms = new List<IFsm>();
            _fsms = new Dictionary<string, IFsm>();
        }


        /// <summary>
        /// 轮询状态机管理器
        /// </summary>
        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            _tempFsms.Clear();
            if (_fsms.Count <= 0)
            {
                return;
            }

            foreach (var fsm in _fsms)
            {
                _tempFsms.Add(fsm.Value);
            }

            foreach (var fsm in _tempFsms)
            {
                if (fsm.IsDestroyed)
                {
                    continue;
                }

                fsm.Update(elapseSeconds, realElapseSeconds);
            }
        }

        /// <summary>
        /// 创建状态机。
        /// </summary>
        /// <typeparam name="T">状态机持有者类型</typeparam>
        /// <param name="name">状态机名称</param>
        /// <param name="owner">状态机持有者</param>
        /// <param name="states">状态机状态集合</param>
        /// <returns>要创建的状态机</returns>
        public Fsm<T> CreateFsm<T>(T owner, string name = "", params FsmState<T>[] states) where T : class
        {
            if (HasFsm<T>())
            {
                Debug.LogError("要创建的状态机已存在");
                return null;
            }

            if (string.IsNullOrEmpty(name))
            {
                name = typeof(T).FullName;
            }

            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("要创建的状态机未找到名称");
                return null;
            }

            var fsm = new Fsm<T>(name, owner, states);
            _fsms.Add(name, fsm);
            return fsm;
        }

        /// <summary>
        /// 是否存在状态机
        /// </summary>
        private bool HasFsm(string fullName)
        {
            return _fsms.ContainsKey(fullName);
        }

        /// <summary>
        /// 是否存在状态机
        /// </summary>
        public bool HasFsm<T>()
        {
            return HasFsm(typeof(T));
        }

        /// <summary>
        /// 是否存在状态机
        /// </summary>
        public bool HasFsm(Type type)
        {
            return HasFsm(type.FullName);
        }

        /// <summary>
        /// 销毁状态机
        /// </summary>
        public bool DestroyFsm(string name)
        {
            if (_fsms.TryGetValue(name, out var fsm))
            {
                fsm.Shutdown();
                return _fsms.Remove(name);
            }

            return false;
        }

        public bool DestroyFsm<T>() where T : class
        {
            return DestroyFsm(typeof(T).FullName);
        }

        public bool DestroyFsm(IFsm fsm)
        {
            return DestroyFsm(fsm.Name);
        }

        /// <summary>
        /// 关闭并清理状态机管理器
        /// </summary>
        public override void Shutdown()
        {
            foreach (var fsm in _fsms)
            {
                fsm.Value.Shutdown();
            }

            _fsms.Clear();

            _tempFsms.Clear();
        }
    }
}