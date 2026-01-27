using System;
using LitGameFramework.UIFramework.Core.Panel;
namespace LitGameFramework.UIFramework.Core.Loader
{
    public interface IPanelLoader
    {
        UIBase Load(Type panelType);
    }

}
