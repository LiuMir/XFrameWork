using System.IO;
using UnityEditor;
using UnityEngine;

public class BundleConfigDefine : ScriptableObject
{
    [Header("bundle资源根目录")]
    public string BundleResRootFolder;

    [Header("单个文件打成一个bundle")]
    public string SingleFileOneBundleFolder;

    [Header("当前文件夹下所有资源打成一个bundle")]
    public string CurFolerOneBundle;

    [Header("子文件夹下的所有资源打成一个bundle")]
    public string SubFolderOneBundle;

    [Header("单个场景文件一个bundle")]
    public string SceneFileOneBundle;
}

[CustomEditor(typeof(BundleConfigDefine), true)]
public class BundleConfigTool:Editor
{
    private string[] propertyNames= {"BundleResRootFolder", "SingleFileOneBundleFolder", "CurFolerOneBundle", "SubFolderOneBundle", "SceneFileOneBundle" };
    private SerializedProperty[] propertys;
    private Object[] FolderObjs;

    private static string configPath = "_Framework/Editor/BundleConfig";

    private void OnEnable()
    {
        propertys = new SerializedProperty[propertyNames.Length];
        FolderObjs = new Object[propertyNames.Length];
        for (int i = 0; i < propertyNames.Length; i++)
        {
            propertys[i] = serializedObject.FindProperty(propertyNames[i]);
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        SetFolderPathDragList();
    }

    //设置文件夹拖拽列表
    void SetFolderPathDragList()
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("拖拽需要打包的文件夹到对应的属性中即可");
        for (int i = 0; i < propertyNames.Length; i++)
        {
            FolderObjs[i] = EditorGUILayout.ObjectField(propertyNames[i], FolderObjs[i], typeof(Object), false);
            string path = AssetDatabase.GetAssetPath(FolderObjs[i]);
            if (!propertys[i].stringValue.Equals(path) && path.Length > 0)
            {
                propertys[i].stringValue = path;
                EditorUtility.SetDirty(target);
                serializedObject.ApplyModifiedProperties();
            }
        }
    }

    [MenuItem("Tool/CreateOrUpdateBundleConfig")]
    static void CreateOrUpdateConfig()
    {
        string path = Path.Combine(Application.dataPath, configPath);
        string fileName = "BundleConfig.asset";
        if (File.Exists(Path.Combine(path, fileName)))
        {
            AssetDatabase.DeleteAsset(Path.Combine("Assets", configPath, fileName));
        }
        ScriptableObject config = ScriptableObject.CreateInstance(typeof(BundleConfigDefine));
        AssetDatabase.CreateAsset(config, Path.Combine("Assets", configPath, fileName));
        AssetDatabase.Refresh();
        Debug.LogError(Path.Combine("Assets", configPath, fileName));
    }

}
