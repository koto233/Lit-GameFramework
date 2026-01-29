using LitGameFramework.Core;
using UnityEngine;

namespace LitGameFramework.Entry
{
    public abstract class EntryBase : MonoBehaviour
    {
        protected abstract void Register(LifetimeScope scope);
        protected abstract void Initialize(LifetimeScope scope);

        protected virtual void Start()
        {
            var scope = GameRoot.I.CurrentSceneScope;

            Register(scope);
            Initialize(scope);
        }
    }
}
