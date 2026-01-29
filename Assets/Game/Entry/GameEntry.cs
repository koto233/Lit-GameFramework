using LitGameFramework.Core;
using LitGameFramework.Entry;
using UnityEngine;

/// <summary>
/// 游戏入口
/// </summary>
public class GameEntry : EntryBase
{
    protected override void Register(LifetimeScope scope)
    {
        Debug.Log("注册全局服务");
        // TODO: 注册全局服务
    }

    protected override void Initialize(LifetimeScope scope)
    {
        Debug.Log("初始化全局服务");
    }
}
