using Koto.UIFramework.Core.Panel;

namespace Koto.UIFramework.Core.Router
{
    public interface IUIRouter
    {
        void Push(UIBase panel, object param);
        void Pop();
    }
}
