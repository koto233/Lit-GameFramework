using System;
using System.Collections.Generic;
using UIFramework.Core.Panel;

namespace UIFramework.Core.Management
{
    public class UIManager
    {
        private readonly Dictionary<Type, UIBase> _cache =
            new Dictionary<Type, UIBase>();

        public TPanel Open<TPanel, TParam>(TParam param)
            where TPanel : UIPanel<TParam>
        {
            var type = typeof(TPanel);

            if (!_cache.TryGetValue(type, out var panel))
            {
                // panel = CreatePanel(type);
                _cache[type] = panel;
            }

            panel.ShowInternal(param);
            return (TPanel)panel;
        }

        public TPanel Open<TPanel>()
            where TPanel : UIPanel
        {
            return Open<TPanel, Unit>(Unit.Default);
        }

        // private UIBase CreatePanel(Type panelType)
        // {
        //     // 这里后续可以接 Addressables / DI / Factory
        //     var prefab = LoadPrefab(panelType);
        //     return Instantiate(prefab).GetComponent<UIBase>();
        // }
    }

}
