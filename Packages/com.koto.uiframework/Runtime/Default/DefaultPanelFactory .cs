using System;
using System.Collections.Generic;
using Koto.UIFramework.Core.Loader;
using Koto.UIFramework.Core.Panel;
using Koto.UIFramework.Core.Policy;

namespace Koto.UIFramework.Core.Factory
{
    public class DefaultPanelFactory : IPanelFactory
    {
        private readonly Dictionary<Type, UIBase> _singletons = new();
        private readonly IPanelLoader _loader;
        private readonly IPanelPolicyProvider _policyProvider;
        public DefaultPanelFactory(IPanelLoader loader)
        {
            _loader = loader;
        }

        public UIBase Create(Type panelType)
        {
            var policy = _policyProvider.GetPolicy(panelType);

            if (policy.IsSingleton &&
                _singletons.TryGetValue(panelType, out var cached))
            {
                return cached;
            }

            var panel = _loader.Load(panelType);

            if (policy.IsSingleton)
                _singletons[panelType] = panel;

            panel.OnCreateInternal();
            return panel;
        }
    }

}
