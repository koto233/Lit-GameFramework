using System;
using LitGameFramework.UIFramework.Core.Policy;
namespace LitGameFramework.UIFramework.Core.Policy
{
    public class PanelPolicySOProvider : IPanelPolicyProvider
    {
        private readonly UIPanelPolicyAsset _asset;

        public PanelPolicySOProvider(UIPanelPolicyAsset asset)
        {
            _asset = asset;
        }

        public PanelPolicy GetPolicy(Type panelType)
        {
            return _asset.GetPolicy(panelType);
        }
    }

}
