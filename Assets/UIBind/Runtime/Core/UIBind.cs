using UnityEngine;

public class UIBind : MonoBehaviour
{
    [SerializeField] private int index;

    [SerializeField] private Component target;

    [SerializeField] private bool manualOverride; // ⭐ 新增

    public int Index => index;
    public Component Target => target;
    public bool ManualOverride => manualOverride;

#if UNITY_EDITOR
    public void Editor_SetIndex(int i) => index = i;
    public void Editor_SetTarget(Component c) => target = c;
    public void Editor_SetManual(bool v) => manualOverride = v;
#endif
}
