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
            GameRoot.I.BeginSceneScope();
            var sceneScope = GameRoot.ResolverHub.SceneScope;
            Register(sceneScope);
            Initialize(sceneScope);
        }
    }
}
