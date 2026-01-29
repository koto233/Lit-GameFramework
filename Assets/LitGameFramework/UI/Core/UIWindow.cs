namespace LitGameFramework.UI.Core.Window
{
    public abstract class UIWindow : UIBase
    {
        public virtual void OnOpen(object args) { }
        public virtual void OnClose() { }

    }

}
