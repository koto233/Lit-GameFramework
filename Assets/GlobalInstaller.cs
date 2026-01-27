using LitGameFramework.UI.Core.Management;
using UnityEngine;
using LitGameFramework.UIFramework.Core.Factory;
using LitGameFramework.UIFramework.Core.Router;
using LitGameFramework.UIFramework.Core.Service;
namespace LitGameFramework.Core
{
    public class GlobalInstaller : MonoInstaller
    {
        public override bool IsGlobal => true;

        public override void Install(LifetimeScope scope)
        {
            Debug.Log("注册");

            scope.RegisterSingleton(() => new UIManager(scope.Resolve<IPanelFactory>(), scope.Resolve<IUIRouter>(), scope.Resolve<IUILayerService>()));
        }


    }
}