using Koto.UIFramework.Core.Factory;
using Koto.UIFramework.Core.Panel;
using Koto.UIFramework.Core.Router;
using Koto.UIFramework.Core.Service;

namespace Koto.UI.Core.Management
{
    public sealed class UIManager
    {
        private readonly IPanelFactory _factory;
        private readonly IUIRouter _router;
        private readonly IUILayerService _layer;

        public UIManager(IPanelFactory factory, IUIRouter router, IUILayerService layer)
        {
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

        public TPanel Open<TPanel>()  where TPanel : UIPanel
        {
            return Open<TPanel, Unit>(Unit.Default);
        }

        public void Back()
        {
            _router.Pop();
        }
    }
}
