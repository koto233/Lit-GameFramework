using System;
using LitGameFramework.UIFramework.Core.Policy;
namespace LitGameFramework.UIFramework.Core.Policy
{
public interface IPanelPolicyProvider
{
    PanelPolicy GetPolicy(Type panelType);
}
}