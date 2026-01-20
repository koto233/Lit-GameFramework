using System;
using Koto.UIFramework.Core.Policy;
namespace Koto.UIFramework.Core.Policy
{
public interface IPanelPolicyProvider
{
    PanelPolicy GetPolicy(Type panelType);
}
}