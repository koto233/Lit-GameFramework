using System;
using LitGameFramework.UIFramework.Core.Panel;

namespace LitGameFramework.UIFramework.Core.Factory
{
    public interface IPanelFactory
    {
        UIBase Create(Type panelType);
    }
}
