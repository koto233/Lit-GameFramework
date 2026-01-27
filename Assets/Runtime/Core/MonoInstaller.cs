using UnityEngine;
namespace LitGameFramework.Core
{
    public abstract class MonoInstaller : MonoBehaviour
    {
        public abstract bool IsGlobal { get; }
        public abstract void Install(LifetimeScope scope);
    }

}
