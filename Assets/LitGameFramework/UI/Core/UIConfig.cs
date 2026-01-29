namespace LitGameFramework.UI.Core.Service
{
    public sealed class UIConfig
    {
        public readonly string PrefabPath;
        public readonly UILayer Layer;

        public UIConfig(string prefabPath, UILayer layer)
        {
            PrefabPath = prefabPath;
            Layer = layer;
        }
    }

}