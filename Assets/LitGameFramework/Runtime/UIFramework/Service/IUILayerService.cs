using LitGameFramework.Services;
using LitGameFramework.UIFramework.Core.Panel;

namespace LitGameFramework.UIFramework.Core.Service
{
    public interface IUILayerService: IService
    {
        void Attach(UIBase panel);
    }
}
