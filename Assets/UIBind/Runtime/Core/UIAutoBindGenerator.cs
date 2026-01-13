using UnityEditor;
using UnityEngine;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public static class UIAutoBindGenerator
{
    public static void Generate(UIBase ui)
    {
        if (ui == null) return;

        var binds = ui.GetComponentsInChildren<UIBind>(true);

        // Hierarchy 顺序稳定排序（深度优先）
        var ordered = binds
            .OrderBy(b => GetHierarchyPath(b.transform))
            .ToArray();

        for (int i = 0; i < ordered.Length; i++)
        {
            Undo.RecordObject(ordered[i], "Assign UIBind Index");
            ordered[i].Editor_SetIndex(i);
            EditorUtility.SetDirty(ordered[i]);
        }

        GeneratePartial(ui, ordered);

        Debug.Log($"[UIAutoBind] 生成完成：{ui.name}（{ordered.Length} 个绑定）", ui);
    }

    static void GeneratePartial(UIBase ui, UIBind[] binds)
    {
        var type = ui.GetType();
        var ns = type.Namespace;
        var className = type.Name;

        var namespaces = new HashSet<string>
        {
            "UnityEngine"
        };

        // 收集命名空间
        foreach (var bind in binds)
        {
            var c = UIBindAutoResolver.Resolve(bind);
            if (c != null && !string.IsNullOrEmpty(c.GetType().Namespace))
                namespaces.Add(c.GetType().Namespace);
        }

        var sb = new StringBuilder();

        // using
        foreach (var use in namespaces)
            sb.AppendLine($"using {use};");

        sb.AppendLine();

        if (!string.IsNullOrEmpty(ns))
        {
            sb.AppendLine($"namespace {ns}");
            sb.AppendLine("{");
        }

        sb.AppendLine($"    public partial class {className}");
        sb.AppendLine("    {");

        // 字段
        for (int i = 0; i < binds.Length; i++)
        {
            var bind = binds[i];
            var comp = UIBindAutoResolver.Resolve(bind);
            if (comp == null) continue;

            var fieldName = "_" + MakeSafeFieldName(bind.name);
            sb.AppendLine($"         [SerializeField] private {comp.GetType().Name} {fieldName};");
        }

        sb.AppendLine();
        sb.AppendLine("        protected override void GetUI()");
        sb.AppendLine("        {");
        sb.AppendLine("            base.GetUI();");

        // 赋值
        for (int i = 0; i < binds.Length; i++)
        {
            var bind = binds[i];
            var comp = UIBindAutoResolver.Resolve(bind);
            if (comp == null) continue;

            var fieldName = "_" + MakeSafeFieldName(bind.name);
            sb.AppendLine($"            {fieldName} = GetBind<{comp.GetType().Name}>({i});");
        }

        sb.AppendLine("        }");
        sb.AppendLine("    }");

        if (!string.IsNullOrEmpty(ns))
            sb.AppendLine("}");

        var dir = "Assets/UI/Generated";
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var path = $"{dir}/{className}.Bind.g.cs";
        File.WriteAllText(path, sb.ToString(), Encoding.UTF8);

        AssetDatabase.Refresh();
    }

    static string MakeSafeFieldName(string name)
    {
        var sb = new StringBuilder(name.Length + 2);

        if (!char.IsLetter(name[0]) && name[0] != '_')
            sb.Append('_');

        foreach (var c in name)
        {
            if (char.IsLetterOrDigit(c) || c == '_')
                sb.Append(c);
            else
                sb.Append('_');
        }
        return sb.ToString();
    }

    static string GetHierarchyPath(Transform t)
    {
        var path = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }
}
