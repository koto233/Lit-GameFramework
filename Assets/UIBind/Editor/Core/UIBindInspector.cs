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
        }

        EditorGUILayout.Space(6);

        DrawStatus(bind);
    }

    void DrawStatus(UIBind bind)
    {
        Color bg = bind.Target == null ? new Color(1f, 0.4f, 0.4f) : new Color(0.6f, 1f, 0.6f); // 红色 / 绿色
        string msg = bind.Target == null ? "未绑定任何组件（运行时会报错）" : $"已绑定：{bind.Target.GetType().Name}";

        // 自定义颜色背景 HelpBox
        var rect = GUILayoutUtility.GetRect(0, 40, GUILayout.ExpandWidth(true));
        EditorGUI.DrawRect(rect, bg);
        GUIStyle style = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleLeft,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.black }
        };
        EditorGUI.LabelField(rect, msg, style);
    }
}
