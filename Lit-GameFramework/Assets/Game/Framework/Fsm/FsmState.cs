using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class FsmState<T> where T : class
    {
        /// <summary>
        /// 状态订阅的事件字典
        /// </summary>
        private readonly Dictionary<int, FsmEventHandler<T>> _eventHandlers;

        public FsmState()
        {
            _eventHandlers = new Dictionary<int, FsmEventHandler<T>>();
        }

        #region 事件

        /// <summary>
        /// 订阅状态机事件。
        /// </summary>
        protected void SubscribeEvent(int eventId, FsmEventHandler<T> eventHandler)
        {
            if (eventHandler == null)
            {
                Debug.LogError("状态机事件响应方法为空，无法订阅状态机事件");
            }

            if (!_eventHandlers.TryAdd(eventId, eventHandler))
            {
                _eventHandlers[eventId] += eventHandler;
            }
        }

        /// <summary>
        /// 取消订阅状态机事件。
        /// </summary>
        protected void UnsubscribeEvent(int eventId, FsmEventHandler<T> eventHandler)
        {
            if (eventHandler == null)
            {
                Debug.LogError("状态机事件响应方法为空，无法取消订阅状态机事件");
            }

            if (_eventHandlers.ContainsKey(eventId))
            {
                _eventHandlers[eventId] -= eventHandler;
            }
        }

        /// <summary>
        /// 响应状态机事件。
        /// </summary>
        public void OnEvent(Fsm<T> fsm, object sender, int eventId, object userData)
        {
            if (_eventHandlers.TryGetValue(eventId, out var eventHandlers) && eventHandlers != null)
            {
                eventHandlers(fsm, sender, userData);
            }
        }

        #endregion

        /// <summary>
        /// 切换状态
        /// </summary>
        protected void ChangeState<TState>(Fsm<T> fsm) where TState : FsmState<T>
        {
            ChangeState(fsm, typeof(TState));
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        protected void ChangeState(Fsm<T> fsm, Type type)
        {
            if (fsm == null)
            {
                Debug.Log("需要切换状态的状态机为空，无法切换");
                return;
            }

            if (type == null)
            {
                Debug.Log("需要切换到的状态为空，无法切换");
                return;
            }

            if (!typeof(FsmState<T>).IsAssignableFrom(type))
            {
                Debug.Log("要切换的状态没有直接或间接实现FsmState<T>，无法切换");
            }

            fsm.ChangeState(type);
        }

        #region 生命周期

        /// <summary>
        /// 状态机状态初始化时调用
        /// </summary>
        /// <param name="fsm">状态机引用</param>
        public virtual void OnInit(Fsm<T> fsm)
        {
        }

        /// <summary>
        /// 状态机状态进入时调用
        /// </summary>
        /// <param name="fsm">状态机引用</param>
        public virtual void OnEnter(Fsm<T> fsm)
        {
        }

        /// <summary>
        /// 状态机状态轮询时调用
        /// </summary>
        /// <param name="fsm">状态机引用</param>
        public virtual void OnUpdate(Fsm<T> fsm, float elapseSeconds, float realElapseSeconds)
        {
        }

        /// <summary>
        /// 状态机状态离开时调用。
        /// </summary>
        /// <param name="fsm">状态机引用。</param>
        /// <param name="isShutdown">是关闭状态机时触发</param>
        public virtual void OnLeave(Fsm<T> fsm, bool isShutdown)
        {
        }

        /// <summary>
        /// 状态机状态销毁时调用
        /// </summary>
        /// <param name="fsm">状态机引用。</param>
        public virtual void OnDestroy(Fsm<T> fsm)
        {
            _eventHandlers.Clear();
        }

        #endregion
    }

    /// <summary>
    /// 状态机事件的响应方法模板
    /// </summary>
    public delegate void FsmEventHandler<T>(Fsm<T> fsm, object sender, object userData) where T : class;
}