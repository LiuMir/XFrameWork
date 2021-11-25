using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
[CustomEditor(typeof(UIView), true)]
public class UIViewEditor:Editor
{
    UIView view;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        view = target as UIView;
        SerializedProperty uiName = serializedObject.FindProperty("UIName");
        EditorGUI.BeginDisabledGroup(true);
        if (!Application.isPlaying) // 游戏处在运行时 不再使用这段逻辑去更改UIName
        {
            view.UIName = view.gameObject.name;
        }
        GUILayout.Space(10);
        EditorGUILayout.PropertyField(uiName);
        EditorGUI.BeginDisabledGroup(false);
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}