using System;
using Koto.UIFramework.Core.Panel;
namespace Koto.UIFramework.Core.Loader
{
    public interface IPanelLoader
    {
        UIBase Load(Type panelType);
    }

}
