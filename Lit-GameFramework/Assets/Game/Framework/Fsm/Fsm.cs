using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class Fsm<T> : IFsm where T : class
    {
        /// <summary>
        /// 状态机名字
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 获取状态机持有者类型
        /// </summary>
        public Type OwnerType
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// 状态机是否被销毁
        /// </summary>
        public bool IsDestroyed { get; private set; }

        /// <summary>
        /// 当前状态运行时间
        /// </summary>
        public float CurrentStateTime { get; private set; }

        /// <summary>
        /// 状态机里所有状态的字典
        /// </summary>
        private Dictionary<string, FsmState<T>> _states;

        /// <summary>
        /// 状态机里所有数据的字典
        /// </summary>
        private Dictionary<string, object> _data;

        /// <summary>
        /// 当前状态
        /// </summary>
        public FsmState<T> CurrentState { get; private set; }

        /// <summary>
        /// 状态机持有者
        /// </summary>
        public T Owner { get; private set; }

        #region 构造

        public Fsm(string name, T owner, params FsmState<T>[] states)
        {
            if (owner == null)
            {
                Debug.LogError("状态机持有者为空");
                return;
            }

            if (states == null || states.Length == 0)
            {
                Debug.LogError("没有要添加进状态机的状态");
                return;
            }

            Name = name;
            Owner = owner;
            _states = new Dictionary<string, FsmState<T>>();
            _data = new Dictionary<string, object>();
            foreach (var state in states)
            {
                if (state == null)
                {
                    Debug.LogError("待添加进状态机的状态为空");
                    continue;
                }

                var stateName = state.GetType().Name;
                if (!_states.TryAdd(stateName, state))
                {
                    Debug.LogError("要添加进状态机的状态已存在：" + stateName);
                    continue;
                }

                state.OnInit(this);
            }
        }

        #endregion

        /// <summary>
        /// 开始状态机
        /// </summary>
        /// <typeparam name="TState">开始的状态类型</typeparam>
        public void Start<TState>() where TState : FsmState<T>
        {
            Start(typeof(TState));
        }

        /// <summary>
        /// 开始状态机
        /// </summary>
        /// <param name="stateType">要开始的状态类型。</param>
        private void Start(Type stateType)
        {
            if (CurrentState != null)
            {
                Debug.LogError("当前状态机已开始，无法再次开始");
                return;
            }

            if (stateType == null)
            {
                Debug.LogError("要开始的状态为空，无法开始");
                return;
            }

            var state = GetState(stateType);
            if (state == null)
            {
                Debug.Log("获取到的状态为空，无法开始");
                return;
            }

            CurrentStateTime = 0f;
            CurrentState = state;
            CurrentState.OnEnter(this);
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        public TState GetState<TState>() where TState : FsmState<T>
        {
            return GetState(typeof(TState)) as TState;
        }

        private FsmState<T> GetState(Type stateType)
        {
            if (stateType == null)
            {
                Debug.LogError("要获取的状态为空");
                return null;
            }

            if (!typeof(FsmState<T>).IsAssignableFrom(stateType))
            {
                Debug.LogError("要获取的状态" + stateType.FullName + "没有直接或间接的实现" + typeof(FsmState<T>).FullName);
            }


            return _states.GetValueOrDefault(stateType.FullName);
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        public void ChangeState<TState>() where TState : FsmState<T>
        {
            ChangeState(typeof(TState));
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        public void ChangeState(Type type)
        {
            if (CurrentState == null)
            {
                Debug.LogError("当前状态机状态为空，无法切换状态");
                return;
            }

            var state = GetState(type);
            if (state == null)
            {
                Debug.Log("获取到的状态为空，无法切换：" + type.FullName);
                return;
            }

            CurrentState.OnLeave(this, false);
            CurrentStateTime = 0f;
            CurrentState = state;
            CurrentState.OnEnter(this);
        }

        /// <summary>
        /// 抛出状态机事件
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="eventId">事件编号</param>
        public void FireEvent(object sender, int eventId)
        {
            if (CurrentState == null)
            {
                Debug.Log("当前状态为空，无法抛出事件");
                return;
            }

            CurrentState.OnEvent(this, sender, eventId, null);
        }

        /// <summary>
        /// 关闭并清理状态机。
        /// </summary>
        public void Shutdown()
        {
            if (CurrentState != null)
            {
                CurrentState.OnLeave(this, true);
                CurrentState = null;
                CurrentStateTime = 0;
            }

            foreach (var stat in _states)
            {
                stat.Value.OnDestroy(this);
            }

            _states.Clear();
            _data.Clear();
            IsDestroyed = true;
        }

        /// <summary>
        /// 轮询状态机
        /// </summary>
        /// <param name="elapseSeconds"></param>
        /// <param name="realElapseSeconds"></param>
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (CurrentState == null)
            {
                return;
            }

            CurrentStateTime += elapseSeconds;
            CurrentState.OnUpdate(this, elapseSeconds, realElapseSeconds);
        }

        /// <summary>
        /// 是否存在状态机数据
        /// </summary>
        public bool HasData(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.Log("要查询的状态机数据名字为空");
                return false;
            }

            return _data.ContainsKey(name);
        }

        /// <summary>
        /// 获取状态机数据
        /// </summary>
        public TDate GetData<TDate>(string name)
        {
            return (TDate)GetData(name);
        }

        /// <summary>
        /// 获取状态机数据
        /// </summary>
        private object GetData(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.Log("要获取的状态机数据名字为空");
                return null;
            }

            return _data.GetValueOrDefault(name);
        }

        /// <summary>
        /// 设置状态机数据
        /// </summary>
        public void SetData(string name, object data)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.Log("要设置的状态机数据名字为空");
                return;
            }

            _data[name] = data;
        }

        /// <summary>
        /// 移除状态机数据
        /// </summary>
        public bool RemoveData(string name)
        {
            if (string.IsNullOrEmpty(name))
            { 
                Debug.Log("要移除的状态机数据名字为空");
                return false;
            }

            return _data.Remove(name);
        }
    }
}