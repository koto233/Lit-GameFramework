using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public static class UIBindAutoResolver
{
    // 常见 UI 组件优先级
    static readonly Type[] PreferredTypes =
    {
        typeof(Button),
        typeof(Image),
        typeof(Text),
        typeof(Toggle),
        typeof(Slider),
        typeof(ScrollRect),
        typeof(Dropdown),
    };

    public static Component Resolve(UIBind bind)
    {
        // 1️⃣ 手动指定永远最高优先级
        if (bind.Target != null)
            return bind.Target;

        // 2️⃣ 常见 UI 组件
        foreach (var t in PreferredTypes)
        {
            var c = bind.GetComponent(t);
            if (c != null)
                return c;
        }

        // 3️⃣ 排除型兜底
        return bind.GetComponents<Component>()
            .FirstOrDefault(c =>
                !(c is Transform) &&
                !(c is CanvasRenderer) &&
                !(c is UIBind));
    }

    public static Type[] GetCandidateTypes(GameObject go)
    {
        return go.GetComponents<Component>()
            .Where(c =>
                !(c is Transform) &&
                !(c is CanvasRenderer) &&
                !(c is UIBind))
            .Select(c => c.GetType())
            .Distinct()
            .ToArray();
    }
}
