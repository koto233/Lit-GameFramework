using UnityEngine;
using System;
using System.Collections.Generic;

namespace LitGameFramework.UIFramework.Core.Policy
{
    [CreateAssetMenu(
     menuName = "UI框架/Panel Policy Config",
     fileName = "UIPanelPolicies"
 )]
    public class UIPanelPolicyAsset : ScriptableObject
    {
        [Serializable]
        private struct Entry
        {
            public string PanelTypeName;
            public PanelPolicy Policy;
        }

        [SerializeField]
        private List<Entry> _entries = new();

        private Dictionary<Type, PanelPolicy> _cache;

        public PanelPolicy GetPolicy(Type panelType)
        {
            _cache ??= BuildCache();

            if (_cache.TryGetValue(panelType, out var policy))
                return policy;

            return PanelPolicyDefaults.Default;
        }

        private Dictionary<Type, PanelPolicy> BuildCache()
        {
            var dict = new Dictionary<Type, PanelPolicy>();

            foreach (var entry in _entries)
            {
                var type = Type.GetType(entry.PanelTypeName);
                if (type == null)
                {
                    Debug.LogWarning($"Panel type not found: {entry.PanelTypeName}");
                    continue;
                }

                dict[type] = entry.Policy;
            }

            return dict;
        }
    }

}
