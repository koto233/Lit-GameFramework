using LitGameFramework.UIFramework.Core.Panel;

namespace LitGameFramework.UIFramework.Core.Router
{
    public interface IUIRouter
    {
        void Push(UIBase panel, object param);
        void Pop();
    }
}
