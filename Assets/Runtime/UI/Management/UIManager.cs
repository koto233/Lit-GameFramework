using LitGameFramework.UIFramework.Core.Factory;
using LitGameFramework.UIFramework.Core.Panel;
using LitGameFramework.UIFramework.Core.Router;
using LitGameFramework.UIFramework.Core.Service;
using LitGameFramework.Controllers;

namespace LitGameFramework.UI.Core.Management
{
    public sealed class UIManager : IManager
    {
        private readonly IPanelFactory _factory;
        private readonly IUIRouter _router;
        private readonly IUILayerService _layer;

        public UIManager(IPanelFactory factory, IUIRouter router, IUILayerService layer)
        {
            Guard.NotNull(factory, nameof(factory));
            Guard.NotNull(router, nameof(router));
            Guard.NotNull(layer, nameof(layer));
            _factory = factory;
            _router = router;
            _layer = layer;
        }

        public TPanel Open<TPanel, TParam>(TParam param) where TPanel : UIPanel<TParam>
        {
            var panel = (TPanel)_factory.Create(typeof(TPanel));
            _layer.Attach(panel);
            _router.Push(panel, param);
            return panel;
        }

        public TPanel Open<TPanel>() where TPanel : UIPanel
        {
            return Open<TPanel, Unit>(Unit.Default);
        }

        public void Back()
        {
            _router.Pop();
        }
    }
}
