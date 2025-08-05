using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 模块管理器基类
    /// </summary>
    public abstract class ManagerBase
    {
        /// <summary>
        /// 模块优先级，优先级高的模块会先被轮询，并且后关闭
        /// </summary>
        public virtual int Priority => 0;

        /// <summary>
        /// 初始化模块
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// 轮询模块
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝秒</param>
        /// <param name="realElapseSeconds">现实流逝秒</param>
        public abstract void Update(float elapseSeconds, float realElapseSeconds);

        /// <summary>
        /// 关闭并清理模块
        /// </summary>
        public abstract void Shutdown();
    }
}