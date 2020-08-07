using System.IO;
using System.Threading.Tasks;
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
    private static string configFileName = "BundleConfig.asset";
    private static string abExtension = ".ab";

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
        CreateFolderPathDragList();
    }

    //设置文件夹拖拽列表
    void CreateFolderPathDragList()
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("拖拽需要打包的文件夹到对应的属性中即可");
        for (int i = 0; i < propertyNames.Length; i++)
        {
            FolderObjs[i] = EditorGUILayout.ObjectField(propertyNames[i], FolderObjs[i], typeof(Object), false);
            string path = AssetDatabase.GetAssetPath(FolderObjs[i]).Replace("Assets/", "");
            if (!propertys[i].stringValue.Equals(path) && path.Length > 0)
            {
                propertys[i].stringValue = path;
                EditorUtility.SetDirty(target);
                serializedObject.ApplyModifiedProperties();
            }
        }
    }

    [MenuItem("Tool/CreateBundleConfig")]
    static void CreateConfig()
    {
        string path = Path.Combine(Application.dataPath, configPath);
        if (!File.Exists(Path.Combine(path, configFileName)))
        {
            ScriptableObject config = CreateInstance(typeof(BundleConfigDefine));
            AssetDatabase.CreateAsset(config, Path.Combine("Assets", configPath, configFileName));
            AssetDatabase.Refresh();
        }
    }

    //构建ab包
    [MenuItem("Tool/BuildBundle")]
    static void BuildBundle()
    {
        Caching.ClearCache();
        SetBundleName();
        BuildPipeline.BuildAssetBundles(PathUtility.Instance.GetAbOutPath(), 
            BuildAssetBundleOptions.ChunkBasedCompression | 
            BuildAssetBundleOptions.DisableWriteTypeTree, BuildTarget.StandaloneWindows);
        AssetDatabase.Refresh();
    }

    //强制重新构建ab包
    [MenuItem("Tool/ForceRebuildBundle")]
    static void ForceRebuildBundle()
    {

    }

    private static BundleConfigDefine bundleConfig;
    static void SetBundleName()
    {
        bundleConfig = AssetDatabase.LoadAssetAtPath(Path.Combine("Assets", configPath, configFileName), typeof(BundleConfigDefine)) as BundleConfigDefine;
        ClearBundleName();
        SetCurFolderAsBundleName(bundleConfig.CurFolerOneBundle);
        SetSubFolderAsBundleName(bundleConfig.SubFolderOneBundle);
        SetFileNamsAsBundleName(bundleConfig.SingleFileOneBundleFolder);
        SetFileNamsAsBundleName(bundleConfig.SceneFileOneBundle);
    }

    static void ClearBundleName()
    {
        string[] BundleNames = AssetDatabase.GetAllAssetBundleNames();
        for (int i = 0; i < BundleNames.Length; i++)
        {
            EditorUtility.DisplayProgressBar("Clear AssetBundle Name", string.Format("{0}/{1}", i, BundleNames.Length), (float)i / BundleNames.Length);
            AssetDatabase.RemoveAssetBundleName(BundleNames[i], true);
        }
        EditorUtility.ClearProgressBar();
    }
       
    static void SetCurFolderAsBundleName(string folderPath)
    {
        SetFileBundleName(folderPath, (ai, filePath) => {
            ai.assetBundleName = folderPath + abExtension;
        });
    }

    static void SetSubFolderAsBundleName(string folderPath)
    {
        string dir = Path.Combine(Application.dataPath, folderPath);
        string[] dirs = Directory.GetDirectories(dir, "*", SearchOption.TopDirectoryOnly);
        string[] filePaths = Directory.GetFiles(dir, "*", SearchOption.TopDirectoryOnly);
        for (int i = 0; i < filePaths.Length; i++)
        {
            if (Path.GetExtension(filePaths[i]) != ".meta")
            {
                string localFilePath = PathUtility.Instance.GetAssetPath(filePaths[i]);
                AssetImporter ai = AssetImporter.GetAtPath(localFilePath);
                if (ai)
                {
                    ai.assetBundleName = Path.GetDirectoryName(localFilePath).ToLower() + abExtension;
                }
            }
        }
        if (dirs.Length > 0)
        {
            for (int i = 0; i < dirs.Length; i++)
            {
                SetSubFolderAsBundleName(dirs[i]);
            }
        }
    }

    static void SetFileNamsAsBundleName(string folderPath)
    {
        SetFileBundleName(folderPath, (ai, filePath) => {
            ai.assetBundleName = Path.Combine(folderPath, Path.GetFileNameWithoutExtension(filePath)).ToLower() + abExtension;
        });
    }

    static void SetFileBundleName(string folderPath, System.Action<AssetImporter, string> action)
    {
        string dir = Path.Combine(Application.dataPath, folderPath);
        string[] filePaths = Directory.GetFiles(dir);
        for (int i = 0; i < filePaths.Length; i++)
        {
            if (Path.GetExtension(filePaths[i]) != ".meta")
            {
                string localFilePath = PathUtility.Instance.GetAssetPath(filePaths[i]);
                AssetImporter ai = AssetImporter.GetAtPath(localFilePath);
                if (ai)
                {
                    action?.Invoke(ai, localFilePath);
                }
            }
        }
    }

}
