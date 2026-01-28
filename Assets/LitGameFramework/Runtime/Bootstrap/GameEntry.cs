using LitGameFramework.UI.Core.Management;
using UnityEngine;
using LitGameFramework.UIFramework.Core.Factory;
using LitGameFramework.UIFramework.Core.Router;
using LitGameFramework.UIFramework.Core.Service;
namespace LitGameFramework.Core
{
    /// <summary>
    /// 游戏入口
    /// </summary>
    public class GameEntry : MonoBehaviour
    {
        void Start()
        {
            Register(GameRoot.I.GlobalScope);
        }

        private void Register(LifetimeScope scope)
        {
            Debug.Log("注册全局服务");
            // TODO: 注册全局服务
            scope.RegisterSingleton(() => new UIManager(scope.Resolve<IPanelFactory>(), scope.Resolve<IUIRouter>(), scope.Resolve<IUILayerService>()));
        }


    }
}