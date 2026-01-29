using LitGameFramework.Core;
using LitGameFramework.Entry;
using UnityEngine;

/// <summary>
/// 场景入口
/// </summary>
public class SceneEntry : EntryBase
{

    protected override void Register(LifetimeScope scope)
    {
        Debug.Log("注册场景服务");
        // TODO: 注册场景服务
    }

    protected override void Initialize(LifetimeScope scope)
    {
        Debug.Log("初始化场景服务");
    }
}
