using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(UIBind))]
public class UIBindInspector : Editor
{
    public override void OnInspectorGUI()
    {
        var bind = (UIBind)target;
        var go = bind.gameObject;

        EditorGUILayout.LabelField("UI Bind", EditorStyles.boldLabel);

        // ===== Index（只读）=====
        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.IntField("Index", bind.Index);
        }

        EditorGUILayout.Space(6);

        // ===== 手动覆盖开关 =====
        EditorGUI.BeginChangeCheck();
        bool manual = EditorGUILayout.Toggle("手动指定组件", bind.ManualOverride);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(bind, "Toggle Manual Override");
            bind.Editor_SetManual(manual);
            EditorUtility.SetDirty(bind);
        }

        EditorGUILayout.Space(6);

        // ===== 自动推断 =====
        var auto = UIBindAutoResolver.Resolve(bind);

        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.ObjectField(
                "自动推断结果",
                auto,
                typeof(Component),
                true);
        }

        // ⭐ 核心逻辑：默认自动写入
        if (!bind.ManualOverride)
        {
            if (auto != null && bind.Target != auto)
            {
                Undo.RecordObject(bind, "Auto Assign UIBind Target");
                bind.Editor_SetTarget(auto);
                EditorUtility.SetDirty(bind);
            }
        }

        EditorGUILayout.Space(6);

        // ===== 手动模式 =====
        if (bind.ManualOverride)
        {
            EditorGUI.BeginChangeCheck();
            var newTarget = EditorGUILayout.ObjectField(
                "绑定组件",
                bind.Target,
                typeof(Component),
                true) as Component;

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(bind, "Set UIBind Target");
                bind.Editor_SetTarget(newTarget);
                EditorUtility.SetDirty(bind);
            }

            DrawTypeQuickSelect(bind, go);
        }

        EditorGUILayout.Space(6);

        DrawStatus(bind);
    }

    void DrawTypeQuickSelect(UIBind bind, GameObject go)
    {
        var types = UIBindAutoResolver.GetCandidateTypes(go);
        if (types.Length == 0)
            return;

        var labels = types.Select(t => t.Name).ToArray();

        EditorGUILayout.LabelField("快捷选择组件类型");

        int choice = EditorGUILayout.Popup(-1, labels);
        if (choice >= 0)
        {
            var comp = go.GetComponent(types[choice]);
            if (comp != null)
            {
                Undo.RecordObject(bind, "UIBind Quick Select");
                bind.Editor_SetTarget(comp);
                EditorUtility.SetDirty(bind);
            }
        }
    }

    void DrawStatus(UIBind bind)
    {
        if (bind.Target == null)
        {
            EditorGUILayout.HelpBox(
                "未绑定任何组件（运行时会报错）",
                MessageType.Error);
        }
        else
        {
            EditorGUILayout.HelpBox(
                $"已绑定：{bind.Target.GetType().Name}",
                MessageType.Info);
        }
    }
}
