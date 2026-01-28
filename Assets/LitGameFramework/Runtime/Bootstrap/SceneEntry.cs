using LitGameFramework.Core;
using UnityEngine;

public class SceneEntry : MonoBehaviour
{
    void Start()
    {
        Register(GameRoot.I.CurrentSceneScope);
    }
    public void Register(LifetimeScope scope)
    {
        Debug.Log("注册场景服务");
        // TODO: 注册场景服务

    }
}
