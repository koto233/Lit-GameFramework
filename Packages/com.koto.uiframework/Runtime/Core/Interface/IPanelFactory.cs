using System;
using Koto.UIFramework.Core.Panel;

namespace Koto.UIFramework.Core.Factory
{
    public interface IPanelFactory
    {
        UIBase Create(Type panelType);
    }
}
