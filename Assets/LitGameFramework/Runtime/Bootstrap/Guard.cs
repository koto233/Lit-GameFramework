/// <summary>
/// 必须用 Guard 的地方
// Controller 构造函数
// System 构造函数
// Scope.Get
// Bind / Init
// Dispose 前状态校验
// 禁止用 Guard 的地方
// 点击按钮
// 签到 / 商店 / 战斗逻辑
// UI 刷新
// 玩家输入相关
/// </summary>
public static class Guard
{
    public static void NotNull(object obj, string name)
    {
#if UNITY_EDITOR 
        if (obj == null)
            throw new System.ArgumentNullException(name);
#endif
    }

    public static void IsTrue(bool condition, string message)
    {
#if UNITY_EDITOR 
        if (!condition)
            throw new System.Exception(message);
#endif
    }
}
