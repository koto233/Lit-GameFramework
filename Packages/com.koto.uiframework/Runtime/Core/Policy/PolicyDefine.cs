using System;
namespace Koto.UIFramework.Core.Policy
{
    public enum UIPanelMode
    {
        Fullscreen,
        Popup
    }

    [Serializable]
    public class PanelPolicy
    {
        public bool IsSingleton;
        public UIPanelMode Mode;
    }

}
