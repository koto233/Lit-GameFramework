
namespace Koto.UIFramework.Core.Policy
{
    public static class PanelPolicyDefaults
    {
        /// <summary>
        /// 框架级兜底策略
        /// </summary>
        public static readonly PanelPolicy Default = new PanelPolicy
        {
            IsSingleton = false,
            Mode = UIPanelMode.Fullscreen
        };
    }
}
